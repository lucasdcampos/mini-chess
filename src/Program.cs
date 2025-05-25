class Program
{
    static void Main(string[] args)
    {
        var board = new Board();

        while (true)
        {
            Utils.DrawBoard(board);

            if (board.IsWhiteToMove)
            {
                Console.Write("> ");
                string? input = Console.ReadLine()?.Trim().ToLower();

                if (input == "exit" || input == "quit" || input == "stop")
                    break;

                if (string.IsNullOrEmpty(input))
                    continue;

                if (!Utils.IsValidMoveInput(input))
                {
                    Console.WriteLine("Invalid move format. Use format like 'e2e4'.");
                    continue;
                }

                int startSquare = Utils.ConvertToSquareIndex(input[0], input[1]);
                int targetSquare = Utils.ConvertToSquareIndex(input[2], input[3]);

                var legalMoves = Generator.GenerateMoves(board);
                var playerMove = new Move(startSquare, targetSquare);

                if (!legalMoves.Contains(playerMove))
                {
                    Console.WriteLine("Illegal move. Try again.");
                    continue;
                }

                board.MakeMove(playerMove);
            }
            else
            {
                // Computer's turn
                var computerMove = Engine.ChooseMove(board);

                if (computerMove.Equals(new Move(0, 0)))
                {
                    Console.WriteLine("No legal moves for computer. Game over.");
                    break;
                }

                Console.WriteLine($"Computer plays: {Utils.SquareToString(computerMove.StartSquare)}{Utils.SquareToString(computerMove.TargetSquare)}");
                board.MakeMove(computerMove);
            }
        }
    }
}