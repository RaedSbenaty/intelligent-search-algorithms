using System;
using System.Collections.Generic;
using System.Linq;

class State
{
    //   private State? parentState;
    private int n;
    private Game.Letter?[,] grid;
    private bool isXTurn = true;
    private string lastUpdate = "";


    public State(int n)
    {
        this.n = n;
        this.grid = new Game.Letter?[n, n];
    }

    public State(State s)
    {
        this.n = s.n;
        this.grid = (Game.Letter?[,])s.grid.Clone();
    }

    public State NextStateWithMove(int i, int j, Game.Letter letter)
    {
        if (!CanPlayOn(i, j)) throw new Exception("Wrong move.");

        State nextState = new State(this);
        nextState.lastUpdate = $"{letter} on: {(i + 1, j + 1)}";
        nextState.SetTurn(!isXTurn);
        nextState.SetMove(i, j, letter);


        return nextState;
    }

    public void SetTurn(bool Xturn) => isXTurn = Xturn;

    public void SetMove(int i, int j, Game.Letter letter) => grid[i, j] = letter;

    public bool CanPlayOn(int i, int j) => i >= 0 && j >= 0 && i < n && j < n && grid[i, j] == null;

    public bool isTie() => DivisionsAchive(l => l != null) == n + n + 2;

    public bool isWinForLetter(Game.Letter letter) => DivisionsAchive(l => l.Equals(letter)) > 0;

    public List<State> GetNextStates(Game.Letter myLetter)
    {
        var states = new List<State>();

        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                if (CanPlayOn(i, j))
                {
                    var newState = NextStateWithMove(i, j, myLetter);
                    states.Add(newState);
                }
        return states;
    }

    public State GetBestStateUsingMinMax(int level, Game.Letter myLetter, Game.Letter opponentLetter)
    {
        IEnumerable<(State, int)> nextStatesWithEvaluation
        = from s in GetNextStates(myLetter)
          select (s, s.MinMax(level - 1, false, myLetter, opponentLetter));

        var result = nextStatesWithEvaluation.MaxBy(s => s.Item2);
        Console.WriteLine($"MinMax: at level {level}: {result.Item2}");
        return result.Item1;
    }

    public int MinMax(int level, bool myTurn, Game.Letter myLetter, Game.Letter opponentLetter)
    {
        if (isWinForLetter(myLetter)) return myTurn ? int.MaxValue - 1 : int.MinValue + 1;
        if (isWinForLetter(opponentLetter)) return myTurn ? int.MinValue + 1 : int.MaxValue - 1;
        if (level == 0 || isTie()) return EvaluateForFirstLetter(myLetter, opponentLetter);

        IEnumerable<int> nextStatesWithEvaluation = from s in GetNextStates(myTurn ? myLetter : opponentLetter)
                                                    select s.MinMax(level - 1, !myTurn, myLetter, opponentLetter);

        return myTurn ? nextStatesWithEvaluation.Max() : nextStatesWithEvaluation.Min();
    }

    public State GetBestStateUsingAlphaBeta(int level, Game.Letter myLetter, Game.Letter opponentLetter)
    {
        IEnumerable<(State, int)> nextStatesWithEvaluation
        = from s in GetNextStates(myLetter)
          select (s, s.AlphaBeta(level - 1, false, myLetter, opponentLetter, int.MinValue));

        var result = nextStatesWithEvaluation.MaxBy(s => s.Item2);
        Console.WriteLine($"AlphaBeta: at level {level}: {result.Item2}");
        return result.Item1;
    }

    public int AlphaBeta(int level, bool myTurn, Game.Letter myLetter
    , Game.Letter opponentLetter, int parentValue)
    {
        if (isWinForLetter(myLetter)) return myTurn ? int.MaxValue - 1 : int.MinValue + 1;
        if (isWinForLetter(opponentLetter)) return myTurn ? int.MinValue + 1 : int.MaxValue - 1;
        if (level == 0 || isTie()) return EvaluateForFirstLetter(myLetter, opponentLetter);

        int currentValue;
        var nextStates = GetNextStates(myTurn ? myLetter : opponentLetter);

        if (myTurn)
        {
            currentValue = int.MinValue;
            foreach (var s in nextStates)
            {
                int childValue = s.AlphaBeta(level - 1, !myTurn, myLetter, opponentLetter, currentValue);
                currentValue = Math.Max(currentValue, childValue);
                if (childValue >= parentValue) break;
            }
        }
        else
        {
            currentValue = int.MaxValue;
            foreach (var s in nextStates)
            {
                int childValue = s.AlphaBeta(level - 1, !myTurn, myLetter, opponentLetter, currentValue);
                currentValue = Math.Min(currentValue, childValue);
                if (childValue <= parentValue) break;
            }
        }

        return currentValue;
    }



    public int EvaluateForFirstLetter(Game.Letter myLetter, Game.Letter opponentLetter)
    => DivisionsAchive(l => !l.Equals(opponentLetter)) - DivisionsAchive(l => !l.Equals(myLetter));


    public int DivisionsAchive(Predicate<Game.Letter?> predicate)
    {
        int result = 0;
        for (int i = 0; i < n; i++)
        {
            Game.Letter?[] row = Enumerable.Range(0, n).Select(x => grid[i, x]).ToArray();
            Game.Letter?[] column = Enumerable.Range(0, n).Select(x => grid[x, i]).ToArray();
            result += IsTrueForAllValues(row, predicate) + IsTrueForAllValues(column, predicate);
        }

        Game.Letter?[] forwardDiameter = Enumerable.Range(0, n).Select(x => grid[x, x]).ToArray();
        Game.Letter?[] backwardDiameter = Enumerable.Range(0, n).Select(x => grid[x, n - x - 1]).ToArray();
        return result + IsTrueForAllValues(forwardDiameter, predicate) + IsTrueForAllValues(backwardDiameter, predicate);
    }

    private int IsTrueForAllValues(Game.Letter?[] array, Predicate<Game.Letter?> predicate)
        => Array.TrueForAll(array, predicate) ? 1 : 0;

    public override string ToString()
    {
        string value = lastUpdate + "\n\n";
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
                value += grid[i, j] == null ? '-' : grid[i, j].ToString();
            value += '\n';
        }
        return value + '\n';
    }
}

