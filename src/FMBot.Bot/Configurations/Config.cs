﻿using Newtonsoft.Json;

namespace FMBot.Bot.Configurations
{
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("fmkey")]
        public string FMKey { get; private set; }

        [JsonProperty("fmsecret")]
        public string FMSecret { get; private set; }

        [JsonProperty("prefix")]
        public string CommandPrefix { get; set; }

        [JsonProperty("baseserver")]
        public string BaseServer { get; private set; }

        [JsonProperty("announcementchannel")]
        public string AnnouncementChannel { get; private set; }

        [JsonProperty("featuredchannel")]
        public string FeaturedChannel { get; private set; }

        [JsonProperty("botowner")]
        public string BotOwner { get; private set; }

        [JsonProperty("timerinit")]
        public string TimerInit { get; set; }

        [JsonProperty("timerrepeat")]
        public string TimerRepeat { get; set; }

        [JsonProperty("spotifykey")]
        public string SpotifyKey { get; private set; }

        [JsonProperty("spotifysecret")]
        public string SpotifySecret { get; private set; }

        [JsonProperty("exceptionchannel")]
        public string ExceptionChannel { get; private set; }

        [JsonProperty("cooldown")]
        public string Cooldown { get; set; }

        [JsonProperty("nummessages")]
        public string NumMessages { get; set; }

        [JsonProperty("inbetweentime")]
        public string InBetweenTime { get; set; }

        [JsonProperty("suggestionschannel")]
        public string SuggestionsChannel { get; private set; }

        [JsonProperty("dblapitoken")]
        public string DblApiToken { get; private set; }
    }
}