using System;
using System.Collections.Generic;
using System.Linq;

public enum GameMode
{
    PlayerVsPlayer,
    PlayerVsEngine,
    EngineVsPlayer,
    EngineVsEngine
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Mini-Chess Engine");

        var board = new Board();

        while (true)
        {
            string? input = Console.ReadLine()?.Trim().ToLower();
            string[]? parts = input?.Split(' ');
            string? cmd = parts![0];
            switch (cmd)
            {
                case "quit":
                    Environment.Exit(0);
                    break;
                case "eval":
                    var eval = Evaluator.Evaluate(board);
                    Utils.DrawBoard(board);
                    Console.WriteLine($"Evaluation: {eval}");
                    continue;
                case "fen":
                    Console.WriteLine(board.ToFen());
                    continue;
                case "draw":
                case "d":
                    Utils.DrawBoard(board);
                    Console.WriteLine("Fen: " + board.ToFen());
                    continue;
                case "go":
                    int depth = 5;
                    if (parts.Length == 2)
                        int.TryParse(parts[1], out depth);

                    var bestMove = Engine.ChooseMove(board, depth);
                    Console.WriteLine($"Best move: {Utils.SquareToString(bestMove.StartSquare)}{Utils.SquareToString(bestMove.TargetSquare)}");
                    continue;
                case "move":
                    if (parts.Length != 2 || !Utils.IsValidMoveInput(parts[1]))
                    {
                        Console.WriteLine("Invalid move format. Use 'move e2e4' or similar.");
                        continue;
                    }

                    int startSquare = Utils.ConvertToSquareIndex(parts[1][0], parts[1][1]);
                    int targetSquare = Utils.ConvertToSquareIndex(parts[1][2], parts[1][3]);

                    if (startSquare < 0 || startSquare >= 64 || targetSquare < 0 || targetSquare >= 64)
                    {
                        Console.WriteLine("Invalid square index.");
                        continue;
                    }

                    var move = new Move(startSquare, targetSquare);
                    var legalMoves = Generator.GenerateMoves(board);
                    if (!legalMoves.Contains(move))
                    {
                        Console.WriteLine("Illegal move.");
                        continue;
                    }
                    board.MakeMove(move);
                    continue;
                default:
                    Console.WriteLine("Unknown command.");
                    continue;
            }
        }
    }
}
