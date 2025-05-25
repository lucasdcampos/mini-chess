public static class Engine
{
    public static Move ChooseMove(Board board, int maxDepth = 5)
    {
        var legalMoves = Generator.GenerateMoves(board);
        if (legalMoves.Count == 0)
            return new Move(0, 0); // No moves available

        var transTable = new TranspositionTable();

        Move bestMove = legalMoves[0];
        int alpha = int.MinValue;
        int beta = int.MaxValue;

        foreach (var move in legalMoves)
        {
            board.MakeMove(move);

            int score = Minimax(board, maxDepth - 1, alpha, beta, !board.IsWhiteToMove, transTable);

            board.UnmakeMove();

            if (board.IsWhiteToMove)
            {
                if (score > alpha)
                {
                    alpha = score;
                    bestMove = move;
                }
            }
            else
            {
                if (score < beta)
                {
                    beta = score;
                    bestMove = move;
                }
            }
        }

        return bestMove;
    }


    private static int Quiescence(Board board, int alpha, int beta, int depth = 0, int maxDepth = 5)
    {
        int standPat = Evaluator.Evaluate(board);

        if (standPat >= beta)
            return beta;
        if (alpha < standPat)
            alpha = standPat;

        if (depth >= maxDepth)
            return standPat;

        var captureMoves = Generator.GenerateCaptureMoves(board);
        foreach (var move in captureMoves)
        {
            board.MakeMove(move);
            int score = -Quiescence(board, -beta, -alpha, depth + 1, maxDepth);
            board.UnmakeMove();

            if (score >= beta)
                return beta;
            if (score > alpha)
                alpha = score;
        }
        return alpha;
    }

    private static int Minimax(Board board, int depth, int alpha, int beta, bool maximizingPlayer, TranspositionTable transTable)
    {
        ulong hash = Zobrist.ComputeHash(board);
        var transEntry = transTable.Lookup(hash);

        if (transEntry != null && transEntry.Depth >= depth)
        {
            if (transEntry.Flag == Flag.Exact)
                return transEntry.Score;
            if (transEntry.Flag == Flag.Alpha && transEntry.Score <= alpha)
                return alpha;
            if (transEntry.Flag == Flag.Beta && transEntry.Score >= beta)
                return beta;
        }

        if (depth == 0)
            return Quiescence(board, alpha, beta);

        var moves = Generator.GenerateMoves(board);
        if (moves.Count == 0)
            return Evaluator.Evaluate(board);

        int bestScore;
        int originalAlpha = alpha;
        Move bestMove = new Move(0,0);

        if (maximizingPlayer)
        {
            bestScore = int.MinValue;
            moves.Sort((a, b) =>
            {
                int scoreA = MoveHeuristic(board, a);
                int scoreB = MoveHeuristic(board, b);
                return scoreB.CompareTo(scoreA);
            });

            foreach (var move in moves)
            {
                board.MakeMove(move);

                if (depth == 1 && !move.IsCapture() && Evaluator.Evaluate(board) + 100 < alpha)
                {
                    board.UnmakeMove();
                    continue;
                }

                int eval = Minimax(board, depth - 1, alpha, beta, false, transTable);
                board.UnmakeMove();

                if (eval > bestScore)
                {
                    bestScore = eval;
                    bestMove = move;
                }

                alpha = Math.Max(alpha, eval);
                if (beta <= alpha)
                    break;
            }
        }
        else
        {
            bestScore = int.MaxValue;
            foreach (var move in moves)
            {
                board.MakeMove(move);

                if (depth == 1 && !move.IsCapture() && Evaluator.Evaluate(board) - 100 > beta)
                {
                    board.UnmakeMove();
                    continue;
                }

                int eval = Minimax(board, depth - 1, alpha, beta, true, transTable);
                board.UnmakeMove();

                if (eval < bestScore)
                {
                    bestScore = eval;
                    bestMove = move;
                }

                beta = Math.Min(beta, eval);
                if (beta <= alpha)
                    break;
            }
        }

        var entry = new TranspositionEntry
        {
            Depth = depth,
            Score = bestScore,
            Flag = bestScore <= originalAlpha ? Flag.Alpha : bestScore >= beta ? Flag.Beta : Flag.Exact,
            BestMove = bestMove
        };
        transTable.Store(hash, entry);

        return bestScore;
    }


    private static int MoveHeuristic(Board board, Move move)
    {
        int attacker = board.Squares![move.StartSquare];
        int victim = board.Squares[move.TargetSquare];

        int mvvLva = 0;
        if (victim != Piece.None)
        {
            mvvLva = 10 * Evaluator.GetPieceValue(victim.Type()) - Evaluator.GetPieceValue(attacker.Type());
        }
        return mvvLva;
    }
}
