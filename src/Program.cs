class Program
{
    static void Main(string[] args)
    {
        var board = new Board();
        DrawBoard(board);
    }

    static void DrawBoard(Board board)
    {
        for (int y = 7; y >= 0; y--)
        {
            Console.Write("  +---+---+---+---+---+---+---+---+\n");
            Console.Write($"{y + 1} ");

            for (int x = 0; x < 8; x++)
            {
                int index = y * 8 + x;
                int piece = board.Squares![index];

                char symbol = GetPieceSymbol(piece);
                Console.Write($"| {symbol} ");
            }

            Console.WriteLine("|");
        }

        Console.Write("  +---+---+---+---+---+---+---+---+\n");
        Console.Write("    a   b   c   d   e   f   g   h\n");
    }


    static char GetPieceSymbol(int piece)
    {
        return piece switch
        {
            Piece.Pawn | Piece.White => 'P',
            Piece.Pawn | Piece.Black => 'p',
            Piece.Rook | Piece.White => 'R',
            Piece.Rook | Piece.Black => 'r',
            Piece.Knight | Piece.White => 'N',
            Piece.Knight | Piece.Black => 'n',
            Piece.Bishop | Piece.White => 'B',
            Piece.Bishop | Piece.Black => 'b',
            Piece.Queen | Piece.White => 'Q',
            Piece.Queen | Piece.Black => 'q',
            Piece.King | Piece.White => 'K',
            Piece.King | Piece.Black => 'k',
            _ => ' '
        };
    }
}