//using HarmonyLib;
//using Microsoft.Xna.Framework.Graphics;
//using StardewValley;
//using StardewValley.Menus;
//using StardropScroll.Helper;

//namespace StardropScroll.Patches
//{
//    [HarmonyPatch]
//    public static class HoverItemDesc
//    {
//        private static Dictionary<string, string> hovered = new();
//        [HarmonyPatch(typeof(IClickableMenu), nameof(IClickableMenu.drawToolTip))]
//        [HarmonyPrefix]
//        internal static void Postfix(SpriteBatch b, ref string hoverTitle, Item hoveredItem)
//        {
//            if (hoveredItem == null)
//                return;

//            string unique = hoveredItem.QualifiedItemId;
//            string name = hoveredItem.Name;
//            if (Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl)
//                && hovered.TryAdd(unique, $"{name}.{hoveredItem.DisplayName}"))
//            {
//                Main.WriteJsonFile(Path.Combine("Assets", "ItemID.json"), hovered);
//            }
//            hoverTitle += $"\n{name} {unique}";
//            // 在 Tooltip 下方添加额外信息
//            /* string extraText = $"[ID: {hoveredItem.QualifiedItemId}]";
//             Vector2 position = new Vector2(Game1 .getMouseX(), Game1.getMouseY() - 50);
//             b.DrawString(Game1.smallFont, extraText, position, Color.White);*/
//        }
//    }
//}
