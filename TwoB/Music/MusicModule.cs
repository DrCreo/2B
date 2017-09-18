using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            await PlayMusicAsync();
        }

        /// <summary>
        /// Connects to all voice channels provided in the BotConfig.
        /// </summary>
        /// <returns></returns>
        private async Task ConnectToVoiceChannels()
        {
            var vnext = this._voice.Client.GetVoiceNextClient();
            _vncList = new List<VoiceNextConnection>();
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

            var musicPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            musicPath = Path.Combine(musicPath, @"Music");
            var files = Directory.GetFiles(musicPath);
            Console.WriteLine(musicPath);
            try
            {
                while (playMusic)
                {
                    foreach (var vnc in _vncList)
                    {
                        await vnc.SendSpeakingAsync(true); // send a speaking indicator
                    }
                    var currentSong = files[rand.Next(files.Length)];
                    Console.WriteLine(currentSong);

                    var psi = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $@"-i ""{currentSong}"" -ac 2 -f s16le -ar 48000 pipe:1",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };
                    var ffmpeg = Process.Start(psi);
                    var ffout = ffmpeg.StandardOutput.BaseStream;

                    UpdatePlayingStatus(currentSong);

                    var buff = new byte[3840];
                    var br = 0;
                    while ((br = ffout.Read(buff, 0, buff.Length)) > 0 && playMusic)
                    {
                        if (br < buff.Length) // not a full sample, mute the rest
                            for (var i = br; i < buff.Length; i++)
                                buff[i] = 0;

                        foreach (var vnc in _vncList)
                        {
                            await vnc.SendAsync(buff, 20);
                        }
                    }
                    foreach (var vnc in _vncList)
                    {
                        await vnc.SendSpeakingAsync(false); // we're not speaking anymore
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine($"{exc.Message}\n{exc.StackTrace}");
                RestartMusicModule();
            }

        }

        /// <summary>
        /// Emergency Restart to be called from out side the MusicModule Class
        /// </summary>
        internal void EmergencyRestart()
        {
            RestartMusicModule();
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
            await _client.UpdateStatusAsync(new Game(strings[strings.Length - 1].Split('.')[0]));
        }

        /// <summary>
        /// Disconnects the bot from all voice channels and nulls the voice channel list and VoiceNextClient 
        /// so that it can be restarted.
        /// </summary>
        private void DisconnectAllVoiceChannels()
        {
            _vncList.ForEach(vc => vc.Disconnect());
            _vncList = null;
            _voice = null;
        }

        /// <summary>
        /// Disconnects the bot from voice channels and restarts the Music Module then reconnects to the voice 
        /// channels.
        /// </summary>
        private async void RestartMusicModule()
        {
            DisconnectAllVoiceChannels();
            SetupVoiceNextClient();
            await ConnectToVoiceChannels();
            await PlayMusicAsync();
        }
    }
}
