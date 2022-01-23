using System;
using System.Collections.Generic;
class Game
{
    public static void Main(string[] args)
    {
        Player player = new ConsolePlayer();
        Level? level = Level.Create("Car-Park/levels/level1.json");
        if (level == null) throw new Exception("Error in reading input");

        level.UserMood(player);
        // level.DFSMood(player);
        // level.BFSMood(player);
        // level.DijkstraMood(player);
        // level.AStarMood(player);
    }
}


