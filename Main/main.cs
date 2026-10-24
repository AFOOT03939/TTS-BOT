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

                string[] splitText;
                List<string> dirtySyllables = new List<string>();
                List<string> syllables = new List<string>();

                splitText = _syllabificationService.splitWord(text);

                foreach(var split in splitText)
                {
                    dirtySyllables.Add(_syllabificationService.syllabificate(split));

                }

                foreach (var dirtySyllab in dirtySyllables)
                {
                    syllables.AddRange(dirtySyllab.Split("[s]", StringSplitOptions.RemoveEmptyEntries));
                }

                Console.ForegroundColor = ConsoleColor.Red;

                foreach (var sy in syllables)
                {
                    Console.WriteLine("Silaba\n" + sy);
                }

                Console.ForegroundColor = ConsoleColor.White;

                _audioPlayerService.audioPlay(syllables);

            } while (true);
        }
    }
}
