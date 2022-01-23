using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

class Level
{
    private int n, m;
    private Position exit;
    private Car mainCar;
    private State currentState;

    public int N { get => n; }
    public int M { get => m; }
    public Position Exit { get => exit; }
    public Car MainCar { get => mainCar; }

    public static Level? Create(string path)
    {
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        Level? level = JsonConvert.DeserializeObject<Level>(json);
        return level;
    }

    public Level(int n, int m, Position exit, List<Car> cars, List<Position> initalCarsPositions)
    {
        (this.n, this.m, this.exit, this.mainCar) = (n, m, exit, cars[0]);
        var carPlaces = cars.Zip(initalCarsPositions, (c, p) => new { c, p })
              .ToDictionary(x => x.c, x => x.p);

        this.currentState = new State(this, null, "Initial state", carPlaces);
    }

    public bool IsWon(State state) => state.IsPositionOccupiedByMainCar(exit);

    public float HeuristicFunction(State state) =>
     state.CarHeuristicToExit(mainCar, exit) + state.CarHerusticToEmptyRoad(mainCar, exit);

    public void UserMood(Player player)
    {
        while (!IsWon(currentState))
        {
            State nextState = player.ChoiceNextState(currentState);
            currentState = nextState;
        }
    }

    public void DFSMood(Player player)
    {
        var (winnerState, statesCounter) = currentState.DFS(new Stack<State>(), new HashSet<State>());
        if (winnerState != null)
            player.ShowSolution(winnerState, statesCounter);
    }

    public void BFSMood(Player player)
    {
        var (winnerState, statesCounter) = currentState.BFS(new Queue<State>(), new HashSet<State>());
        if (winnerState != null)
            player.ShowSolution(winnerState, statesCounter);
    }

    public void DijkstraMood(Player player)
    {
        var (winnerState, statesCounter) = currentState.Dijkstra(new PriorityQueue<State, int>(), new Dictionary<State, int>(), new HashSet<State>());
        if (winnerState != null)
            player.ShowSolution(winnerState, statesCounter);
    }

    public void AStarMood(Player player)
    {
        var (winnerState, statesCounter) = currentState.AStar(new PriorityQueue<State, float>(), new Dictionary<State, int>(), new HashSet<State>());
        if (winnerState != null)
            player.ShowSolution(winnerState, statesCounter);
    }

}