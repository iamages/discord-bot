using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IamagesDiscordBot.Commands
{
    [Group("Iamages")]
    [Description(":camera:")]
    public class IamagesCmds : BaseCommandModule
    {
        [Command("ping"), Description("Check ping to server")] // to be added here
        public async Task Ping(CommandContext ctx)
        {
            var msg = await ctx.Channel.SendMessageAsync("Pong! : measuring...").ConfigureAwait(false);
            await msg.ModifyAsync($"Pong! : {ctx.Client.Ping} ms").ConfigureAwait(false);
        }
    }
}
