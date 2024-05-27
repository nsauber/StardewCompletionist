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
        var myMenu = new MyTrialMenu();
        Game1.activeClickableMenu = myMenu;



    }

    public class MyTrialMenu : IClickableMenu
    {
        public MyTrialMenu()
            : base(0, 0, 0, 0, showUpperRightCloseButton: true)
        {
            Game1.dayTimeMoneyBox.DismissQuestPing();
            Game1.playSound("bigSelect");

            // initialize size/positioning values
            base.width = 832;
            base.height = 576;
            Vector2 topLeft = Utility.getTopLeftPositionForCenteringOnScreen(base.width, base.height);
            base.xPositionOnScreen = (int)topLeft.X;
            base.yPositionOnScreen = (int)topLeft.Y + 32;            
        }

        public override void draw(SpriteBatch b)
        {
            var xPos = base.xPositionOnScreen;
            var yPos = base.yPositionOnScreen;
            var w = base.width; 
            var h = base.height;


            // draw dimmed background, if enabled
            if (!Game1.options.showClearBackgrounds)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            }

            // draw header
            SpriteText.drawStringWithScrollCenteredAt(b,
                "Hello World!",
                base.xPositionOnScreen + base.width / 2, base.yPositionOnScreen - 64);

            // draw main box frame/background
            IClickableMenu.drawTextureBox(b,
                Game1.mouseCursors,
                new Rectangle(384, 373, 18, 18),
                base.xPositionOnScreen, base.yPositionOnScreen, base.width, base.height,
                Color.White, 4f);


            // initialize list buttons click-handling
            var numberOfButtons = 8;
            var listButtons = new List<ClickableComponent>();
            for (int i = 0; i < numberOfButtons; i++)
            {
                listButtons.Add(
                    new ClickableComponent(
                        new Rectangle(
                            xPos + 16,
                            yPos + 16 + i * ((h - 32) / numberOfButtons),
                            w - 32,
                            (h - 32) / numberOfButtons + 4),
                        i.ToString() ?? "")
                    {
                        myID = i,
                        downNeighborID = -7777,
                        upNeighborID = ((i > 0) ? (i - 1) : (-1)),
                        rightNeighborID = -7777,
                        leftNeighborID = -7777,
                        fullyImmutable = true
                    });
            }
            for (int i = 0; i < listButtons.Count; i++)
            {
                var listButton = listButtons[i];

                // draw per-item frame/background
                IClickableMenu.drawTextureBox(b,
                    Game1.mouseCursors,
                    new Rectangle(384, 396, 15, 15),
                    listButton.bounds.X, listButton.bounds.Y, listButton.bounds.Width, listButton.bounds.Height,
                    listButton.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White,
                    4f, drawShadow: false);

                // draw item name
                //SpriteText.drawString(b,
                //    $"Entry number {i}",
                //    listButton.bounds.X + 128 + 4, listButton.bounds.Y + 12);
                Utility.drawTextWithShadow(b,
                    $"Entry number {i}",
                    Game1.dialogueFont,
                    new Vector2(listButton.bounds.X + 128 + 4, listButton.bounds.Y + 12),
                    Game1.textColor);
                
            }





            base.draw(b);

            Game1.mouseCursorTransparency = 1f;
            base.drawMouse(b);
        }
    }
}
