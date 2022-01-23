using System;
using System.Collections.Generic;

class State
{
    private Level level;
    private State? parentState;
    private Position trackPosition;
    private HashSet<Position> pickedPackagesPositions;
    private HashSet<Position> deliverdPackagesPositions;

    public State? ParentState { get => parentState; }
    public Position TrackPosition { get => trackPosition; }


    public State(Level level, State? parentState, Position trackPosition)
    : this(level, parentState, trackPosition, new HashSet<Position>(), new HashSet<Position>()) { }

    public State(Level level, State? parentState, Position trackPosition,
     HashSet<Position> pickedPackagesPositions, HashSet<Position> deliverdPackagesPositions)
    {
        (this.level, this.parentState, this.trackPosition) = (level, parentState, trackPosition);
        this.pickedPackagesPositions = new HashSet<Position>(pickedPackagesPositions);
        this.deliverdPackagesPositions = new HashSet<Position>(deliverdPackagesPositions);
    }

    public bool IsPositionOutOfBoundry(Position p) =>
         p.X < 1 || p.Y < 1 || p.X > level.N || p.Y > level.M;

    public bool CanMoveOnPosition(Position p)
    => !IsPositionOutOfBoundry(p) && !level.BuildingsPositions.Contains(p);

    public bool IsWon() =>
         deliverdPackagesPositions.Count == level.PackagesPositions.Count
         && trackPosition.Equals(level.InitialTrackPosition);

    public bool CanPickPackage()
    {
        if (!level.IsDeliverySource(trackPosition)) return false;  //no package to pick
        if (pickedPackagesPositions.Contains(trackPosition)) return false; //package is already picked

        Position currentDeliveryDestination = level.GetDeliveryDestinationForSource(trackPosition);
        return !deliverdPackagesPositions.Contains(currentDeliveryDestination); //package is already deliverd or not
    }

    public bool CanDeliverPackage()
    {
        if (!level.IsDeliveryDestination(trackPosition)) return false;  //trackPosition is not a destination
        if (deliverdPackagesPositions.Contains(trackPosition)) return false; //package is already deliverd

        Position currentDeliverySource = level.GetDeliverySourceForDestination(trackPosition);
        return pickedPackagesPositions.Contains(currentDeliverySource); //package is already picked or not
    }

    public void PickPackage() => pickedPackagesPositions.Add(trackPosition);

    public void DeliverPackage()
    {
        var source = level.GetDeliverySourceForDestination(trackPosition);
        pickedPackagesPositions.Remove(source);
        deliverdPackagesPositions.Add(trackPosition);
    }

    public List<State> GetNextStates()
    {
        var states = new List<State>();

        foreach (var p in trackPosition.GetNeighbors())
            if (CanMoveOnPosition(p))
            {
                var nextState = new State(level, this, p, pickedPackagesPositions, deliverdPackagesPositions);
                states.Add(nextState);
            }

        if (CanPickPackage())
        {
            var nextState = new State(level, this, trackPosition, pickedPackagesPositions, deliverdPackagesPositions);
            nextState.PickPackage();
            states.Add(nextState);
        }

        if (CanDeliverPackage())
        {
            var nextState = new State(level, this, trackPosition, pickedPackagesPositions, deliverdPackagesPositions);
            nextState.DeliverPackage();
            states.Add(nextState);
        }

        return states;
    }

    public bool HasSameTrackPosition(State state) => trackPosition.Equals(state.trackPosition);

    public int MovementCost() => pickedPackagesPositions.Count + 1;

    public float PackageDelieverCost(Position packageSource)
    {
        if (deliverdPackagesPositions.Contains(packageSource)) return 0;

        var packageDestination = level.GetDeliveryDestinationForSource(packageSource);

        if (pickedPackagesPositions.Contains(packageSource))
            return Position.CalculateDistanceBetween(trackPosition, packageDestination) * MovementCost()
                 + Position.CalculateDistanceBetween(packageDestination, level.InitialTrackPosition) * (MovementCost() - 1); // 1 deleted since i delivered the package

        else
            return Position.CalculateDistanceBetween(trackPosition, packageSource) * MovementCost()
                 + Position.CalculateDistanceBetween(packageSource, packageDestination) * (MovementCost() + 1)
                 + Position.CalculateDistanceBetween(packageDestination, level.InitialTrackPosition) * MovementCost();
    }

    public (State, int, int) FindSolution(Func<State, float>? HeuristicFunction = null)
    {
        PriorityQueue<(State, int), float> queue = new PriorityQueue<(State, int), float>();
        Dictionary<State, int> cost = new Dictionary<State, int>();

        int statesCounter = 0;
        queue.Enqueue((this, 0), 0);
        cost[this] = 0;

        while (queue.Count != 0)
        {
            (State currentState, int currentCost) = queue.Dequeue();
            statesCounter++;

            if (currentCost > cost[currentState]) continue;
            if (currentState.IsWon()) return (currentState, currentCost, statesCounter);

            foreach (var nextState in currentState.GetNextStates())
            {
                int newCost = !currentState.HasSameTrackPosition(nextState) ? currentState.MovementCost() : 0;

                if (!cost.ContainsKey(nextState) || cost[nextState] > currentCost + newCost)
                {
                    cost[nextState] = currentCost + newCost;
                    float heuristicValue = HeuristicFunction == null ? 0 : HeuristicFunction(nextState);
                    queue.Enqueue((nextState, cost[nextState]), cost[nextState] + heuristicValue);
                }
            }
        }
        throw new Exception("There some packages you can't arrive.");
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        State state = (State)obj;

        return this.trackPosition.Equals(state.trackPosition)
        && this.pickedPackagesPositions.SetEquals(state.pickedPackagesPositions)
        && this.deliverdPackagesPositions.SetEquals(state.deliverdPackagesPositions);
    }

    public override int GetHashCode()
    {
        int hashValue = 0;
        for (int i = 1; i <= level.N; i++)
            for (int j = 1; j <= level.M; j++)
            {
                Position p = new Position(i, j);
                int value = 0;
                if (trackPosition.Equals(p)) value += 1;
                if (pickedPackagesPositions.Contains(p)) value += 64;
                if (deliverdPackagesPositions.Contains(p)) value += 128;
                hashValue += (value * i + value * j) + 10000 * i + 10000000 * j;
            }
        return hashValue;
    }

    public override string ToString()
    {
        string value = "___________________________________________\n\n";

        value += $"trackPosition: {trackPosition}\n\n";

        value += "pickedPackagesPositions:\n";
        foreach (var p in pickedPackagesPositions)
            value += $"{p}\n";

        value += "\ndeliverdPackagesPositions:\n";
        foreach (var p in deliverdPackagesPositions)
            value += $"{p}\n";

        value += "\n___________________________________________";
        return value;
    }
}
