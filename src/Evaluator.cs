public static class Evaluator
{
    private static readonly int[] PawnTable = {
        0,  0,  0,  0,  0,  0,  0,  0,
        50, 50, 50, 50, 50, 50, 50, 50,
        10, 10, 20, 30, 30, 20, 10, 10,
        5,  5, 10, 25, 25, 10,  5,  5,
        0,  0,  0, 20, 20,  0,  0,  0,
        5, -5,-10,  0,  0,-10, -5,  5,
        5, 10, 10,-20,-20, 10, 10,  5,
        0,  0,  0,  0,  0,  0,  0,  0
    };

    private static readonly int[] KnightTable = {
        -50,-40,-30,-30,-30,-30,-40,-50,
        -40,-20,  0,  5,  5,  0,-20,-40,
        -30,  5, 10, 15, 15, 10,  5,-30,
        -30,  0, 15, 20, 20, 15,  0,-30,
        -30,  5, 15, 20, 20, 15,  5,-30,
        -30,  0, 10, 15, 15, 10,  0,-30,
        -40,-20,  0,  0,  0,  0,-20,-40,
        -50,-40,-30,-30,-30,-30,-40,-50
    };

    private static readonly int[] BishopTable = {
        -20,-10,-10,-10,-10,-10,-10,-20,
        -10,  5,  0,  0,  0,  0,  5,-10,
        -10, 10, 10, 10, 10, 10, 10,-10,
        -10,  0, 10, 10, 10, 10,  0,-10,
        -10,  5,  5, 10, 10,  5,  5,-10,
        -10,  0,  5, 10, 10,  5,  0,-10,
        -10,  0,  0,  0,  0,  0,  0,-10,
        -20,-10,-10,-10,-10,-10,-10,-20
    };

    private static readonly int[] RookTable = {
        0,   0,   0,   0,   0,   0,   0,   0,
        5,  10,  10,  10,  10,  10,  10,   5,
        -5,   0,   0,   0,   0,   0,   0,  -5,
        -5,   0,   0,   0,   0,   0,   0,  -5,
        -5,   0,   0,   0,   0,   0,   0,  -5,
        -5,   0,   0,   0,   0,   0,   0,  -5,
        -5,   0,   0,   0,   0,   0,   0,  -5,
        0,   0,   0,   5,   5,   0,   0,   0
    };

    private static readonly int[] QueenTable = {
        -20,-10,-10, -5, -5,-10,-10,-20,
        -10,  0,  0,  0,  0,  0,  0,-10,
        -10,  0,  5,  5,  5,  5,  0,-10,
        -5,  0,  5,  5,  5,  5,  0, -5,
        0,  0,  5,  5,  5,  5,  0, -5,
        -10,  5,  5,  5,  5,  5,  0,-10,
        -10,  0,  5,  0,  0,  0,  0,-10,
        -20,-10,-10, -5, -5,-10,-10,-20
    };

    private static readonly int[] KingMiddleGameTable = {
        -30,-40,-40,-50,-50,-40,-40,-30,
        -30,-40,-40,-50,-50,-40,-40,-30,
        -30,-40,-40,-50,-50,-40,-40,-30,
        -30,-40,-40,-50,-50,-40,-40,-30,
        -20,-30,-30,-40,-40,-30,-30,-20,
        -10,-20,-20,-20,-20,-20,-20,-10,
        20, 20,  0,  0,  0,  0, 20, 20,
        20, 30, 10,  0,  0, 10, 30, 20
    };

    private static readonly int[] KingEndgameTable = {
        -50,-40,-30,-20,-20,-30,-40,-50,
        -30,-20,-10,  0,  0,-10,-20,-30,
        -30,-10, 20, 30, 30, 20,-10,-30,
        -30,-10, 30, 40, 40, 30,-10,-30,
        -30,-10, 30, 40, 40, 30,-10,-30,
        -30,-10, 20, 30, 30, 20,-10,-30,
        -30,-30,  0,  0,  0,  0,-30,-30,
        -50,-30,-30,-30,-30,-30,-30,-50
    };

    private static Dictionary<int, int> PieceValues = new()
    {
        { Piece.Pawn, 100 },
        { Piece.Knight, 320 },
        { Piece.Bishop, 330 },
        { Piece.Rook, 500 },
        { Piece.Queen, 900 },
        { Piece.King, 20000 }
    };

    public static int Evaluate(Board board)
    {
        int score = 0;

        bool isEndgame = board.WhitePieces.Count() <= 4 && board.BlackPieces.Count() <= 4;

        foreach (int index in board.WhitePieces)
        {
            int piece = board.Squares![index];
            int type = piece.Type();

            if (!PieceValues.ContainsKey(type))
                throw new Exception($"Unknown piece type: {type}");

            int baseValue = PieceValues[type];
            int positionalBonus = type switch
            {
                Piece.Pawn   => PawnTable[index],
                Piece.Knight => KnightTable[index],
                Piece.Bishop => BishopTable[index],
                Piece.Rook   => RookTable[index],
                Piece.Queen  => QueenTable[index],
                Piece.King   => isEndgame ? KingEndgameTable[index] : KingMiddleGameTable[index],
                _ => 0
            };

            score += baseValue + positionalBonus;
        }

        foreach (int index in board.BlackPieces)
        {
            int piece = board.Squares![index];
            int type = piece.Type();

            if (!PieceValues.ContainsKey(type))
                throw new Exception($"Unknown piece type: {type}");

            int baseValue = PieceValues[type];
            int mirroredIndex = MirrorIndex(index);
            int positionalBonus = type switch
            {
                Piece.Pawn   => PawnTable[mirroredIndex],
                Piece.Knight => KnightTable[mirroredIndex],
                Piece.Bishop => BishopTable[mirroredIndex],
                Piece.Rook   => RookTable[mirroredIndex],
                Piece.Queen  => QueenTable[mirroredIndex],
                Piece.King   => isEndgame ? KingEndgameTable[mirroredIndex] : KingMiddleGameTable[mirroredIndex],
                _ => 0
            };

            score -= baseValue + positionalBonus;
        }

        return score;
    }

    public static int GetPieceValue(int type)
    {
        return PieceValues.TryGetValue(type, out int value) ? value : 0;
    }

    private static int MirrorIndex(int i)
    {
        int rank = i / 8;
        int file = i % 8;
        int mirroredRank = 7 - rank;
        return mirroredRank * 8 + file;
    }

}