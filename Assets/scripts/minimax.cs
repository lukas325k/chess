using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using System.IO;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.UIElements;

public class Ai
{
    List<(int,int, string,(int,int))> scoresPos = new List<(int, int, string,(int,int))>();
    List<(int,int)> startCoords;
    List<int> scores;
    Stopwatch stopwatch = new Stopwatch();
    private Main main;
    List<(int,int)> validMoves;
    List<(int, int)> toDoMoves;
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

    ulong[,,] zobristTable ;

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
    int[,] bishopSquareTable = {{-20,-10,-10,-10,-10,-10,-10,-20},
                                {-10,  0,  0,  0,  0,  0,  0,-10},
                                {-10,  0,  5, 10, 10,  5,  0,-10},
                                {-10,  5,  5, 10, 10,  5,  5,-10},
                                {-10,  0, 10, 10, 10, 10,  0,-10},
                                {-10, 10, 10, 10, 10, 10, 10,-10},
                                {-10,  5,  0,  0,  0,  0,  5,-10},
                                {-20,-10,-10,-10,-10,-10,-10,-20}};
    int[,] rookSquareTable = {{  0,  0,  0,  0,  0,  0,  0,  0},
                            {  5, 10, 10, 10, 10, 10, 10,  5},
                            { -5,  0,  0,  0,  0,  0,  0, -5},
                            { -5,  0,  0,  0,  0,  0,  0, -5},
                            { -5,  0,  0,  0,  0,  0,  0, -5},
                            { -5,  0,  0,  0,  0,  0,  0, -5},
                            { -5,  0,  0,  0,  0,  0,  0, -5},
                            {  0,  0,  0,  5,  5,  0,  0,  0}};
    int[,] queenSquareTable = {{-20,-10,-10, -5, -5,-10,-10,-20},
                                {-10,  0,  0,  0,  0,  0,  0,-10},
                                {-10,  0,  5,  5,  5,  5,  0,-10},
                                {-5,  0,  5,  5,  5,  5,  0, -5},
                                {0,  0,  5,  5,  5,  5,  0, -5},
                                {-10,  5,  5,  5,  5,  5,  0,-10},
                                {-10,  0,  5,  0,  0,  0,  0,-10},
                                {-20,-10,-10, -5, -5,-10,-10,-20}};
    int[,] kingSquareTable = {{-30,-40,-40,-50,-50,-40,-40,-30},
                                {-30,-40,-40,-50,-50,-40,-40,-30},
                                {-30,-40,-40,-50,-50,-40,-40,-30},
                                {-30,-40,-40,-50,-50,-40,-40,-30},
                                {-20,-30,-30,-40,-40,-30,-30,-20},
                                {-10,-20,-20,-20,-20,-20,-20,-10},
                                { 20, 20,  0,  0,  0,  0, 20, 20},
                                { 20, 30, 10,  0,  0, 10, 30, 20}};
   
    Dictionary<string, int> pieceToIndex = new Dictionary<string, int>
    {
        {"wPawn", 0}, {"bPawn", 1}, {"wKnight", 2}, {"bKnight", 3},
        {"wBishop", 4}, {"bBishop", 5}, {"wRook", 6}, {"bRook", 7},
        {"wQueen", 8}, {"bQueen", 9}, {"wKing", 10}, {"bKing", 11}
    };
   
    Dictionary<ulong, (List<int>, int)> transpositionsDic = new Dictionary<ulong, (List<int>, int)>();
    
    Dictionary<string, int[,]> pieceSquareTables = new Dictionary<string, int[,]>();
    ulong boardHash;
    Zobrist zobrist = new Zobrist();

    HashSet<ulong> killerMove = new HashSet<ulong>();

    public Ai(Main caller)
    {
        
        zobristTable = zobrist.ZobristHashing();
        
        int score;
        int indexOfBestScore;
        
        List<int> scores = new List<int>();
        List<(int,int)> startCoords = new List<(int, int)>();
        this.main = caller;
        pieceSquareTables = new Dictionary<string, int[,]>
        {
            { "Pawn", pawnSquareTable },
            { "Knight", knightSquareTable },
            { "Bishop", bishopSquareTable },
            { "Rook", rookSquareTable },
            { "Queen", queenSquareTable },
            { "King", kingSquareTable }
        };

        SetCounts(main.board);

        UnityEngine.Debug.Log(boardHash);
        int alpha = -1000000;
        int beta = 1000000;

        stopwatch.Start();

        const int startDepth = 1;
        const int maxDepth = 20;
        const int step = 1;
        const float maxTime = 1;
        int depth = 0;

        // iterative deepining logic
        for (int i = 2; i <= maxDepth; i += step)
        {
            if (stopwatch.Elapsed.TotalSeconds < maxTime)
            {
                depth = i;
                scoresPos.Clear();
                transpositionsDic.Clear();
                alpha = -1000000;
                beta = 1000000;
                scores = minimax('b', i, startDepth, i, (string[,])main.board.Clone(), alpha, beta, startCoords, clickedCoords, boardHash, scores);
                score = scores.Max();
                indexOfBestScore = scores.IndexOf(score);
                clickedCoords = (scoresPos[indexOfBestScore].Item1, scoresPos[indexOfBestScore].Item2);
                startCoords.Clear();
                UnityEngine.Debug.Log(scoresPos[indexOfBestScore]);
                UnityEngine.Debug.Log(scores[indexOfBestScore]);
                foreach (var (y,x, _, _) in scoresPos)
                {
                    startCoords.Add((y,x)); 
                }
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
    
        // all the console writing
        // for (int i = 0;  i < scoresPos.Count(); i ++)
        // {
        //     UnityEngine.Debug.Log(scoresPos[i]);
        //     UnityEngine.Debug.Log(scores[i]);
        // }

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

    List<int> minimax(char parentColour, int iterdepth, int depth, int maxDepth, string[,] board, int alpha, int beta, List<(int, int)> startCoords, (int,int) clickedCoords, ulong boardHash, List<int> scores)
    {
        // piece finding
        List<int> score = new List<int>();
        List<(int, int)> piecesCoords = new List<(int, int)>();
        if(iterdepth == 2 || depth > 1)
        {
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if (!string.IsNullOrEmpty(board[y,x]))
                    {
                        if (parentColour == board[y,x][0])
                        {
                            piecesCoords.Add((y,x));
                        }
                    }
                }
            }
        }
        else
        {
            piecesCoords = startCoords.Zip(scores, (move, score) => (move, score))
                                             .OrderByDescending(pair => pair.score)
                                             .Select(pair => pair.move)
                                             .ToList();
            piecesCoords = new List<(int, int)>(new HashSet<(int,int)> (piecesCoords));
        }

        // if the same position occured dont compute it once more
        if (depth > 1 && transpositionsDic.TryGetValue(boardHash, out var dicInfo) && dicInfo.Item2 <= depth)
        {
            return dicInfo.Item1;
        }
         // iterate thru every piece
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
                

                toDoMoves = toDoMoves.OrderByDescending(move => EvaluateMove(move, (y,x), board, boardHash, pieceName)).ToList();
                

                foreach ((int,int) move in toDoMoves)
                {
                    // gettin all the info and making the move
                    string capturedPiecename = board[move.Item1, move.Item2];
                    board[move.Item1, move.Item2] = pieceName;
                    board[y,x] = null;
                    
                    // square tables
                    int multiplier = (pieceName[0] == 'b') ? 1 : -1; 
                    int tableY = (pieceName[0] == 'b') ? (7 - move.Item1) : move.Item1; // flip y coord for black
                    if (pieceSquareTables.TryGetValue(pieceName[1..], out int[,] squareTable))
                    {
                        piecePositionsValue[pieceName] += multiplier * squareTable[tableY, move.Item2];
                    }

                    //board hasshing
                    int pieceIndex = pieceToIndex[pieceName];
                    boardHash ^= zobristTable[pieceIndex, y, x];
                    boardHash ^= zobristTable[pieceIndex, move.Item1, move.Item2];

                    // capturing and updating board hash accordingly
                    if (!string.IsNullOrEmpty(capturedPiecename))
                    {
                        int capturedPieceIndex = pieceToIndex[capturedPiecename];
                        boardHash ^= zobristTable[capturedPieceIndex, move.Item1, move.Item2];
                        pieceCounts[capturedPiecename] --;
                    }

                    // main logic
                    if (depth == 1)
                    {
                        score.Add(parentColour == 'b' ? minimax('w', iterdepth, depth+1, maxDepth, board, alpha, beta, startCoords, clickedCoords, boardHash,scores).Min() 
                                                    : minimax('b', iterdepth,depth+1, maxDepth, board, alpha, beta, startCoords, clickedCoords, boardHash,scores).Max());
                        scoresPos.Add((y,x,pieceName,(move.Item1, move.Item2)));
                    }
                    else if (maxDepth != depth)
                    {
                        score.Add(parentColour == 'b' ? minimax('w', iterdepth, depth+1, maxDepth, board, alpha, beta, startCoords, clickedCoords, boardHash,scores).Min() 
                                                    : minimax('b', iterdepth,depth+1, maxDepth, board, alpha, beta, startCoords, clickedCoords, boardHash,scores).Max());
                    }
                    else
                    {
                        score.Add(Evaluation());
                    }

                    // undiong capturing, moves and boadr hashes
                    if (!string.IsNullOrEmpty(capturedPiecename))
                    {
                        int capturedPieceIndex = pieceToIndex[capturedPiecename];
                        boardHash ^= zobristTable[capturedPieceIndex, move.Item1, move.Item2];
                        pieceCounts[capturedPiecename] ++;
                    }

                    boardHash ^= zobristTable[pieceIndex, move.Item1, move.Item2];
                    boardHash ^= zobristTable[pieceIndex, y, x];
                    
                    board[move.Item1, move.Item2] = capturedPiecename;
                    board[y,x] = pieceName;

                    

                    if (pieceSquareTables.TryGetValue(pieceName[1..], out squareTable))
                    {
                        piecePositionsValue[pieceName] -= multiplier * squareTable[tableY, move.Item2];
                    }
                    


                    // alpha beta pruning
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
                        killerMove.Add(boardHash);
                        transpositionsDic.TryAdd(boardHash, (score, depth));
                        return score;
                    }
                    

                }
                
            }
        }
        
        transpositionsDic.TryAdd(boardHash, (score, depth));
        return score;
        
    }


    int EvaluateMove((int,int) move, (int,int) movedFrom, string[,] board, ulong boardHash, string pieceName)
    {
        int moveScore = 0;
        string toMovePieceName = board[move.Item1, move.Item2];
        if (!string.IsNullOrEmpty(toMovePieceName))
        {
            moveScore += piecesWeight[toMovePieceName[1..]] - piecesWeight[pieceName[1..]];

        }
        if (killerMove.Contains(boardHash))
        {
            moveScore += 100;
        }
        int tableY = (pieceName[0] == 'b') ? (7 - move.Item1) : move.Item1; // flip y coord for black
        if (pieceSquareTables.TryGetValue(pieceName[1..], out int[,] squareTable))
        {
            moveScore += squareTable[tableY, move.Item2];
        }


        return moveScore == 0 ? -1000 : moveScore;
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
        for (int y = 0; y<8; y++)
        {
            for (int x = 0; x<8; x++)
            {
                string pieceName = board[y,x];
                if (!string.IsNullOrEmpty(pieceName))
                {
                    pieceCounts[pieceName]++;

                    int pieceIndex = pieceToIndex[pieceName];
                    boardHash ^= zobristTable[pieceIndex, y, x];
                }
                
            }
        }

    }
}
