public struct Move : IEquatable<Move>
{
    public readonly int StartSquare;
    public readonly int TargetSquare;
    public readonly int MovedPiece;
    public readonly int CapturedPiece;
    public readonly int PromotionPiece;

    public readonly bool WasWhiteToMove;
    public readonly bool DoublePawnPush;
    public readonly bool EnPassant;

    // Primary constructor
    public Move(
        int startSquare, 
        int targetSquare, 
        int movedPiece = 0, 
        int capturedPiece = 0, 
        int promotionPiece = 0, 
        bool wasWhiteToMove = true, 
        bool doublePawnPush = false, 
        bool enPassant = false)
    {
        if (startSquare < 0 || startSquare > 63) throw new ArgumentOutOfRangeException(nameof(startSquare));
        if (targetSquare < 0 || targetSquare > 63) throw new ArgumentOutOfRangeException(nameof(targetSquare));

        StartSquare = startSquare;
        TargetSquare = targetSquare;

        MovedPiece = movedPiece;
        CapturedPiece = capturedPiece;
        PromotionPiece = promotionPiece;

        WasWhiteToMove = wasWhiteToMove;
        DoublePawnPush = doublePawnPush;
        EnPassant = enPassant;
    }

    public bool IsCapture => CapturedPiece != 0;
    public bool IsPromotion => PromotionPiece != 0;

    public override bool Equals(object? obj) =>
    obj is Move other &&
    StartSquare == other.StartSquare &&
    TargetSquare == other.TargetSquare;

    public bool Equals(Move other)
    {
        return StartSquare == other.StartSquare &&
                TargetSquare == other.TargetSquare;
    }

    public override int GetHashCode() =>
        HashCode.Combine(StartSquare, TargetSquare);

    public override string ToString() =>
        $"Move({StartSquare} -> {TargetSquare}, Moved: {MovedPiece}, Captured: {CapturedPiece}, Promotion: {PromotionPiece}, EnPassant: {EnPassant}, DoublePush: {DoublePawnPush})";
}
