﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardropScroll.Content.Mission;

namespace StardropScroll.UI
{
    public class MissionMenu : IClickableMenu
    {
        public const int missionsPerPage = 6;

        public const int region_forwardButton = 101;

        public const int region_backButton = 102;

        public const int region_rewardBox = 103;

        public const int region_cancelMissionButton = 104;

        protected List<List<Mission>> pages;

        public List<ClickableComponent> missionLogButtons;

        protected int currentPage;

        protected int missionPage = -1;

        public ClickableTextureComponent forwardButton;

        public ClickableTextureComponent backButton;

        public ClickableTextureComponent rewardBox;

        public ClickableTextureComponent cancelMissionButton;

        protected Mission focus;

        protected List<string> _objectiveText;

        protected float _contentHeight;

        protected float _scissorRectHeight;

        public float scrollAmount;

        public ClickableTextureComponent upArrow;

        public ClickableTextureComponent downArrow;

        public ClickableTextureComponent scrollBar;

        protected bool scrolling;

        public Rectangle scrollBarBounds;

        private string hoverText = "";
        public MissionMenu() : base(0, 0, 0, 0, true)
        {
            Game1.dayTimeMoneyBox.DismissQuestPing();
            Game1.playSound("bigSelect", null);
            PaginateMissions();
            width = 832;
            height = 576;
            if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
            {
                height += 64;
            }
            Vector2 topLeft = Utility.getTopLeftPositionForCenteringOnScreen(width, height, 0, 0);
            xPositionOnScreen = (int)topLeft.X;
            yPositionOnScreen = (int)topLeft.Y + 32;
            missionLogButtons = new List<ClickableComponent>();
            for (int i = 0; i < 6; i++)
            {
                missionLogButtons.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + 16, yPositionOnScreen + 16 + i * ((height - 32) / 6), width - 32, (height - 32) / 6 + 4), i.ToString() ?? "")
                {
                    myID = i,
                    downNeighborID = -7777,
                    upNeighborID = i > 0 ? i - 1 : -1,
                    rightNeighborID = -7777,
                    leftNeighborID = -7777,
                    fullyImmutable = true
                });
            }
            upperRightCloseButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width - 20, yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f, false);
            backButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - 64, yPositionOnScreen + 8, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f, false)
            {
                myID = 102,
                rightNeighborID = -7777
            };
            forwardButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + 64 - 48, yPositionOnScreen + height - 48, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f, false)
            {
                myID = 101
            };
            rewardBox = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width / 2 - 80, yPositionOnScreen + height - 32 - 96, 96, 96), Game1.mouseCursors, new Rectangle(293, 360, 24, 24), 4f, true)
            {
                myID = 103
            };
            cancelMissionButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + 4, yPositionOnScreen + height + 4, 48, 48), Game1.mouseCursors, new Rectangle(322, 498, 12, 12), 4f, true)
            {
                myID = 104
            };
            int scrollbar_x = xPositionOnScreen + width + 16;
            upArrow = new ClickableTextureComponent(new Rectangle(scrollbar_x, yPositionOnScreen + 96, 44, 48), Game1.mouseCursors, new Rectangle(421, 459, 11, 12), 4f, false);
            downArrow = new ClickableTextureComponent(new Rectangle(scrollbar_x, yPositionOnScreen + height - 64, 44, 48), Game1.mouseCursors, new Rectangle(421, 472, 11, 12), 4f, false);
            scrollBarBounds = default;
            scrollBarBounds.X = upArrow.bounds.X + 12;
            scrollBarBounds.Width = 24;
            scrollBarBounds.Y = upArrow.bounds.Y + upArrow.bounds.Height + 4;
            scrollBarBounds.Height = downArrow.bounds.Y - 4 - scrollBarBounds.Y;
            scrollBar = new ClickableTextureComponent(new Rectangle(scrollBarBounds.X, scrollBarBounds.Y, 24, 40), Game1.mouseCursors, new Rectangle(435, 463, 6, 10), 4f, false);
            if (Game1.options.SnappyMenus)
            {
                populateClickableComponentList();
                snapToDefaultClickableComponent();
            }
            exitFunction += CheckClaimed;
        }

        protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
        {
            if (oldID >= 0 && oldID < 6 && missionPage == -1)
            {
                switch (direction)
                {
                    case 1:
                        if (currentPage < pages.Count - 1)
                        {
                            currentlySnappedComponent = getComponentWithID(101);
                            currentlySnappedComponent.leftNeighborID = oldID;
                        }
                        break;
                    case 2:
                        if (oldID < 5 && pages[currentPage].Count - 1 > oldID)
                        {
                            currentlySnappedComponent = getComponentWithID(oldID + 1);
                        }
                        break;
                    case 3:
                        if (currentPage > 0)
                        {
                            currentlySnappedComponent = getComponentWithID(102);
                            currentlySnappedComponent.rightNeighborID = oldID;
                        }
                        break;
                }
            }
            else if (oldID == 102)
            {
                if (missionPage != -1)
                {
                    return;
                }
                currentlySnappedComponent = getComponentWithID(0);
            }
            snapCursorToCurrentSnappedComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            currentlySnappedComponent = getComponentWithID(0);
            snapCursorToCurrentSnappedComponent();
        }

        public override void receiveGamePadButton(Buttons button)
        {
            if (button != Buttons.RightTrigger)
            {
                if (button != Buttons.LeftTrigger)
                {
                    return;
                }
                if (missionPage == -1 && currentPage > 0)
                {
                    NonMissionPageBackButton();
                }
            }
            else if (missionPage == -1 && currentPage < pages.Count - 1)
            {
                NonMissionPageForwardButton();
                return;
            }
        }

        private void PaginateMissions()
        {
            pages = new List<List<Mission>>();
            var missions = MissionManager.Missions.Values.ToList();
            int startIndex = 0;
            while (startIndex < missions.Count)
            {
                List<Mission> page = new();
                int i = 0;
                while (i < 6 && startIndex < missions.Count)
                {
                    page.Add(missions[startIndex]);
                    startIndex++;
                    i++;
                }
                pages.Add(page);
            }
            if (pages.Count == 0)
            {
                pages.Add(new());
            }
            currentPage = Utility.Clamp(currentPage, 0, pages.Count - 1);
            missionPage = -1;
        }

        public bool NeedsScroll()
        {
            return (focus == null || !focus.CanSubmit) && missionPage != -1 && _contentHeight > _scissorRectHeight;
        }

        public override void receiveScrollWheelAction(int direction)
        {
            if (NeedsScroll())
            {
                float new_scroll = scrollAmount - Math.Sign(direction) * 64 / 2;
                if (new_scroll < 0f)
                {
                    new_scroll = 0f;
                }
                if (new_scroll > _contentHeight - _scissorRectHeight)
                {
                    new_scroll = _contentHeight - _scissorRectHeight;
                }
                if (scrollAmount != new_scroll)
                {
                    scrollAmount = new_scroll;
                    Game1.playSound("shiny4", null);
                    SetScrollBarFromAmount();
                }
            }
            base.receiveScrollWheelAction(direction);
        }

        public override void performHoverAction(int x, int y)
        {
            hoverText = "";
            base.performHoverAction(x, y);
            if (missionPage == -1)
            {
                for (int i = 0; i < missionLogButtons.Count; i++)
                {
                    if (pages.Count > 0 && pages[0].Count > i && missionLogButtons[i].containsPoint(x, y) && !missionLogButtons[i].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
                    {
                        Game1.playSound("Cowboy_gunshot", null);
                    }
                }
            }
            forwardButton.tryHover(x, y, 0.2f);
            backButton.tryHover(x, y, 0.2f);
            cancelMissionButton.tryHover(x, y, 0.2f);
            if (NeedsScroll())
            {
                upArrow.tryHover(x, y, 0.1f);
                downArrow.tryHover(x, y, 0.1f);
                scrollBar.tryHover(x, y, 0.1f);
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (Game1.isAnyGamePadButtonBeingPressed() && missionPage != -1 && Game1.options.doesInputListContain(Game1.options.menuButton, key))
            {
                ExitMissionPage();
            }
            else
            {
                base.receiveKeyPress(key);
            }
            if (Game1.options.doesInputListContain(Game1.options.journalButton, key) && readyToClose())
            {
                Game1.exitActiveMenu();
                Game1.playSound("bigDeSelect", null);
            }
        }

        private void NonMissionPageForwardButton()
        {
            currentPage++;
            Game1.playSound("shwip", null);
            if (Game1.options.SnappyMenus && currentPage == pages.Count - 1)
            {
                currentlySnappedComponent = getComponentWithID(0);
                snapCursorToCurrentSnappedComponent();
            }
        }

        private void NonMissionPageBackButton()
        {
            currentPage--;
            Game1.playSound("shwip", null);
            if (Game1.options.SnappyMenus && currentPage == 0)
            {
                currentlySnappedComponent = getComponentWithID(0);
                snapCursorToCurrentSnappedComponent();
            }
        }

        public override void leftClickHeld(int x, int y)
        {
            if (GameMenu.forcePreventClose)
            {
                return;
            }
            base.leftClickHeld(x, y);
            if (scrolling)
            {
                SetScrollFromY(y);
            }
        }

        public override void releaseLeftClick(int x, int y)
        {
            if (GameMenu.forcePreventClose)
            {
                return;
            }
            base.releaseLeftClick(x, y);
            scrolling = false;
        }

        public virtual void SetScrollFromY(int y)
        {
            int y2 = scrollBar.bounds.Y;
            float percentage = (y - scrollBarBounds.Y) / (float)(scrollBarBounds.Height - scrollBar.bounds.Height);
            percentage = Utility.Clamp(percentage, 0f, 1f);
            scrollAmount = percentage * (_contentHeight - _scissorRectHeight);
            SetScrollBarFromAmount();
            if (y2 != scrollBar.bounds.Y)
            {
                Game1.playSound("shiny4", null);
            }
        }

        public void UpArrowPressed()
        {
            upArrow.scale = upArrow.baseScale;
            scrollAmount -= 64f;
            if (scrollAmount < 0f)
            {
                scrollAmount = 0f;
            }
            SetScrollBarFromAmount();
        }

        public void DownArrowPressed()
        {
            downArrow.scale = downArrow.baseScale;
            scrollAmount += 64f;
            if (scrollAmount > _contentHeight - _scissorRectHeight)
            {
                scrollAmount = _contentHeight - _scissorRectHeight;
            }
            SetScrollBarFromAmount();
        }

        private void SetScrollBarFromAmount()
        {
            if (!NeedsScroll())
            {
                scrollAmount = 0f;
                return;
            }
            if (scrollAmount < 8f)
            {
                scrollAmount = 0f;
            }
            if (scrollAmount > _contentHeight - _scissorRectHeight - 8f)
            {
                scrollAmount = _contentHeight - _scissorRectHeight;
            }
            scrollBar.bounds.Y = (int)(scrollBarBounds.Y + (scrollBarBounds.Height - scrollBar.bounds.Height) / Math.Max(1f, _contentHeight - _scissorRectHeight) * scrollAmount);
        }

        public override void applyMovementKey(int direction)
        {
            base.applyMovementKey(direction);
            if (NeedsScroll())
            {
                if (direction == 0)
                {
                    UpArrowPressed();
                    return;
                }
                if (direction != 2)
                {
                    return;
                }
                DownArrowPressed();
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if (Game1.activeClickableMenu == null)
            {
                return;
            }
            if (missionPage != -1)
            {
                int yOffset = 0;
                if (missionPage != -1 && focus.CanSubmit && !focus.Claimed && rewardBox.containsPoint(x, y + yOffset))
                {
                    focus.OnMoneyRewardClaimed();
                }
                else if (!NeedsScroll() || backButton.containsPoint(x, y))
                {
                    ExitMissionPage();
                }
                if (NeedsScroll())
                {
                    if (downArrow.containsPoint(x, y) && scrollAmount < _contentHeight - _scissorRectHeight)
                    {
                        DownArrowPressed();
                        Game1.playSound("shwip", null);
                        return;
                    }
                    if (upArrow.containsPoint(x, y) && scrollAmount > 0f)
                    {
                        UpArrowPressed();
                        Game1.playSound("shwip", null);
                        return;
                    }
                    if (scrollBar.containsPoint(x, y))
                    {
                        scrolling = true;
                        return;
                    }
                    if (scrollBarBounds.Contains(x, y))
                    {
                        scrolling = true;
                        return;
                    }
                    if (!downArrow.containsPoint(x, y) && x > xPositionOnScreen + width && x < xPositionOnScreen + width + 128 && y > yPositionOnScreen && y < yPositionOnScreen + height)
                    {
                        scrolling = true;
                        leftClickHeld(x, y);
                        releaseLeftClick(x, y);
                    }
                }
                return;
            }
            for (int i = 0; i < missionLogButtons.Count; i++)
            {
                if (pages.Count > 0 && pages[currentPage].Count > i && missionLogButtons[i].containsPoint(x, y))
                {
                    Game1.playSound("smallSelect", null);
                    missionPage = i;
                    focus = pages[currentPage][i];
                    _objectiveText = focus.GetObjectiveDescriptions();
                    scrollAmount = 0f;
                    SetScrollBarFromAmount();
                    if (Game1.options.SnappyMenus)
                    {
                        currentlySnappedComponent = getComponentWithID(102);
                        currentlySnappedComponent.rightNeighborID = -7777;
                        currentlySnappedComponent.downNeighborID = 103;
                        snapCursorToCurrentSnappedComponent();
                    }
                    return;
                }
            }
            if (currentPage < pages.Count - 1 && forwardButton.containsPoint(x, y))
            {
                NonMissionPageForwardButton();
                return;
            }
            if (currentPage > 0 && backButton.containsPoint(x, y))
            {
                NonMissionPageBackButton();
                return;
            }
            Game1.playSound("bigDeSelect", null);
            exitThisMenu(true);
        }

        public void ExitMissionPage()
        {
            missionPage = -1;
            focus.OnLeaveQuestPage();
            PaginateMissions();
            Game1.playSound("shwip", null);
            if (Game1.options.SnappyMenus)
            {
                snapToDefaultClickableComponent();
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);
            if (missionPage != -1)
            {
                rewardBox.scale = rewardBox.baseScale + Game1.dialogueButtonScale / 20f;
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.options.showClearBackgrounds)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            }
            SpriteText.drawStringWithScrollCenteredAt(b, I18n.Mission(), xPositionOnScreen + width / 2, yPositionOnScreen - 64, "", 1f, null, 0, 0.88f, false);
            if (missionPage == -1)
            {
                drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, height, Color.White, 4f, true, -1f);
                for (int i = 0; i < missionLogButtons.Count; i++)
                {
                    if (pages.Count > 0 && pages[currentPage].Count > i)
                    {
                        ClickableComponent button = missionLogButtons[i];
                        Mission mission = pages[currentPage][i];
                        drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 396, 15, 15), button.bounds.X, button.bounds.Y, button.bounds.Width, button.bounds.Height, button.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White, 4f, false, -1f);
                        if (mission.CanSubmit)
                        {
                            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(button.bounds.X + 64 + 4, button.bounds.Y + 44), new Rectangle(341, 410, 23, 9), Color.White, 0f, new Vector2(11f, 4f), 4f + Game1.dialogueButtonScale * 10f / 250f, false, 0.99f, -1, -1, 0.35f);
                        }
                        else if (!mission.Completed)
                        {
                            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(button.bounds.X + 32, button.bounds.Y + 28), new Rectangle(395, 497, 3, 8), Color.White, 0f, Vector2.Zero, 4f, false, 0.99f, -1, -1, 0.35f);
                        }
                        SpriteText.drawString(b, mission.GetName(), button.bounds.X + 128 + 4, button.bounds.Y + 20, 999999, -1, 999999, 1f, 0.88f, false, -1, "", null, SpriteText.ScrollTextAlignment.Left);
                    }
                }
            }
            else
            {
                int titleWidth = SpriteText.getWidthOfString(focus.GetName(), 999999);
                if (titleWidth > width / 2)
                {
                    drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, height + 48, Color.White, 4f, true, -1f);
                    SpriteText.drawStringHorizontallyCenteredAt(b, focus.GetName(), xPositionOnScreen + width / 2, yPositionOnScreen + 32, 999999, -1, 999999, 1f, 0.88f, false, null, 99999);
                }
                else
                {
                    drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), xPositionOnScreen, yPositionOnScreen, width, height, Color.White, 4f, true, -1f);
                    SpriteText.drawStringHorizontallyCenteredAt(b, focus.GetName(), xPositionOnScreen + width / 2, yPositionOnScreen + 32, 999999, -1, 999999, 1f, 0.88f, false, null, 99999);
                }
                float extraYOffset = 0f;
                string description = Game1.parseText(focus.GetDescription(), Game1.dialogueFont, width - 128);
                Rectangle cached_scissor_rect = b.GraphicsDevice.ScissorRectangle;
                Vector2 description_size = Game1.dialogueFont.MeasureString(description);
                Rectangle scissor_rect = default;
                scissor_rect.X = xPositionOnScreen + 32;
                scissor_rect.Y = yPositionOnScreen + 96 + (int)extraYOffset;
                scissor_rect.Height = yPositionOnScreen + height - 32 - scissor_rect.Y;
                scissor_rect.Width = width - 64;
                _scissorRectHeight = scissor_rect.Height;
                scissor_rect = Utility.ConstrainScissorRectToScreen(scissor_rect);
                b.End();
                b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState
                {
                    ScissorTestEnable = true
                }, null, null);
                Game1.graphics.GraphicsDevice.ScissorRectangle = scissor_rect;
                Utility.drawTextWithShadow(b, description, Game1.dialogueFont, new Vector2(xPositionOnScreen + 64, yPositionOnScreen - scrollAmount + 96f + extraYOffset), Game1.textColor, 1f, -1f, -1, -1, 1f, 3);
                float yPos = yPositionOnScreen + 96 + description_size.Y + 32f - scrollAmount + extraYOffset;
                if (focus.CanSubmit)
                {
                    b.End();
                    b.GraphicsDevice.ScissorRectangle = cached_scissor_rect;
                    b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);
                    SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11376"), xPositionOnScreen + 32 + 4, rewardBox.bounds.Y + 21 + 4 + (int)extraYOffset, 999999, -1, 999999, 1f, 0.88f, false, -1, "", null, SpriteText.ScrollTextAlignment.Left);
                    rewardBox.draw(b, Color.White, 0.9f, 0, 0, (int)extraYOffset);
                    if (!focus.Claimed)
                    {
                        b.Draw(Game1.mouseCursors, new Vector2(rewardBox.bounds.X + 16, rewardBox.bounds.Y + 16 - Game1.dialogueButtonScale / 2f + extraYOffset), new Rectangle?(new Rectangle(280, 410, 16, 16)), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                        SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", focus.Money), xPositionOnScreen + 448, rewardBox.bounds.Y + 21 + 4 + (int)extraYOffset, 999999, -1, 999999, 1f, 0.88f, false, -1, "", null, SpriteText.ScrollTextAlignment.Left);
                    }
                }
                else
                {
                    var objectives = focus.GetObjectives();
                    for (int j = 0; j < _objectiveText.Count; j++)
                    {
                        string text = _objectiveText[j];
                        int text_width = width - 192;
                        string parsed_text = Game1.parseText(text, Game1.dialogueFont, text_width);
                        int current = objectives[j].Current;
                        int target = objectives[j].Target;
                        bool canSubmit = current >= target;
                        Color text_color = Game1.unselectedOptionColor;
                        if (!canSubmit)
                        {
                            text_color = Color.DarkBlue;
                            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(xPositionOnScreen + 96 + 8f * Game1.dialogueButtonScale / 10f, yPos), new Rectangle(412, 495, 5, 4), Color.White, 1.5707964f, Vector2.Zero, -1f, false, -1f, -1, -1, 0.35f);
                        }
                        Utility.drawTextWithShadow(b, parsed_text, Game1.dialogueFont, new Vector2(xPositionOnScreen + 128, yPos - 8f), text_color, 1f, -1f, -1, -1, 1f, 3);
                        yPos += Game1.dialogueFont.MeasureString(parsed_text).Y;
                        Color dark_bar_color = Color.DarkRed;
                        Color bar_color = Color.Red;
                        if (canSubmit)
                        {
                            bar_color = Color.LimeGreen;
                            dark_bar_color = Color.Green;
                        }
                        int inset = 64;
                        int objective_count_draw_width = 160;
                        int notches = Math.Min(target, 4);
                        Rectangle bar_background_source = new(0, 224, 47, 12);
                        Rectangle bar_notch_source = new(47, 224, 1, 12);
                        int bar_horizontal_padding = 3;
                        int bar_vertical_padding = 3;
                        int slice_width = 5;
                        string objective_count_text = $"{current}/{target}";
                        int max_text_width = (int)Game1.dialogueFont.MeasureString(objective_count_text).X;
                        int count_text_width = (int)Game1.dialogueFont.MeasureString(objective_count_text).X;
                        int text_draw_position = xPositionOnScreen + width - inset - count_text_width;
                        int max_text_draw_position = xPositionOnScreen + width - inset - max_text_width;
                        Utility.drawTextWithShadow(b, objective_count_text, Game1.dialogueFont, new Vector2(text_draw_position, yPos), Color.DarkBlue, 1f, -1f, -1, -1, 1f, 3);
                        Rectangle bar_draw_position = new(xPositionOnScreen + inset, (int)yPos, width - inset * 2 - objective_count_draw_width, bar_background_source.Height * 4);
                        if (bar_draw_position.Right > max_text_draw_position - 16)
                        {
                            int adjustment = bar_draw_position.Right - (max_text_draw_position - 16);
                            bar_draw_position.Width -= adjustment;
                        }
                        b.Draw(Game1.mouseCursors2, new Rectangle(bar_draw_position.X, bar_draw_position.Y, slice_width * 4, bar_draw_position.Height), new Rectangle?(new Rectangle(bar_background_source.X, bar_background_source.Y, slice_width, bar_background_source.Height)), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                        b.Draw(Game1.mouseCursors2, new Rectangle(bar_draw_position.X + slice_width * 4, bar_draw_position.Y, bar_draw_position.Width - 2 * slice_width * 4, bar_draw_position.Height), new Rectangle?(new Rectangle(bar_background_source.X + slice_width, bar_background_source.Y, bar_background_source.Width - 2 * slice_width, bar_background_source.Height)), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                        b.Draw(Game1.mouseCursors2, new Rectangle(bar_draw_position.Right - slice_width * 4, bar_draw_position.Y, slice_width * 4, bar_draw_position.Height), new Rectangle?(new Rectangle(bar_background_source.Right - slice_width, bar_background_source.Y, slice_width, bar_background_source.Height)), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                        float mission_progress = target == 0 ? 1 : (current / (float)target);
                        bar_draw_position.X += 4 * bar_horizontal_padding;
                        bar_draw_position.Width -= 4 * bar_horizontal_padding * 2;
                        for (int k = 1; k < notches; k++)
                        {
                            b.Draw(Game1.mouseCursors2, new Vector2(bar_draw_position.X + bar_draw_position.Width * (k / (float)notches), bar_draw_position.Y), new Rectangle?(bar_notch_source), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);
                        }
                        bar_draw_position.Y += 4 * bar_vertical_padding;
                        bar_draw_position.Height -= 4 * bar_vertical_padding * 2;
                        Rectangle rect = new(bar_draw_position.X, bar_draw_position.Y, (int)(bar_draw_position.Width * mission_progress) - 4, bar_draw_position.Height);
                        b.Draw(Game1.staminaRect, rect, null, bar_color, 0f, Vector2.Zero, SpriteEffects.None, rect.Y / 10000f);
                        rect.X = rect.Right;
                        rect.Width = 4;
                        b.Draw(Game1.staminaRect, rect, null, dark_bar_color, 0f, Vector2.Zero, SpriteEffects.None, rect.Y / 10000f);
                        yPos += (bar_background_source.Height + 4) * 4;
                        _contentHeight = yPos + scrollAmount - scissor_rect.Y;
                    }
                    b.End();
                    b.GraphicsDevice.ScissorRectangle = cached_scissor_rect;
                    b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

                    if (NeedsScroll())
                    {
                        if (scrollAmount > 0f)
                        {
                            b.Draw(Game1.staminaRect, new Rectangle(scissor_rect.X, scissor_rect.Top, scissor_rect.Width, 4), Color.Black * 0.15f);
                        }
                        if (scrollAmount < _contentHeight - _scissorRectHeight)
                        {
                            b.Draw(Game1.staminaRect, new Rectangle(scissor_rect.X, scissor_rect.Bottom - 4, scissor_rect.Width, 4), Color.Black * 0.15f);
                        }
                    }
                }
            }
            if (NeedsScroll())
            {
                upArrow.draw(b);
                downArrow.draw(b);
                scrollBar.draw(b);
            }
            if (currentPage < pages.Count - 1 && missionPage == -1)
            {
                forwardButton.draw(b);
            }
            if (currentPage > 0 || missionPage != -1)
            {
                backButton.draw(b);
            }
            base.draw(b);
            Game1.mouseCursorTransparency = 1f;
            drawMouse(b, false, -1);
            if (hoverText.Length > 0)
            {
                drawHoverText(b, hoverText, Game1.dialogueFont, 0, 0, -1, null, -1, null, null, 0, null, -1, -1, -1, 1f, null, null, null, null, null, null, 1f, -1, -1);
            }
        }
        public void CheckClaimed()
        {
            foreach (var page in pages)
            {
                foreach (var m in page)
                {
                    if (m.Claimed)
                    {
                        MissionManager.SubmitMission(m);
                    }
                }
            }
        }
    }
}
