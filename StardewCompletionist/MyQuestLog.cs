using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Quests;
using StardewValley.SpecialOrders.Objectives;
using StardewValley.SpecialOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Netcode;

namespace StardewCompletionist;

public class MyQuestLog : IClickableMenu
{
    public readonly QuestPageManager _questPageManager = new();
    public class QuestPageManager
    {
        protected List<List<IQuest>> _pages = new();

        /// <summary>Get the paginated list of quests which should be shown in the quest log.</summary>
        public virtual void paginateQuests()
        {
            _pages = new List<List<IQuest>>();
            IList<IQuest> quests = GetAllQuests();
            int startIndex = 0;
            while (startIndex < quests.Count)
            {
                var page = new List<IQuest>();
                for (int i = 0; i < 6; i++)
                {
                    if (startIndex >= quests.Count)
                    {
                        break;
                    }
                    page.Add(quests[startIndex]);
                    startIndex++;
                }
                _pages.Add(page);
            }
            if (_pages.Count == 0)
            {
                _pages.Add(new List<IQuest>());
            }

            _currentPage = Utility.Clamp(_currentPage, 0, _pages.Count - 1);

            ClearCurrentQuestOnPage();
        }
        /// <summary>Get the quests which should be shown in the quest log.</summary>
        private static IList<IQuest> GetAllQuests()
        {
            var quests = new List<IQuest>();
            for (int j = Game1.player.team.specialOrders.Count - 1; j >= 0; j--)
            {
                SpecialOrder order = Game1.player.team.specialOrders[j];
                if (!order.IsHidden())
                {
                    quests.Add(order);
                }
            }
            for (int i = Game1.player.questLog.Count - 1; i >= 0; i--)
            {
                Quest quest = Game1.player.questLog[i];
                if (quest == null || (bool)quest.destroy)
                {
                    Game1.player.questLog.RemoveAt(i);
                }
                else if (!quest.IsHidden())
                {
                    quests.Add(quest);
                }
            }
            return quests;
        }



        protected int _currentPage;

        public List<IQuest> GetCurrentPage()
        {
            return _pages[_currentPage];
        }

        public bool IsOnLastPage()
        {
            return _currentPage == _pages.Count - 1;
        }
        public bool IsNotOnLastPage()
        {
            return _currentPage < _pages.Count - 1;
        }

        public bool IsOnFirstPage()
        {
            return _currentPage == 0;
        }
        public bool IsNotOnFirstPage()
        {
            return _currentPage > 0;
        }

        public bool HasAnyPages()
        {
            return _pages.Count > 0;
        }

        public int NumberOfQuestsOnFirstPage()
        {
            return _pages[0].Count;
        }

        public void MoveToNextPage()
        {
            _currentPage++;
        }
        public void MoveToPreviousPage()
        {
            _currentPage--;
        }



        protected int _questPage = -1;

        public bool IsNotCurrentQuestOnPageSet()
        {
            return _questPage == -1;
        }
        public bool IsCurrentQuestOnPageSet()
        {
            return _questPage != -1;
        }

        public void ClearCurrentQuestOnPage()
        {
            _questPage = -1;
        }
        public void SetCurrentQuestOnPage(int i)
        {
            _questPage = i;
        }

        public IQuest GetCurrentQuestOnPage()
        {
            return GetCurrentPage()[_questPage];
        }
        public void RemoveCurrentQuestOnPage()
        {
            GetCurrentPage().RemoveAt(_questPage);
        }



        protected IQuest? _shownQuest;

        public void SetShownQuest(IQuest quest)
        {
            _shownQuest = quest;
        }
        public IQuest GetShownQuest()
        {
            return _shownQuest;
        }
    }

    private readonly HoverDrawHelper _hoverDrawHelper = new();
    public class HoverDrawHelper
    {
        private string _hoverText = "";

        public void SetHoverText(string text)
        {
            _hoverText = text;
        }

        public void ClearHoverText()
        {
            SetHoverText(string.Empty);
        }

        public void DrawHoverTextIfSet(SpriteBatch b)
        {
            if (_hoverText.Length > 0)
            {
                IClickableMenu.drawHoverText(b, _hoverText, Game1.dialogueFont);
            }
        }
    }

    public readonly ScrollbarHelper _scrollbarHelper = new();
    public class ScrollbarHelper
    {
        public void InitializeScrollbarComponents(int xPositionOnScreen, int yPositionOnScreen, int width, int height)
        {
            int scrollbar_x = xPositionOnScreen + width + 16;
            _upArrow = new ClickableTextureComponent(
                new Rectangle(scrollbar_x, yPositionOnScreen + 96, 44, 48),
                Game1.mouseCursors,
                new Rectangle(421, 459, 11, 12),
                4f);
            _downArrow = new ClickableTextureComponent(
                new Rectangle(scrollbar_x, yPositionOnScreen + height - 64, 44, 48),
                Game1.mouseCursors,
                new Rectangle(421, 472, 11, 12),
                4f);

            _scrollBarBounds = default(Rectangle);
            _scrollBarBounds.X = _upArrow.bounds.X + 12;
            _scrollBarBounds.Width = 24;
            _scrollBarBounds.Y = _upArrow.bounds.Y + _upArrow.bounds.Height + 4;
            _scrollBarBounds.Height = _downArrow.bounds.Y - 4 - _scrollBarBounds.Y;

            _scrollBar = new ClickableTextureComponent(
                new Rectangle(_scrollBarBounds.X, _scrollBarBounds.Y, 24, 40),
                Game1.mouseCursors,
                new Rectangle(435, 463, 6, 10),
                4f);
        }


        private ClickableTextureComponent _upArrow;
        public ClickableTextureComponent GetUpArrowComponent()
        {
            return _upArrow;
        }
        public void ResetUpArrowScaleToBaseScale()
        {
            _upArrow.scale = _upArrow.baseScale;
        }


        private ClickableTextureComponent _downArrow;
        public ClickableTextureComponent GetDownArrowComponent()
        {
            return _downArrow;
        }
        public void ResetDownArrowScaleToBaseScale()
        {
            _downArrow.scale = _downArrow.baseScale;
        }


        private ClickableTextureComponent _scrollBar;
        public ClickableTextureComponent GetScrollBarComponent()
        {
            return _scrollBar;
        }


        private Rectangle _scrollBarBounds;
        public Rectangle GetScrollBarBounds()
        {
            return _scrollBarBounds;
        }


        private float _scrollAmount;
        public void SetScrollAmountToZero()
        {
            _scrollAmount = 0f;
        }
        public void SetScrollAmount(float amount)
        {
            _scrollAmount = amount;
        }
        public float GetScrollAmount()
        {
            return _scrollAmount;
        }
        public void DecrementScrollAmountBy(float amount)
        {
            _scrollAmount -= amount;
        }
        public void IncrementScrollAmountBy(float amount)
        {
            _scrollAmount += amount;
        }


        private bool _isScrolling;
        public bool IsScrolling()
        {
            return _isScrolling;
        }
        public void SetIsScrolling(bool value = true)
        {
            _isScrolling = value;
        }
    }






    public List<ClickableComponent> questLogButtons;

    //public const int region_forwardButton = 101; // unused?
    //public const int region_backButton = 102; // unused?
    //public const int region_rewardBox = 103; // unused?
    //public const int region_cancelQuestButton = 104; // unused?
    public ClickableTextureComponent forwardButton;
    public ClickableTextureComponent backButton;
    public ClickableTextureComponent rewardBox;
    public ClickableTextureComponent cancelQuestButton;

    protected List<string> _objectiveText;
    protected float _contentHeight; // only set when drawing selected quest screen
    protected float _scissorRectHeight; // only set when drawing selected quest screen

    public MyQuestLog()
        : base(0, 0, 0, 0, showUpperRightCloseButton: true)
    {
        Game1.dayTimeMoneyBox.DismissQuestPing();
        Game1.playSound("bigSelect");

        this._questPageManager.paginateQuests();

        base.width = 832;
        base.height = 576;
        if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ko || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.fr)
        {
            base.height += 64;
        }

        Vector2 topLeft = Utility.getTopLeftPositionForCenteringOnScreen(base.width, base.height);
        base.xPositionOnScreen = (int)topLeft.X;
        base.yPositionOnScreen = (int)topLeft.Y + 32;

        this.questLogButtons = new List<ClickableComponent>();
        for (int i = 0; i < 6; i++)
        {
            this.questLogButtons.Add(new ClickableComponent(new Rectangle(base.xPositionOnScreen + 16, base.yPositionOnScreen + 16 + i * ((base.height - 32) / 6), base.width - 32, (base.height - 32) / 6 + 4), i.ToString() ?? "")
            {
                myID = i,
                downNeighborID = -7777,
                upNeighborID = ((i > 0) ? (i - 1) : (-1)),
                rightNeighborID = -7777,
                leftNeighborID = -7777,
                fullyImmutable = true
            });
        }

        base.upperRightCloseButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width - 20, base.yPositionOnScreen - 8, 48, 48), Game1.mouseCursors, new Rectangle(337, 494, 12, 12), 4f);
        this.backButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen - 64, base.yPositionOnScreen + 8, 48, 44), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f)
        {
            myID = 102,
            rightNeighborID = -7777
        };
        this.forwardButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width + 64 - 48, base.yPositionOnScreen + base.height - 48, 48, 44), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f)
        {
            myID = 101
        };
        this.rewardBox = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + base.width / 2 - 80, base.yPositionOnScreen + base.height - 32 - 96, 96, 96), Game1.mouseCursors, new Rectangle(293, 360, 24, 24), 4f, drawShadow: true)
        {
            myID = 103
        };
        this.cancelQuestButton = new ClickableTextureComponent(new Rectangle(base.xPositionOnScreen + 4, base.yPositionOnScreen + base.height + 4, 48, 48), Game1.mouseCursors, new Rectangle(322, 498, 12, 12), 4f, drawShadow: true)
        {
            myID = 104
        };

        _scrollbarHelper.InitializeScrollbarComponents(base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height);

        if (Game1.options.SnappyMenus)
        {
            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }
    }





    #region snap behavior (I think this jumps cursor to clickable components for controller input)

    protected override void customSnapBehavior(int direction, int oldRegion, int oldID)
    {
        if (oldID >= 0 && oldID < 6 && _questPageManager.IsNotCurrentQuestOnPageSet())
        {
            switch (direction)
            {
                case 2:
                    if (oldID < 5 && _questPageManager.GetCurrentPage().Count - 1 > oldID)
                    {
                        base.currentlySnappedComponent = base.getComponentWithID(oldID + 1);
                    }
                    break;
                case 1:
                    if (_questPageManager.IsNotOnLastPage())
                    {
                        base.currentlySnappedComponent = base.getComponentWithID(101);
                        base.currentlySnappedComponent.leftNeighborID = oldID;
                    }
                    break;
                case 3:
                    if (_questPageManager.IsNotOnFirstPage())
                    {
                        base.currentlySnappedComponent = base.getComponentWithID(102);
                        base.currentlySnappedComponent.rightNeighborID = oldID;
                    }
                    break;
            }
        }
        else if (oldID == 102)
        {
            if (_questPageManager.IsCurrentQuestOnPageSet())
            {
                return;
            }
            base.currentlySnappedComponent = base.getComponentWithID(0);
        }
        this.snapCursorToCurrentSnappedComponent();
    }

    public override void snapToDefaultClickableComponent()
    {
        base.currentlySnappedComponent = base.getComponentWithID(0);
        this.snapCursorToCurrentSnappedComponent();
    }

    #endregion





    #region handle user input

    public override void receiveGamePadButton(Buttons b)
    {
        switch (b)
        {
            case Buttons.RightTrigger:
                if (_questPageManager.IsNotCurrentQuestOnPageSet() && _questPageManager.IsNotOnLastPage())
                {
                    this.nonQuestPageForwardButton();
                }
                break;
            case Buttons.LeftTrigger:
                if (_questPageManager.IsNotCurrentQuestOnPageSet() && _questPageManager.IsNotOnFirstPage())
                {
                    this.nonQuestPageBackButton();
                }
                break;
        }
    }

    private void SetScrollBarFromAmount()
    {
        if (!this.NeedsScroll())
        {
            _scrollbarHelper.SetScrollAmountToZero();
            return;
        }
        if (_scrollbarHelper.GetScrollAmount() < 8f)
        {
            _scrollbarHelper.SetScrollAmountToZero();
        }
        if (_scrollbarHelper.GetScrollAmount() > this._contentHeight - this._scissorRectHeight - 8f)
        {
            _scrollbarHelper.SetScrollAmount(this._contentHeight - this._scissorRectHeight);
        }
        _scrollbarHelper.GetScrollBarComponent().bounds.Y = (int)((float)_scrollbarHelper.GetScrollBarBounds().Y + (float)(_scrollbarHelper.GetScrollBarBounds().Height - _scrollbarHelper.GetScrollBarComponent().bounds.Height) / Math.Max(1f, this._contentHeight - this._scissorRectHeight) * _scrollbarHelper.GetScrollAmount());
    }

    public override void receiveScrollWheelAction(int direction)
    {
        if (this.NeedsScroll())
        {
            float new_scroll = _scrollbarHelper.GetScrollAmount() - (float)(Math.Sign(direction) * 64 / 2);
            if (new_scroll < 0f)
            {
                new_scroll = 0f;
            }
            if (new_scroll > this._contentHeight - this._scissorRectHeight)
            {
                new_scroll = this._contentHeight - this._scissorRectHeight;
            }
            if (_scrollbarHelper.GetScrollAmount() != new_scroll)
            {
                _scrollbarHelper.SetScrollAmount(new_scroll);
                Game1.playSound("shiny4");
                this.SetScrollBarFromAmount();
            }
        }
        base.receiveScrollWheelAction(direction);
    }

    public override void receiveRightClick(int x, int y, bool playSound = true) { }

    public override void performHoverAction(int x, int y)
    {
        _hoverDrawHelper.ClearHoverText();
        base.performHoverAction(x, y);

        if (_questPageManager.IsNotCurrentQuestOnPageSet())
        {
            for (int i = 0; i < this.questLogButtons.Count; i++)
            {
                if (_questPageManager.HasAnyPages() && _questPageManager.NumberOfQuestsOnFirstPage() > i && this.questLogButtons[i].containsPoint(x, y) && !this.questLogButtons[i].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()))
                {
                    Game1.playSound("Cowboy_gunshot");
                }
            }
        }
        else if (_questPageManager.GetShownQuest().CanBeCancelled() && this.cancelQuestButton.containsPoint(x, y))
        {
            _hoverDrawHelper.SetHoverText(Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11364"));
        }

        this.forwardButton.tryHover(x, y, 0.2f);
        this.backButton.tryHover(x, y, 0.2f);
        this.cancelQuestButton.tryHover(x, y, 0.2f);

        if (this.NeedsScroll())
        {
            _scrollbarHelper.GetUpArrowComponent().tryHover(x, y);
            _scrollbarHelper.GetDownArrowComponent().tryHover(x, y);
            _scrollbarHelper.GetScrollBarComponent().tryHover(x, y);
        }
    }

    public override void receiveKeyPress(Keys key)
    {
        if (Game1.isAnyGamePadButtonBeingPressed() && _questPageManager.IsCurrentQuestOnPageSet() && Game1.options.doesInputListContain(Game1.options.menuButton, key))
        {
            this.exitQuestPage();
        }
        else
        {
            base.receiveKeyPress(key);
        }
        if (Game1.options.doesInputListContain(Game1.options.journalButton, key) && this.readyToClose())
        {
            Game1.exitActiveMenu();
            Game1.playSound("bigDeSelect");
        }
    }

    private void nonQuestPageForwardButton()
    {
        _questPageManager.MoveToNextPage();
        Game1.playSound("shwip");
        if (Game1.options.SnappyMenus && _questPageManager.IsOnLastPage())
        {
            base.currentlySnappedComponent = base.getComponentWithID(0);
            this.snapCursorToCurrentSnappedComponent();
        }
    }

    private void nonQuestPageBackButton()
    {
        _questPageManager.MoveToPreviousPage();
        Game1.playSound("shwip");
        if (Game1.options.SnappyMenus && _questPageManager.IsOnFirstPage())
        {
            base.currentlySnappedComponent = base.getComponentWithID(0);
            this.snapCursorToCurrentSnappedComponent();
        }
    }

    public override void leftClickHeld(int x, int y)
    {
        if (!GameMenu.forcePreventClose)
        {
            base.leftClickHeld(x, y);
            if (_scrollbarHelper.IsScrolling())
            {
                this.SetScrollFromY(y);
            }
        }
    }

    public override void releaseLeftClick(int x, int y)
    {
        if (!GameMenu.forcePreventClose)
        {
            base.releaseLeftClick(x, y);
            _scrollbarHelper.SetIsScrolling(false);
        }
    }

    public virtual void SetScrollFromY(int y)
    {
        int y2 = _scrollbarHelper.GetScrollBarComponent().bounds.Y;
        float percentage = (float)(y - _scrollbarHelper.GetScrollBarBounds().Y) / (float)(_scrollbarHelper.GetScrollBarBounds().Height - _scrollbarHelper.GetScrollBarComponent().bounds.Height);
        percentage = Utility.Clamp(percentage, 0f, 1f);
        _scrollbarHelper.SetScrollAmount(percentage * (this._contentHeight - this._scissorRectHeight));
        this.SetScrollBarFromAmount();
        if (y2 != _scrollbarHelper.GetScrollBarComponent().bounds.Y)
        {
            Game1.playSound("shiny4");
        }
    }

    public void UpArrowPressed()
    {
        _scrollbarHelper.ResetUpArrowScaleToBaseScale();
        _scrollbarHelper.DecrementScrollAmountBy(64f);
        if (_scrollbarHelper.GetScrollAmount() < 0f)
        {
            _scrollbarHelper.SetScrollAmountToZero();
        }
        this.SetScrollBarFromAmount();
    }

    public void DownArrowPressed()
    {
        _scrollbarHelper.ResetDownArrowScaleToBaseScale();
        _scrollbarHelper.IncrementScrollAmountBy(64f);
        if (_scrollbarHelper.GetScrollAmount() > this._contentHeight - this._scissorRectHeight)
        {
            _scrollbarHelper.SetScrollAmount(this._contentHeight - this._scissorRectHeight);
        }
        this.SetScrollBarFromAmount();
    }

    public override void applyMovementKey(int direction)
    {
        base.applyMovementKey(direction);
        if (this.NeedsScroll())
        {
            switch (direction)
            {
                case 0:
                    this.UpArrowPressed();
                    break;
                case 2:
                    this.DownArrowPressed();
                    break;
            }
        }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        base.receiveLeftClick(x, y, playSound);
        if (Game1.activeClickableMenu == null)
        {
            return;
        }

        if (_questPageManager.IsNotCurrentQuestOnPageSet())
        {
            for (int i = 0; i < this.questLogButtons.Count; i++)
            {
                if (_questPageManager.HasAnyPages() && _questPageManager.GetCurrentPage().Count > i && this.questLogButtons[i].containsPoint(x, y))
                {
                    Game1.playSound("smallSelect");
                    _questPageManager.SetCurrentQuestOnPage(i);
                    _questPageManager.SetShownQuest(_questPageManager.GetCurrentPage()[i]);
                    this._objectiveText = _questPageManager.GetShownQuest().GetObjectiveDescriptions();
                    _questPageManager.GetShownQuest().MarkAsViewed();
                    _scrollbarHelper.SetScrollAmountToZero();
                    this.SetScrollBarFromAmount();
                    if (Game1.options.SnappyMenus)
                    {
                        base.currentlySnappedComponent = base.getComponentWithID(102);
                        base.currentlySnappedComponent.rightNeighborID = -7777;
                        base.currentlySnappedComponent.downNeighborID = (_questPageManager.GetShownQuest().HasMoneyReward()
                            ? 103
                            : (_questPageManager.GetShownQuest().CanBeCancelled()
                                ? 104
                                : (-1)));
                        this.snapCursorToCurrentSnappedComponent();
                    }
                    return;
                }
            }
            if (_questPageManager.IsNotOnLastPage() && this.forwardButton.containsPoint(x, y))
            {
                this.nonQuestPageForwardButton();
                return;
            }
            if (_questPageManager.IsNotOnFirstPage() && this.backButton.containsPoint(x, y))
            {
                this.nonQuestPageBackButton();
                return;
            }
            Game1.playSound("bigDeSelect");
            base.exitThisMenu();
            return;
        }

        Quest quest = _questPageManager.GetShownQuest() as Quest;
        int yOffset = (_questPageManager.GetShownQuest().IsTimedQuest()
            && _questPageManager.GetShownQuest().GetDaysLeft() > 0
            && SpriteText.getWidthOfString(_questPageManager.GetShownQuest().GetName()) > base.width / 2)
            ? (-48)
            : 0;
        if (_questPageManager.IsCurrentQuestOnPageSet()
            && _questPageManager.GetShownQuest().ShouldDisplayAsComplete()
            && _questPageManager.GetShownQuest().HasMoneyReward()
            && this.rewardBox.containsPoint(x, y + yOffset))
        {
            Game1.player.Money += _questPageManager.GetShownQuest().GetMoneyReward();
            Game1.playSound("purchaseRepeat");
            _questPageManager.GetShownQuest().OnMoneyRewardClaimed();
        }
        else if (_questPageManager.IsCurrentQuestOnPageSet()
            && quest != null
            && !quest.completed
            && (bool)quest.canBeCancelled
            && this.cancelQuestButton.containsPoint(x, y))
        {
            quest.accepted.Value = false;
            if (quest.dailyQuest.Value && quest.dayQuestAccepted.Value == Game1.Date.TotalDays)
            {
                Game1.player.acceptedDailyQuest.Set(newValue: false);
            }
            Game1.player.questLog.Remove(quest);
            _questPageManager.RemoveCurrentQuestOnPage();
            _questPageManager.ClearCurrentQuestOnPage();
            Game1.playSound("trashcan");
            if (Game1.options.SnappyMenus && _questPageManager.IsOnFirstPage())
            {
                base.currentlySnappedComponent = base.getComponentWithID(0);
                this.snapCursorToCurrentSnappedComponent();
            }
        }
        else if (!this.NeedsScroll() || this.backButton.containsPoint(x, y))
        {
            this.exitQuestPage();
        }

        if (this.NeedsScroll())
        {
            if (_scrollbarHelper.GetDownArrowComponent().containsPoint(x, y)
                && _scrollbarHelper.GetScrollAmount() < this._contentHeight - this._scissorRectHeight)
            {
                this.DownArrowPressed();
                Game1.playSound("shwip");
            }
            else if (_scrollbarHelper.GetUpArrowComponent().containsPoint(x, y)
                && _scrollbarHelper.GetScrollAmount() > 0f)
            {
                this.UpArrowPressed();
                Game1.playSound("shwip");
            }
            else if (_scrollbarHelper.GetScrollBarComponent().containsPoint(x, y))
            {
                _scrollbarHelper.SetIsScrolling();
            }
            else if (_scrollbarHelper.GetScrollBarBounds().Contains(x, y))
            {
                _scrollbarHelper.SetIsScrolling();
            }
            else if (!_scrollbarHelper.GetDownArrowComponent().containsPoint(x, y)
                && x > base.xPositionOnScreen + base.width
                && x < base.xPositionOnScreen + base.width + 128
                && y > base.yPositionOnScreen
                && y < base.yPositionOnScreen + base.height)
            {
                _scrollbarHelper.SetIsScrolling();
                this.leftClickHeld(x, y);
                this.releaseLeftClick(x, y);
            }
        }
    }

    #endregion






    public bool NeedsScroll()
    {
        if (_questPageManager.GetShownQuest() != null && _questPageManager.GetShownQuest().ShouldDisplayAsComplete())
        {
            return false;
        }
        if (_questPageManager.IsCurrentQuestOnPageSet())
        {
            return this._contentHeight > this._scissorRectHeight;
        }
        return false;
    }

    public void exitQuestPage()
    {
        if (_questPageManager.GetShownQuest().OnLeaveQuestPage())
        {
            _questPageManager.RemoveCurrentQuestOnPage();
        }
        _questPageManager.ClearCurrentQuestOnPage();
        this._questPageManager.paginateQuests();
        Game1.playSound("shwip");
        if (Game1.options.SnappyMenus)
        {
            this.snapToDefaultClickableComponent();
        }
    }




    public override void update(GameTime time)
    {
        base.update(time);
        if (_questPageManager.IsCurrentQuestOnPageSet() && _questPageManager.GetShownQuest().HasReward())
        {
            this.rewardBox.scale = this.rewardBox.baseScale + Game1.dialogueButtonScale / 20f;
        }
    }




    #region draw()

    public override void draw(SpriteBatch b)
    {
        DimBackgroundIfEnabled(b);
        DrawHeader(b);

        if (_questPageManager.IsNotCurrentQuestOnPageSet()) // if no quest selected
            DrawQuestsListScreen(b);
        else
            DrawSelectedQuestScreen(b);
        
        DrawScrollBarIfNeeded(b);
        DrawForwardBackButtonsIfNeeded(b);

        base.draw(b);

        DrawMousePointer(b);
        _hoverDrawHelper.DrawHoverTextIfSet(b);
    }

    private static void DimBackgroundIfEnabled(SpriteBatch b)
    {
        if (!Game1.options.showClearBackgrounds)
        {
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
        }
    }

    private void DrawHeader(SpriteBatch b)
    {
        SpriteText.drawStringWithScrollCenteredAt(b,
            //Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11373"),
            "My Quest Log",
            base.xPositionOnScreen + base.width / 2, base.yPositionOnScreen - 64);
    }



    private void DrawQuestsListScreen(SpriteBatch b)
    {
        // draw main box frame/background
        IClickableMenu.drawTextureBox(b,
            Game1.mouseCursors,
            new Rectangle(384, 373, 18, 18),
            base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height,
            Color.White, 4f);

        for (int i = 0; i < this.questLogButtons.Count; i++)
        {            
            if (_questPageManager.HasAnyPages() && _questPageManager.GetCurrentPage().Count > i)
            {
                var currentQuestLogButton = this.questLogButtons[i];
                var currentQuest = _questPageManager.GetCurrentPage()[i];
                DrawQuestsListEntry(b, currentQuestLogButton, currentQuest);
            }
        }
    }
    private static void DrawQuestsListEntry(SpriteBatch b, ClickableComponent questLogButton, IQuest quest)
    {
        // draw per-quest frame/background
        IClickableMenu.drawTextureBox(b,
            Game1.mouseCursors,
            new Rectangle(384, 396, 15, 15),
            questLogButton.bounds.X, questLogButton.bounds.Y, questLogButton.bounds.Width, questLogButton.bounds.Height,
            questLogButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White,
            4f, drawShadow: false);

        // draw status/type indicator icons (e.g. "new", "done", timed, regular)
        if (quest.ShouldDisplayAsNew() || quest.ShouldDisplayAsComplete())
        {
            Utility.drawWithShadow(b,
                Game1.mouseCursors,
                new Vector2(questLogButton.bounds.X + 64 + 4, questLogButton.bounds.Y + 44),
                new Rectangle(quest.ShouldDisplayAsComplete() ? 341 : 317, 410, 23, 9),
                Color.White, 0f, new Vector2(11f, 4f), 4f + Game1.dialogueButtonScale * 10f / 250f, flipped: false, 0.99f);
        }
        else
        {
            Utility.drawWithShadow(b,
                Game1.mouseCursors,
                new Vector2(questLogButton.bounds.X + 32, questLogButton.bounds.Y + 28),
                quest.IsTimedQuest() ? new Rectangle(410, 501, 9, 9) : new Rectangle(395 + (quest.IsTimedQuest() ? 3 : 0), 497, 3, 8),
                Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.99f);
        }

        quest.IsTimedQuest(); // prob unnecessary?

        // draw quest name
        SpriteText.drawString(b,
            quest.GetName(),
            questLogButton.bounds.X + 128 + 4, questLogButton.bounds.Y + 20);
    }



    private void DrawSelectedQuestScreen(SpriteBatch b)
    {
        int titleWidth = SpriteText.getWidthOfString(_questPageManager.GetShownQuest().GetName());
        if (titleWidth > base.width / 2)
        {
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height + (_questPageManager.GetShownQuest().ShouldDisplayAsComplete() ? 48 : 0), Color.White, 4f);
            SpriteText.drawStringHorizontallyCenteredAt(b, _questPageManager.GetShownQuest().GetName(), base.xPositionOnScreen + base.width / 2, base.yPositionOnScreen + 32);
        }
        else
        {
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height, Color.White, 4f);
            SpriteText.drawStringHorizontallyCenteredAt(b, _questPageManager.GetShownQuest().GetName(), base.xPositionOnScreen + base.width / 2 + ((_questPageManager.GetShownQuest().IsTimedQuest() && _questPageManager.GetShownQuest().GetDaysLeft() > 0) ? (Math.Max(32, SpriteText.getWidthOfString(_questPageManager.GetShownQuest().GetName()) / 3) - 32) : 0), base.yPositionOnScreen + 32);
        }
        float extraYOffset = 0f;
        if (_questPageManager.GetShownQuest().IsTimedQuest() && _questPageManager.GetShownQuest().GetDaysLeft() > 0)
        {
            int xOffset = 0;
            if (titleWidth > base.width / 2)
            {
                xOffset = 28;
                extraYOffset = 48f;
            }
            Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2(base.xPositionOnScreen + xOffset + 32, (float)(base.yPositionOnScreen + 48 - 8) + extraYOffset), new Rectangle(410, 501, 9, 9), Color.White, 0f, Vector2.Zero, 4f, flipped: false, 0.99f);
            Utility.drawTextWithShadow(b, Game1.parseText((_questPageManager.GetCurrentQuestOnPage().GetDaysLeft() > 1) ? Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11374", _questPageManager.GetCurrentQuestOnPage().GetDaysLeft()) : Game1.content.LoadString("Strings\\StringsFromCSFiles:Quest_FinalDay"), Game1.dialogueFont, base.width - 128), Game1.dialogueFont, new Vector2(base.xPositionOnScreen + xOffset + 80, (float)(base.yPositionOnScreen + 48 - 8) + extraYOffset), Game1.textColor);
        }
        string description = Game1.parseText(_questPageManager.GetShownQuest().GetDescription(), Game1.dialogueFont, base.width - 128);
        Rectangle cached_scissor_rect = b.GraphicsDevice.ScissorRectangle;
        Vector2 description_size = Game1.dialogueFont.MeasureString(description);
        Rectangle scissor_rect = default(Rectangle);
        scissor_rect.X = base.xPositionOnScreen + 32;
        scissor_rect.Y = base.yPositionOnScreen + 96 + (int)extraYOffset;
        scissor_rect.Height = base.yPositionOnScreen + base.height - 32 - scissor_rect.Y;
        scissor_rect.Width = base.width - 64;
        this._scissorRectHeight = scissor_rect.Height;
        scissor_rect = Utility.ConstrainScissorRectToScreen(scissor_rect);
        b.End();
        b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, new RasterizerState
        {
            ScissorTestEnable = true
        });
        Game1.graphics.GraphicsDevice.ScissorRectangle = scissor_rect;
        Utility.drawTextWithShadow(b, description, Game1.dialogueFont, new Vector2(base.xPositionOnScreen + 64, (float)base.yPositionOnScreen - _scrollbarHelper.GetScrollAmount() + 96f + extraYOffset), Game1.textColor);
        float yPos = (float)(base.yPositionOnScreen + 96) + description_size.Y + 32f - _scrollbarHelper.GetScrollAmount() + extraYOffset;
        if (_questPageManager.GetShownQuest().ShouldDisplayAsComplete())
        {
            b.End();
            b.GraphicsDevice.ScissorRectangle = cached_scissor_rect;
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:QuestLog.cs.11376"), base.xPositionOnScreen + 32 + 4, this.rewardBox.bounds.Y + 21 + 4 + (int)extraYOffset);
            this.rewardBox.draw(b, Color.White, 0.9f, 0, 0, (int)extraYOffset);
            if (_questPageManager.GetShownQuest().HasMoneyReward())
            {
                b.Draw(Game1.mouseCursors, new Vector2(this.rewardBox.bounds.X + 16, (float)(this.rewardBox.bounds.Y + 16) - Game1.dialogueButtonScale / 2f + extraYOffset), new Rectangle(280, 410, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                SpriteText.drawString(b, Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", _questPageManager.GetShownQuest().GetMoneyReward()), base.xPositionOnScreen + 448, this.rewardBox.bounds.Y + 21 + 4 + (int)extraYOffset);
            }
        }
        else
        {
            for (int j = 0; j < this._objectiveText.Count; j++)
            {
                string parsed_text = Game1.parseText(this._objectiveText[j], width: base.width - 192, whichFont: Game1.dialogueFont);
                bool num2 = _questPageManager.GetShownQuest() is SpecialOrder o && o.objectives[j].IsComplete();
                Color text_color = Game1.unselectedOptionColor;
                if (!num2)
                {
                    text_color = Color.DarkBlue;
                    Utility.drawWithShadow(b, Game1.mouseCursors, new Vector2((float)(base.xPositionOnScreen + 96) + 8f * Game1.dialogueButtonScale / 10f, yPos), new Rectangle(412, 495, 5, 4), Color.White, (float)Math.PI / 2f, Vector2.Zero);
                }
                Utility.drawTextWithShadow(b, parsed_text, Game1.dialogueFont, new Vector2(base.xPositionOnScreen + 128, yPos - 8f), text_color);
                yPos += Game1.dialogueFont.MeasureString(parsed_text).Y;
                if (_questPageManager.GetShownQuest() is SpecialOrder order)
                {
                    OrderObjective order_objective = order.objectives[j];
                    if (order_objective.GetMaxCount() > 1 && order_objective.ShouldShowProgress())
                    {
                        Color dark_bar_color = Color.DarkRed;
                        Color bar_color = Color.Red;
                        if (order_objective.GetCount() >= order_objective.GetMaxCount())
                        {
                            bar_color = Color.LimeGreen;
                            dark_bar_color = Color.Green;
                        }
                        int inset = 64;
                        int objective_count_draw_width = 160;
                        int notches = 4;
                        Rectangle bar_background_source = new Rectangle(0, 224, 47, 12);
                        Rectangle bar_notch_source = new Rectangle(47, 224, 1, 12);
                        int bar_horizontal_padding = 3;
                        int bar_vertical_padding = 3;
                        int slice_width = 5;
                        string objective_count_text = order_objective.GetCount() + "/" + order_objective.GetMaxCount();
                        int max_text_width = (int)Game1.dialogueFont.MeasureString(order_objective.GetMaxCount() + "/" + order_objective.GetMaxCount()).X;
                        int count_text_width = (int)Game1.dialogueFont.MeasureString(objective_count_text).X;
                        int text_draw_position = base.xPositionOnScreen + base.width - inset - count_text_width;
                        int max_text_draw_position = base.xPositionOnScreen + base.width - inset - max_text_width;
                        Utility.drawTextWithShadow(b, objective_count_text, Game1.dialogueFont, new Vector2(text_draw_position, yPos), Color.DarkBlue);
                        Rectangle bar_draw_position = new Rectangle(base.xPositionOnScreen + inset, (int)yPos, base.width - inset * 2 - objective_count_draw_width, bar_background_source.Height * 4);
                        if (bar_draw_position.Right > max_text_draw_position - 16)
                        {
                            int adjustment = bar_draw_position.Right - (max_text_draw_position - 16);
                            bar_draw_position.Width -= adjustment;
                        }
                        b.Draw(Game1.mouseCursors2, new Rectangle(bar_draw_position.X, bar_draw_position.Y, slice_width * 4, bar_draw_position.Height), new Rectangle(bar_background_source.X, bar_background_source.Y, slice_width, bar_background_source.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                        b.Draw(Game1.mouseCursors2, new Rectangle(bar_draw_position.X + slice_width * 4, bar_draw_position.Y, bar_draw_position.Width - 2 * slice_width * 4, bar_draw_position.Height), new Rectangle(bar_background_source.X + slice_width, bar_background_source.Y, bar_background_source.Width - 2 * slice_width, bar_background_source.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                        b.Draw(Game1.mouseCursors2, new Rectangle(bar_draw_position.Right - slice_width * 4, bar_draw_position.Y, slice_width * 4, bar_draw_position.Height), new Rectangle(bar_background_source.Right - slice_width, bar_background_source.Y, slice_width, bar_background_source.Height), Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.5f);
                        float quest_progress = (float)order_objective.GetCount() / (float)order_objective.GetMaxCount();
                        if (order_objective.GetMaxCount() < notches)
                        {
                            notches = order_objective.GetMaxCount();
                        }
                        bar_draw_position.X += 4 * bar_horizontal_padding;
                        bar_draw_position.Width -= 4 * bar_horizontal_padding * 2;
                        for (int k = 1; k < notches; k++)
                        {
                            b.Draw(Game1.mouseCursors2, new Vector2((float)bar_draw_position.X + (float)bar_draw_position.Width * ((float)k / (float)notches), bar_draw_position.Y), bar_notch_source, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.5f);
                        }
                        bar_draw_position.Y += 4 * bar_vertical_padding;
                        bar_draw_position.Height -= 4 * bar_vertical_padding * 2;
                        Rectangle rect = new Rectangle(bar_draw_position.X, bar_draw_position.Y, (int)((float)bar_draw_position.Width * quest_progress) - 4, bar_draw_position.Height);
                        b.Draw(Game1.staminaRect, rect, null, bar_color, 0f, Vector2.Zero, SpriteEffects.None, (float)rect.Y / 10000f);
                        rect.X = rect.Right;
                        rect.Width = 4;
                        b.Draw(Game1.staminaRect, rect, null, dark_bar_color, 0f, Vector2.Zero, SpriteEffects.None, (float)rect.Y / 10000f);
                        yPos += (float)((bar_background_source.Height + 4) * 4);
                    }
                }
                this._contentHeight = yPos + _scrollbarHelper.GetScrollAmount() - (float)scissor_rect.Y;
            }
            b.End();
            b.GraphicsDevice.ScissorRectangle = cached_scissor_rect;
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
            if (_questPageManager.GetShownQuest().CanBeCancelled())
            {
                this.cancelQuestButton.draw(b);
            }
            if (this.NeedsScroll())
            {
                if (_scrollbarHelper.GetScrollAmount() > 0f)
                {
                    b.Draw(Game1.staminaRect, new Rectangle(scissor_rect.X, scissor_rect.Top, scissor_rect.Width, 4), Color.Black * 0.15f);
                }
                if (_scrollbarHelper.GetScrollAmount() < this._contentHeight - this._scissorRectHeight)
                {
                    b.Draw(Game1.staminaRect, new Rectangle(scissor_rect.X, scissor_rect.Bottom - 4, scissor_rect.Width, 4), Color.Black * 0.15f);
                }
            }
        }
    }



    private void DrawScrollBarIfNeeded(SpriteBatch b)
    {
        if (this.NeedsScroll())
        {
            _scrollbarHelper.GetUpArrowComponent().draw(b);
            _scrollbarHelper.GetDownArrowComponent().draw(b);
            _scrollbarHelper.GetScrollBarComponent().draw(b);
        }
    }

    private void DrawForwardBackButtonsIfNeeded(SpriteBatch b)
    {
        if (_questPageManager.IsNotOnLastPage() && _questPageManager.IsNotCurrentQuestOnPageSet())
        {
            this.forwardButton.draw(b);
        }
        if (_questPageManager.IsNotOnFirstPage() || _questPageManager.IsCurrentQuestOnPageSet())
        {
            this.backButton.draw(b);
        }
    }

    private void DrawMousePointer(SpriteBatch b)
    {
        Game1.mouseCursorTransparency = 1f;
        base.drawMouse(b);
    }

    #endregion
}
