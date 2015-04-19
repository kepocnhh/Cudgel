using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;

namespace Cudgel
{
    public interface IMicroListener
    {
        void getAudioLevel(int al);
        void toRecognize(byte[] data);
    }

    class Micro
    {

#region FIELDS
//__________FIELDS____________________________\\

        //public Label mixer;
        //private int min;
        //private int mid;
        //private int max;
        private int audiolevel = 0;
        private int recordlevel = 0;
        private WaveIn recorder;
        private BufferedWaveProvider bufferedWaveProvider;
        private List<byte[]> buffer;
        private VoiceRecognition voice;
        private int TALK_VOLUME = 555;
        private int TALK_RANG = 4;
        private int ONE_BUFFER_SIZE = 1600;
        private IMicroListener microListener;

//____________________________________________//
#endregion


        public Micro()
        {
            //mixer = l;
            //min = 0;
            //mid = 0;
            //max = 0;
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
            //mixer.Text = "start";
            recorder = new WaveIn();
            recorder.DataAvailable += RecorderOnDataAvailable;
            //bufferedWaveProvider = new BufferedWaveProvider(recorder.WaveFormat);
            recorder.StartRecording();
            buffer = new List<byte[]>();
            voice = new VoiceRecognition();
        }
        private void RecorderOnDataAvailable(object sender, WaveInEventArgs w)
        {
            //mixer.Text = GetAudioLevel(waveInEventArgs.Buffer) + " " + waveInEventArgs.BytesRecorded;
            //bufferedWaveProvider.AddSamples(waveInEventArgs.Buffer, 0, waveInEventArgs.BytesRecorded);
            audiolevel = GetAudioLevel(w.Buffer);
            if (microListener != null)
            {
                microListener.getAudioLevel(audiolevel);
            }
            //mixer.Text = audiolevel  + " ";
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
                    voice.recognize(audiobuffer);
                }
            }
            else
            {
                buffer.Clear();
            }
        }
        public void StopRecording()
        {
            //mixer.Text = "stop";
            recorder.StopRecording();
            buffer = null;
        }
    }
}