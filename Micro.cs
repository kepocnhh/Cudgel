using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace Cudgel
{

#region Interface _______________________________
//                                                                                        \\

    public interface IMicroListener
    {
        void getAudioLevel(int al);
        void toRecognize(byte[] data);
    }

//____________________________________________//
#endregion

    class Micro : IMicroListener
    {

#region FIELDS------------------------------------------------------\\

        private int audiolevel = 0;
        private WaveIn recorder;
        private List<byte[]> buffer;
        private int TALK_VOLUME = 555;
        private int TALK_RANG = 4;
        private int ONE_BUFFER_SIZE = 3200;
        private IMicroListener microListener;
        
#endregion _____________________________________//


#region MicroListener-----------------------------------------------\\

        public virtual void getAudioLevel(int al)
        {
        }
        public virtual void toRecognize(byte[] data)
        {
        }

#endregion _____________________________________//


        public Micro()
        {
            setMicroListener(this);
        }
        public Micro(IMicroListener ml)
        {
            setMicroListener(ml);
        }
        public void setMicroListener(IMicroListener ml)
        {
            microListener = ml;
        }

        private int GetAudioLevel(byte[] buf)
        {
            int lvl = 0;
            for (int index = 0; index < buf.Length; index += 2)
            {
                short sample = (short)((buf[index + 1] << 8) | buf[index]);
                lvl += (int)Math.Abs(sample / 2768f);
            }
            return lvl;
        }
        private int GetRecordLevel()
        {
            return buffer.Count;
        }
        public void StarTrek()
        {
            recorder = new WaveIn();
            recorder.DataAvailable += RecorderOnDataAvailable;
            recorder.StartRecording();
            buffer = new List<byte[]>();
        }
        private void RecorderOnDataAvailable(object sender, WaveInEventArgs w)
        {
            audiolevel = GetAudioLevel(w.Buffer);
                microListener.getAudioLevel(audiolevel);
            buffer.Add(w.Buffer);
            if (audiolevel > TALK_VOLUME)
            {
                byte[] audiobuffer = new byte[ONE_BUFFER_SIZE * GetRecordLevel()];
                for (int i = 0; i < buffer.Count; i++)
                {
                    buffer[i].CopyTo(audiobuffer, i * ONE_BUFFER_SIZE);
                }
                if (GetRecordLevel() > TALK_RANG)
                {
                        microListener.toRecognize(audiobuffer);
                }
            }
            else
            {
                buffer.Clear();
            }
        }
        public void StopRecording()
        {
            recorder.StopRecording();
            buffer = null;
        }
    }
}