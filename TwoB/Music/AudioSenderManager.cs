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
            if (activeConnetionList.Contains(_audioSenders[0]._vnc))
                await _audioSenders[0].SendAudio(buff);           
        }
    }
}
