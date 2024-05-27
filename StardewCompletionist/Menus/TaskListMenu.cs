using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;


namespace StardewCompletionist.Menus;

public class TaskListMenu : IClickableMenu
{
    public TaskListMenu()
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


        DrawButtonListContents(b, xPos, yPos, w, h);


        base.draw(b);

        Game1.mouseCursorTransparency = 1f;
        base.drawMouse(b);
    }

    private static void DrawButtonListContents(SpriteBatch b, int xPos, int yPos, int w, int h)
    {
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
    }
}
