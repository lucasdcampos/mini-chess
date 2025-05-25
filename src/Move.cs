public struct Move
{
    public readonly int StartSquare;
    public readonly int TargetSquare;

    public Move(int startSquare, int targetSquare)
    {
        StartSquare = startSquare;
        TargetSquare = targetSquare;
    }

    public override bool Equals(object? obj) =>
        obj is Move other && StartSquare == other.StartSquare && TargetSquare == other.TargetSquare;

    public override int GetHashCode() => HashCode.Combine(StartSquare, TargetSquare);
}