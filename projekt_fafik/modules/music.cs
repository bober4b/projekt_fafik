using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using fafikspace.helping;
using Victoria;
using fafikspace.services;




namespace fafikspace.musicmodule
{
    public sealed class Music : ModuleBase<SocketCommandContext>
    {
        private AudioServices _musicService;
        private Helping help;

        public Music(AudioServices musicService)
        {
            _musicService = musicService;
            help = new Helping();
            
        }

        [Command("Join")]
        public async Task Join()
        {
            var user = Context.User as SocketGuildUser;

            if (user.VoiceChannel is null)
            {
                await ReplyAsync("musisz być na jakimś kanale!!");
                help.log_write("nie ma użytkownika na kanale");
                return;

            }
            //else if(1==1)
           // {

           // }
            else
            {
                await _musicService.ConnectAsync(user.VoiceChannel, Context.Channel as ITextChannel);
                Task task = _musicService.EndTimeNextAsync(Context.Guild.Id);
                await ReplyAsync($"Dołądczyłem na {user.VoiceChannel.Name}");
            }
        }
        [Command("leave")]
        public async Task Leave()
        {
            var user =Context.User as SocketGuildUser;
            if(user.VoiceChannel is null)
            {
                await ReplyAsync("dołącz na kanał zanim bot wyjdzie.");
            }
            else
            {
                await _musicService.LeaveAsync(user.VoiceChannel);
                await ReplyAsync($"Bot opuścił kanał {user.VoiceChannel.Name}");
            }
        }

        [Command("Play")]
        public async Task Play([Remainder] string query) 
            => await ReplyAsync(await _musicService.PlayAsync(query, Context.Guild.Id));

        [Command("Stop")]
        public async Task Stop()
            => await ReplyAsync(await _musicService.StopAsync(Context.Guild.Id));

        [Command("Skip")]
        public async Task Skip()
            => await ReplyAsync(await _musicService.SkipAsync(Context.Guild.Id));

        [Command("Volume")]
        public async Task Volume(int vol)
            => await ReplyAsync(await _musicService.SetVolumeAsync(vol, Context.Guild.Id));

        [Command("Pause")]
        public async Task Pause()
            => await ReplyAsync(await _musicService.PauseOrResumeAsync(Context.Guild.Id));

        [Command("Resume")]
        public async Task Resume()
            => await ReplyAsync(await _musicService.ResumeAsync(Context.Guild.Id));
        [Command ("Queue")]
        public async Task Queue()
            => await ReplyAsync(await _musicService.QueueAsync(Context.Guild.Id));

      

    }
}
