public class Board
{
    public int[]? Squares { get; private set; }
    public const string InitialPositionFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public bool IsWhiteToMove { get; private set; }
    private Stack<Move> m_history;

    public IEnumerable<int> WhitePieces => GetPiecesByColor(Piece.White);
    public IEnumerable<int> BlackPieces => GetPiecesByColor(Piece.Black);

    public Board()
    {
        Squares = new int[64];
        m_history = new Stack<Move>();
        IsWhiteToMove = true; // Should be set based on the FEN string later
        LoadPositionFromFen(InitialPositionFEN);
    }

    public Move LastMove => m_history.Count > 0 ? m_history.Peek() : new Move(0,0);

    public void MakeMove(Move move)
    {
        int movedPiece = Squares![move.StartSquare];
        int capturedPiece = move.EnPassant
            ? Squares[move.TargetSquare - (IsWhiteToMove ? 8 : -8)]
            : Squares[move.TargetSquare];

        // En Passant handling
        if (move.EnPassant)
        {
            int enPassantCaptureSquare = move.TargetSquare - (IsWhiteToMove ? 8 : -8);
            Squares[enPassantCaptureSquare] = Piece.None; // Remove the captured pawn
        }

        // Update board state
        Squares[move.TargetSquare] = movedPiece;
        Squares[move.StartSquare] = Piece.None;

        // Push move to history
        var fullMove = new Move(
            move.StartSquare,
            move.TargetSquare,
            movedPiece,
            capturedPiece,
            move.PromotionPiece,
            IsWhiteToMove,
            move.DoublePawnPush,
            move.EnPassant
        );

        m_history.Push(fullMove);

         IsWhiteToMove = !IsWhiteToMove;
    }

    public void UnmakeMove()
    {
        if (m_history.Count == 0)
            throw new InvalidOperationException("No moves to unmake.");

        var lastState = m_history.Pop();

        Squares![lastState.StartSquare] = lastState.MovedPiece;
        Squares[lastState.TargetSquare] = lastState.CapturedPiece;
        IsWhiteToMove = lastState.WasWhiteToMove;
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

    private IEnumerable<int> GetPiecesByColor(int color)
    {
        for (int i = 0; i < 64; i++)
        {
            int piece = Squares![i];
            if (piece != Piece.None && piece.Color() == color)
                yield return i;
        }
    }
}
