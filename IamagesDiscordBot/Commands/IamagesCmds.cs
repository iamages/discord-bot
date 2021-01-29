using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using IamagesDiscordBot.Services;
using System.Threading.Tasks;

namespace IamagesDiscordBot.Commands
{
    public class IamagesCmds : BaseCommandModule
    {
        [Command("iping"), Description("Check ping to server")] // to be added here
        [GroupName("Iamages", ":camera:")]
        public async Task Ping(CommandContext ctx)
        {
            var msg = await ctx.Channel.SendMessageAsync("Pong! : measuring...").ConfigureAwait(false);
            await msg.ModifyAsync($"Pong! : {ctx.Client.Ping} ms").ConfigureAwait(false);
        }
    }
}
