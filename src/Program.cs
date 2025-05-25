class Program
{
    static void Main(string[] args)
    {
        var board = new Board();
        while (true)
        {
            DrawBoard(board);
            Console.Write("> ");
            string? input = Console.ReadLine()?.Trim().ToLower();

            if (input == "exit" || input == "quit")
                break;

            if (string.IsNullOrEmpty(input))
                continue;

            if (!IsValidMoveInput(input))
            {
                Console.WriteLine("Invalid move format. Use format like 'e2e4'.");
                continue;
            }

            int startSquare = ConvertToSquareIndex(input[0], input[1]);
            int targetSquare = ConvertToSquareIndex(input[2], input[3]);

            var legalMoves = Generator.GenerateMoves(board);
            var move = new Move(startSquare, targetSquare);
            if (!legalMoves.Contains(move))
            {
                Console.WriteLine("Illegal move. Try again.");
                continue;
            }
            board.MakeMove(new Move(startSquare, targetSquare));
        }

    }

    static bool IsValidMoveInput(string input)
    {
        if (input.Length != 4)
            return false;

        return
            input[0] is >= 'a' and <= 'h' &&
            input[1] is >= '1' and <= '8' &&
            input[2] is >= 'a' and <= 'h' &&
            input[3] is >= '1' and <= '8';
    }


    static int ConvertToSquareIndex(char file, char rank)
    {
        int x = file - 'a';
        int y = rank - '1';
        return y * 8 + x;
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