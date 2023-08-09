using System;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using fafikspace.services;
using fafikspace.entities;
using fafikspace.Log;
using fafikspace.config;

namespace fafikspace.client
{
    internal class StreamMusicBotClient
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _cmdService;
        private IServiceProvider _services;
        private readonly LogService _logService;
        private readonly ConfigService _configService;
        private readonly Config _config;


        public StreamMusicBotClient()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 50,
                LogLevel=LogSeverity.Debug
            });
            _cmdService = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                CaseSensitiveCommands = false
            });
            _logService = new LogService();
            _configService= new ConfigService();
            _config = _configService.GetConfig();
        }

        public async Task InitializeAsync()
        {
            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();
            _client.Log += LogAsync;
            _services = SetupServices();


            var cmdHandler = new CommandHandler(_client, _cmdService, _services);
            await cmdHandler.InitializeAsync();


            await _services.GetRequiredService<AudioServices>().InitializeAsync();
            await Task.Delay(-1);
        }

        private async Task LogAsync(LogMessage logMessage)
        {
            await _logService.LogAsync(logMessage);
        }

        private IServiceProvider SetupServices()
            =>new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_cmdService)
            .AddSingleton(_logService)
            .AddSingleton<LavaRestClient>()
            .AddSingleton<LavaSocketClient>()
            .AddSingleton<AudioServices>()
            .BuildServiceProvider();
    }
}
