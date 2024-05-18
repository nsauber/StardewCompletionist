using Microsoft.Xna.Framework.Media;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using static StardewValley.Menus.CoopMenu;
using static StardewValley.Minigames.TargetGame;
using QuikGraph;

namespace StardewCompletionist.Tasks;

internal class TaskDataBuilder
{
    public TaskData Build()
    {
        var taskData = new TaskData();

        //...
        //AddPerfectionPrerequisites(...); // doubtless split into multiple methods
        //...

        AddAchievements(taskData);

        AddPerfectionTasks(taskData);

        //AddOptionalTasks(taskData);
        //...

        return taskData;
    }

    // https://stardewvalleywiki.com/Achievements
    private void AddAchievements(TaskData taskData)
    {
        var ach01 = new Task() { Category = TaskCategory.Achievement, Name = "Greenhorn", Description = "Earn 15,000g" };
        taskData.AddTask(ach01);
        var ach02 = new Task() { Category = TaskCategory.Achievement, Name = "Cowpoke", Description = "Earn 50,000g" };
        taskData.AddTask(ach02, new[] { ach01 });
        var ach03 = new Task() { Category = TaskCategory.Achievement, Name = "Homesteader", Description = "Earn 250,000g" };
        taskData.AddTask(ach03, new[] { ach02 });
        var ach04 = new Task() { Category = TaskCategory.Achievement, Name = "Millionaire", Description = "Earn 1,000,000g" };
        taskData.AddTask(ach04, new[] { ach03 });
        var ach05 = new Task() { Category = TaskCategory.Achievement, Name = "Legend", Description = "Earn 10,000,000g" };
        taskData.AddTask(ach05, new[] { ach04 });

        var ach06 = new Task() { Category = TaskCategory.Achievement, Name = "A Complete Collection", Description = "Complete the Museum collection" };
        taskData.AddTask(ach06);

        var ach07 = new Task() { Category = TaskCategory.Achievement, Name = "A New Friend", Description = "Reach a 5 - Heart friend level with someone" };
        taskData.AddTask(ach07);
        var ach08 = new Task() { Category = TaskCategory.Achievement, Name = "Best Friends", Description = "Reach a 10 - Heart friend level with someone" };
        taskData.AddTask(ach08, new[] { ach07 });
        var ach09 = new Task() { Category = TaskCategory.Achievement, Name = "The Beloved Farmer", Description = "Reach a 10 - Heart friend level with 8 people" };
        taskData.AddTask(ach09, new[] { ach08 });
        var ach10 = new Task() { Category = TaskCategory.Achievement, Name = "Cliques", Description = "Reach a 5 - Heart friend level with 4 people" };
        taskData.AddTask(ach10, new[] { ach07 });
        var ach11 = new Task() { Category = TaskCategory.Achievement, Name = "Networking", Description = "Reach a 5 - Heart friend level with 10 people" };
        taskData.AddTask(ach11, new[] { ach10 });
        var ach12 = new Task() { Category = TaskCategory.Achievement, Name = "Popular", Description = "Reach a 5 - Heart friend level with 20 people" };
        taskData.AddTask(ach12, new[] { ach11 });

        var ach13 = new Task() { Category = TaskCategory.Achievement, Name = "Cook", Description = "Cook 10 different recipes" };
        taskData.AddTask(ach13);
        var ach14 = new Task() { Category = TaskCategory.Achievement, Name = "Sous Chef", Description = "Cook 25 different recipes" };
        taskData.AddTask(ach14, new[] { ach13 });
        var ach15 = new Task() { Category = TaskCategory.Achievement, Name = "Gourmet Chef", Description = "Cook every recipe" };
        taskData.AddTask(ach15, new[] { ach14 });

        var ach16 = new Task() { Category = TaskCategory.Achievement, Name = "Moving Up", Description = "Upgrade your house" };
        taskData.AddTask(ach16);
        var ach17 = new Task() { Category = TaskCategory.Achievement, Name = "Living Large", Description = "Upgrade your house to the maximum size" };
        taskData.AddTask(ach17, new[] { ach16 });

        var ach18 = new Task() { Category = TaskCategory.Achievement, Name = "D.I.Y.", Description = "Craft 15 different items" };
        taskData.AddTask(ach18);
        var ach19 = new Task() { Category = TaskCategory.Achievement, Name = "Artisan", Description = "Craft 30 different items" };
        taskData.AddTask(ach19, new[] { ach18 });
        var ach20 = new Task() { Category = TaskCategory.Achievement, Name = "Craft Master", Description = "Craft every item" };
        taskData.AddTask(ach20, new[] { ach19 });

        var ach21 = new Task() { Category = TaskCategory.Achievement, Name = "Fisherman", Description = "Catch 10 different fish" };
        taskData.AddTask(ach21);
        var ach22 = new Task() { Category = TaskCategory.Achievement, Name = "Ol' Mariner", Description = "Catch 24 different fish" };
        taskData.AddTask(ach22, new[] { ach21 });
        var ach23 = new Task() { Category = TaskCategory.Achievement, Name = "Master Angler", Description = "Catch every fish" };
        taskData.AddTask(ach23, new[] { ach22 });
        var ach24 = new Task() { Category = TaskCategory.Achievement, Name = "Mother Catch", Description = "Catch 100 fish" };
        taskData.AddTask(ach24);

        var ach25 = new Task() { Category = TaskCategory.Achievement, Name = "Treasure Trove", Description = "Donate 40 different items to the museum" };
        taskData.AddTask(ach25);

        var ach26 = new Task() { Category = TaskCategory.Achievement, Name = "Gofer", Description = "Complete 10 'Help Wanted' requests" };
        taskData.AddTask(ach26);
        var ach27 = new Task() { Category = TaskCategory.Achievement, Name = "A Big Help", Description = "Complete 40 'Help Wanted' requests" };
        taskData.AddTask(ach27, new[] { ach26 });

        var ach28 = new Task() { Category = TaskCategory.Achievement, Name = "Polyculture", Description = "Ship 15 of each crop" };
        taskData.AddTask(ach28);
        var ach29 = new Task() { Category = TaskCategory.Achievement, Name = "Monoculture", Description = "Ship 300 of one crop" };
        taskData.AddTask(ach29);
        var ach30 = new Task() { Category = TaskCategory.Achievement, Name = "Full Shipment", Description = "Ship every item" };
        taskData.AddTask(ach30, new[] { ach28 });
    }

    // https://stardewvalleywiki.com/Qi%27s_Walnut_Room#Perfection_Tracker
    private void AddPerfectionTasks(TaskData taskData)
    {




    }
}
