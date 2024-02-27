using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using fafikspace.helping;
namespace fafikspace.commands
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        private Helping sup = new();

        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("pong");
        }
        
        [Command("help")]
        public async Task Help()
        {
            string Path = "C:\\Users\\bober\\OneDrive\\Pulpit\\projekt_fafik\\projekt_fafik\\fafik pliki\\help.txt";
            string wyn = sup.help(Path);
            await ReplyAsync(wyn);

        }
        
    }
}