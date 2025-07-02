using Newtonsoft.Json;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardropScroll.Helper;
using StardropScroll.IDs;
using System.Collections.ObjectModel;
using static StardropScroll.IDs.MissionID;

namespace StardropScroll.Content.Mission
{
    public class PlayerMission
    {
        public string ID;

        public int Current;

        [JsonProperty("target")]
        public int Target { get; set; }

        public int Level;

        [JsonProperty("step")]
        public int Step { get; set; }

        [JsonProperty("max")]
        public int MaxLevel { get; set; }

        public string Data => $"{Current}.{Level}";

        public bool Completed => Level >= MaxLevel;
        public NetMission Net => new() { id = ID, data = Data };
    }
    public struct NetMission
    {
        public string id;
        public string data;
    }
    public static class MissionManager
    {
        public static ReadOnlyDictionary<string, PlayerMission> Missions { get; private set; }
        public static void LoadMissionData()
        {
            var data = Main.ReadJsonFile<Dictionary<string, PlayerMission>>(Path.Combine("Assets", "MissionDatas.json"));
            if (data == null)
            {
                Main.LogError("Failed to load Mission Data!");
                return;
            }
            foreach (var (name, mission) in data)
            {
                mission.ID = name;
            }
            Missions = new(data);
        }
        public static void LoadData(Farmer player)
        {
            var data = player.modData;
            foreach (var (name, m) in Missions)
            {
                if (data.TryGetValue(MissionKey(name), out var info))
                {
                    string[] infos = info.Split('.');
                    m.Current = int.Parse(infos[0]);
                    m.Level = int.Parse(infos[1]);
                }
                else
                {
                    m.Current = 0;
                    m.Level = 0;
                }
                m.Target += m.Level * m.Step;
            }
            Main.WriteJsonFile(Path.Combine("Assets", "Stats.json"), Game1.player.stats.Values);
        }
        public static void SubmitMission(PlayerMission m)
        {
            if (m.Current < m.Target || m.Completed)
                return;
            m.Current -= m.Target;
            m.Level++;
            if (Context.IsMainPlayer || !Context.IsMultiplayer)
                return;

            var player = Game1.player;
            player.modData[MissionKey(m.ID)] = m.Data;

            if (!Context.IsMultiplayer || Context.IsMainPlayer)
                return;
            Main.NetSend(m.Net, NetMessageID.Mission);
        }
        public static void Increase(string name, int amount = 1)
        {
            if (!Main.IsLocal)
                return;
            if (Missions.TryGetValue(name, out var mission))
            {
                mission.Current += amount;
                Main.Log($"{name} increase by {amount}");
                return;
            }
            Main.LogWarn("Error mission name");
        }
        public static void NetReceive(ModMessageReceivedEventArgs e)
        {
            Farmer? player = Game1.GetPlayer(e.FromPlayerID);
            if (player == null)
            {
                Main.LogWarn($"Receive mission data but player id {e.FromPlayerID} error");
                return;
            }
            var m = e.ReadAs<NetMission>();
            player!.modData[MissionKey(m.id)] = m.data;
        }

        /// <summary>
        /// 无数据返回0级
        /// </summary>
        /// <param name="player"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetMissionLevel(this Farmer player, string name)
        {
            if (player.modData.TryGetValue(MissionKey(name), out var data))
            {
                return int.Parse(data.Split('.')[1]);
            }
            return 0;
        }
    }
}
