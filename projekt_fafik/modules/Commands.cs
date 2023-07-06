using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace fafikspace.commands
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("pong");
        }
        [Command("chuj")]
        public async Task Chuj()
        {
            await ReplyAsync("Paweł to chuj jebany w dupe i krowa cię jebała");
        }
    }
}