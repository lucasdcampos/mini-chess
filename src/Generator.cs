public static class Generator
{
    public static List<Move> GenerateMoves(Board board)
    {
        var moves = new List<Move>();
        bool isWhiteToMove = board.IsWhiteToMove;
        int ownColor = isWhiteToMove ? Piece.White : Piece.Black;

        for (int start = 0; start < 64; start++)
        {
            int piece = board.Squares![start];
            if (piece == Piece.None || piece.Color() != ownColor)
                continue;

            switch (piece.Type())
            {
                case Piece.Pawn:
                    GeneratePawnMoves(board, start, piece, moves);
                    break;
                case Piece.Knight:
                    GenerateKnightMoves(board, start, piece, moves);
                    break;
                case Piece.Bishop:
                case Piece.Rook:
                case Piece.Queen:
                    GenerateSlidingMoves(board, start, piece, moves);
                    break;
                case Piece.King:
                    GenerateKingMoves(board, start, piece, moves);
                    break;
            }
        }

        return moves;
    }

    private static void GeneratePawnMoves(Board board, int start, int piece, List<Move> moves)
    {
        int ownColor = piece.Color();
        int direction = ownColor == Piece.White ? 8 : -8;
        int startRank = ownColor == Piece.White ? 1 : 6;
        int startY = start / 8;
        int startX = start % 8;

        int forwardSquare = start + direction;

        // Simple push
        if (IsOnBoard(forwardSquare) && board.Squares![forwardSquare] == Piece.None)
        {
            moves.Add(new Move(start, forwardSquare));

            // Double push
            if (startY == startRank)
            {
                int doubleForward = forwardSquare + direction;
                if (IsOnBoard(doubleForward) && board.Squares[doubleForward] == Piece.None)
                    moves.Add(new Move(start, doubleForward));
            }
        }

        // Captures
        int[] captureOffsets = { direction - 1, direction + 1 };
        foreach (int offset in captureOffsets)
        {
            int captureSquare = start + offset;
            if (!IsOnBoard(captureSquare)) continue;

            int captureX = captureSquare % 8;

            // Ensure the capture is diagonal
            if (Math.Abs(captureX - startX) != 1) continue;

            int targetPiece = board.Squares![captureSquare];
            if (targetPiece != Piece.None && targetPiece.Color() != ownColor)
            {
                moves.Add(new Move(start, captureSquare));
            }
        }
    }

    private static void GenerateKnightMoves(Board board, int start, int piece, List<Move> moves)
    {
        int[] knightOffsets = { -17, -15, -10, -6, 6, 10, 15, 17 };
        int startX = start % 8;
        int startY = start / 8;
        int ownColor = piece.Color();

        foreach (int offset in knightOffsets)
        {
            int target = start + offset;

            if (target < 0 || target >= 64)
                continue;

            int targetX = target % 8;
            int targetY = target / 8;

            int dx = Math.Abs(targetX - startX);
            int dy = Math.Abs(targetY - startY);
            if (!(dx == 1 && dy == 2 || dx == 2 && dy == 1))
                continue;

            int targetPiece = board.Squares![target];

            if (targetPiece != Piece.None && targetPiece.Color() == ownColor)
                continue;

            moves.Add(new Move(start, target));
        }
    }

    private static void GenerateSlidingMoves(Board board, int start, int piece, List<Move> moves)
    {
        int ownColor = piece.Color();

        int[] allDirections = { -8, -1, 1, 8, -9, -7, 7, 9 };

        int[] directions = piece.Type() switch
        {
            Piece.Rook => allDirections[..4],    // N, W, E, S
            Piece.Bishop => allDirections[4..],  // NW, NE, SW, SE
            Piece.Queen => allDirections,
            _ => Array.Empty<int>()
        };

        foreach (int dir in directions)
        {
            int current = start;

            while (true)
            {
                int target = current + dir;

                if (!IsOnBoard(target) || CrossesEdge(current, target, dir))
                    break;

                int targetPiece = board.Squares![target];

                if (targetPiece == Piece.None)
                {
                    moves.Add(new Move(start, target));
                }
                else
                {
                    if (targetPiece.Color() != ownColor)
                        moves.Add(new Move(start, target)); // Capture
                    break;
                }

                current = target;
            }
        }
    }

    private static bool IsOnBoard(int square) => square >= 0 && square < 64;

    private static bool CrossesEdge(int from, int to, int direction)
    {
        int fromX = from % 8;
        int toX = to % 8;

        int dx = Math.Abs(toX - fromX);

        if (direction == 1 || direction == -1)
            return dx != 1;

        if (Math.Abs(direction) == 7 || Math.Abs(direction) == 9)
            return dx != 1;

        return false;
    }

    private static void GenerateKingMoves(Board board, int start, int piece, List<Move> moves)
    {
        int[] kingOffsets = { -1, 1, -8, 8, -7, 7, -9, 9 };
        int startX = start % 8;
        int startY = start / 8;
        int ownColor = piece.Color();

        foreach (int offset in kingOffsets)
        {
            int target = start + offset;

            if (target < 0 || target >= 64)
                continue;

            int targetX = target % 8;
            int targetY = target / 8;

            int dx = Math.Abs(targetX - startX);
            int dy = Math.Abs(targetY - startY);
            if (!(dx <= 1 && dy <= 1))
                continue;

            int targetPiece = board.Squares![target];

            if (targetPiece != Piece.None && targetPiece.Color() == ownColor)
                continue;

            moves.Add(new Move(start, target));
        }
    }
}