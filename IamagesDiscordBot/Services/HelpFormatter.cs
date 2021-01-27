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
        private string _title = "Help"; 
        private string _cmdHelpTitle;

        public HelpFormatter(CommandContext ctx) : base(ctx)
        {
            _output = new DiscordEmbedBuilder()
                .WithColor(SharedData.defaultColour)
                .WithTimestamp(DateTime.Now);
        }

        public override CommandHelpMessage Build()
        {
            var desc = $"[Discord]({SharedData.serverInv}) [Github]({SharedData.GithubLink}) [API Docs]({SharedData.APIdocs})";
            _output.WithTitle(_title);
            _output.WithDescription(desc);
            return new CommandHelpMessage(embed: _output);
        }

        //first method it will look to
        public override BaseHelpFormatter WithCommand(Command cmd)
        {
            _cmdHelpTitle = $"{cmd.Name.Substring(0,1).ToUpper()}{cmd.Name.Substring(1)} Help {cmd.Description}";
            _output.ClearFields();

            if (cmd is CommandGroup) // check which help is chosen for the main title
                _title = _cmdHelpTitle;

            
            if (cmd.Aliases?.Any() ?? false)
                _output.AddField("Aliases", string.Join(", ", cmd.Aliases.Select(Formatter.InlineCode)), true);

            return this;
        }

        //second method it looks to (used as main help formatter)
        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {      
            var enumerable = subcommands.ToList();
            if (enumerable.Any())
                foreach (var cmd in enumerable)
                {
                    if (cmd.Name != "help")
                    {
                        if (cmd is CommandGroup)
                            _output.AddField(cmd.Description + " " + Formatter.Bold(cmd.Name), Formatter.InlineCode($"do i!help {cmd.Name} for more info"));
                        else
                        {
                            foreach (var overload in cmd.Overloads)
                            {
                                StringBuilder sb = new StringBuilder();
                                sb.Append($"i!{cmd.Name}"); // default just a name
                                if (overload.Arguments.Count >= 1)
                                {
                                    sb.Append(" " + string.Join(" ", overload.Arguments.Select(xarg => xarg.IsOptional ? $"<{xarg.Name}>" : $"[{xarg.Name}]")));
                                }                                
                                _output.AddField(Formatter.Bold(sb.ToString()), Formatter.InlineCode(cmd.Description));                              
                            }
                            
                        }
                    }
                }
            _output.WithFooter("Official Iamages Discord Bot");
            return this;
        }
    }
}
