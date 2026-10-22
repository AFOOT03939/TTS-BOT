using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;
using TTS_BOT.Services;

namespace TTS_BOT.NewFolder
{
    public class main
    {
        private readonly SyllabificationService _syllabificationService;
        private readonly AudioPlayerService _audioPlayerService;

        public main(SyllabificationService syllabificationService, AudioPlayerService audioPlayerService)
        {
            _syllabificationService = syllabificationService;
            _audioPlayerService = audioPlayerService;
        }

        public void writeText()
        {
            do
            {
                Console.WriteLine("Escribe un texto a continuación ");
                var text = Console.ReadLine();
                var syllables = _syllabificationService.syllabificate(text);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Silaba\n" + syllables);
                Console.ForegroundColor = ConsoleColor.White;

                string[] silabas = syllables.Split(
                    new string[] { "[s]" },
                    StringSplitOptions.RemoveEmptyEntries
                );

                /*
                foreach (var silaba in silabas)
                {
                    _audioPlayerService.audioPlay(silaba);
                }
                */

            } while (true);
        }
    }
}
