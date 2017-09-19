using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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

        private bool _isWindows => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        private bool _isLinux => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        private byte[] _buffer { get; set; }

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
            var t3 = Task.Run(() => SendBufferAsync());
        }

        private async void SendBufferAsync()
        {
            byte[] lastBuffer = { };
            while (playMusic)
            {
                if (_asManager != null && _buffer != null)

                        await _asManager.SendAudio(_buffer, _vncActiveList);
            }

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
            _voice = _client.UseVoiceNext();
        }

        /// <summary>
        /// Enters the main loop where music is played.
        /// </summary>
        /// <returns></returns>
        private async Task PlayMusicAsync()
        {
            var rand = new Random();
            var musicPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            musicPath = Path.Combine(musicPath, @"Music");
            var files = Directory.GetFiles(musicPath);

            _asManager = new AudioSenderManager(_vncList);

            try
            {
                foreach (var vnc in _vncList)
                    await vnc.SendSpeakingAsync(true); // send a speaking indicator

                while (playMusic)
                {
                    var currentSong = files[rand.Next(files.Length)];
                    
                    //-loglevel quiet
                    var psi = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $@"-i ""{currentSong}"" -ac 2 -f s16le  -ar 48000 pipe:1",
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

                        await _asManager.SendAudio(buff, _vncActiveList);
                    }
                }
            }
            catch (Exception exc)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{exc.Message}\n{exc.StackTrace}");
                Console.ResetColor();
            }

        }

        private async Task TrackActiveChannelAsync()
        {
            
            var guilds = _vncList.Select(t => t.Channel.Guild).ToList();

            try
            {
                while (playMusic)
                {
                    var tempActiveChannels = new List<VoiceNextConnection>();
                    foreach (var guild in guilds)
                    {
                        DiscordMember[] filteredMembers = guild.GetAllMembersAsync().Result
                        // Make sure they're not a bot
                        .Where(t => !t.IsBot)
                        // Make sure they're participating in voice / have a channel.
                        .Where(t => t.VoiceState != null && t.VoiceState.Channel != null)
                        // Make sure the VNC channel ID is the same as the VoiceState ID
                        .Where(t => _vncList.Any(v => v.Channel.Id == t.VoiceState.Channel.Id))
                        .ToArray();

                        foreach (var member in filteredMembers)
                            foreach (var vnc in _vncList)
                                if (vnc.Channel.Id == member.VoiceState.Channel.Id && !tempActiveChannels.Contains(vnc))
                                    tempActiveChannels.Add(vnc);
                    }
                    Console.WriteLine($"Active Channel Count: {tempActiveChannels.Count}\n");
                    _vncActiveList = tempActiveChannels;
                    await Task.Delay(3000);
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
            string[] strings;
            if (_isWindows)
                strings = currentSong.Split('\\');
            else
                strings = currentSong.Split('/');
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
}
