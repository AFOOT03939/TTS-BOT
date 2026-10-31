using Microsoft.Extensions.DependencyInjection;
using DotNetEnv;
using Discord.LibDave;
using TTS_BOT.Services;
using TTS_BOT.Commands;

Env.Load("E:\\Users\\victo\\source\\repos\\TTS BOT\\TTS BOT\\.env");

if (Dave.CheckAvailability())
    Dave.SetLogSink(static (_, _, _, _) => { });

var services = new ServiceCollection();

services.AddTransient<SyllabificationService>();
//services.AddTransient<Connect>();
//services.AddTransient<AudioPlayerService>();
services.AddTransient<Main>();

services.AddSingleton<DiscordBotService>();

using var provider = services.BuildServiceProvider();

var bot = provider.GetRequiredService<Main>();
await bot.mainAsync();