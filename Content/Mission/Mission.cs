using Newtonsoft.Json;
using StardewValley;
using StardropScroll.Helper;

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

        public bool Notified { get; set; }

        [JsonIgnore]
        public int Target { get; set; }

        [JsonIgnore]
        public int Money { get; set; }

        [JsonIgnore]
        public bool Claimed { get; set; }

        [JsonIgnore]
        public MissionData Data { get; private set; }

        [JsonIgnore]
        public bool Completed => Level >= Data.MaxLevel;

        [JsonIgnore]
        public bool CanSubmit => Current >= Target && !Completed;

        public void LoadData(MissionData data)
        {
            Data = data;
            GetTarget();
        }

        public void NextState()
        {
            Current -= Target;
            Level++;
            Claimed = false;
            Notified = false;
            GetTarget();
            CheckProgress();
            MissionManager.AddReward(this);
            MissionBonus.RefreshBuffs();
        }
        public string GetName() => I18n.GetByKey(Prefix + Name) + $" Lv.{Level}";

        public string GetDescription() => I18n.GetByKey(Prefix + Name + Desc);
        public void OnMoneyRewardClaimed()
        {
            Claimed = true;
            if (!Main.IsMaster)
                return;
            Game1.player.Money += Money;
            Game1.playSound("purchaseRepeat", null);
        }
        public void OnLeaveQuestPage()
        {
            if (Claimed)
            {
                Claimed = false;
                MissionManager.SubmitMission(this);
            }
        }

        public List<(int Current, int Target)> GetObjectives() => new() { (Current, Target) };

        public List<string> GetObjectiveDescriptions() => new()
        {
            I18n.MissionNextLevel() + I18n.GetByKey(Prefix + Name + Objective, new { Amount = Target })
        };

        public void GetTarget()
        {
            Money = (Level + 1) * 500;
            Target = Data.Target + Level * Data.Step;
        }

        public void Increase(int amount)
        {
            Current += amount;
            CheckProgress();
        }

        public void CheckProgress()
        {
            if (!CanSubmit || Notified)
                return;
            Notified = true;
            Game1.playSound("questcomplete", null);
            Game1.addHUDMessage(new HUDMessage(I18n.MissionCompleted(GetName()), 2));
        }
    }
}
