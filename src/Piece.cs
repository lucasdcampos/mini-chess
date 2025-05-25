public static class Piece
{
    public const int None = 0;
    public const int Pawn = 1;
    public const int Knight = 2;
    public const int Bishop = 3;
    public const int Rook = 4;
    public const int Queen = 5;
    public const int King = 6;

    public const int White = 8;
    public const int Black = 16;

    public static int Type(this int piece) => piece & 0b111; // bits 0–2
    public static int Color(this int piece) => piece & (White | Black);
}