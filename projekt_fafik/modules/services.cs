using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Victoria;
using Victoria.Entities;
using Discord;
using Discord.WebSocket;
using fafikspace.helping;

namespace fafikspace.services
{
    public class AudioServices
    {
        private readonly LavaRestClient _lavaRestClient;
        private readonly LavaSocketClient _lavaSocketClient;
        private readonly DiscordSocketClient _client;
        private readonly Helping _logService;


        public AudioServices(LavaRestClient lavaRestClient, DiscordSocketClient client, LavaSocketClient lavaSocketClient)
        {
            _client = client;
            _lavaRestClient=lavaRestClient;
            _lavaSocketClient=lavaSocketClient;
            _logService = new();
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
            => await _lavaSocketClient.DisconnectAsync(voiceChannel);

        public async Task <string> PlayAsync(string query, ulong guildId)
        {
            var _player = _lavaSocketClient.GetPlayer(guildId);
            var results = await _lavaRestClient.SearchYouTubeAsync(query);

            if(results.LoadType == LoadType.NoMatches || results.LoadType == LoadType.LoadFailed)
            {
                return "nie znaleziono nagrania.";

            }

            var track = results.Tracks.FirstOrDefault();

            if(_player.IsPlaying)
            {
                _player.Queue.Enqueue(track);
                return $"{track.Title} został dodany do kolejki";
            }
            else
            {
                await _player.PlayAsync(track);
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
            if (_player is null || _player.Queue.Items.Count() is 0)
                return "kolejka jest pusta!";

            var oldTrack = _player.CurrentTrack;
            await _player.SkipAsync();
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
                return "Muzyka została zapauzowana";
            }
            else
            {
                await _player.ResumeAsync();
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
                return "muzyka została wznowiona";
            }
            else
                return "muzyka ciągle gra!";

        }

        private async Task ClientReadyAsync()
        {
            Console.WriteLine(_lavaSocketClient is null);
            await _lavaSocketClient.StartAsync(_client, new Configuration
            {
                LogSeverity = LogSeverity.Verbose
            });

        }

        private async Task TrackFinished(LavaPlayer player, LavaTrack track, TrackEndReason reason)
        {
            if (!reason.ShouldPlayNext())
                return;

            if(!player.Queue.TryDequeue(out var item) || !(item is LavaTrack nextTrack))
            {
                await player.TextChannel.SendMessageAsync("brak piosenek w kolejce!!!");
                return;
            }

            await player.PlayAsync(nextTrack);
        }
        private async Task LogAsync(LogMessage logMessage)
        {
             _logService.log_write(logMessage);
        }
    }
}


