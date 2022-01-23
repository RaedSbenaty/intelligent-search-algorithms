using System;
using System.Collections.Generic;
using System.Linq;

class State
{
    private Level stateLevel;
    private String lastUpdate;
    private State? parentState;
    private Dictionary<Car, Position> carPlaces;
    private Dictionary<Position, Car> occupiedPositions;

    public Level StateLevel { get => stateLevel; }
    public String LastUpdate { get => lastUpdate; }
    public State? ParentState { get => parentState; }
    public Dictionary<Car, Position> CarPlaces { get => carPlaces; }
    public Dictionary<Position, Car> OccupiedPositions { get => occupiedPositions; }


    public State(Level stateLevel, State? parentState, String lastUpdate, Dictionary<Car, Position> carPlaces)
    {
        (this.stateLevel, this.lastUpdate, this.parentState) = (stateLevel, lastUpdate, parentState);
        this.carPlaces = new Dictionary<Car, Position>();
        this.occupiedPositions = new Dictionary<Position, Car>();

        foreach (var (car, place) in carPlaces)
            SetCarPlace(car, place);
    }

    public Position GetCarPlace(Car car) => carPlaces[car];

    public void SetCarPlace(Car car, Position carPosition)
    {
        if (carPlaces.ContainsKey(car))
            car.WillOccupiesPositions(carPlaces[car]).ForEach(postition => occupiedPositions.Remove(postition));

        carPlaces[car] = carPosition;
        car.WillOccupiesPositions(carPosition).ForEach(postition => occupiedPositions[postition] = car);
    }

    public bool IsPositionOutOfBoundry(Position p)
    {
        if (stateLevel.Exit.Equals(p)) return false;
        return p.X < 1 || p.Y < 1 || p.X > stateLevel.N || p.Y > stateLevel.M;
    }

    public bool IsPositionOccupiedByMainCar(Position position)
    {
        if (!occupiedPositions.ContainsKey(position)) return false;
        Car car = occupiedPositions[position];
        return car.IsMainCar;
    }

    public bool CanMoveOnPosition(Position p)
    => !occupiedPositions.ContainsKey(p) && !IsPositionOutOfBoundry(p);

    public List<State> GetNextStates()
    {
        var states = new List<State>();

        foreach (Car car in carPlaces.Keys)
        {
            if (car.CanMoveForward(this))
            {
                var newState = new State(stateLevel, this, "Car #" + car.ID + ": forward move", carPlaces);
                car.MoveForward(newState);
                states.Add(newState);
            }

            if (car.CanMoveBackward(this))
            {
                var newState = new State(stateLevel, this, "Car #" + car.ID + ": backward move", carPlaces);
                car.MoveBackward(newState);
                states.Add(newState);
            }
        }
        return states;
    }

    public float CarHeuristicToExit(Car car, Position exit)
    => Position.CalculateDistanceBetween(carPlaces[car], exit);

    public float CarHerusticToEmptyRoad(Car car, Position exit)
    {
        //getting main car position
        Position mainPosition = carPlaces[car];

        //getting information about car & exit
        //position but on 1D (direction diminsion)
        int carProjected = mainPosition.Project(car.Direction);
        int exitProjected = exit.Project(car.Direction);

        //kowing whether the car should move
        //forward or backward to reach the exit
        int step = exitProjected > carProjected ? 1 : -1;

        //for all positions between the car and the exit
        //if there was an occupied positionbetween them
        //then the final result will be related tohow 
        //this position is close tothe car (how is far from exit)
        int result = 0;
        for (int i = carProjected; i != exitProjected; i += step)
        {
            //getting the next position
            Position position =
            (mainPosition.X + (1 - car.Direction) * i
            , mainPosition.Y + car.Direction * i);

            if (occupiedPositions.ContainsKey(position))
                result += Math.Abs(exitProjected - i);
        }

        return result;
    }

    public (State?, int) DFS(Stack<State> stack, HashSet<State> visited)
    {
        int statesCounter = 0;
        stack.Push(this);
        visited.Add(this);

        while (stack.Count != 0)
        {
            State currentState = stack.Pop();
            if (stateLevel.IsWon(currentState)) return (currentState, statesCounter);

            foreach (var nextState in currentState.GetNextStates())
            {
                if (!visited.Contains(nextState))
                {
                    statesCounter++;
                    visited.Add(nextState);
                    stack.Push(nextState);
                }
            }
        }
        return (null, statesCounter);
    }

    public (State?, int) BFS(Queue<State> queue, HashSet<State> visited)
    {
        int statesCounter = 0;
        queue.Enqueue(this);
        visited.Add(this);

        while (queue.Count != 0)
        {
            State currentState = queue.Dequeue();
            if (stateLevel.IsWon(currentState)) return (currentState, statesCounter);

            foreach (var nextState in currentState.GetNextStates())
            {
                if (!visited.Contains(nextState))
                {
                    statesCounter++;
                    visited.Add(nextState);
                    queue.Enqueue(nextState);
                }
            }
        }
        return (null, statesCounter);
    }

    public (State?, int) Dijkstra(PriorityQueue<State, int> queue, Dictionary<State, int> distance, HashSet<State> visited)
    {
        int statesCounter = 0;
        queue.Enqueue(this, 0);
        distance[this] = 0;

        while (queue.Count != 0)
        {
            State currentState = queue.Dequeue();
            if (stateLevel.IsWon(currentState)) return (currentState, statesCounter);

            if (visited.Contains(currentState)) continue;
            visited.Add(currentState);

            foreach (var nextState in currentState.GetNextStates())
                if (!distance.ContainsKey(nextState) || distance[nextState] > distance[currentState] + 1)
                {
                    statesCounter++;
                    distance[nextState] = distance[currentState] + 1;
                    queue.Enqueue(nextState, distance[nextState]);
                }
        }
        return (null, statesCounter);
    }

    public (State?, int) AStar(PriorityQueue<State, float> queue, Dictionary<State, int> distance, HashSet<State> visited)
    {
        int statesCounter = 0;
        queue.Enqueue(this, 0);
        distance[this] = 0;

        while (queue.Count != 0)
        {
            State currentState = queue.Dequeue();
            if (stateLevel.IsWon(currentState)) return (currentState, statesCounter);

            if (visited.Contains(currentState)) continue;
            visited.Add(currentState);

            foreach (var nextState in currentState.GetNextStates())
                if (!distance.ContainsKey(nextState) || distance[nextState] > distance[currentState] + 1)
                {
                    statesCounter++;
                    distance[nextState] = distance[currentState] + 1;
                    queue.Enqueue(nextState, stateLevel.HeuristicFunction(nextState) + distance[nextState]);
                }
        }
        return (null, statesCounter);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;

        State state = (State)obj;

        return this.carPlaces.Keys.Count == state.carPlaces.Keys.Count &&
               this.carPlaces.Keys.All(
                  k => state.carPlaces.ContainsKey(k) && this.carPlaces[k].Equals(state.carPlaces[k]));
    }

    public override int GetHashCode()
    {
        int hashValue = 0;
        foreach (var (car, position) in carPlaces)
            hashValue += (car.ID * position.X * position.Y) + 10000000 * position.X + 1000000000 * position.Y;

        return hashValue;
    }
}


/*
  String arena = "";
        foreach (var carplace in carPlaces)
            arena += carplace.Value.X + ',' + carplace.Value.Y + '/';
        return arena.GetHashCode();
*/

/*

    public int IsCarBlockingExitForOtherCar(Car car, Car other, Position exit)
    {
        int projectOther = carPlaces[other].Project(other.Direction);
        int projectNormalCar = carPlaces[car].Project(other.Direction) - projectOther;
        int projectNormalExit = exit.Project(other.Direction) - projectOther;
        return projectNormalCar * projectNormalExit < 0 ? 0 :
        projectNormalExit > 0 ? 1 : -1;
    }
*/