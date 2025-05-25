public struct Move
{
    public readonly int StartSquare;
    public readonly int TargetSquare;

    public Move(int startSquare, int targetSquare)
    {
        StartSquare = startSquare;
        TargetSquare = targetSquare;
    }
}