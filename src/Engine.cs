public static class Engine
{
    public static Move ChooseMove(Board board)
    {
        var legalMoves = Generator.GenerateMoves(board);
        if (legalMoves.Count == 0)
            return new Move(0,0); // No legal moves available

        Move bestMove = legalMoves[0];
        int bestScore = board.IsWhiteToMove ? int.MinValue : int.MaxValue;

        foreach (var move in legalMoves)
        {
            board.MakeMove(move);

            int score = Evaluator.Evaluate(board);

            board.UnmakeMove(move);

            if (board.IsWhiteToMove)
            {
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }
            else
            {
                if (score < bestScore)
                {
                    bestScore = score;
                    bestMove = move;
                }
            }
        }
        return bestMove;
    }
}