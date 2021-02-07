using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IamagesDiscordBot.Services
{
    public sealed class HelpFormatter : BaseHelpFormatter
    {        
        private readonly DiscordEmbedBuilder _output;
        public HelpFormatter(CommandContext ctx) : base(ctx)
        {
            _output = new DiscordEmbedBuilder()
                .WithColor(SharedData.defaultColour)
                .WithAuthor("Command List", null, SharedData.LogoURL)
                .WithTimestamp(DateTime.Now)
                .WithFooter("Official Iamages Discord Bot");
        }

        public override CommandHelpMessage Build()
        {
            var desc = Formatter.Bold("i!help [cmd] For More Information\nLinks:") + $"\n[Discord]({SharedData.serverInv}) | [Github]({SharedData.GithubLink}) | [API Docs]({SharedData.APIdocs})";
            _output.WithDescription(desc);
            return new CommandHelpMessage(embed: _output);
        }

        //first method it will look to i!help <cmd> // might want to make a non embed help for this
        public override BaseHelpFormatter WithCommand(Command cmd)
        {
            _output.ClearFields();
            // for now it is static Help per Command but will add a more sophisticated command help with examples of usage and aliases
            bool argExist = false;
            foreach (var overload in cmd.Overloads)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"i!{cmd.Name}"); // default just a name
                if (overload.Arguments.Count >= 1)
                {
                    argExist = true;
                    sb.Append(" " + string.Join(" ", overload.Arguments.Select(xarg => xarg.IsOptional ? $"<{xarg.Name}>" : $"[{xarg.Name}]")));
                }
                _output.AddField(Formatter.Bold("# Usage"), Formatter.InlineCode(sb.ToString()));
            }

            _output.AddField(Formatter.Bold("# Description"), Formatter.InlineCode(cmd.Description));

            if (argExist)
                _output.AddField("Others", Formatter.BlockCode("// Remove brackets when typing commands\n// [] = Needed Arguments\n// <> = Optional Arguments", "cs"));

            if (cmd.Aliases?.Any() ?? false) //needs changing
                _output.AddField("Aliases", string.Join(", ", cmd.Aliases.Select(Formatter.InlineCode)), true);

            return this;
        }

        //second method it looks to (used as main help formatter) i!help
        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            List<cmdGroup> module = new List<cmdGroup>();
            var enumerable = subcommands.ToList();
            if (enumerable.Any())
            {
                foreach (var cmd in enumerable)
                {
                    if (cmd.Name != "help")
                    {
                        // getting the cmd's group through their attributes
                        string groupName = string.Empty;
                        string groupEmoji = string.Empty;
                        foreach (var attr in cmd.CustomAttributes)
                        {
                            if (attr is GroupNameAttribute)
                            {
                                GroupNameAttribute a = (GroupNameAttribute)attr;
                                groupName = a.Name;
                                groupEmoji = a.emoji;
                            }
                        }
                        bool exists = false;
                        foreach (var group in module)
                        {
                            if (group.GroupName == groupName)
                                exists = true;
                        }

                        // put outside to ensure it is one once per cmd
                        if (exists) //gets that a field already exists
                        {
                            module.Find(x => x.GroupName == groupName).cmdList.Add(cmd); //finds the groupname by name, and then adds the cmd
                        }
                        else // else it makes a new group and adds in the main module (filled with groups)
                        {
                            cmdGroup group = new cmdGroup(groupName, groupEmoji);
                            group.cmdList.Add(cmd);
                            module.Add(group);
                        }
                    }
                }
            }
            //where we output all the groups
            foreach (var group in module)
            {
                _output.AddField(group.emoji + " " + Formatter.Bold(group.GroupName), string.Join(" ", group.cmdList.Select(c => Formatter.InlineCode(c.Name))));
            }
            return this;
        }
    }
}
