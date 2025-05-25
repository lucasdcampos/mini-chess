public static class Evaluator
{
    public static int Evaluate(Board board)
    {
        int score = 0;

        var pieceValues = new Dictionary<int, int>
        {
            { Piece.Pawn, 100 },
            { Piece.Knight, 320 },
            { Piece.Bishop, 330 },
            { Piece.Rook, 500 },
            { Piece.Queen, 900 },
            { Piece.King, 99999 }
        };

        foreach (var square in board.Squares!)
        {
            if (square == Piece.None)
                continue;

            int value = pieceValues[square.Type()];

            if (square.Color() == Piece.White)
                score += value;
            else
                score -= value;
        }

        return score;
    }
}