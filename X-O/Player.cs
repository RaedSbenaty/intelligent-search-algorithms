using System;
using System.Collections.Generic;

abstract class Player
{
    public static int IDCounter = 0;
    protected int ID;

    protected Game.Letter myLetter;
    public Game.Letter MyLetter { get => myLetter; }

    public Player(Game.Letter myLetter) => (ID, this.myLetter) = (++IDCounter, myLetter);
    public abstract void DrawState(State state);
    public abstract State ChoiceNextState(State currentState);
    public abstract void ShowWinMessage();
}


class AIPlayer : Player
{
    private int level;
    private Game.Letter opponentLetter;

    public AIPlayer(Game.Letter myLetter, Game.Letter opponentLetter, int level) : base(myLetter)
    => (this.opponentLetter, this.level) = (opponentLetter, level);

    public override void DrawState(State state) => Console.WriteLine(state);

    public override State ChoiceNextState(State currentState)
    //  => currentState.GetBestStateUsingMinMax(level, myLetter, opponentLetter);
      => currentState.GetBestStateUsingAlphaBeta(level, myLetter, opponentLetter);

    public override void ShowWinMessage() => Console.WriteLine($"Player#{ID} Win!");
}

class ConsolePlayer : Player
{
    public ConsolePlayer(Game.Letter myLetter) : base(myLetter) { }

    public override void DrawState(State state) => Console.WriteLine(state);

    public override State ChoiceNextState(State currentState)
    {
        Console.Write("\nChoose your next move:");

        var input = Console.ReadLine()?.Split().Select(int.Parse);
        return input != null ? currentState.NextStateWithMove(input.ElementAt(0) - 1, input.ElementAt(1) - 1, myLetter)
                             : currentState;
    }

    public override void ShowWinMessage() => Console.WriteLine($"Player#{ID} Win!");
}