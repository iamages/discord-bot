using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using IamagesDiscordBot.Services;
using IamagesDiscordBot.Services.API;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IamagesDiscordBot.Commands
{
    public class IamagesCmds : BaseCommandModule
    {
        [Command("img"), Description("Retrieves a specified image by FileId from the Iamages API")] // to be added here
        [GroupName(Group.Iamages)]
        public async Task GetImgByID(CommandContext ctx, int FileId)
        {
            IamagesAPIWrapper api = new IamagesAPIWrapper();
            var iamage = api.getImgInfo(FileId);
            var embed = defaultImgEmbed(iamage, api);
            await ctx.Channel.SendMessageAsync(embed:embed).ConfigureAwait(false);
        }

        [Command("imgrandom"), Description("Retrieves a random unprivated image for the user")]
        [GroupName(Group.Iamages)]
        [Cooldown(2, 5, CooldownBucketType.Channel)]
        public async Task Random(CommandContext ctx)
        {
            IamagesAPIWrapper api = new IamagesAPIWrapper();
            var imgInfo = api.getRandom(); // random image info
            var embed = defaultImgEmbed(imgInfo, api);
            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("imgsearch"), Description("Searches using an inputted search tag for similar image descriptions from the API. Returns a paginated embed for easy look throughs")]
        [GroupName(Group.Iamages)]
        [Cooldown(2, 5, CooldownBucketType.User)]
        public async Task ImgSearch(CommandContext ctx, string searchTag)
        {
            var emojis = new PaginationEmojis() //emojis to be used in the pagination
            {
                Left = DiscordEmoji.FromName(ctx.Client, ":arrow_backward:"),
                Right = DiscordEmoji.FromName(ctx.Client, ":arrow_forward:"),
                SkipLeft = null,
                SkipRight = null
            };

            IamagesAPIWrapper api = new IamagesAPIWrapper();
            var response = api.postSearch(searchTag);
            if (response.FileIDs.Length != 0) // no search was found
            {
                List<Page> pages = new List<Page>(); // paginated embeds to be sent
                var interactivity = ctx.Client.GetInteractivity();
                int count = 0;
                foreach (int fileID in response.FileIDs)
                {
                    count++;
                    var imgInfo = api.getImgInfo(fileID);
                    var embed = defaultImgEmbed(imgInfo, api);
                    embed.WithTitle($"Image #{count}/{response.FileIDs.Length}");
                    var page = new Page($"Images Found! Searching for: `{response.searchTag}`\nPress :stop_button: to stop interacting", embed);
                    pages.Add(page);
                }
                await interactivity.SendPaginatedMessageAsync(ctx.Channel, ctx.User, pages, emojis);
            }
            else
            {
                await BotServices.SendEmbedAsync(ctx, "Sorry! No Iamages were found!", "Similar descriptions were either not found or something else went wrong!", ResponseType.Warning);
            }
        }

        private DiscordEmbedBuilder defaultImgEmbed(IamageModel imgInfo, IamagesAPIWrapper api)
        {
            string thumbUrl = api.getImgThumb(imgInfo.ImageID);
            string fileType = BotServices.ConvertToString(imgInfo.FileMime);
            string purity = BotServices.ConvertToString(imgInfo.FileNSFW);

            string desc =
                $"**ID** = {imgInfo.ImageID}\n" +
                $"**Description** = {imgInfo.FileDesc ?? "No description provided."}\n" +
                $"**ImageType** = {fileType}\n" +
                $"**Purity** = {purity}\n" +
                $"**Dimensions** = {imgInfo.FileWidth}px x {imgInfo.FileHeight}px\n" +
                $"**Created at** = {imgInfo.created_at.DateTime}";

            var embed = new DiscordEmbedBuilder()
                .WithAuthor("Iamages Discord Bot", null, SharedData.LogoURL)
                .WithFooter("Retrieved from Iamages API")
                .WithColor(SharedData.defaultColour)
                .WithTimestamp(DateTime.Now)
                .WithDescription(desc)
                .WithImageUrl(thumbUrl);

            return embed;
        }
    }
}
