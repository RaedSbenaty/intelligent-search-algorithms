using System;
using System.Collections.Generic;

struct Position
{
    private int x, y;
    public int X { get => x; }
    public int Y { get => y; }

    public static float CalculateDistanceBetween(Position p1, Position p2)
       => (float)Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));

    public Position(int x, int y) => (this.x, this.y) = (x, y);

    public List<Position> GetNeighbors()
    {
        List<Position> neighbors = new List<Position>();
        neighbors.Add(new Position(x, y - 1));
        neighbors.Add(new Position(x, y + 1));
        neighbors.Add(new Position(x + 1, y));
        neighbors.Add(new Position(x - 1, y));
        return neighbors;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        Position p = (Position)obj;
        return x == p.x && y == p.y;
    }

    public override int GetHashCode() => base.GetHashCode();

    public override string ToString() => $"({x}, {y})";

    //     public static int CalculateDistanceBetween(Position p1, Position p2)
    //   => Math.Abs(p1.x - p2.x) + Math.Abs(p1.y - p2.y);

}

