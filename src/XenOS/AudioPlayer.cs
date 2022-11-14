using Cosmos.System.Audio;
using Cosmos.System.Audio.DSP.Processing;
using Cosmos.System.Audio.IO;
using System;
using System.IO;

namespace XenOS
{
    internal class AudioPlayer
    {
        public void PlayWAV(string path)
        {
            try
            {
                if (!Drivers.AudioEnabled)
                {
                    throw new Exception("There are no sound devices that can be used!");
                }
                byte[] audiodata = File.ReadAllBytes(path);
                Drivers.audioStream = MemoryAudioStream.FromWave(audiodata);
                Drivers.audioStream.PostProcessors.Add(new GainPostProcessor(0.5f));
                Drivers.mixer.Streams.Add(Drivers.audioStream);
                Drivers.audioManager = new AudioManager()
                {
                    Stream = Drivers.mixer,
                    Output = Drivers.driver
                };
                Drivers.audioManager.Enable();
            }
            catch(Exception EX)
            {
                Console.WriteLine("ERROR: " + EX.Message);
            }
        }

        public void PlayWAVFromBytes(byte[] audiodata)
        {
            try
            {
                if (!Drivers.AudioEnabled)
                {
                    throw new Exception("There are no sound devices that can be used!");
                }
                Drivers.audioStream = MemoryAudioStream.FromWave(audiodata);
                Drivers.audioStream.PostProcessors.Add(new GainPostProcessor(0.5f));
                Drivers.mixer.Streams.Add(Drivers.audioStream);
                Drivers.audioManager = new AudioManager()
                {
                    Stream = Drivers.mixer,
                    Output = Drivers.driver
                };
                Drivers.audioManager.Enable();
            }
            catch (Exception EX)
            {
                Console.WriteLine("ERROR: " + EX.Message);
            }
        }
    }
}
