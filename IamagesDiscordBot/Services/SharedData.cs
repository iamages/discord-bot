using DSharpPlus.Entities;
using System;
using System.Collections.Generic;

namespace IamagesDiscordBot.Services
{
    public class SharedData
    {
        //Just a class for Global Interclass Data that we might need
        public static DateTime startTime; 
        public static DiscordColor defaultColour = new DiscordColor("#F57C00");
        public static string GithubLink = "https://github.com/iamages";
        public static List<string> prefixes = new List<string>();
        public static string serverInv = "https://discord.gg/hGwGkZsuXB";
        public static string APIdocs = "https://iamages.uber.space/iamages/api/";
        public static string LogoURL = "https://iamages.uber.space/iamages/api/img/20";
        public static string mailto = "ybmirz.freelance@gmail.com";
        public static DiscordChannel reportChannel;
        public static ulong reportChannelid = 797519599603548202;
    }

    public enum ResponseType
    {
        Default,
        Warning,
        Missing,
        Error
    }

    public enum Group// group enums to make attributing prettier
    {
        Utilities,
        Iamages,
        Fun // to be
    }

}
