using System;
using System.Collections.Generic;
using Newtonsoft.Json; //to install: dotnet add package Newtonsoft.Json

class Level
{
    private int n, m;
    private State currentState;
    private Position initialTrackPosition;
    private HashSet<Position> buildingsPositions;
    private Dictionary<Position, Position> sourceDestinationDelivery = new Dictionary<Position, Position>();
    private Dictionary<Position, Position> destinationSourceDelivery = new Dictionary<Position, Position>();

    public int N { get => n; }
    public int M { get => m; }
    public Position InitialTrackPosition { get => initialTrackPosition; }

    public List<Position> PackagesPositions { get => sourceDestinationDelivery.Keys.ToList(); }
    public HashSet<Position> BuildingsPositions { get => buildingsPositions; }

    public static Level? Create(string path)
    {
        StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        reader.Close();
        Level? level = JsonConvert.DeserializeObject<Level>(json);
        return level;
    }

    public Level(int n, int m, HashSet<Position> buildingsPositions, Position initialTrackPosition,
                        Position[] delievrySources, Position[] delievryDestinations)
    {
        (this.n, this.m) = (n, m);
        (this.buildingsPositions, this.initialTrackPosition) = (buildingsPositions, initialTrackPosition);
        this.currentState = new State(this, null, initialTrackPosition);

        for (int i = 0; i < delievrySources.Length; i++)
        {
            Position source = delievrySources[i], destination = delievryDestinations[i];
            sourceDestinationDelivery[source] = destination;
            destinationSourceDelivery[destination] = source;
        }
    }

    public Position GetDeliveryDestinationForSource(Position p) => sourceDestinationDelivery[p];

    public Position GetDeliverySourceForDestination(Position p) => destinationSourceDelivery[p];

    public bool IsDeliverySource(Position p) => sourceDestinationDelivery.ContainsKey(p);

    public bool IsDeliveryDestination(Position p) => destinationSourceDelivery.ContainsKey(p);

    public (State, int, int) FindSolution(Func<State, float>? HeuristicFunction = null)
    => currentState.FindSolution(HeuristicFunction);

    public float HeuristicFunction1(State state)
    => Position.CalculateDistanceBetween(state.TrackPosition, InitialTrackPosition);

    public float HeuristicFunction2(State state)
    => state.PackageDelieverCost(PackagesPositions[0]);

    public float HeuristicFunction3(State state)
    => PackagesPositions.Max(packageSource => state.PackageDelieverCost(packageSource));

}


