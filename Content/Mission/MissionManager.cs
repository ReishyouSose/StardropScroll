using HarmonyLib;
using Newtonsoft.Json;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardropScroll.Helper;
using StardropScroll.IDs;
using System.Reflection.Emit;

namespace StardropScroll.Content.Mission
{
    public struct MissionData
    {
        [JsonProperty("target")]
        public int Target { get; set; }

        [JsonProperty("max")]
        public int MaxLevel { get; set; }

        [JsonProperty("step")]
        public int Step { get; set; }
    }
    public class Mission
    {
        private const string Prefix = "Mission.";
        private const string Desc = "_Desc";
        private const string Objective = "_Objective";
        public string Name { get; set; }
        public int Current { get; set; }
        public int Level { get; set; }

        [JsonIgnore]
        public int Target;

        [JsonIgnore]
        public MissionData Data { get; private set; }

        [JsonIgnore]
        public bool Completed => Level >= Data.MaxLevel;

        [JsonIgnore]
        public bool CanSubmit => Current >= Target;

        public void LoadData(MissionData data)
        {
            Data = data;
            GetTarget();
        }

        public void NextState()
        {
            Current -= Target;
            Level++;
            GetTarget();
        }
        public string GetName() => I18n.GetByKey(Prefix + Name);

        public string GetDescription() => I18n.GetByKey(Prefix + Name + Desc);
        public int GetMoneyReward() => (Level + 1) * 500;
        public void OnMoneyRewardClaimed()
        {

        }
        public void OnLeaveQuestPage()
        {

        }

        public List<(int Current, int Target)> GetObjectives() => new() { (Current, Target) };

        public List<string> GetObjectiveDescriptions() => new()
        {
            I18n.GetByKey(Prefix + Name  + Objective, new { Amount = (Target) })
        };

        public void GetTarget() => Target = Data.Target + Level * Data.Step;
    }

    public static class MissionManager
    {
        public static Dictionary<string, Mission> Missions { get; private set; }
        private static Dictionary<string, int> missionIncrease;
        private static Dictionary<string, MissionData> missionDatas;
        public static void LoadMissionData()
        {
            missionIncrease = new();
            missionDatas = Main.ReadJsonFile<Dictionary<string, MissionData>>(Path.Combine("Assets", "MissionDatas.json"));
            if (missionDatas == null)
                Main.LogError("Failed to load Mission Data!");
        }

        public static void LoadData()
        {
            Missions = Main.LoadData<Dictionary<string, Mission>>(Main.UniqueID + ".Missions") ?? new();
            foreach (var (name, data) in missionDatas)
            {
                if (!Missions.TryGetValue(name, out var mission))
                    Missions.Add(name, mission = new() { Name = name });
                mission.LoadData(data);
            }
        }

        /// <summary>
        /// add为0则是升级数据包
        /// </summary>
        /// <param name="m"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        private static string MissionPack(string mission, int add = 0) => $"{mission}.{add}";
        public static bool SubmitMission(Mission m)
        {
            if (!m.CanSubmit)
                return false;
            if (Game1.IsMasterGame)
                m.NextState();
            Main.NetSend(MissionPack(m.Name), NetMessageID.Mission);
            return true;
        }

        public static void NetReceive(ModMessageReceivedEventArgs e)
        {
            string[] info = e.ReadAs<string>().Split('.');
            string name = info[0];
            int amount = int.Parse(info[1]);
            bool master = Main.IsMaster;
            Mission m = Missions[name];
            if (amount == 0)
            {
                if (master)
                    SubmitMission(m);
                else
                    m.NextState();
                return;
            }
            if (master)
                Increase(name, amount);
            else
                m.Current += amount;
        }

        public static void Increase(string name, int amount = 1)
        {
            if (!missionDatas.ContainsKey(name))
            {
                Main.LogWarn("Error mission name");
                return;
            }
            missionIncrease.TryGetValue(name, out int add);
            missionIncrease[name] = add + amount;
            Main.Log($"{name} add {amount}");
        }

        public static void CheckIncrease(object? sender, OneSecondUpdateTickedEventArgs e)
        {
            if (!missionIncrease.Any())
                return;
            if (Main.IsMaster)
            {
                foreach (var (name, add) in missionIncrease)
                {
                    Missions[name].Current += add;
                    Main.NetSend(MissionPack(name, add), NetMessageID.Mission);
                }
            }
            else
            {
                foreach (var (name, add) in missionIncrease)
                {
                    Main.NetSend(MissionPack(name, add), NetMessageID.Mission);
                }
            }
            missionIncrease.Clear();
        }

        public static int GetLevel(string name)
        {
            if (!Missions.TryGetValue(name, out var m))
                Main.LogWarn("Error mission name!");
            return m.Level;
        }
        private static CodeInstruction Call() => ILHelper.Call(typeof(MissionManager), "Increase");
        public static void MissionIncrease(this List<CodeInstruction> codes, ref int index, string mission, int amount = 1)
        {
            List<CodeInstruction> list = new()
            {
                new(OpCodes .Ldstr, mission),
                ILHelper.Int(amount),
                Call()
            };
            codes.InsertRange(index + 1, list);
            index += 3;
        }

        public static int GetBonusTimes(int level, double init, double fix = 0.95, Random r = null, bool failureBreak = false)
        {
            r ??= Random.Shared;
            int amount = 0;
            bool successOnce = false;
            for (int i = 0; i < level; i++)
            {
                if (r.NextBool(init))
                {
                    amount++;
                    init *= fix;
                    successOnce = true;
                }
                else if (failureBreak && successOnce)
                    break;
            }
            return amount;
        }
    }
}
