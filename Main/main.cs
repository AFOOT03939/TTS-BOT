using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.Commands;
using NAudio.CoreAudioApi;
using TTS_BOT.Commands;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DSharpPlus.Voice;

namespace TTS_BOT.Services
{
    public class Main
    {
        private DiscordBotService _discordBotService;
        private SyllabificationService _syllabificationService;

        public Main(DiscordBotService discordBotService, SyllabificationService syllabificationService)
        {
            _discordBotService = discordBotService;
            _syllabificationService = syllabificationService;
        }


        public async Task mainAsync()
        {
            string? token = Environment.GetEnvironmentVariable("Token");
            DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(token, DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents);

            /*
            builder.ConfigureEventHandlers
            (
                b => b.HandleMessageCreated(async (s, e) =>
                {
                    if (e.Message.Content.ToLower().StartsWith("ping"))
                    {
                        await e.Message.RespondAsync("Orlando es XXXXX");
                    }
                })
            );
            */

            builder.ConfigureEventHandlers
            (
                b => b.HandleMessageCreated(async (s, e) =>
                {
                    if (SlashJoin.Connection != null)
                    {
                        var syllables = _syllabificationService.textReciever(e.Message.Content);

                        foreach(var syllable in syllables)
                        {
                            string path = $"E:\\Users\\victo\\Downloads\\audios_finale\\{syllable}.wav";

                            if (File.Exists(path) && !e.Author.IsBot)
                            {
                                await _discordBotService.reproduceAudios(SlashJoin.Connection, path);
                            }
                        }
                    }
                })
            );


            builder.UseCommands((serviceProvider, extension) =>
            {
                extension.AddCommands([typeof(SlashLeave), typeof(SlashJoin)]);
            });

            builder.UseVoice();

            DiscordClient client = builder.Build();

            await client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}


//Este servicio era para pruebas locales dentro de windows, ahora mismo ya no se utilizará debido a la implementación del bot
//sin embargo, lo voy a mantener por si las dudas

/*
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
*/