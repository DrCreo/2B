using DSharpPlus.VoiceNext;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwoB
{
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

                if (activeConnetionList.Any(acl => acl.Channel.Id == a._vnc.Channel.Id))
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
}
