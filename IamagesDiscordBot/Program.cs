using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using IamagesDiscordBot.Commands;
using IamagesDiscordBot.Services;
using IamagesDiscordBot.Services.Handlers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace IamagesDiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {
            //here will be where we create a log text file everytime bot turns on, with date and time of starting. //might need to create a log writer class
            string token = string.Join("", args); //create into one string
            if (token == null || token == string.Empty)
            {
                Console.WriteLine("Input code here:");
                token = Console.ReadLine();
            }
            var bot = new Bot();
            bot.RunAsync(token).GetAwaiter().GetResult();
            //text file closing 
        }
    }

    // need to remember this fricking is the latest stable release not the nightly
    public class Bot
    {
        public readonly EventId BotEventId = new EventId(21, "IamagesBot");
        public DiscordClient _Client { get; private set; }
        public CommandsNextExtension _Commands { get; private set; }
        public InteractivityExtension _Interactivity { get; private set; } // not yet set for now

        public async Task RunAsync(string token)
        {

            var config = new DiscordConfiguration
            {
                Token = token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug
            };

            _Client = new DiscordClient(config);
            // every client ready, guild available and client error
            _Client.Ready += Client_OnReady;
            _Client.GuildAvailable += Client_GuildConnected;
            _Client.ClientErrored += Client_ClientError;
            

            SharedData.prefixes.Add("i!"); //default prefix
            //might wanna add a interactivity here along with its config
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = SharedData.prefixes, // for now it is default prefix can turn this to a list 
                EnableDms = true, // so botDev can set himself as dev lmao
                EnableMentionPrefix = false,
                EnableDefaultHelp = true, 
                DmHelp = false
            };

            _Commands = _Client.UseCommandsNext(commandsConfig);
            _Commands.CommandExecuted += Command_CommandExecuted;
            _Commands.CommandErrored += Command_CommandError;

             
            _Commands.RegisterCommands<IamagesCmds>();
            _Commands.RegisterCommands<UtilCmds>();
            _Commands.SetHelpFormatter<HelpFormatter>();

            var emojis = new PaginationEmojis() //emojis to be used in the pagination
            {
                Left = DiscordEmoji.FromName(_Client, ":arrow_backward:"),
                Right = DiscordEmoji.FromName(_Client, ":arrow_forward:"),
                SkipLeft = null,
                SkipRight = null
            };

            _Interactivity = _Client.UseInteractivity (new InteractivityConfiguration
            {
                PaginationBehaviour = DSharpPlus.Interactivity.Enums.PaginationBehaviour.WrapAround,
                PaginationEmojis = emojis,
               Timeout = TimeSpan.FromSeconds(60) //increase timeout
            });

            SharedData.reportChannel = await _Client.GetChannelAsync(SharedData.reportChannelid);

            SharedData.startTime = DateTime.Now;
            //client connection to bot application on the discord api
            await _Client.ConnectAsync();

            await Task.Delay(-1); // <------ needs to re looked upon
        }
        //disconnect method when key is pressed,



        //logging stuff onto the console line (for all of these might want to log into a text file if needed)
        private Task Client_OnReady(DiscordClient sender, ReadyEventArgs e)
        {
            sender.Logger.LogInformation(BotEventId, "Iamages Bot is Alive and Ready");
            return Task.CompletedTask;
        }
        private Task Client_GuildConnected(DiscordClient sender, GuildCreateEventArgs e)
        {
            sender.Logger.LogInformation(BotEventId, $"Iamages Bot is now connected to {e.Guild.Name}({e.Guild.Id})");
            //read the prefixes json and then read (not needed right now, as only one main prefix at hand)
            return Task.CompletedTask;
        }
        private Task Client_ClientError(DiscordClient sender, ClientErrorEventArgs e)
        {
            sender.Logger.LogError(BotEventId, $"Oh no, [Type: {e.Exception.GetType()}] [{e.Exception.Message}]");
            return Task.CompletedTask;
        }
        private Task Command_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
        {
            sender.Client.Logger.LogInformation(BotEventId, message: $"{e.Context.User} has executed \"{e.Command.Name}\" cmd with message {e.Context.Message} [{e.Context.Channel.Name} ({e.Context.Channel.Id})]"); //for now, not yet implement guilds in the log
            return Task.CompletedTask;
        }
        private async Task Command_CommandError(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            await ErrorHandlers.Process(e, BotEventId);
            //default logging to console below:
            sender.Client.Logger.LogWarning(BotEventId, $"{e.Command.Name} Command Error: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"} by [{e.Context.User}] in [{e.Context.Channel.Name} ({e.Context.Channel.Id})] "); //changes from time to time
        }

        // code underneath is basically for json properties (unneeded for now)
        //public struct ConfigJson
        //{
        //    [JsonProperty("token")]
        //    public string Token { get; private set; }
        //}
        //public class Prefix
        //{
        //    public long GuildID { get; set; }
        //    public string prefix { get; set; }
        //}

        //public class Root
        //{
        //    public List<Prefix> prefixes { get; set; }
        //}

    }
}
