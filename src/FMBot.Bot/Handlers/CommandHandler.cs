﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using FMBot.Bot.Configurations;

namespace FMBot.Bot.Handlers
{
    public class CommandHandler
    {
        private readonly DiscordShardedClient _discord;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;

        private static readonly List<DateTimeOffset> StackCooldownTimer = new List<DateTimeOffset>();
        private static readonly List<SocketUser> StackCooldownTarget = new List<SocketUser>();

        // DiscordSocketClient, CommandService, IConfigurationRoot, and IServiceProvider are injected automatically from the IServiceProvider
        public CommandHandler(
            DiscordShardedClient discord,
            CommandService commands,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;

            _discord.MessageReceived += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;     // Ensure the message is from a user/bot
            if (msg == null) return;
            if (msg.Author.Id == _discord.CurrentUser.Id) return;     // Ignore self when checking commands

            var context = new ShardedCommandContext(_discord, msg);     // Create the command context

            var argPos = 0;     // Check if the message has a valid command prefix
            if (msg.HasStringPrefix(ConfigData.Data.CommandPrefix, ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                if (StackCooldownTarget.Contains(msg.Author))
                {
                    //If they have used this command before, take the time the user last did something, add 2 seconds, and see if it's greater than this very moment.
                    if (StackCooldownTimer[StackCooldownTarget.IndexOf(msg.Author)].AddSeconds(2) >= DateTimeOffset.Now)
                    {
                        //If enough time hasn't passed, reply letting them know how much longer they need to wait, and end the code.
                        var secondsLeft = (int)(StackCooldownTimer[StackCooldownTarget.IndexOf(msg.Author)].AddSeconds(2) - DateTimeOffset.Now).TotalSeconds;
                        await context.Channel.SendMessageAsync($"Please wait {secondsLeft + 1} seconds before you use that command again!");
                        return;
                    }

                    StackCooldownTimer[StackCooldownTarget.IndexOf(msg.Author)] = DateTimeOffset.Now;
                }
                else
                {
                    //If they've never used this command before, add their username and when they just used this command.
                    StackCooldownTarget.Add(msg.Author);
                    StackCooldownTimer.Add(DateTimeOffset.Now);
                }

                var result = await _commands.ExecuteAsync(context, argPos, _provider);     // Execute the command

                if (!result.IsSuccess)     // If not successful, reply with the error.
                    await context.Channel.SendMessageAsync(result.ToString());
            }
        }
    }
}
