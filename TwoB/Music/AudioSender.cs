using DSharpPlus.VoiceNext;
using System.Threading.Tasks;

namespace TwoB
{
    class AudioSender
    {

        public VoiceNextConnection _vnc { get; set; }

        public AudioSender(VoiceNextConnection vnc)
        {
            this._vnc = vnc;
        }

        public async Task SendAudio(byte[] buff)
        {
            await _vnc.SendAsync(buff, 20);
        }
    }
}
