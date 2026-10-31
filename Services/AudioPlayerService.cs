//Este servicio era para pruebas locales dentro de windows, ahora mismo ya no se utilizará debido a la implementación del bot
//sin embargo, lo voy a mantener por si las dudas.

/*
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
                var reader = new AudioFileReader("E:\\Users\\victo\\Downloads\\audios_wav\\" + syllable + ".wav");

                readers.Add(reader);

            }

            var playlist = new ConcatenatingSampleProvider(readers);

            var volumeProvider = new VolumeSampleProvider(playlist)
            {
                Volume = 10.0f
            };

            var outputDevice = new WasapiPlayerBuilder().Build();
            outputDevice.Init(playlist);
            outputDevice.Play();
        }
    }
}
*/