using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using StardewValley.BellsAndWhistles;
using StardewCompletionist.Tasks;


namespace StardewCompletionist.Menus;

public class TaskListMenu : IClickableMenu
{
    private const int OuterMenuBorderWidth = 16;

    private readonly TaskListHelper _taskListHelper;

    public TaskListMenu()
        : base(0, 0, 0, 0, showUpperRightCloseButton: true)
    {
        // initialize size/positioning values
        base.width = 832;
        base.height = 576;
        Vector2 topLeft = Utility.getTopLeftPositionForCenteringOnScreen(base.width, base.height);
        base.xPositionOnScreen = (int)topLeft.X;
        base.yPositionOnScreen = (int)topLeft.Y + 32;

        base.upperRightCloseButton = new ClickableTextureComponent(
            new Rectangle(base.xPositionOnScreen + base.width - 20, base.yPositionOnScreen - 8, 48, 48),
            Game1.mouseCursors,
            new Rectangle(337, 494, 12, 12),
            4f);

        _taskListHelper = new(GetCanvassBounds());
    }

    /// <summary>
    /// Compute draw-able surface area inside menu border
    /// </summary>
    private Rectangle GetCanvassBounds()
    {
        var menuBounds = new Rectangle(
            base.xPositionOnScreen,
            base.yPositionOnScreen,
            base.width,
            base.height);
        return new Rectangle(
            menuBounds.X + OuterMenuBorderWidth,
            menuBounds.Y + OuterMenuBorderWidth,
            menuBounds.Width - (2 * OuterMenuBorderWidth),
            menuBounds.Height - (2 * OuterMenuBorderWidth));
    }

    public override void draw(SpriteBatch b)
    {
        //----------------------------------------------------------------
        // draw common menu header, frame, and other background "chrome"

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


        //----------------------------------------------------------------
        // draw contents appropriate to the current screen/page

        _taskListHelper.DrawButtonListContents(b);


        //----------------------------------------------------------------
        // draw shared buttons and other chrome that should be visible on top of screen contents

        base.draw(b); // handles close button, among other things

        Game1.mouseCursorTransparency = 1f;
        base.drawMouse(b);
    }

    public class TaskListHelper
    {
        private const int NumberOfItemsPerPage = 8;

        private readonly TaskDataBuilder _taskDataBuilder;
        private readonly Rectangle _canvassBounds;
        private readonly List<ClickableComponent> _buttonComponents;

        private List<List<Tasks.Task>> _pagesOfTasks;

        public TaskListHelper(Rectangle canvassBounds)
        {
            _taskDataBuilder = new();
            _canvassBounds = canvassBounds;            
            
            // initialize list of button components
            _buttonComponents = new List<ClickableComponent>();
            var buttonHeight = _canvassBounds.Height / NumberOfItemsPerPage;
            for (int i = 0; i < NumberOfItemsPerPage; i++)
            {
                _buttonComponents.Add(
                    new ClickableComponent(
                        new Rectangle(
                            _canvassBounds.X,
                            _canvassBounds.Y + (i * buttonHeight),
                            _canvassBounds.Width,
                            buttonHeight + 4), // +4 to height so each button overlaps by one "pixel", merging their borders
                        i.ToString() ?? "")
                    {
                        myID = i,
                        downNeighborID = -7777,
                        upNeighborID = (i > 0) ? (i - 1) : (-1),
                        rightNeighborID = -7777,
                        leftNeighborID = -7777,
                        fullyImmutable = true
                    });
            }

            RefreshPagination();
        }

        private void RefreshPagination()
        {
            //TODO: replace with logic that actually paginates the list of available tasks
            var tasksInOrder = GetAvailableTasks().OrderBy(x => x.Name);

            // for each task, put it in an object that also contains it's index in the list
            var tasksWithIndex = tasksInOrder.Zip(
                Enumerable.Range(0, tasksInOrder.Count()),
                (task, index) => new { Index = index, Item = task });
            var tasksGroupedbyPage = tasksWithIndex
                .GroupBy(x => x.Index / NumberOfItemsPerPage, x => x.Item);

            _pagesOfTasks = tasksGroupedbyPage
                .Select(x => x.ToList())
                .ToList();
        }

        private IEnumerable<Tasks.Task> GetAvailableTasks()
        {
            return _taskDataBuilder.Build()
                            .GetAllTasks();
        }

        private IList<Tasks.Task> GetCurrentPageOfTasks()
        {
            //TODO: add support in class for navigating multiple pages
            return _pagesOfTasks[3];
        }

        public void DrawButtonListContents(SpriteBatch b)
        {
            var tasks = GetCurrentPageOfTasks();

            for (int i = 0; i < _buttonComponents.Count && i < tasks.Count; i++)
            {
                var button = _buttonComponents[i];
                var task = tasks[i];

                // draw per-item frame/background
                IClickableMenu.drawTextureBox(b,
                    Game1.mouseCursors,
                    new Rectangle(384, 396, 15, 15),
                    button.bounds.X, button.bounds.Y, button.bounds.Width, button.bounds.Height,
                    button.containsPoint(Game1.getOldMouseX(), Game1.getOldMouseY()) ? Color.Wheat : Color.White,
                    4f, drawShadow: false);

                // draw item name
                //SpriteText.drawString(b,
                //    $"Entry number {i}",
                //    button.bounds.X + 128 + 4, button.bounds.Y + 12);
                //Utility.drawBoldText(b,
                //    task.Name,
                //    Game1.dialogueFont,
                //    new Vector2(button.bounds.X + 128 + 4, button.bounds.Y + 12),
                //    Game1.textColor);
                Utility.drawTextWithShadow(b,
                    task.Name,
                    Game1.dialogueFont,
                    //Game1.smallFont,
                    //Game1.tinyFont,
                    new Vector2(button.bounds.X + 128 + 4, button.bounds.Y + 12),
                    Game1.textColor);
                


            }
        }
    }
}
