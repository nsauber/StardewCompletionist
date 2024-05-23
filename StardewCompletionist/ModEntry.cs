using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using StardewValley.SpecialOrders.Objectives;


namespace StardewCompletionist;

internal sealed class ModEntry : Mod
{
    private ModConfig _config = new ModConfig();

    public override void Entry(IModHelper helper)
    {
        _config = helper.ReadConfig<ModConfig>();

        // when special key is pressed, open menu
        helper.Events.Input.ButtonPressed += OnButtonPressed;        

        //TODO: eventually want to add a button on Quests screen so you can open our menu without needing this special key (e.g. when using controller)
    }


    /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event data.</param>
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        // ignore if player isn't free to move/play (e.g. hasn't loaded a save yet, has menu open, etc)
        if (!Context.IsPlayerFree)
            return;

        // print button presses to the console window
        Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug); //TODO: remove log message after initial testing

        if (_config.ShowMenuKey.JustPressed())
        {
            Monitor.Log($"You pressed the magic key!!! ({e.Button}).", LogLevel.Debug); //TODO: remove log message after initial testing

            OnShowMenuKeyPressed();
        }
    }

    private void OnShowMenuKeyPressed()
    {
        //TODO: gather current state of game/tasks
        //TODO: output findings to text file (for initial testing)
        //TODO: eventually render in a window (prob initially just text, but eventually want a proper game-friendly UI)

        // (temporary code to test/verify can write a text file while running inside Stardew)
        var filePath = Path.Combine(Helper.DirectoryPath, "zz_test_output.txt");
        File.WriteAllText(filePath, $"{Game1.player.Name} pressed the magic key at {DateTime.Now}.");

        // (temporary code while exploring game menu behavior)
        //var myMenu = new MyTrialMenu();
        var myMenu = new MyQuestLog();
        Game1.activeClickableMenu = myMenu;



    }

    public class MyTrialMenu : IClickableMenu
    {
        public MyTrialMenu()
            : base(0, 0, 0, 0, showUpperRightCloseButton: true)
        {
            Game1.dayTimeMoneyBox.DismissQuestPing();
            Game1.playSound("bigSelect");

            base.width = 832;
            base.height = 576;

            Vector2 topLeft = Utility.getTopLeftPositionForCenteringOnScreen(base.width, base.height);
            base.xPositionOnScreen = (int)topLeft.X;
            base.yPositionOnScreen = (int)topLeft.Y + 32;            
        }

        public override void draw(SpriteBatch b)
        {
            if (!Game1.options.showClearBackgrounds)
            {
                // dim background (draw 75% opaque black rectangle over entire viewport)
                b.Draw(Game1.fadeToBlackRect,
                    Game1.graphics.GraphicsDevice.Viewport.Bounds,
                    Color.Black * 0.75f);
            }

            // header
            SpriteText.drawStringWithScrollCenteredAt(b,
                "Hello World!",
                base.xPositionOnScreen + base.width / 2, base.yPositionOnScreen - 64);

            // menu main "box"
            drawTextureBox(b,
                Game1.mouseCursors,
                new Rectangle(384, 373, 18, 18),
                base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height,
                Color.White,
                scale: 4f);












            var questLogButtons = new List<ClickableComponent>();
            for (int i = 0; i < 6; i++)
            {
                questLogButtons.Add(
                    new ClickableComponent(
                        new Rectangle(
                            base.xPositionOnScreen + 16,
                            base.yPositionOnScreen + 16 + i * ((base.height - 32) / 6),
                            base.width - 32,
                            (base.height - 32) / 6 + 4
                        ),
                        i.ToString() ?? "")
                    {
                        myID = i,
                        downNeighborID = -7777,
                        upNeighborID = ((i > 0) ? (i - 1) : (-1)),
                        rightNeighborID = -7777,
                        leftNeighborID = -7777,
                        fullyImmutable = true
                    }
                );
            }

            var pages = new List<List<IQuest>>();
            IList<IQuest> quests = this.GetAllQuests();
            int startIndex = 0;
            while (startIndex < quests.Count)
            {
                List<IQuest> page = new List<IQuest>();
                for (int i = 0; i < 6; i++)
                {
                    if (startIndex >= quests.Count)
                    {
                        break;
                    }
                    page.Add(quests[startIndex]);
                    startIndex++;
                }
                pages.Add(page);
            }
            if (pages.Count == 0)
            {
                pages.Add(new List<IQuest>());
            }
            var currentPage = 0;
            currentPage = Utility.Clamp(currentPage, 0, pages.Count - 1);
            //var questPage = -1;


            for (int i = 0; i < questLogButtons.Count; i++)
            {
                if (pages.Count > 0 && pages[currentPage].Count > i)
                {
                    // per-quest box
                    drawTextureBox(b,
                        Game1.mouseCursors,
                        new Rectangle(384, 396, 15, 15),
                        questLogButtons[i].bounds.X, questLogButtons[i].bounds.Y, questLogButtons[i].bounds.Width, questLogButtons[i].bounds.Height,
                        questLogButtons[i].containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White,
                        4f, drawShadow: false);
                    if (pages[currentPage][i].ShouldDisplayAsNew() || pages[currentPage][i].ShouldDisplayAsComplete())
                    {
                        // "new" indicator
                        Utility.drawWithShadow(b,
                            Game1.mouseCursors,
                            new Vector2(questLogButtons[i].bounds.X + 64 + 4, questLogButtons[i].bounds.Y + 44),
                            new Rectangle(pages[currentPage][i].ShouldDisplayAsComplete() ? 341 : 317, 410, 23, 9),
                            Color.White,
                            0f,
                            new Vector2(11f, 4f), 4f + Game1.dialogueButtonScale * 10f / 250f, flipped: false, 0.99f);
                    }
                    else
                    {
                        // unsure....... ???
                        Utility.drawWithShadow(b,
                            Game1.mouseCursors,
                            new Vector2(questLogButtons[i].bounds.X + 32, questLogButtons[i].bounds.Y + 28),
                            pages[currentPage][i].IsTimedQuest()
                                ? new Rectangle(410, 501, 9, 9)
                                : new Rectangle(395 + (pages[currentPage][i].IsTimedQuest() ? 3 : 0), 497, 3, 8),
                            Color.White,
                            0f, Vector2.Zero, 4f, flipped: false, 0.99f);
                    }
                    pages[currentPage][i].IsTimedQuest();
                    // name of quest (text inside box)
                    SpriteText.drawString(b,
                        pages[currentPage][i].GetName() + " [wtf]",
                        questLogButtons[i].bounds.X + 128 + 4, questLogButtons[i].bounds.Y + 20);
                }
            }



        }

        protected virtual IList<IQuest> GetAllQuests()
        {
            List<IQuest> quests = new List<IQuest>();
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
    }
}
