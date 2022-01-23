using System;
using System.Collections.Generic;
using Newtonsoft.Json; //to install: dotnet add package Newtonsoft.Json
using System.Diagnostics;
class Game
{
    public static void Main(string[] args)
    {
        Level? level = Level.Create("Delivery-App/levels/level3.json");
        if (level == null) throw new Exception("Error in reading input");

          Solve(level, "Dijkstra");
     //   Solve(level, "A* with HF1", level.HeuristicFunction1);
    //    Solve(level, "A* with HF2", level.HeuristicFunction2);
    //    Solve(level, "A* with HF3", level.HeuristicFunction3);
    }

    public static void Solve(Level level, String algorithm, Func<State, float>? HeuristicFunction = null)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        (State finalState, int cost, int statesCounter) = level.FindSolution(HeuristicFunction);
        stopWatch.Stop();

        Console.WriteLine("___________________________________________\n");
        Console.WriteLine($"Algorithm: {algorithm}");
        Console.WriteLine($"States counter: {statesCounter}");
        Console.WriteLine($"Cost: {cost}");
        Console.WriteLine($"Time: { stopWatch.Elapsed.TotalMilliseconds}");
        ShowPath(finalState);
        Console.WriteLine("___________________________________________");
    }

    public static void ShowPath(State? state)
    {
        Stack<State> stack = new Stack<State>();
        while (state != null)
        {
            stack.Push(state);
            state = state.ParentState;
        }
        var pathLength = stack.Count;
        while (stack.Count != 0)
        {
            state = stack.Pop();
            Console.WriteLine($"{state}");
        }
    }
}
