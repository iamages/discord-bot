using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IamagesDiscordBot.Services;

namespace IamagesDiscordBot.Commands
{
    [Group("Utilities")]
    [Description(":tools:")] // is used for emoji lmao 
    public class UtilCmds : BaseCommandModule
    {
        [Command("ping"), Description("Check ping to server")]
        public async Task Ping(CommandContext ctx)
        {
            var msg = await ctx.Channel.SendMessageAsync("Pong! : measuring...").ConfigureAwait(false);
            await msg.ModifyAsync($"Pong! : {ctx.Client.Ping} ms").ConfigureAwait(false);
        }

        [Command("uptime"), Description("Returns Bot's Current Uptime")]
        public async Task Uptime(CommandContext ctx)
        {
            var timespan = DateTime.Now - SharedData.startTime;
            string desc = $"**{timespan.Days}** days, **{timespan.Hours}** hours, **{timespan.Minutes}** minutes, and **{timespan.Seconds}** seconds.";
            var shardEmote = DiscordEmoji.FromName(ctx.Client, ":gem:"); // might need to set used emojis into a list in shareddata
            string footer = $"Ping {ctx.Client.Ping} ms {shardEmote}";

            var output = new DiscordEmbedBuilder()
            {
                Title = "Uptime",
                Description = desc,
                Color = SharedData.defaultColour
            }.WithFooter(footer); // cos footer property != string

            await ctx.RespondAsync(embed: output).ConfigureAwait(false);
        }

        [Command("purge"), Description("Deletes the Specified amount of Messages in the channel [Not including cmd msg]")] //might move to moderation commands
        public async Task purge(CommandContext ctx, int amount)
        {
            IEnumerable<DiscordMessage> msgsToDel = await ctx.Channel.GetMessagesAsync(amount + 1);
            await ctx.Channel.DeleteMessagesAsync(msgsToDel).ConfigureAwait(false);
        }
    }
}
