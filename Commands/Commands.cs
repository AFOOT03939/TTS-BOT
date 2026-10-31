using DSharpPlus.Commands;
using DSharpPlus.Voice;
using TTS_BOT.Services;

namespace TTS_BOT.Commands
{
    public class SlashJoin
    {
        public static VoiceConnection? Connection;

        [Command("join")]
        public static async ValueTask join(CommandContext context)
        {
            //obtiene el estado del usuario que mandó el command
            var voiceState = context?.Member?.VoiceState;

            if(voiceState == null)
            {
                await context.RespondAsync("You must be in a voice channel");

                return;
            }

            //obtiene su guiId que creo que es el ID del servidor
            var guild = context.Guild;

            //Obtiene el canal de donde se ejecutó el comando
            var channel = await guild.GetChannelAsync(voiceState.ChannelId.Value);

            Connection = await channel.ConnectAsync();

            await context.RespondAsync("Bot connected");
        }
    }

    public class SlashLeave
    {
        [Command("leave")]
        public static async ValueTask leave(CommandContext context)
        {
            var voiceState = context?.Member?.VoiceState;

            if (voiceState == null)
            {
                await context.RespondAsync("You must be in a voice channel");

                return;
            }

            await SlashJoin.Connection.DisconnectAsync();

            await context.RespondAsync("Bot disconnected");
        }
    }
}

