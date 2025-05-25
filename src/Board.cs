public class Board
{
    public int[]? Squares { get; private set; }
    public const string InitialPositionFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public Board()
    {
        Squares = new int[64];
        LoadPositionFromFen(InitialPositionFEN);
    }

    public void MakeMove(Move move)
    {
        Squares![move.TargetSquare] = Squares[move.StartSquare];
        Squares[move.StartSquare] = Piece.None;
    }

    public void LoadPositionFromFen(string fen)
    {
        string piecePlacement = fen.Split(' ')[0];

        int x = 0;
        int y = 7;

        foreach (char c in piecePlacement)
        {
            if (c == '/')
            {
                x = 0;
                y--;
            }
            else if (char.IsDigit(c))
            {
                int emptySquares = c - '0';
                for (int i = 0; i < emptySquares; i++)
                {
                    Squares![y * 8 + x] = Piece.None;
                    x++;
                }
            }
            else
            {
                int piece = Piece.None;
                switch (c)
                {
                    case 'p': piece = Piece.Pawn | Piece.Black; break;
                    case 'r': piece = Piece.Rook | Piece.Black; break;
                    case 'n': piece = Piece.Knight | Piece.Black; break;
                    case 'b': piece = Piece.Bishop | Piece.Black; break;
                    case 'q': piece = Piece.Queen | Piece.Black; break;
                    case 'k': piece = Piece.King | Piece.Black; break;
                    case 'P': piece = Piece.Pawn | Piece.White; break;
                    case 'R': piece = Piece.Rook | Piece.White; break;
                    case 'N': piece = Piece.Knight | Piece.White; break;
                    case 'B': piece = Piece.Bishop | Piece.White; break;
                    case 'Q': piece = Piece.Queen | Piece.White; break;
                    case 'K': piece = Piece.King | Piece.White; break;
                    default:
                        throw new ArgumentException($"Invalid character in FEN: '{c}'");
                }

                Squares![y * 8 + x] = piece;
                x++;
            }
        }
    }

}