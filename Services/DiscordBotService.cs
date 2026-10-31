
using DSharpPlus.Voice;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.Diagnostics;
using TTS_BOT.Commands;

namespace TTS_BOT.Services
{
    public class DiscordBotService
    {
        public async Task reproduceAudios(VoiceConnection connection, string path)
        {
            //Crea el escritor de audio
            var writer = SlashJoin.Connection.CreateAudioWriter(AudioFormat.S16LE48KHzStereoPCM);

            //Abre el archivo
            using var reader = new WaveFileReader(path);

            //Comprueba que el formato sea compatible
            if (reader.WaveFormat.SampleRate != 48000 ||
                reader.WaveFormat.Channels != 2 ||
                reader.WaveFormat.BitsPerSample != 16)
            {
                throw new InvalidOperationException(
                    "El WAV debe ser PCM 48 kHz, estéreo y 16 bits."
                );
            }

            //Obtiene un stream para enviar el audio a discord
            using var output = writer.AsStream(leaveOpen: true);

            //Copia el audio al canal
            await reader.CopyToAsync(output);

            //Indicar que terminó el audio por ahora
            writer.SignalSilence();
        }

    }
}



/*

using Discord;
using Discord.Audio;
using Discord.Net;
using Discord.WebSocket;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TTS_BOT.Services
{
    public class DiscordBotService
    {
        private static DiscordSocketClient _client;
        private IAudioClient? _audioClient;

        private readonly SyllabificationService _syllabificationService;

        public DiscordBotService(SyllabificationService syllabificationService)
        {
            _syllabificationService = syllabificationService;
        }

        public async Task main()
        {
            var config = new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.DirectMessages | GatewayIntents.GuildVoiceStates | GatewayIntents.MessageContent, EnableVoiceDaveEncryption = true
            };

            _client = new DiscordSocketClient(config);
            _client.Log += Log;

            _client.MessageReceived += messageHandler;
            _client.Ready += clientReady;
            _client.SlashCommandExecuted += slashCommandHandlerStartup;

            var token = Environment.GetEnvironmentVariable("Token");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        public async Task clientReady()
        {
            //código copiado de discord

            // Let's do our global command
            var join = new SlashCommandBuilder();
            join.WithName("join");
            join.WithDescription("adds the bot to the voice channel");

            var leave = new SlashCommandBuilder();
            leave.WithName("leave");
            leave.WithDescription("disconnects the bot from the voice channel");

            try
            {
                // With global commands we don't need the guild.
                await _client.CreateGlobalApplicationCommandAsync(join.Build());
                await _client.CreateGlobalApplicationCommandAsync(leave.Build());
                // Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
                // For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
            }
            catch (HttpException exception)
            {
                // If our command was invalid, we should catch an HttpException. This exception contains the Discord error code, the request object that was sent, the reason of the exception and a list of of errors to explain what went wrong with the request. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }

        public Task messageHandler(SocketMessage message)
        {
            if (message.Author.IsBot || _audioClient == null)
                return Task.CompletedTask;

            if (string.IsNullOrWhiteSpace(message.Content))
                return Task.CompletedTask;

            List<string> syllables = _syllabificationService.textReciever(message.Content);

            // Ejecuta la reproducción sin bloquear el handler del Gateway.
            _ = Task.Run(async () =>
            {
                try
                {
                    await reproduceAudios(_audioClient, syllables);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reproduciendo audio: {ex}");
                }
            });

            return Task.CompletedTask;
        }

        private Task slashCommandHandlerStartup(SocketSlashCommand command)
        {
            _ = slashCommandHandler(command);
            return Task.CompletedTask;
        }

        private async Task slashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "join":
                    await handleJoin(command);
                    break;
                case "leave":
                    await handleLeave(command);
                    break;
            }
        }

        private async Task handleJoin(SocketSlashCommand command)
        {
            var user = command.User as SocketGuildUser;

            if (user?.VoiceChannel == null)
            {
                await command.RespondAsync(
                    "Primero tienes que entrar a un canal de voz.",
                    ephemeral: true
                );
                return;
            }

            await command.DeferAsync();
            var voiceChannel = user.VoiceChannel;

            _audioClient = await voiceChannel.ConnectAsync();

            await command.ModifyOriginalResponseAsync(properties => properties.Content = "Bienvenido");
        }

        private async Task handleLeave(SocketSlashCommand command)
        {
            var user = command.User as SocketGuildUser;

            if (user?.VoiceChannel == null)
            {
                await command.RespondAsync("Adiós",ephemeral: true);
                return;
            }

            await command.DeferAsync();
            var voiceChannel = user.VoiceChannel;

            await voiceChannel.DisconnectAsync();

            await command.ModifyOriginalResponseAsync(properties =>
                properties.Content = "Adiós"
            );
        }



        private async Task reproduceAudios(IAudioClient audioClient, List<string> syllables)
        {
            using var pcmStream = audioClient.CreatePCMStream(
                AudioApplication.Mixed,
                bufferMillis: 200
            );

            try
            {
                foreach (var syllable in syllables)
                {
                    string filePath = $@"E:\Users\victo\Downloads\audios_finale\{syllable}.wav";

                    if (!File.Exists(filePath))
                    {
                        continue;
                    }

                    Console.WriteLine("Iniciando reproducción...");
                    using var reader = new WaveFileReader(filePath);

                    var buffer = new byte[3840];
                    int bytesRead;

                    while ((bytesRead = await reader.ReadAsync(
                        buffer, 0, buffer.Length)) > 0)
                    {
                        await pcmStream.WriteAsync(buffer, 0, bytesRead);
                    }

                    Console.WriteLine("Reproducción finalizada.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error de transmisión: {ex}");
            }
            finally
            {
                Console.WriteLine("Antes de FlushAsync");

                await pcmStream.FlushAsync();

                Console.WriteLine("Después de FlushAsync");
            }
        }



        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
*/