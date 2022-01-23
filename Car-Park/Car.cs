using System;
using System.Collections.Generic;

class Car
{
    private static int carsNum = 0;
    private int length, direction, id;
    bool isMainCar;

    public int Length { get => length; }
    public int Direction { get => direction; }
    public int ID { get => id; }
    public bool IsMainCar { get => isMainCar; }

    public Car(int length, int direction, bool isMainCar)
    => (this.length, this.direction, this.id, this.isMainCar) = (length, direction, carsNum++, isMainCar);

    public List<Position> WillOccupiesPositions(Position position)
    {
        var positions = new List<Position>();

        for (var i = 0; i < length; i++)
            positions.Add((position.X + ((1 - direction) * i), position.Y + (direction * i)));

        return positions;
    }

    public bool CanMoveForward(State state)
    {
        Position oldPosition = state.GetCarPlace(this);
        Position newPosition = (oldPosition.X + ((1 - direction) * length), oldPosition.Y + (direction * length));
        return state.CanMoveOnPosition(newPosition);
    }

    public bool CanMoveBackward(State state)
    {
        Position oldPosition = state.GetCarPlace(this);
        Position newPosition = (oldPosition.X + ((1 - direction) * -1), oldPosition.Y + (direction * -1));
        return state.CanMoveOnPosition(newPosition);
    }

    public void MoveForward(State state)
    {
        Position oldPosition = state.GetCarPlace(this);
        Position newPosition = (oldPosition.X + ((1 - direction) * 1), oldPosition.Y + direction);
        state.SetCarPlace(this, newPosition);
    }

    public void MoveBackward(State state)
    {
        Position oldPosition = state.GetCarPlace(this);
        Position newPosition = (oldPosition.X + ((1 - direction) * -1), oldPosition.Y - direction);
        state.SetCarPlace(this, newPosition);
    }

    public override string ToString() => "" + direction;
}