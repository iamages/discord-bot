using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IamagesDiscordBot.Commands
{
    public class UtilCmds : BaseCommandModule
    {
        [Command("ping"), Description("Check ping to server")]
        public async Task Ping(CommandContext ctx)
        {
            var msg = await ctx.Channel.SendMessageAsync("Pong! : measuring...").ConfigureAwait(false);
            await msg.ModifyAsync($"Pong! : {ctx.Client.Ping}").ConfigureAwait(false);
        }
    }
}
