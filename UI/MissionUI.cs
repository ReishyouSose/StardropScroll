using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardropScroll.Content.Mission;

namespace StardropScroll.UI
{
    public class MissionUI : IClickableMenu
    {
        public static int menuWidth = 632 + borderWidth * 2;
        public static int menuHeight = 600 + borderWidth * 2 + Game1.tileSize;
        private List<Item> requiredItems; // 需要提交的物品列表
        private List<ClickableComponent> itemSlots; // 可点击的物品槽位
        private Dictionary<string, string> localize = new();
        public MissionUI() : base((int)GetAppropriateMenuPosition().X, (int)GetAppropriateMenuPosition().Y, menuWidth, menuHeight)
        {
        }
        public override void update(GameTime time)
        {
            base.update(time);
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            //readyToClose();
            //Game1.playSound(closeSound);
        }
        public override void draw(SpriteBatch b)
        {
            Game1.drawDialogueBox(0, 0, Game1.viewport.Width, Game1.viewport.Height, false, true);
            int x = 0, y = 0;
            foreach (var (key, value) in Game1.player.stats.Values)
            {
                if (!localize.TryGetValue(key, out string local))
                {
                    local = I18n.GetByKey(key);
                    localize.Add(key, local);
                }
                SpriteText.drawString(b, $"{local}: {value}", x * 400 + 50, y * 30 + 100);
                if (y >= 25)
                {
                    y = 0;
                    x++;
                }
                else
                    y++;
            }
            drawMouse(b);
        }
        private List<Item> LoadRequiredItems()
        {
            var items = new List<Item>();

            // 示例：从当前社区中心收集包获取需要的物品
            var missions = MissionManager.MissionProgress;
            /*foreach (var (id, mission) in missions)
            {
                items.Add(new StardewValley.Object(ingredient.index, ingredient.stack));
            }*/

            return items;
        }
        public static Vector2 GetAppropriateMenuPosition()
        {
            Vector2 defaultPosition = new Vector2(Game1.viewport.Width / 2 - menuWidth / 2, Game1.viewport.Height / 2 - menuHeight / 2);

            //Force the viewport into a position that it should fit into on the screen???
            if (defaultPosition.X + menuWidth > Game1.viewport.Width)
            {
                defaultPosition.X = 0;
            }

            if (defaultPosition.Y + menuHeight > Game1.viewport.Height)
            {
                defaultPosition.Y = 0;
            }
            return defaultPosition;
        }
    }
}
