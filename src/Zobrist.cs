public class TranspositionEntry
{
    public int Depth { get; set; }
    public int Score { get; set; }
    public int Flag { get; set; }
    public Move BestMove { get; set; }
}

public static class Flag
{
    public const int Exact = 0;
    public const int Alpha = 1;
    public const int Beta = 2;
}


public static class Zobrist
{
    private static ulong[,] ZobristTable = new ulong[64, 12];
    private static ulong SideToMoveKey;

    static Zobrist()
    {
        var random = new Random();
        for (int square = 0; square < 64; square++)
            for (int piece = 0; piece < 12; piece++)
                ZobristTable[square, piece] = RandomUlong(random);

        SideToMoveKey = RandomUlong(random);
    }

    private static ulong RandomUlong(Random random)
    {
        byte[] buffer = new byte[8];
        random.NextBytes(buffer);
        return BitConverter.ToUInt64(buffer, 0);
    }

    public static ulong ComputeHash(Board board)
    {
        ulong hash = 0;

        for (int square = 0; square < 64; square++)
        {
            int piece = board.Squares![square];
            if (piece != Piece.None)
            {
                int type = piece.Type(); // 1 to 6
                if (type < 1 || type > 6)
                    throw new ArgumentOutOfRangeException(nameof(type), $"Invalid piece type: {type} at square {square}.");

                int colorOffset = piece.Color() == Piece.White ? 0 : 6;
                int pieceIndex = (type - 1) + colorOffset; // 0–11

                if (pieceIndex < 0 || pieceIndex >= 12)
                    throw new ArgumentOutOfRangeException(nameof(pieceIndex), $"Piece index out of range: {pieceIndex} at square {square}.");

                hash ^= ZobristTable[square, pieceIndex];
            }
        }

        if (!board.IsWhiteToMove)
            hash ^= SideToMoveKey;

        return hash;
    }
}

public class TranspositionTable
{
    private readonly Dictionary<ulong, TranspositionEntry> _table = new();
    private readonly int _maxSize;

    public TranspositionTable(int maxSize = 1000000)
    {
        _maxSize = maxSize;
    }

    public void Store(ulong hash, TranspositionEntry entry)
    {
        if (_table.Count >= _maxSize)
            _table.Remove(_table.Keys.First()); // FIFO

        _table[hash] = entry;
    }

    public TranspositionEntry? Lookup(ulong hash)
    {
        return _table.TryGetValue(hash, out var entry) ? entry : null;
    }
}
