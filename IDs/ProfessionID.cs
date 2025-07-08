using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardropScroll.IDs
{
    /// <summary>
    /// 星露谷物语职业ID常量表（中英对照）
    /// 格式：所属技能_解锁等级_职业名 = ID
    /// 数据版本：1.6.0
    /// </summary>
    public static class ProfessionID
    {
        //耕种
        /// <summary>畜牧人 畜牧产品价值提升20%</summary>
        public const int Farming_5_Rancher = 0;

        /// <summary>农耕人 农作产品价值提升10%</summary>
        public const int Farming_5_Tiller = 1;

        /// <summary>鸡舍大师 更快的与鸡舍里的动物交好，孵化时间减半</summary>
        public const int Farming_10_Coopmaster = 2;

        /// <summary>牧羊人 更快的与畜棚的动物交好，绵羊产毛变快</summary>
        public const int Farming_10_Shepherd = 3;

        /// <summary>工匠 工匠物品价值上升40%（注意油实际上并没有受益于工匠职业）</summary>
        public const int Farming_10_Artisan = 4;

        /// <summary>农业学家 作物生长速度提高10%</summary>
        public const int Farming_10_Agriculturist = 5;

        //钓鱼
        /// <summary>渔夫 鱼的价值提高25%</summary>
        public const int Fishing_5_Fisher = 6;

        /// <summary>捕猎者 建造蟹笼所需的材料用量会减少</summary>
        public const int Fishing_5_Trapper = 7;

        /// <summary>垂钓者 鱼价值增加50%</summary>
        public const int Fishing_10_Angler = 8;

        /// <summary>海盗 找到宝藏的机会加倍</summary>
        public const int Fishing_10_Pirate = 9;

        /// <summary>水手 蟹篓不再产生垃圾物品</summary>
        public const int Fishing_10_Mariner = 10;

        /// <summary>诱饵大师 蟹篓不再需要诱饵</summary>
        public const int Fishing_10_Luremaster = 11;

        //采集
        /// <summary>护林人 伐木时木材掉落增加25%（适用于木材和硬木）</summary>
        public const int Foraging_5_Forester = 12;

        /// <summary>收集者 采摘时有概率获得双倍作物数量（20％的几率）</summary>
        public const int Foraging_5_Gathere = 13;

        /// <summary>伐木工人 所有树木都有几率掉落硬木</summary>
        public const int Foraging_10_Lumberjack = 14;

        /// <summary>萃取者 树脂产品价值增加25%</summary>
        public const int Foraging_10_Tapper = 15;

        /// <summary>植物学家 采集获得的物品总是铱星</summary>
        public const int Foraging_10_Botanist = 16;

        /// <summary>追踪者 显示当前场景可采集物品的位置</summary>
        public const int Foraging_10_Tracker = 17;

        //采矿
        /// <summary>矿工 每个金属矿脉+1块矿石</summary>
        public const int Mining_5_Miner = 18;

        /// <summary>地质学家 宝石可能会成对出现（50%的几率。也适用于破碎岩石产生的晶球。）</summary>
        public const int Mining_5_Geologist = 19;

        /// <summary>铁匠 金属棒价值增加50%（适用于铜锭、铁锭、金锭、铱锭和放射性矿锭）</summary>
        public const int Mining_10_Blacksmith = 20;

        /// <summary>勘探者 找到煤的几率变得双倍</summary>
        public const int Mining_10_Prospector = 21;

        /// <summary>挖掘者 找到晶球的几率加倍</summary>
        public const int Mining_10_Excavator = 22;

        /// <summary>宝石专家 宝石价值提高30%（适用于全部矿物和宝石）</summary>
        public const int Mining_10_Gemologist = 23;

        //战斗
        /// <summary>战士 所有攻击造成的伤害值将增加10%，+15 生命值</summary>
        public const int Combat_5_Fighter = 24;

        /// <summary>侦查员 暴击几率提高50%</summary>
        public const int Combat_5_Scout = 25;

        /// <summary>野蛮人 造成的伤害值增加15%</summary>
        public const int Combat_10_Brute = 26;

        /// <summary>防御者 +25 生命值</summary>
        public const int Combat_10_Defender = 27;

        /// <summary>特技者 特技的冷却时间减少一半</summary>
        public const int Combat_10_Acrobat = 28;

        /// <summary>亡命徒 对敌人的暴击将产生更致命效果</summary>
        public const int Combat_10_Desperado = 29;

    }
}
