using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using IamagesDiscordBot.Services;
using System;
using System.Threading.Tasks;

namespace IamagesDiscordBot.Commands
{
    public class UtilCmds : BaseCommandModule
    {
        [Command("ping"), Description("Check ping to server")]
        [GroupName("Utilities", ":tools:")]
        public async Task Ping(CommandContext ctx)
        {
            var msg = await ctx.Channel.SendMessageAsync("Pong! : measuring...").ConfigureAwait(false);
            await msg.ModifyAsync($"Pong! : {ctx.Client.Ping} ms").ConfigureAwait(false);
        }

        [Command("uptime"), Description("Returns Bot's Current Uptime")]
        [GroupName("Utilities", ":tools:")]
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

        [Command("purge"), Description("Deletes the Specified amount of Messages in the channel\n[Not including cmd msg]")] //might move to moderation commands
        [GroupName("Utilities", ":tools:")]
        public async Task purge(CommandContext ctx, int amount)
        {
            try
            {
                var msgsToDel = await ctx.Channel.GetMessagesAsync(amount + 1);
                await ctx.Channel.DeleteMessagesAsync(msgsToDel).ConfigureAwait(false);
            }
            catch (Exception e)
            { await ctx.RespondAsync("This will need the following permissions:\n**Manage Messages**\n**Read Messages**\n**Send Messages**\n").ConfigureAwait(false); } //for now,, will have full error responding
        }
    }
}
