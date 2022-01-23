using System;
using System.Collections.Generic;

struct Position
{
    private int x, y;
    public int X { get => x; }
    public int Y { get => y; }

    public static implicit operator Position((int x, int y) value) => new Position(value.x, value.y);

    public static float CalculateDistanceBetween(Position p1, Position p2)
    => (float)Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y));

    public Position(int x, int y) => (this.x, this.y) = (x, y);

    public int Project(int axis) => (x * (1 - axis) + y * axis);

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        Position p = (Position)obj;
        return x == p.x && y == p.y;
    }

    public override int GetHashCode() => base.GetHashCode();

    public override string ToString() => $"({x}, {y})";
}