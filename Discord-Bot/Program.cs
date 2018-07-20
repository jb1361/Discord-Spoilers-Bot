using System;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using System.Timers;

namespace Discord_Bot
{
    class Bot
    {
        static void Main(string[] args) => new Bot().Connect().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandService _commands;
        private IServiceProvider _services;

        
        public async Task Connect()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();
    
            _services = new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .BuildServiceProvider();

            _client.Log += Log;

         
            await RegisterCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, "Insert-Bot-Client-ID");         

            await _client.StartAsync();      

            await Task.Delay(-1);

           


        }

     
        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        public async Task RegisterCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());          
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;

            if (message is null || message.Author.IsBot) return;

            int argPos = 0;

            if (message.HasStringPrefix("!", ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(_client, message);

                var result = await _commands.ExecuteAsync(context, argPos, _services);

                if (!result.IsSuccess) Console.WriteLine(result.ErrorReason);
            }
        }
    }//Class
}
