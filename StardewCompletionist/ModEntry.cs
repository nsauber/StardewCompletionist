using System;
using Microsoft.Xna.Framework.Input;
using StardewCompletionist.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;


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
        var myMenu = new TaskListMenu();
        Game1.activeClickableMenu = myMenu;



    }
}
