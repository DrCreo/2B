using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TwoB
{
    public class MusicModule
    {
        private DiscordClient _client { get; set; }
        private VoiceNextClient _voice { get; set; }
        private List<VoiceNextConnection> _vncList { get; set; }
        private List<VoiceNextConnection> _vncActiveList { get; set; }
        private AudioSenderManager _asManager { get; set; }


        /// <summary>
        /// Used to exit the music loop.
        /// </summary>
        private bool playMusic = true;


        /// <summary>
        /// Starts the music Module.
        /// </summary>
        public async void StartMusicModule()
        {
            _client = BotConfig.Instance.Client;
            SetupVoiceNextClient();
            await ConnectToVoiceChannels();
            var t = Task.Run(() => TrackActiveChannelAsync()); ;
            var t2 = Task.Run(() => PlayMusicAsync());
        }

        /// <summary>
        /// Connects to all voice channels provided in the BotConfig.
        /// </summary>
        /// <returns></returns>
        private async Task ConnectToVoiceChannels()
        {

            var vnext = this._voice.Client.GetVoiceNextClient();
            _vncList = new List<VoiceNextConnection>();
            _vncActiveList = new List<VoiceNextConnection>();
            foreach (var id in BotConfig.Instance.MusicChannelIds)
            {
                var channel = await this._voice.Client.GetChannelAsync(id);
                var guild = channel.Guild;
                var vnc = vnext.GetConnection(guild);
                if (vnc != null)
                    throw new InvalidOperationException("Already in a channel in this guild.");

                vnc = await vnext.ConnectAsync(channel);
                _vncList.Add(vnc);
            }
        }

        /// <summary>
        /// Setup VoiceNectClient 
        /// </summary>
        private void SetupVoiceNextClient()
        {
            //setup Voice nextClient
            _voice = _client.UseVoiceNext();
        }

        /// <summary>
        /// Enters the main loop where music is played.
        /// </summary>
        /// <returns></returns>
        private async Task PlayMusicAsync()
        {
            var rand = new Random();
            var nextUpdate = DateTime.Now.AddSeconds(5);
            var musicPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            musicPath = Path.Combine(musicPath, @"Music");
            var files = Directory.GetFiles(musicPath);

            _asManager = new AudioSenderManager(_vncList);

            try
            {
                foreach (var vnc in _vncList)
                {
                    await vnc.SendSpeakingAsync(true); // send a speaking indicator
                }
                while (playMusic)
                {

                    var currentSong = files[rand.Next(files.Length)];

                    var psi = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $@"-i ""{currentSong}"" -ac 2 -f s16le -loglevel quiet -ar 48000 pipe:1",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                    };
                    var ffmpeg = Process.Start(psi);
                    var ffout = ffmpeg.StandardOutput.BaseStream;

                    UpdatePlayingStatus(currentSong);

                    var buff = new byte[3840];
                    var br = 0;
                    while ((br = ffout.Read(buff, 0, buff.Length)) > 0 && playMusic)
                    {
                        if (br < buff.Length)
                            for (var i = br; i < buff.Length; i++)
                                buff[i] = 0;

                        if (nextUpdate < DateTime.Now)
                        {
                            nextUpdate = DateTime.Now.AddSeconds(5);
                        }
                        await _asManager.SendAudio(buff, _vncActiveList);

                        /*foreach (var vnc in _vncList)
                        {
                            if (_connectionsWithUsers.Any(c => c.Channel.Id == vnc.Channel.Id))
                                await vnc.SendAsync(buff, 20);
                            else
                        }*/
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"{exc.Message}\n{exc.StackTrace}");
            }

        }


        private async Task TrackActiveChannelAsync()
        {
            var vncCache = _vncList;
            List<DiscordGuild> guilds = new List<DiscordGuild>();
            List<DiscordMember> users = new List<DiscordMember>();

            foreach (var c in vncCache)
                guilds.Add(c.Channel.Guild);
            try
            {
                while (playMusic)
                {
                    var tempActiveChannels = new List<VoiceNextConnection>();
                    foreach (var g in guilds)
                    {
                        var tempMembers = await g.GetAllMembersAsync();

                        foreach (var member in tempMembers)
                        {
                            if (!member.IsBot && member.VoiceState != null)
                                foreach (var vnc in vncCache)
                                {
                                    try
                                    {
                                        if (member.VoiceState.Channel.Id == vnc.Channel.Id)
                                            if (!tempActiveChannels.Contains(vnc))
                                                tempActiveChannels.Add(vnc);
                                    }
                                    catch (Exception e)
                                    {
                                    }
                                }
                        }
                        await Task.Delay(3000);
                    }
                    Console.WriteLine($"Active Channel Count: {tempActiveChannels.Count}\n");
                    _vncActiveList = tempActiveChannels;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }


        /// <summary>
        /// Emergency Disconnect to be called from out side the MusicModule Class
        /// </summary>
        internal void EmergencyDisconnect()
        {
            DisconnectAllVoiceChannels();
        }

        /// <summary>
        /// Updates the status to show the current song using the files path.
        /// </summary>
        /// <param name="currentSong"></param>
        private async void UpdatePlayingStatus(string currentSong)
        {
            var strings = currentSong.Split('\\');
            await _client.UpdateStatusAsync(new Game($"{strings[strings.Length - 1].Split('.')[0]} ~ [{_vncActiveList.Count}/{_vncList.Count}] Active Music Channels."));
        }

        /// <summary>
        /// Disconnects the bot from all voice channels and nulls the voice channel list and VoiceNextClient 
        /// so that it can be restarted.
        /// </summary>
        private async void DisconnectAllVoiceChannels()
        {
            _vncList.ForEach(vc => vc.Disconnect());
            _vncList = null;
            _voice = null;
            playMusic = false;
            await Task.Delay(0);
        }

    }

    class AudioSenderManager
    {
        public List<AudioSender> _audioSenders { get; private set; }



        public AudioSenderManager(List<VoiceNextConnection> vncList)
        {
            _audioSenders = new List<AudioSender>();
            foreach (var vnc in vncList)
            {
                _audioSenders.Add(new AudioSender(vnc));
            }
        }

        public async Task SendAudio(byte[] buff, List<VoiceNextConnection> activeConnetionList)
        {
            foreach (var a in _audioSenders)
            {
                if (activeConnetionList.Any(acl => acl.Channel.Id == a.vnc.Channel.Id))
                {
                    await a.SendAudio(buff);
                    //Console.WriteLine($"Audio Sent to {a.vnc.Channel.Name}\n");
                }
                else
                {
                    //Console.WriteLine($"No Audio Sent to {a.vnc.Channel.Name}\n");
                }
            }
        }
    }


    class AudioSender
    {

        public VoiceNextConnection vnc { get; set; }

        public AudioSender(VoiceNextConnection vnc)
        {
            this.vnc = vnc;
        }

        public async Task SendAudio(byte[] buff)
        {
            await vnc.SendAsync(buff, 20);
        }
    }
}
