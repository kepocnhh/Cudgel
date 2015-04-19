using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CUETools.Codecs;
using CUETools.Codecs.FLAKE;
using System.IO;
using System.Threading;

namespace Cudgel
{
    class VoiceRecognition
    {
        object lockObj = new object();
        public VoiceRecognition()
        {

        }
        private byte[] Wav2FlacBuffConverter(byte[] Buffer)
        {
            Stream OutWavStream = new MemoryStream();
            Stream OutFlacStream = new MemoryStream();
            AudioPCMConfig pcmconf = new AudioPCMConfig(16, 1, 16000);
            WAVWriter wr = new WAVWriter(null, OutWavStream, pcmconf);
            wr.Write(new AudioBuffer(pcmconf, Buffer, Buffer.Length / 2));
            OutWavStream.Seek(0, SeekOrigin.Begin);
            WAVReader audioSource = new WAVReader(null, OutWavStream);
            if (audioSource.PCM.SampleRate != 16000)
            {
                return null;
            }
            AudioBuffer buff = new AudioBuffer(audioSource, 0x10000);
            FlakeWriter flakeWriter = new FlakeWriter(null, OutFlacStream, audioSource.PCM);
            flakeWriter.CompressionLevel = 8;
            while (audioSource.Read(buff, -1) != 0)
            {
                flakeWriter.Write(buff);
            }
            OutFlacStream.Seek(0, SeekOrigin.Begin);
            byte[] barr = new byte[OutFlacStream.Length];
            OutFlacStream.Read(barr, 0, (int)OutFlacStream.Length);
            return barr;
        }
        public void recognize(byte[] data)
        {
            data = Wav2FlacBuffConverter(data);
            new Thread(postrecognize).Start(data);
        }

        private void postrecognize(object obj)
        {
            byte[] datain = (byte[]) obj;
            lock (lockObj)
            {
            }
        }
    }
}