using System;
using System.Collections.Generic;

abstract class Player
{
    public abstract void DrawState(State state);
    public abstract State ChoiceNextState(State currentState);
    public abstract void WinMessage();
    public abstract void ShowSolution(State winnerState, int stateCounter);
    public abstract void ShowTotalStates(int stateCounter);
    public abstract void ShowPathToState(State winnerState);
}

class ConsolePlayer : Player
{
    public override void DrawState(State state)
    {
        Console.WriteLine(state.LastUpdate);
        Console.ForegroundColor = ConsoleColor.Black;
        for (var i = 0; i <= state.StateLevel.N + 1; i++)
        {
            for (var j = 0; j <= state.StateLevel.M + 1; j++)
            {
                var carId = -1;
                Position p = (i, j);

                if (!state.OccupiedPositions.ContainsKey((i, j)))
                {
                    if (i == 0 || j == 0 || i == state.StateLevel.N + 1 || j == state.StateLevel.M + 1)
                    {
                        if (state.StateLevel.Exit.Equals(new Position(i, j)))
                            Console.BackgroundColor = ConsoleColor.Magenta;
                    }
                    else
                        Console.BackgroundColor = ConsoleColor.Gray;
                }
                else
                {
                    Console.BackgroundColor = state.OccupiedPositions[p].IsMainCar ? ConsoleColor.Red : ConsoleColor.Green;
                    var car = state.OccupiedPositions[p];
                    carId = state.CarPlaces[car].Equals(p) ? car.ID : -1;
                }

                Console.Write(carId == -1 ? "  " : carId + (carId > 9 ? "" : " "));
                Console.ResetColor();
            }
            Console.WriteLine();
        }
    }

    public override State ChoiceNextState(State currentState)
    {
        Console.WriteLine("\nCurrent state:\n");
        DrawState(currentState);

        Console.WriteLine("\nChoose your next move:");
        List<State> nextStates = currentState.GetNextStates();

        for (var i = 0; i < nextStates.Count; i++)
        {
            Console.WriteLine($"\n{(i + 1)}-");
            DrawState(nextStates[i]);
            Console.WriteLine();
        }

        int index = 0; //assuming that always there'll be at least one next state
        while (index > nextStates.Count || index < 1)
        {
            Console.Write("\nYour choice: ");
            index = Convert.ToInt32(Console.ReadLine());
        }

        return nextStates[index - 1];
    }

    public override void ShowSolution(State winnerState, int stateCounter)
    {
        ShowTotalStates(stateCounter);
        ShowPathToState(winnerState);
    }

    public override void ShowTotalStates(int stateCounter) => Console.WriteLine("Total states: " + stateCounter);

    public override void ShowPathToState(State winnerState)
    {
        Stack<State> stack = new Stack<State>();
        State? currentState = winnerState;
        while (currentState != null)
        {
            stack.Push(currentState);
            currentState = currentState.ParentState;
        }
        var i = 0;
        while (stack.Count != 0)
        {
            Console.Write($"{++i}- ");
            var state = stack.Pop();
            DrawState(state);
        }
    }

    public override void WinMessage() => Console.WriteLine("You Win!");
}