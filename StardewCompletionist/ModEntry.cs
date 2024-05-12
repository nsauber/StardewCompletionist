using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace StardewCompletionist;

internal sealed class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.Input.ButtonPressed += OnButtonPressed;
    }

    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady)
            return;

        // print button presses to the console window
        Monitor.Log($"{Game1.player.Name} pressed {e.Button}.", LogLevel.Debug);
    }
}
