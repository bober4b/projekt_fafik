using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using Victoria;
using Victoria.Entities;
using Discord;
using Discord.WebSocket;
using fafikspace.helping;
using projekt_fafik.trackended;

namespace fafikspace.services
{
    public class AudioServices
    {
        private readonly LavaRestClient _lavaRestClient;
        private readonly LavaSocketClient _lavaSocketClient;
        private readonly DiscordSocketClient _client;
        private readonly Helping _logService;
        public Trackended timer;


        public AudioServices(LavaRestClient lavaRestClient, DiscordSocketClient client, LavaSocketClient lavaSocketClient)
        {
            _client = client;
            _lavaRestClient=lavaRestClient;
            _lavaSocketClient=lavaSocketClient;
            _logService = new();
            timer = new Trackended();
        }
        public Task InitializeAsync()
        {
            _client.Ready += ClientReadyAsync;
            _lavaSocketClient.Log += LogAsync;
            _lavaSocketClient.OnTrackFinished += TrackFinished;
            
            
            
            
            return Task.CompletedTask;
        }

        public async Task ConnectAsync(SocketVoiceChannel voiceChannel, ITextChannel textChannel)
            => await _lavaSocketClient.ConnectAsync(voiceChannel, textChannel);

        public async Task LeaveAsync(SocketVoiceChannel voiceChannel)
        {
            timer.Stop();
            await _lavaSocketClient.DisconnectAsync(voiceChannel);
        }

        public async Task <string> PlayAsync(string query, ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            var results = await _lavaRestClient.SearchYouTubeAsync(query);
            

            if (_player is null)
                return "nie ma mnie na żadnym kanale!!";

            if(results.LoadType == LoadType.NoMatches || results.LoadType == LoadType.LoadFailed)
            {
                return "nie znaleziono nagrania.";

            }

            var track = results.Tracks.FirstOrDefault();
            if(_player.IsPlaying)
            {
                _player.Queue.Enqueue(track);
                
                timer.playnext = true;
                return $"{track.Title} został dodany do kolejki";
                
            }
            else
            {
                await _player.PlayAsync(track);
                timer.Restart();
                return $"właśnie gra: {track.Title}";

            }

        }

        public async Task<string> StopAsync(ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            if (_player is null)
                return "Błąd playera";

            await _player.StopAsync();
            return "Muzyka została zatrzymana";
        }
        public async Task<string> SkipAsync(ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);

            if(_player is null || _player.Queue.Items.Count() is 0)
                return "kolejka jest pusta!";
            var oldTrack = _player.CurrentTrack;
            await _player.SkipAsync();
            timer.Restart();
            return $"skipped: {oldTrack.Title}\n Teraz gram: {_player.CurrentTrack.Title}";
                
        }
        public async Task<string> SetVolumeAsync(int vol, ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);

            if(_player is null)
            {
                return "Player jest pusty";
            }

            if(vol>150 || vol<=2)
            {
                return "użyj wartości z przedziału 2-150!";
            }
            await _player.SetVolumeAsync(vol);
            return $"volume set to: {vol}";

        }
        public async Task<string> PauseOrResumeAsync(ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);

            if(_player is null)
            {
                return "muzyka nie gra!!";

            }
            if (!_player.IsPaused)
            {
                await _player.PauseAsync();
                timer.Stop();
                return "Muzyka została wstrzymana";
            }
            else
            {
                await _player.ResumeAsync();
                timer.Start();
                return "muzyka została wznowiona";
            }

        }

        public async Task<string> ResumeAsync(ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            if (_player is null)
            {
                return "muzyka nie gra!!";

            }

            if (_player.IsPaused)
            {
                await _player.ResumeAsync();
                timer.Start();
                return "muzyka została wznowiona";
            }
            else
                return "muzyka ciągle gra!";

        }

        private async Task ClientReadyAsync()
        {
            Console.WriteLine(_lavaSocketClient is null);
            await _lavaSocketClient.StartAsync(_client, new Configuration());

        }
  
        public async Task TrackFinished(LavaPlayer player, LavaTrack track, TrackEndReason reason)
        {

            
            if (!reason.ShouldPlayNext())
                return;

            if(!player.Queue.TryDequeue(out var item) || item is not LavaTrack nextTrack )
            {
                await player.TextChannel.SendMessageAsync("brak piosenek w kolejce!!!");
                timer.Stop();
                await player.StopAsync();

                return;
            }
            timer.Restart();
            timer.playnext = true;
            await player.PlayAsync(nextTrack);
            await player.TextChannel.SendMessageAsync($"teraz gramy: {player.CurrentTrack.Title}");
        }

        public async Task<string> QueueAsync(ulong guildId)
        {
            var _player= _lavaSocketClient.GetPlayer(guildId);
            if(_player is null)
            {
                return "nie ma nic w kolejce";
            }
            var queue = _player.Queue.Items;
            string wyn = "";
            int i = 1;
            foreach (LavaTrack item in queue)
            {
                Console.WriteLine(item);
                wyn +=i+". "+item.Title.ToString()+"\n";
                i++;

            }
            if (i == 1)
                return "brak piosenek w kolejce!";
            if(_player.CurrentTrack is not null)
            {
                return $"        Teraz gram: {_player.CurrentTrack.Title}\n Kolejka:\n{wyn}";
            }
            return $"Kolejka:\n{wyn}";
        }

            public async Task EndTimeNextAsync( ulong guildId)
            {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            bool check = false;
            while (true)
            {
                if (_player.CurrentTrack is not null && timer.Mili() >= _player.CurrentTrack.Length.TotalMilliseconds+1000 )
                {

                    
                    LavaTrack track = _player.CurrentTrack;
                        var reason = TrackEndReason.Finished;
                        await TrackFinished(_player, track, reason);


                    if (!_player.Queue.TryDequeue(out var item) || !(item is LavaTrack nextTrack) )
                    {
                        
                        timer.playnext = false;
                        timer.Stop();
                        
                    }
                   
                  

                }
                

                await Task.Delay(TimeSpan.FromSeconds(1));
               
            }

            }
        public async Task r()
        {
            timer.Restart();
        }


        private async Task LogAsync(LogMessage logMessage)
        {
             _logService.log_write(logMessage);
        }

    }
}


