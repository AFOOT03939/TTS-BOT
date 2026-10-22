using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace TTS_BOT.Services
{
    public class AudioPlayerService
    {
        public void audioPlay(string audio)
        {
            string audioFilePath = "E:\\Users\\victo\\Downloads\\" + audio + ".mp3";

            using (var audioFile = new AudioFileReader(audioFilePath))
            using (var outputDevice = new WasapiPlayerBuilder().Build())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
