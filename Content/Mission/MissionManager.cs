using HarmonyLib;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardropScroll.Helper;
using StardropScroll.IDs;
using System.Reflection.Emit;

namespace StardropScroll.Content.Mission
{

    public static class MissionManager
    {
        public static Dictionary<string, Mission> Missions { get; private set; }
        private static Dictionary<string, int> missionIncrease;
        private static Dictionary<string, MissionData> datas;
        public static void LoadMissionData()
        {
            missionIncrease = new();
            datas = Main.ReadJsonFile<Dictionary<string, MissionData>>(Path.Combine("Assets", "MissionDatas.json"));
            if (datas == null)
                Main.LogError("Failed to load Mission Data!");
        }

        public static void LoadData()
        {
            Missions = Main.LoadData<Dictionary<string, Mission>>(Main.UniqueID + ".Missions") ?? new();
            foreach (var (name, data) in datas)
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
            {
                Game1.player.Money += m.Money;
                Game1.playSound("purchaseRepeat", null);
                m.NextState();
            }
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
                {
                    Game1.player.Money += m.Money;
                    Game1.playSound("purchaseRepeat", null);
                    m.NextState();
                }
                return;
            }
            if (master)
                Increase(name, amount);
            else
            {
                m.Current += amount;
                if (m.CanSubmit)
                {
                    Game1.playSound("questcomplete", null);
                    Game1.addHUDMessage(new HUDMessage(I18n.MissionCompleted(m.GetName(), m.Level + 1), 2));
                }
            }
        }

        public static void Increase(string name, int amount = 1)
        {
            if (!datas.ContainsKey(name))
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
                    Mission m = Missions[name];
                    m.Current += add;
                    if (m.CanSubmit)
                    {
                        Game1.playSound("questcomplete", null);
                        I18n.MissionCompleted(m.GetName(), m.Level + 1);
                    }
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
