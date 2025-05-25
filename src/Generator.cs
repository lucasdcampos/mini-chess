public static class Generator
{
    public static List<Move> GenerateMoves(Board board)
    {
        var moves = new List<Move>();
        bool isWhiteToMove = board.IsWhiteToMove;
        var pieceIndices = isWhiteToMove ? board.WhitePieces : board.BlackPieces;

        foreach (int start in pieceIndices)
        {
            int piece = board.Squares![start];

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

    public static List<Move> GenerateCaptureMoves(Board board)
    {
        var captureMoves = new List<Move>();
        bool isWhiteToMove = board.IsWhiteToMove;
        int ownColor = isWhiteToMove ? Piece.White : Piece.Black;

        for (int start = 0; start < 64; start++)
        {
            int piece = board.Squares![start];
            if (piece == Piece.None || piece.Color() != ownColor)
                continue;

            int[] captureOffsets = piece.Type() switch
            {
                Piece.Pawn => new[] { (ownColor == Piece.White ? 7 : -9), (ownColor == Piece.White ? 9 : -7) },
                Piece.Knight => new[] { -17, -15, -10, -6, 6, 10, 15, 17 },
                Piece.King => new[] { -1, 1, -8, 8, -7, 7, -9, 9 },
                Piece.Bishop => new[] { -9, -7, 7, 9 },
                Piece.Rook => new[] { -8, -1, 1, 8 },
                Piece.Queen => new[] { -8, -1, 1, 8, -9, -7, 7, 9 },
                _ => Array.Empty<int>()
            };

            foreach (int offset in captureOffsets)
            {
                int target = start;
                bool isSlidingPiece = piece.Type() is Piece.Bishop or Piece.Rook or Piece.Queen;

                do
                {
                    target += offset;

                    if (!IsOnBoard(target) || (isSlidingPiece && CrossesEdge(target - offset, target, offset)))
                        break;

                    int targetPiece = board.Squares![target];

                    if (targetPiece == Piece.None)
                    {
                        if (!isSlidingPiece) break;
                        continue;
                    }

                    if (targetPiece.Color() != ownColor)
                        captureMoves.Add(new Move(start, target));

                    break;
                } while (isSlidingPiece);
            }
        }

        captureMoves.Sort((a, b) =>
        {
            int aScore = 10 * Evaluator.GetPieceValue(board.Squares![a.TargetSquare].Type()) -
                        Evaluator.GetPieceValue(board.Squares[a.StartSquare].Type());

            int bScore = 10 * Evaluator.GetPieceValue(board.Squares[b.TargetSquare].Type()) -
                        Evaluator.GetPieceValue(board.Squares[b.StartSquare].Type());

            return bScore.CompareTo(aScore);
        });

        return captureMoves;
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
            moves.Add(new Move(start, forwardSquare, piece, Piece.None, Piece.None, ownColor == Piece.White, false, false));

            // Double push
            if (startY == startRank)
            {
                int doubleForward = forwardSquare + direction;
                if (IsOnBoard(doubleForward) && board.Squares[doubleForward] == Piece.None)
                {
                    moves.Add(new Move(start, doubleForward, piece, Piece.None, Piece.None, ownColor == Piece.White, true, false));
                }
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
                moves.Add(new Move(start, captureSquare, piece, targetPiece, Piece.None, ownColor == Piece.White, false, false));
            }

            // En Passant
            var lastMove = board.LastMove;
            if (lastMove.DoublePawnPush)
            {
                int enPassantTargetSquare = lastMove.TargetSquare;
                int enPassantCaptureSquare = enPassantTargetSquare - direction;

                // Check if the current pawn can perform En Passant
                if (captureSquare == enPassantTargetSquare)
                {
                    moves.Add(new Move(
                        start,
                        enPassantTargetSquare,
                        piece,
                        lastMove.MovedPiece,
                        Piece.None,
                        ownColor == Piece.White,
                        false,
                        true)); // Mark as En Passant
                }
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