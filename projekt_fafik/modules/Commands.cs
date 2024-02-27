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
        [Command("chuj")]
        public async Task Chuj()
        {
            await ReplyAsync("Paweł to chuj jebany w dupe i krowa cię jebała ");
        }
        [Command("debil")]
        public async Task Debil()
        {
            string path = "C:\\Users\\bober\\OneDrive\\Pulpit\\projekt_fafik\\projekt_fafik\\fafik pliki\\pawel.txt";
            int x = sup.stat_R(path);
            if (x == -1) return;
            x++;
            await ReplyAsync($"Paweł jest DEBILEM {x} razy :DD");
            
            sup.stat_W(path, x);

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