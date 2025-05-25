public static class Generator
{
    public static List<Move> GenerateMoves(Board board)
    {
        var moves = new List<Move>();
        bool isWhiteToMove = board.IsWhiteToMove;
        int ownColor = isWhiteToMove ? Piece.White : Piece.Black;

        int[] knightOffsets = { -17, -15, -10, -6, 6, 10, 15, 17 };

        for (int from = 0; from < 64; from++)
        {
            int piece = board.Squares![from];
            if (piece == Piece.None)
                continue;

            if (piece.Type() != Piece.Knight)
                continue;

            if (piece.Color() != ownColor)
                continue;

            int fromX = from % 8;
            int fromY = from / 8;

            foreach (int offset in knightOffsets)
            {
                int to = from + offset;

                if (to < 0 || to >= 64)
                    continue;

                int toX = to % 8;
                int toY = to / 8;

                int dx = Math.Abs(toX - fromX);
                int dy = Math.Abs(toY - fromY);
                if (!(dx == 1 && dy == 2 || dx == 2 && dy == 1))
                    continue;

                int targetPiece = board.Squares[to];

                if (targetPiece != Piece.None &&
                    targetPiece.Color() == ownColor)
                    continue;

                moves.Add(new Move(from, to));
            }
        }

        return moves;
    }
}