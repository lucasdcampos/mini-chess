public struct Move
{
    public readonly int StartSquare;
    public readonly int TargetSquare;

    public readonly int MovedPiece;
    public readonly int CapturedPiece;
    public readonly int PromotionPiece;

    public readonly bool WasWhiteToMove;

    public Move(int startSquare, int targetSquare)
    {
        StartSquare = startSquare;
        TargetSquare = targetSquare;

        MovedPiece = 0;
        CapturedPiece = 0;
        PromotionPiece = 0;
        WasWhiteToMove = true;
    }

    public Move(int startSquare, int targetSquare, int movedPiece, int capturedPiece, int promotionPiece, bool wasWhiteToMove)
    {
        StartSquare = startSquare;
        TargetSquare = targetSquare;

        MovedPiece = movedPiece;
        CapturedPiece = capturedPiece;
        PromotionPiece = promotionPiece;
        WasWhiteToMove = wasWhiteToMove;
    }

    public override bool Equals(object? obj) =>
        obj is Move other &&
        StartSquare == other.StartSquare &&
        TargetSquare == other.TargetSquare;

    public override int GetHashCode() => HashCode.Combine(StartSquare, TargetSquare);

    public bool IsCapture() => CapturedPiece != 0;
    public bool IsPromotion() => PromotionPiece != 0;
}
