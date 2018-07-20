using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Discord_Bot.Modules
{ 
    public class Commands : ModuleBase<SocketCommandContext>  
    {
       //Run delete messages every 5 minutes
        Timer timer = new Timer(300000);
        
           
        [Command("help")]
        public async Task HelpAsync()
        {
            await ReplyAsync("commands are !start, !stop, and !delete-messages");
        }

        [Command("start")]
        public async Task StartAsync()
        {
            timer.Elapsed += async (sender, e) => await DeleteMessagesAsync();
            timer.AutoReset = true;
            timer.Start();
            Console.WriteLine("Bot Started");
            await ReplyAsync("I am now deleting messages that are over an hour old.");          
        }

        [Command("stop")]
        public async Task StopAsync()
        {
            timer.Stop();
            Console.WriteLine("Bot Stoped");
            await ReplyAsync("I am no longer deleting messages.");
        }


        [Command("watch")]      
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        public async Task DeleteMessagesAsync()
        {
            Console.WriteLine("Scanning Messages");
            try
            {
                var messages = await Context.Channel.GetMessagesAsync().Flatten();
                foreach (IUserMessage message in messages)
                {
                    var now = DateTime.Now.ToUniversalTime();
                    var messageTimestamp = message.Timestamp.UtcDateTime;
                    var elapsed = now - messageTimestamp;

                    if (elapsed.Hours > 0) await message.DeleteAsync();
                    //Can check if minutes are over 20.
                    //if (elapsed.Minutes > 1) await message.DeleteAsync();
                    await Task.Delay(5000);
                }
            }catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
}
