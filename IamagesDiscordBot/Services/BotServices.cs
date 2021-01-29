using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;

namespace IamagesDiscordBot.Services
{
    public class BotServices // all the interclass properties and methods
    {
        //SendEmbedAsync

        //RemoveMsg

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
        
        public GroupNameAttribute(string name, string emote)
        {
            this.Name = name;
            this.emoji = emote;
            
        }
    }
}
