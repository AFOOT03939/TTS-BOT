using Microsoft.Extensions.DependencyInjection;
using TTS_BOT.NewFolder;
using TTS_BOT.Services;

var services = new ServiceCollection();

services.AddTransient<SyllabificationService>();
services.AddTransient<AudioPlayerService>();
services.AddTransient<main>();

using var provider = services.BuildServiceProvider();

var app = provider.GetRequiredService<main>();
app.writeText();