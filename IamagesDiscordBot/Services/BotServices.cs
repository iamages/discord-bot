using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using IamagesDiscordBot.Services.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace IamagesDiscordBot.Services
{
    public class BotServices // all the interclass properties and methods
    {
        //Fast SendEmbedAsync
        public static async Task SendEmbedAsync(CommandContext ctx,[Description("Title of Embed to be sent")] string title,[Description("Message to be sent on the Embed")] string desc,[Description("ResponseType to determine ErrorCode")] ResponseType type = ResponseType.Default)
        {
            var titleEmote = type switch
            {
                ResponseType.Warning => DiscordEmoji.FromName(ctx.Client, ":exclamation:"),
                ResponseType.Error => DiscordEmoji.FromName(ctx.Client, ":mag:"),
                ResponseType.Missing => DiscordEmoji.FromName(ctx.Client, ":no_entry:"),
                _ => null
            };
            var ErrorColour = type switch
            {
                ResponseType.Default => SharedData.defaultColour,
                ResponseType.Warning => new DiscordColor("#ffcc00"), //orange-ish warning colour
                ResponseType.Error => new DiscordColor("#cc3300"), //red error colour
                ResponseType.Missing => new DiscordColor("#999999"), //gray missing colour
                _ => SharedData.defaultColour
            };

            var embed = new DiscordEmbedBuilder()
                .WithTitle(title + " " + titleEmote)
                .WithDescription(desc)
                .WithFooter($"i!help for more Info", SharedData.LogoURL)
                .WithTimestamp(DateTime.Now)
                .WithColor(ErrorColour);

            var msg = await ctx.Channel.SendMessageAsync(embed: embed)
                .ConfigureAwait(false);
            await Task.Delay(10000);
            await msg.DeleteAsync().ConfigureAwait(false);
        }

        //RemoveMsg
        public static async Task RemoveMsg(DiscordMessage msg)
        {
            await msg.DeleteAsync().ConfigureAwait(false);
        }

        //waitForMessage from user
        public static async Task<InteractivityResult<DiscordMessage>> WaitForMessage(CommandContext ctx, string keyword, int secondsToWait)
        {
            return await ctx.Client.GetInteractivity().WaitForMessageAsync(m => m.Author == ctx.User && m.Channel.Id == ctx.Channel.Id && string.Equals(m.Content, keyword, StringComparison.InvariantCultureIgnoreCase),
                TimeSpan.FromSeconds(secondsToWait)
                ).ConfigureAwait(false);
        }

        public static string ConvertToString(FileMime mime)
        {
            var mimestring = mime switch
            {
                FileMime.ImageBMP => "BMP",
                FileMime.ImageGIF => "GIF",
                FileMime.ImageJpeg => "JPEG",
                FileMime.ImagePNG => "PNG",
                _ => "No type specified."
            };
            return mimestring;
        }

        public static string ConvertToString(bool purity)
        {
            var mimestring = purity switch
            {
                true => "NSFW",
                false => "SFW"
            };
            return mimestring;
        }


        //CheckUserInput

    }

    //group module for Help Embed
    public class cmdGroup
    {
        public string GroupName;
        public List<Command> cmdList;
        public string emoji;

        public cmdGroup(string name, string emoji)
        {
            this.emoji = emoji;
            this.GroupName = name;
            this.cmdList = new List<Command>();   
        }
    }

    //GroupAttribute for Help Embed
    [AttributeUsage(AttributeTargets.Method)]
    public class GroupNameAttribute : Attribute
    {
        public string Name;
        public string emoji;
        
        public GroupNameAttribute(Group group)
        {
            this.Name = group switch
            {
                Group.Utilities => "Utilities",
                Group.Iamages => "Iamages",
                Group.Fun => "Fun"
            };
            this.emoji = group switch
            {
                Group.Utilities => ":tools:",
                Group.Iamages => ":camera:",
                Group.Fun => ":game_die:"
            };
        }
    }
}
