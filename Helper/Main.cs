﻿using StardewModdingAPI;
using StardewValley;
using StardropScroll.Config;
using StardropScroll.IDs;

namespace StardropScroll.Helper
{
    public static class Main
    {
        private static StardropScroll Mod => StardropScroll.Ins;
        public static string UniqueID => Mod.ModManifest.UniqueID;

        private static void Log(object message, LogLevel level)
        {
            if (!Mod.config.DebugMode)
                return;
            Mod.Monitor.Log("[StardropScroll] " + message.ToString(), level);
        }
        public static void Log(object message) => Log(message, LogLevel.Debug);
        public static void LogWarn(object message) => Log(message, LogLevel.Warn);
        public static void LogError(object message) => Log(message, LogLevel.Error);
        public static string Key(string key) => $"{UniqueID}/{key}";
        public static T ReadJsonFile<T>(string path) where T : class
        {
            return Mod.Helper.Data.ReadJsonFile<T>(path);
        }
        public static void WriteJsonFile<T>(string path, T data) where T : class
        {
            Mod.Helper.Data.WriteJsonFile(path, data);
        }
        public static void SaveData<T>(string key, T data) where T : class
        {
            Mod.Helper.Data.WriteSaveData(key, data);
        }
        public static T LoadData<T>(string key) where T : class
        {
            return Mod.Helper.Data.ReadSaveData<T>(key);
        }

        /// <summary>
        /// 客机发包仅发给主机，主机发包广播到所有客机
        /// <br/>仅本模组接收包
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="messageID">Use <see cref="NetMessageID"/></param>
        public static void NetSend<T>(T message, int messageID)
        {
            Mod.Helper.Multiplayer.SendMessage(message, messageID.ToString(), new string[] { UniqueID });
        }
        public static void NetSend<T>(T message, int messageID, string[]? receiveMods = null, long[]? receivePlayer = null)
        {
            Mod.Helper.Multiplayer.SendMessage(message, messageID.ToString(), receiveMods, receivePlayer);
        }
        public static bool IsLocal => Game1.player.IsLocalPlayer;
        public static bool IsMaster => Game1.IsMasterGame;

        /// <summary>已游玩天数</summary>
        public static uint DayPlays => Game1.stats.DaysPlayed;
        public static ModConfig Config => Mod.config;
    }
}
