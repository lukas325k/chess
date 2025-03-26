using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using System.IO;
using UnityEngine.Rendering.RenderGraphModule;

public class Ai
{
    List<(int,int, string,(int,int))> scoresPos = new List<(int, int, string,(int,int))>();
    Stopwatch stopwatch = new Stopwatch();
    private Main main;
    List<(int,int)> validMoves;
    (List<(int, int)>,List<(int, int)>) toDoMoves;
    (int,int) clickedCoords = (0,0);

    Dictionary<string,int> pieceCounts = new Dictionary<string, int>
    {
        {"wPawn", 0},
        {"bPawn", 0},
        {"wKnight", 0},
        {"bKnight", 0},
        {"wBishop", 0},
        {"bBishop", 0},
        {"wRook", 0},
        {"bRook", 0},
        {"wQueen", 0},
        {"bQueen", 0},
        {"wKing", 0},
        {"bKing", 0},
    };

    Dictionary<string,int> piecePositionsValue = new Dictionary<string, int>
    {
        {"wPawn", 0},
        {"bPawn", 0},
        {"wKnight", 0},
        {"bKnight", 0},
        {"wBishop", 0},
        {"bBishop", 0},
        {"wRook", 0},
        {"bRook", 0},
        {"wQueen", 0},
        {"bQueen", 0},
        {"wKing", 0},
        {"bKing", 0},
    };

    Dictionary<string, int> piecesWeight = new Dictionary<string, int>
    {
        {"Pawn", 100},
        {"Knight", 320},
        {"Bishop", 330},
        {"Rook", 500},
        {"Queen", 900},
        {"King", 20000},
  
    };

    Dictionary<(string,(int,int), int), ulong> zobristDic ;

    int[,] pawnSquareTable =    {{0,  0,  0,  0,  0,  0,  0,  0},
                                {50, 50, 50, 50, 50, 50, 50, 50},
                                {10, 10, 20, 30, 30, 20, 10, 10},
                                { 5,  5, 10, 25, 25, 10,  5,  5},
                                { 0,  0,  0, 20, 20,  0,  0,  0},
                                { 5, -5,-10,  0,  0,-10, -5,  5},
                                { 5, 10, 10,-20,-20, 10, 10,  5},
                                { 0,  0,  0,  0,  0,  0,  0,  0}};
    int[,] knightSquareTable = {{-50,-40,-30,-30,-30,-30,-40,-50},
                                {-40,-20,  0,  0,  0,  0,-20,-40},
                                {-30,  0, 10, 15, 15, 10,  0,-30},
                                {-30,  5, 15, 20, 20, 15,  5,-30},
                                {-30,  0, 15, 20, 20, 15,  0,-30},
                                {-30,  5, 10, 15, 15, 10,  5,-30},
                                {-40,-20,  0,  5,  5,  0,-20,-40},
                                {-50,-40,-30,-30,-30,-30,-40,-50}};
    HashSet<(int,int, int,string)> killerMoves = new HashSet<(int, int, int, string)>();

    Dictionary<ulong, List<int>> transpositionsDic = new Dictionary<ulong, List<int>>();

    ulong boardHash;
    Zobrist zobrist = new Zobrist();
    private StreamWriter writer;
    public Ai(Main caller)
    {
        
        zobristDic = zobrist.ZobristHashing();
        writer = new StreamWriter("output.txt", true); // Open file in append mode
        
        int score;
        int indexOfBestScore;
        
        List<int> scores = new List<int>();
        this.main = caller;

        SetCounts(main.board);

        UnityEngine.Debug.Log(boardHash);
        int alpha = -1000000;
        int beta = 1000000;

        stopwatch.Start();

        const int startDepth = 1;
        const int maxDepth = 3;
        const int step = 1;
        const float maxTime = 2;
        int depth = 0;

        for (int i = 2; i <= maxDepth; i += step)
        {
            if (stopwatch.Elapsed.TotalSeconds < maxTime)
            {
                depth = i;
                scoresPos.Clear();
                transpositionsDic.Clear();
                alpha = -1000000;
                beta = 1000000;
                scores = minimax('b', startDepth, i, (string[,])main.board.Clone(), alpha, beta, new List<(int, int)>(), clickedCoords, boardHash);
                score = scores.Max();
                indexOfBestScore = scores.IndexOf(score);
                clickedCoords = (scoresPos[indexOfBestScore].Item1, scoresPos[indexOfBestScore].Item2);
            }
            else
            {
                break;
            }
        }

        stopwatch.Stop();
        score = scores.Max();
        indexOfBestScore = scores.IndexOf(score);
        clickedCoords = (scoresPos[indexOfBestScore].Item1, scoresPos[indexOfBestScore].Item2);
        (int,int) unclickedCoords = scoresPos[indexOfBestScore].Item4;
    
        for (int i = 0;  i < scoresPos.Count(); i ++)
        {
            UnityEngine.Debug.Log(scoresPos[i]);
            UnityEngine.Debug.Log(scores[i]);
        }
        UnityEngine.Debug.Log(scoresPos[indexOfBestScore]);
        UnityEngine.Debug.Log(scores[indexOfBestScore]);

        switch (scoresPos[indexOfBestScore].Item3)
        {
            case "bPawn":
                main.board[clickedCoords.Item1, clickedCoords.Item2] = null;
                main.board[unclickedCoords.Item1, unclickedCoords.Item2] = "bPawn";
                break;

            case "bKnight":
                main.board[clickedCoords.Item1, clickedCoords.Item2] = null;
                main.board[unclickedCoords.Item1, unclickedCoords.Item2] = "bKnight";
                break;

            case "bRook":
                main.board[clickedCoords.Item1, clickedCoords.Item2] = null;
                main.board[unclickedCoords.Item1, unclickedCoords.Item2] = "bRook";
                break;

            case "bBishop":
                main.board[clickedCoords.Item1, clickedCoords.Item2] = null;
                main.board[unclickedCoords.Item1, unclickedCoords.Item2] = "bBishop";
                break;

            case "bQueen":
                main.board[clickedCoords.Item1, clickedCoords.Item2] = null;
                main.board[unclickedCoords.Item1, unclickedCoords.Item2] = "bQueen";
                break;

            case "bKing":
                main.board[clickedCoords.Item1, clickedCoords.Item2] = null;
                main.board[unclickedCoords.Item1, unclickedCoords.Item2] = "bKing";
                break;

        }

        UnityEngine.Debug.Log($"Elapsed Time: {stopwatch.Elapsed.TotalSeconds}");
        UnityEngine.Debug.Log($"Elapsed Time avarege: {main.evals / stopwatch.Elapsed.TotalSeconds} ");
        UnityEngine.Debug.Log(depth);
    //ahoooooooooojjjjj

    }

    List<int> minimax(char parentColour, int depth, int maxDepth, string[,] board, int alpha, int beta, List<(int, int)> piecesCoords, (int,int) clickedCoords, ulong boardHash)
    {
        List<int> score = new List<int>();

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (!string.IsNullOrEmpty(board[y,x]))
                {
                    if (parentColour == board[y,x][0])
                    {
                        if ((y,x) == clickedCoords && depth == 1)
                        {
                            piecesCoords.Insert(0,(y,x));
                        }
                        else
                        {
                            piecesCoords.Add((y,x));
                        }
                    }
                }
            }
        }

        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                writer.Write(board[i, j] + " "); // Write element to file
            }
            writer.WriteLine(); // Move to next line
        }
        writer.WriteLine(boardHash);
        writer.WriteLine(depth);
        
        

        if (transpositionsDic.ContainsKey(boardHash) && depth > 1)
        {
            return transpositionsDic[boardHash];
        }
        
        foreach ((int y, int x) in piecesCoords)
        {
            string pieceName = board[y,x];
            if (!string.IsNullOrEmpty(pieceName) && pieceName[0] == parentColour)
            {

                switch (pieceName[1..])
                {
                    case "Pawn":
                    {
                        toDoMoves = main.pawnClass.getValidMoves((y,x), parentColour == 'b' ? "black" : "white", board);
                        break;
                    }
                    case "Knight":
                    {
                        toDoMoves = main.knightClass.getValidMoves((y,x), parentColour == 'b' ? "black" : "white", board);
                        break;
                    }
                    case "Rook":
                    {
                        toDoMoves = main.rookClass.getValidMoves((y,x), parentColour == 'b' ? "black" : "white", board);
                        break;
                    }
                    case "Bishop":
                    {
                        toDoMoves = main.bishopClass.getValidMoves((y,x), parentColour == 'b' ? "black" : "white", board);
                        break;
                    }
                    case "Queen":
                    {
                        toDoMoves = main.queenClass.getValidMoves((y,x), parentColour == 'b' ? "black" : "white", board);
                        break;
                    }
                    case "King":
                    {
                        toDoMoves = main.kingClass.getValidMoves((y,x), parentColour == 'b' ? "black" : "white", board);
                        break;
                    }
                    
                }
                
                toDoMoves.Item2.AddRange(toDoMoves.Item1);
                // ahoooooooooooooooj
                // if (parentColour == 'b')
                // {
                //     toDoMoves.Item2 = toDoMoves.Item2.OrderByDescending(move => EvaluateMove(move, (y,x), depth, pieceName, board)).ToList();
                // }

                foreach ((int,int) move in toDoMoves.Item2)
                {
                    string capturedPiecename = board[move.Item1, move.Item2];
                    board[move.Item1, move.Item2] = pieceName;
                    board[y,x] = null;
                    
                    switch(pieceName)
                    {
                        case "bPawn":
                        {
                            piecePositionsValue[pieceName] += pawnSquareTable[7-move.Item1,move.Item2];
                            break;
                        }
                        case "wPawn":
                        {
                            piecePositionsValue[pieceName] -= pawnSquareTable[move.Item1,move.Item2];
                            break;
                        }
                        case "bKnight":
                        {
                            piecePositionsValue[pieceName] += knightSquareTable[7-move.Item1,move.Item2];
                            break;
                        }
                        case "wKnight":
                        {
                            piecePositionsValue[pieceName] -= knightSquareTable[move.Item1,move.Item2];
                            break;
                        }
                        
                    }

                    boardHash ^= zobristDic[(pieceName,(y,x), 1)];
                    boardHash ^= zobristDic[(pieceName,move, depth)];


                    if (!string.IsNullOrEmpty(capturedPiecename))
                    {
                        boardHash ^= zobristDic[(capturedPiecename,move, 1)];
                        pieceCounts[capturedPiecename] --;
                    }

                    
                    if (depth == 1)
                    {
                        score.Add(parentColour == 'b' ? minimax('w', depth+1, maxDepth, board, alpha, beta, new List<(int, int)>(), clickedCoords, boardHash).Min() : minimax('b', depth+1, maxDepth, board, alpha, beta, new List<(int, int)>(), clickedCoords, boardHash).Max());
                        scoresPos.Add((y,x,pieceName,(move.Item1, move.Item2)));
                    }
                    else if (maxDepth != depth)
                    {
                        score.Add(parentColour == 'b' ? minimax('w', depth+1, maxDepth, board, alpha, beta, new List<(int, int)>(), clickedCoords, boardHash).Min() : minimax('b', depth+1, maxDepth, board, alpha, beta, new List<(int, int)>(), clickedCoords, boardHash).Max());
                    }
                    else
                    {
                        score.Add(Evaluation());
                    }

                    if (!string.IsNullOrEmpty(capturedPiecename))
                    {
                        boardHash ^= zobristDic[(capturedPiecename,move, 1)];
                        pieceCounts[capturedPiecename] ++;
                    }

                    boardHash ^= zobristDic[(pieceName,move, depth)];
                    boardHash ^= zobristDic[(pieceName,(y,x), 1)];
                    

                    

                    switch(pieceName[1..])
                    {
                        case "Pawn":
                        {
                            if (parentColour == 'b')
                            {
                                piecePositionsValue[pieceName] -= pawnSquareTable[7-move.Item1,move.Item2];
                            }
                            else
                            {
                                piecePositionsValue[pieceName] += pawnSquareTable[move.Item1,move.Item2];
                            }
                            break;
                        }
                        case "Knight":
                        {
                            if (parentColour == 'b')
                            {
                                piecePositionsValue[pieceName] -= knightSquareTable[7-move.Item1,move.Item2];
                            }
                            else
                            {
                                piecePositionsValue[pieceName] += knightSquareTable[move.Item1,move.Item2];
                            }
                            break;
                        }
                        
                    }
                    

                    board[move.Item1, move.Item2] = capturedPiecename;
                    board[y,x] = pieceName;


                    if (parentColour == 'b')
                    {
                        alpha = Math.Max(alpha, score.Max());
                    }
                    else
                    {
                        beta = Math.Min(beta, score.Min());
                    }

                    if (beta <= alpha)
                    {
                        // killerMoves.Add((move.Item1, move.Item2, depth, pieceName));
                        if (!transpositionsDic.ContainsKey(boardHash))
                        {
                            transpositionsDic.Add(boardHash, score);
                        }
                        
                        return score;
                    }
                    

                }
                
            }
        }
        
        if (!transpositionsDic.ContainsKey(boardHash))
        {
            transpositionsDic.Add(boardHash, score);
        }
        
        
        return score;
        
    }//p[icus jeden ]


    int EvaluateMove((int,int) move, (int,int) movedFrom, int depth, string pieceName, string[,] board)
    {
        if (killerMoves.Contains((move.Item1, move.Item2, depth, pieceName)))
        {
            return 10000/depth;
        }
        else if (!string.IsNullOrEmpty(board[move.Item1, move.Item2]))
        {
            return (piecesWeight[board[move.Item1, move.Item2][1..]] - piecesWeight[pieceName[1..]]);
        }

        return 0;
    }

    int Evaluation()
    {
        main.evals ++;
        int scoreNow = 0;

        foreach (KeyValuePair<string, int> kvp in pieceCounts)
        {
            string pieceName = kvp.Key;
            int count = kvp.Value;
            int pieceValue = piecesWeight[pieceName[1..]];
            scoreNow += ((pieceName[0] == 'b') ? pieceValue : -pieceValue) * count + piecePositionsValue[pieceName];  
            

        }
        
        return scoreNow;//lalalal blabla blabla hihi haha enter
    }

    void SetCounts(string[,] board)
    {
        int depth = 1; 
        for (int y = 0; y<8; y++)
        {
            for (int x = 0; x<8; x++)
            {
                string pieceName = board[y,x];
                if (!string.IsNullOrEmpty(pieceName))
                {
                    pieceCounts[pieceName]++;

                    boardHash ^= zobristDic[(pieceName, (y,x), depth)];
                }
                
            }
        }

    }
}
