public class Board
{
    public int[]? Squares { get; private set; }
    public const string InitialPositionFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
    public bool IsWhiteToMove { get; private set; }
    public bool CanWhiteCastleKingside { get; set; } = true;
    public bool CanWhiteCastleQueenside { get; set; } = true;
    public bool CanBlackCastleKingside { get; set; } = true;
    public bool CanBlackCastleQueenside { get; set; } = true;
    public int EnPassantSquare { get; set; } = -1; // -1 means no en passant target square
    public int HalfMoveClock { get; set; } = 0;
    public int FullMoveNumber { get; set; } = 1;
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
        string activeColor = fen.Split(' ')[1];
        string castlingAvailability = fen.Split(' ')[2];
        string enPassantTarget = fen.Split(' ')[3];
        string halfMoveClock = fen.Split(' ')[4];
        string fullMoveNumber = fen.Split(' ')[5];

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

        IsWhiteToMove = activeColor == "w";
        CanWhiteCastleKingside = castlingAvailability.Contains('K');
        CanWhiteCastleQueenside = castlingAvailability.Contains('Q');
        CanBlackCastleKingside = castlingAvailability.Contains('k');
        CanBlackCastleQueenside = castlingAvailability.Contains('q');
        EnPassantSquare = enPassantTarget == "-" ? -1 : Utils.ConvertToSquareIndex(enPassantTarget[0], enPassantTarget[1]);
        HalfMoveClock = int.Parse(halfMoveClock);
        FullMoveNumber = int.Parse(fullMoveNumber);
    }

    public string ToFen()
    {
        var fen = new System.Text.StringBuilder();

        for (int y = 7; y >= 0; y--)
        {
            int emptyCount = 0;

            for (int x = 0; x < 8; x++)
            {
                int piece = Squares![y * 8 + x];

                if (piece == Piece.None)
                {
                    emptyCount++;
                }
                else
                {
                    if (emptyCount > 0)
                    {
                        fen.Append(emptyCount);
                        emptyCount = 0;
                    }

                    fen.Append(PieceToFenChar(piece));
                }
            }

            if (emptyCount > 0)
                fen.Append(emptyCount);

            if (y > 0)
                fen.Append('/');
        }

        fen.Append(IsWhiteToMove ? " w " : " b ");
        if (CanWhiteCastleKingside) fen.Append('K');
        if (CanWhiteCastleQueenside) fen.Append('Q');
        if (CanBlackCastleKingside) fen.Append('k');
        if (CanBlackCastleQueenside) fen.Append('q');
        if(EnPassantSquare != -1)
            fen.Append(' ').Append(Utils.SquareToString(EnPassantSquare));
        else
            fen.Append(" -");
        fen.Append($" {HalfMoveClock} {FullMoveNumber}");
        
        return fen.ToString();
    }

    private char PieceToFenChar(int piece)
    {
        int type = piece.Type();
        bool isWhite = piece.Color() == Piece.White;

        char c = type switch
        {
            Piece.Pawn => 'p',
            Piece.Rook => 'r',
            Piece.Knight => 'n',
            Piece.Bishop => 'b',
            Piece.Queen => 'q',
            Piece.King => 'k',
            _ => '?'
        };
        return isWhite ? char.ToUpper(c) : c;
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
