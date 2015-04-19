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
        private int TALK_VOLUME = 500;
        private int TALK_RANG = 6;
        private int ONE_BUFFER_SIZE = 3200;
        public int CHANELS = 1;
        public int PRECISION = 16;
        public int SAMPLE_RATE = 16000;
        //private int ONE_BUFFER_SIZE = 35280;
        //public int CHANELS = 2;
        //public int PRECISION = 32;
        //public int SAMPLE_RATE = 44100;
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
            recorder.WaveFormat = new WaveFormat(SAMPLE_RATE, PRECISION, CHANELS);
            recorder.DataAvailable += RecorderOnDataAvailable;
            recorder.StartRecording();
            buffer = new List<byte[]>();
        }
        private void RecorderOnDataAvailable(object sender, WaveInEventArgs w)
        {
            audiolevel = GetAudioLevel(w.Buffer);
                microListener.getAudioLevel(audiolevel);
            if (audiolevel > TALK_VOLUME)
            {
                var buf = new byte[ONE_BUFFER_SIZE];
                w.Buffer.CopyTo(buf, 0);
                buffer.Add(buf);
                byte[] audiobuffer = prepareVoice();
                if (checkReady())
                {
                    microListener.toRecognize(audiobuffer);
                }
            }
            else
            {
                if (GetRecordLevel() > 0)
                {
                    byte[] audiobuffer = prepareVoice();
                    microListener.toRecognize(audiobuffer);
                    buffer.Clear();
                }
            }
        }
        private bool checkReady()
        {
            return (GetRecordLevel() / TALK_RANG) * TALK_RANG == GetRecordLevel();
        }
        private byte[] prepareVoice()
        {
            byte[] audiobuffer = new byte[ONE_BUFFER_SIZE * GetRecordLevel()];
            for (int i = 0; i < buffer.Count; i++)
            {
                buffer[i].CopyTo(audiobuffer, i * ONE_BUFFER_SIZE);
            }
            return audiobuffer;
        }
        public void StopRecording()
        {
            recorder.StopRecording();
            buffer = null;
        }

    }
}