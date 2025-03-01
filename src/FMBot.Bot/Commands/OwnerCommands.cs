﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FMBot.Bot.Extensions;
using FMBot.Data.Entities;
using FMBot.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bot.Logger;
using static FMBot.Bot.FMBotUtil;

namespace FMBot.Bot.Commands
{
    [Summary("FMBot Owners Only")]
    public class OwnerCommands : ModuleBase
    {
        private readonly Logger _logger;

        private readonly AdminService _adminService = new AdminService();

        public OwnerCommands(Logger logger)
        {
            _logger = logger;
        }

        [Command("fmsetusertype"), Summary("Sets usertype for other users")]
        [Alias("fmsetperms")]
        public async Task fmsetusertypeAsync(string userId = null, string userType = null)
        {
            if (await _adminService.HasCommandAccessAsync(Context.User, UserType.Owner).ConfigureAwait(false))
            {
                if (userId == null || userType == null)
                {
                    await ReplyAsync("Please format your command like this: `.fmsetusertype 'discord id' 'User/Admin/Owner'`").ConfigureAwait(false);
                    return;
                }

                if (!Enum.TryParse(userType, true, out UserType userTypeEnum))
                {
                    await ReplyAsync("Invalid usertype. Please use 'User', 'Admin', or 'Owner'.").ConfigureAwait(false);
                    return;
                }

                if (await _adminService.SetUserTypeAsync(userId, userTypeEnum).ConfigureAwait(false))
                {
                    await ReplyAsync("You got it. User perms changed.").ConfigureAwait(false);
                }
                else
                {
                    await ReplyAsync("Setting user failed. Are you sure the user exists?").ConfigureAwait(false);
                }

            }
            else
            {
                await ReplyAsync("Error: Insufficient rights. Only FMBot owners can change your usertype.").ConfigureAwait(false);
            }
        }


        [Command("fmremovereadonly"), Summary("Removes read only on all directories.")]
        [Alias("fmreadonlyfix")]
        public async Task fmremovereadonlyAsync()
        {
            if (await _adminService.HasCommandAccessAsync(Context.User, UserType.Owner).ConfigureAwait(false))
            {
                try
                {
                    if (Directory.Exists(GlobalVars.CacheFolder))
                    {
                        DirectoryInfo users = new DirectoryInfo(GlobalVars.CacheFolder);
                    }


                    await ReplyAsync("Removed read only on all directories.").ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.LogException(Context.Message.Content, e);
                    await ReplyAsync("Unable to remove read only on all directories due to an internal error.").ConfigureAwait(false);
                }
            }
        }

        [Command("fmstoragecheck"), Summary("Checks how much storage is left on the server.")]
        [Alias("fmcheckstorage", "fmstorage")]
        public async Task fmstoragecheckAsync()
        {
            if (await _adminService.HasCommandAccessAsync(Context.User, UserType.Owner))
            {
                try
                {
                    DriveInfo[] drives = DriveInfo.GetDrives();

                    EmbedBuilder builder = new EmbedBuilder();
                    builder.WithDescription("Server Drive Info");

                    foreach (DriveInfo drive in drives.Where(w => w.IsReady))
                    {
                        builder.AddField(drive.Name + " - " + drive.VolumeLabel + ":", _adminService.FormatBytes(drive.AvailableFreeSpace) + " free of " + _adminService.FormatBytes(drive.TotalSize));
                    }

                    await Context.Channel.SendMessageAsync("", false, builder.Build());
                }
                catch (Exception e)
                {
                    _logger.LogException(Context.Message.Content, e);
                    await ReplyAsync("Unable to delete server drive info due to an internal error.");
                }
            }
        }

        [Command("fmserverlist"), Summary("Displays a list showing information related to every server the bot has joined.")]
        public async Task fmserverlistAsync()
        {
            if (await _adminService.HasCommandAccessAsync(Context.User, UserType.Owner))
            {
                DiscordSocketClient SelfUser = Context.Client as DiscordSocketClient;

                string desc = null;

                foreach (SocketGuild guild in SelfUser.Guilds.OrderByDescending(o => o.MemberCount).Take(100))
                {
                    desc += $"{guild.Name} - Users: {guild.Users.Count()}, Owner: {guild.Owner.ToString()}\n";
                }

                if (!string.IsNullOrWhiteSpace(desc))
                {
                    string[] descChunks = desc.SplitByMessageLength().ToArray();
                    foreach (string chunk in descChunks)
                    {
                        await Context.User.SendMessageAsync(chunk);
                    }
                }

                await Context.Channel.SendMessageAsync("Check your DMs!");
            }
        }

        [Command("fmnameoverride"), Summary("Changes the bot's name.")]
        [Alias("fmsetbotname")]
        public async Task FmnameoverrideAsync(string name = ".fmbot")
        {
            if (await _adminService.HasCommandAccessAsync(Context.User, UserType.Owner))
            {
                try
                {
                    DiscordSocketClient client = Context.Client as DiscordSocketClient;
                    await client.CurrentUser.ModifyAsync(u => u.Username = name);
                    await ReplyAsync("Set name to '" + name + "'");
                }
                catch (Exception e)
                {
                    _logger.LogException(Context.Message.Content, e);
                    await ReplyAsync("Unable to set the name of the bot due to an internal error.");
                }
            }
        }
    }
}
