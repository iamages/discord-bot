using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using IamagesDiscordBot.Services;
using IamagesDiscordBot.Services.API;
using System;
using System.Threading.Tasks;

namespace IamagesDiscordBot.Commands
{
    public class IamagesCmds : BaseCommandModule
    {
        [Command("iping"), Description("Check ping to server")] // to be added here
        [GroupName(Group.Iamages)]
        public async Task Ping(CommandContext ctx)
        {
            var msg = await ctx.Channel.SendMessageAsync("Pong! : measuring...").ConfigureAwait(false);
            await msg.ModifyAsync($"Pong! : {ctx.Client.Ping} ms").ConfigureAwait(false);
        }

        [Command("imgrandom"), Description("Retrieves an unprivated image for the user")]
        [GroupName(Group.Iamages)]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Random(CommandContext ctx)
        {
            IamagesAPIWrapper api = new IamagesAPIWrapper();
            var imgInfo = api.getRandom(); // random image info
            string imgUrl = api.getImage(imgInfo.ImageID); //the image

            string fileType = BotServices.ConvertToString(imgInfo.FileMime);
            string purity = BotServices.ConvertToString(imgInfo.FileNSFW);

            string desc =
                $"**ID** = {imgInfo.ImageID}\n" +
                $"**Description** = {imgInfo.FileDesc ?? "No description provided."}\n" +
                $"**ImageType** = {fileType}\n" +
                $"**Purity** = {purity}\n" +
                $"**Dimensions** = {imgInfo.FileWidth} x {imgInfo.FileHeight}\n" +
                $"**Created at** = {imgInfo.created_at}";

            var embed = new DiscordEmbedBuilder()
                .WithAuthor("Iamages Discord Bot", null, SharedData.LogoURL)
                .WithFooter("Retrieved from Iamages API")
                .WithColor(SharedData.defaultColour)
                .WithTimestamp(DateTime.Now)
                .WithDescription(desc)
                .WithImageUrl(imgUrl);

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }
    }
}
