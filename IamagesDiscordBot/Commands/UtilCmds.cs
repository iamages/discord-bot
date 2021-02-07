using DSharpPlus.CommandsNext;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using IamagesDiscordBot.Services;
using System;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Extensions;

namespace IamagesDiscordBot.Commands
{
    public class UtilCmds : BaseCommandModule
    {
        [Command("ping"), Description("Check ping to server")]
        [GroupName(Group.Utilities)]
        public async Task Ping(CommandContext ctx)
        {
            var msg = await ctx.Channel.SendMessageAsync("Pong! : measuring...").ConfigureAwait(false);
            await msg.ModifyAsync($"Pong! : {ctx.Client.Ping} ms").ConfigureAwait(false);
        }

        [Command("uptime"), Description("Returns Bot's Current Uptime")]
        [GroupName(Group.Utilities)]
        public async Task Uptime(CommandContext ctx)
        {
            var timespan = DateTime.Now - SharedData.startTime;
            string desc = $"**{timespan.Days}** days, **{timespan.Hours}** hours, **{timespan.Minutes}** minutes, and **{timespan.Seconds}** seconds.";
            var shardEmote = DiscordEmoji.FromName(ctx.Client, ":gem:"); // might need to set used emojis into a list in shareddata
            string footer = $"Ping {ctx.Client.Ping} ms {shardEmote}";

            var output = new DiscordEmbedBuilder()
            {
                Title = "Uptime 🚀",
                Description = desc,
                Color = SharedData.defaultColour
            }.WithFooter(footer); // cos footer property != string

            await ctx.RespondAsync(embed: output).ConfigureAwait(false);
        }

        [Command("purge"), Description("Deletes the Specified amount of Messages in the channel\n[Not including cmd msg]")] //might move to moderation commands
        [RequireBotPermissions(Permissions.ManageMessages)]
        [GroupName(Group.Utilities)]
        public async Task purge(CommandContext ctx,[Description("Number of messages to be deleted")] int amount)
        {
             var msgsToDel = await ctx.Channel.GetMessagesAsync(amount + 1);
            await ctx.Channel.DeleteMessagesAsync(msgsToDel).ConfigureAwait(false);  
        }

        [Command("report"), Description("Report a problem to the bot developer. Please do not abuse")]
        [GroupName(Group.Utilities)]
        public async Task Report(CommandContext ctx, [RemainingText, Description("Message to be sent")] string report = null)
        {
            if (string.IsNullOrWhiteSpace(report) || report.Length < 25) // checks it is not abuse nor empty report
            {
                await BotServices.SendEmbedAsync(ctx,
                    "Report Length Error",
                    "Report needs to be longer or not empty. Please do not abuse.",
                    ResponseType.Warning).ConfigureAwait(false);
                return;
            }
            var InfoMsg = await ctx.RespondAsync("The following information are to be recovered: `User ID, Server ID, Server Name and Server Owner Name`\nPlease Also Enable DMs from Server Members.").ConfigureAwait(false); //compliance msg
            await Task.Delay(500);
            var msg = await ctx.RespondAsync("Please type `yes` to comply with the above.").ConfigureAwait(false); //agreement msg
            var msgAnswer = await BotServices.WaitForMessage(ctx, "yes", 45).ConfigureAwait(false); //interactive to wait for agreement
            if (msgAnswer.Result is null) // did not agree or timed our
            {
                await InfoMsg.DeleteAsync().ConfigureAwait(false);
                await msg.ModifyAsync($"~~{msg.Content}~~ Request Timed Out").ConfigureAwait(false);
            }
            else // did agree to the msg
            {
                var tick = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
                await msg.CreateReactionAsync(tick).ConfigureAwait(false);
                await InfoMsg.DeleteAsync().ConfigureAwait(false);
                // creating report:
                var dmToRequester = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);
                string ReportID = generateID();
                var output = new DiscordEmbedBuilder() //build report embed
                    .WithAuthor(ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator,
                        iconUrl: ctx.User.AvatarUrl ?? ctx.User.DefaultAvatarUrl)
                    .AddField($"**# Issue {ReportID}**", Formatter.BlockCode("<Report: " + report + ">", "xml"))
                    .AddField("**# Sent By**", ctx.User.Username + "#" + ctx.User.Discriminator, true)                    
                    .AddField("**# Server**", ctx.Guild.Name + $" (ID: {ctx.Guild.Id})", true)
                    .AddField("**# Owner**", ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator, true)
                    .AddField("**# Github**", $"[Report New Issue on Github]({SharedData.GithubLink}/discord-bot/issues/new)")
                    .WithFooter($"Official Iamages Discord Bot", SharedData.LogoURL)
                    .WithTimestamp(DateTime.Now)
                    .WithColor(SharedData.defaultColour);
                var sentReport = await dmToRequester.SendMessageAsync("Please Confirm the below output", embed: output).ConfigureAwait(false);
                await sentReport.CreateReactionAsync(tick).ConfigureAwait(false);
                var embedAgreed = await ctx.Client.GetInteractivity().WaitForReactionAsync(r => r.Message == sentReport, TimeSpan.FromMinutes(1)).ConfigureAwait(false); //wait compliance from member
                await Task.Delay(1000);
                if (embedAgreed.Result is null)
                {
                    await dmToRequester.SendMessageAsync("Something went wrong, please redo the command").ConfigureAwait(false);
                }
                else 
                {
                    if (SharedData.reportChannel != null)
                    { 
                        await SharedData.reportChannel.SendMessageAsync($"@everyone New Report Just In! (**Issue {ReportID}**)",mentions: Mentions.All,embed: output).ConfigureAwait(false);
                        await dmToRequester.SendMessageAsync("Report Sent to Dev!").ConfigureAwait(false);
                    } else { 
                    await BotServices.SendEmbedAsync(ctx, 
                        "Something went wrong",
                        $"Please contact us through other means: [Discord]({SharedData.serverInv}) Bot Developer: `profiesser toaster.#3125`", ResponseType.Missing).ConfigureAwait(false);
                     }
                }
            }
        }
        [Command("setdev"), Description("Sets the bot Dev account to receive user reports"), Hidden]
        public async Task SetDev(CommandContext ctx)
        {
            SharedData.reportChannel = ctx.Channel;
            await ctx.RespondAsync("This channel has been set to receive user reports!").ConfigureAwait(false);
        }
        private string generateID()
        { return Guid.NewGuid().ToString("N").Substring(0, 6); }
    }
}
