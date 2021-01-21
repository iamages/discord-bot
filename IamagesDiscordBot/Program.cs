﻿using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using IamagesDiscordBot.Services;
using IamagesDiscordBot.Commands;
using DSharpPlus.EventArgs;

namespace IamagesDiscordBot
{
    class Program
    {
        static void Main(string[] args)
        {
            //here will be where we create a log text file everytime bot turns on, with date and time of starting. //might need to create a log writer class
            var bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
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

        public async Task RunAsync()
        {
            //setting up json reading for token
            //var _tokenjson = string.Empty;
            //using (var fs = File.OpenRead("token.json"))
            //using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            //    _tokenjson = await sr.ReadToEndAsync().ConfigureAwait(false);
            //var tokenJson = JsonConvert.DeserializeObject<ConfigJson>(_tokenjson);
            Console.WriteLine("Input code here:");
            var token = Console.ReadLine();

            //setting up json reading for prefixes
            //var _prefixjson = string.Empty;
            //using (var fs = File.OpenRead("prefix.json"))
            //using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
            //    _prefixjson = await sr.ReadToEndAsync().ConfigureAwait(false);
            //var prefixJson = JsonConvert.DeserializeObject<Root>(_prefixjson);

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

            BotServices.prefixes.Add("i!"); //default prefix
            //might wanna add a interactivity here along with its config
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = BotServices.prefixes, // for now it is default prefix can turn this to a list 
                EnableDms = false,
                EnableMentionPrefix = false,
                EnableDefaultHelp = true, //need to add custom help here
                DmHelp = false
            };

            _Commands = _Client.UseCommandsNext(commandsConfig);
            _Commands.CommandExecuted += Command_CommandExecuted;
            _Commands.CommandErrored += Command_CommandError;

            _Commands.RegisterCommands<UtilCmds>();


            //client connection to bot application on the discord api
            await _Client.ConnectAsync();
            await Task.Delay(-1);
        }


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
            sender.Client.Logger.LogInformation(BotEventId, $"{e.Context.User} has executed \"{e.Command.Name}\" cmd with message {e.Context.Message} [{e.Context.Guild.Name}({e.Context.Guild.Id})/{e.Context.Channel}]");
            return Task.CompletedTask;
        }
        private Task Command_CommandError(CommandsNextExtension sender, CommandErrorEventArgs e)
        {
            sender.Client.Logger.LogError(BotEventId, $"{e.Command.Name} Command Error: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"} by [{e.Context.Member}] in [{e.Context.Guild.Name}({e.Context.Guild.Id})/{e.Context.Channel}] "); //might need changing as well
            return Task.CompletedTask;
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