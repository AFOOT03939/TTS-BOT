using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace TTS_BOT.Services
{
    public class AudioPlayerService
    {
        public void audioPlay(List<string> syllables)
        {
            var readers = new List<AudioFileReader>();

            foreach (var syllable in syllables)
            {
                var reader = new AudioFileReader("E:\\Users\\victo\\Downloads\\audios\\" + syllable + ".m4a");

                readers.Add(reader);

            }

            var playlist = new ConcatenatingSampleProvider(readers);

            var outputDevice = new WasapiPlayerBuilder().Build();
            outputDevice.Init(playlist);
            outputDevice.Play();
        }
    }
}
