using StardewValley.Quests;
using StardropScroll.Content.Mission;

namespace StardropScroll.UI
{
    public class MissionEntry : IQuest
    {
        public readonly Mission Mission;
        public MissionEntry(Mission mission) => Mission = mission;
        private const string Prefix = "Mission.";
        private const string Desc = "_Desc";
        private const string Objective = "_Objective";
        public bool CanBeCancelled() => false;

        public int GetDaysLeft() => 0;

        public string GetDescription() => I18n.GetByKey(Prefix + Mission.Name + Desc);

        public int GetMoneyReward() => Mission.Level * 500;

        public string GetName() => I18n.GetByKey(Prefix + Mission.Name);
        public List<(int Current, int Target)> GetObjectives()
        {
            return new()
            {
                (Mission.Current,Mission.Target),
            };
        }

        public List<string> GetObjectiveDescriptions()
        {
            return new()
            {
                I18n.GetByKey(Prefix+Mission.Name+Objective, new { Amount = Mission.Target } )
            };
        }

        public bool HasMoneyReward() => true;

        public bool HasReward() => true;

        public bool IsHidden() => false;

        public bool IsTimedQuest() => false;

        public void MarkAsViewed() { }

        public bool OnLeaveQuestPage() => false;

        public void OnMoneyRewardClaimed() { }

        public bool ShouldDisplayAsComplete() => Mission.CanSubmit;

        public bool ShouldDisplayAsNew() => false;
    }
}
