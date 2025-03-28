using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class Ai
{
    List<(int,int, byte,(int,int))> scoresPos = new List<(int, int, byte,(int,int))>();
    List<(int,int)> startCoords;
    List<int> scores;
    Stopwatch stopwatch = new Stopwatch();
    private Main main;
    List<(int,int)> validMoves;
    List<(int, int)> toDoMoves;
    (int,int) clickedCoords = (0,0);

    Dictionary<byte,int> pieceCounts = new Dictionary<byte, int>();
    

    Dictionary<byte,int> piecePositionsValue = new Dictionary<byte, int>
    {
        {9, 0},
        {17, 0},
        {10, 0},
        {18, 0},
        {11, 0},
        {19, 0},
        {12, 0},
        {20, 0},
        {13, 0},
        {21, 0},
        {14, 0},
        {22, 0},
    };

    Dictionary<byte, int> piecesWeight = new Dictionary<byte, int>
    {
        {1, 100},
        {2, 320},
        {3, 330},
        {4, 500},
        {5, 900},
        {6, 20000},
  
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
   
    public const byte pawn = 1;
    public const byte knight = 2;
    public const byte bishop = 3;
    public const byte rook = 4;
    public const byte queen = 5;
    public const byte king = 6;
    public const byte white = 8;
    public const byte black = 16;
    const byte nameMask = 0b00000111;
    const byte colorMask = 0b00011000;
   
    Dictionary<ulong, (List<int>, int)> transpositionsDic = new Dictionary<ulong, (List<int>, int)>();
    
    Dictionary<byte, int[,]> pieceSquareTables = new Dictionary<byte, int[,]>();
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
        pieceSquareTables = new Dictionary<byte, int[,]>
        {
            { pawn, pawnSquareTable },
            { knight, knightSquareTable },
            { bishop, bishopSquareTable },
            { rook, rookSquareTable },
            { queen, queenSquareTable },
            { king, kingSquareTable }
        };
        
        for (int i = 0; i < main.piecesName.Count(); i ++)
        {
            pieceCounts.Add((byte)(main.piecesName[i] | (i%2 == 0 ? 0b00010000 : 0b00001000)), 0);
        }



        SetCounts(main.board);

        UnityEngine.Debug.Log(boardHash);
        int alpha = -1000000;
        int beta = 1000000;

        stopwatch.Start();

        const int startDepth = 1;
        const int maxDepth = 10;
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
                scores = minimax(black, i, startDepth, i, (byte[,])main.board.Clone(), alpha, beta, startCoords, clickedCoords, boardHash, scores);
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
            case (pawn | black):
                main.board[clickedCoords.Item1, clickedCoords.Item2] = 0;
                main.board[unclickedCoords.Item1, unclickedCoords.Item2] = pawn | black;
                break;

            case (knight | black):
                main.board[clickedCoords.Item1, clickedCoords.Item2] = 0;
                main.board[unclickedCoords.Item1, unclickedCoords.Item2] = knight | black;
                break;

            case (rook | black):
                main.board[clickedCoords.Item1, clickedCoords.Item2] = 0;
                main.board[unclickedCoords.Item1, unclickedCoords.Item2] = rook | black;
                break;

            case (bishop | black):
                main.board[clickedCoords.Item1, clickedCoords.Item2] = 0;
                main.board[unclickedCoords.Item1, unclickedCoords.Item2] = bishop | black;
                break;

            case (queen | black):
                main.board[clickedCoords.Item1, clickedCoords.Item2] = 0;
                main.board[unclickedCoords.Item1, unclickedCoords.Item2] = queen | black;
                break;

            case (king | black):
                main.board[clickedCoords.Item1, clickedCoords.Item2] = 0;
                main.board[unclickedCoords.Item1, unclickedCoords.Item2] = king | black;
                break;

        }

        UnityEngine.Debug.Log($"Elapsed Time: {stopwatch.Elapsed.TotalSeconds}");
        UnityEngine.Debug.Log($"Elapsed Time avarege: {main.evals / stopwatch.Elapsed.TotalSeconds} ");
        UnityEngine.Debug.Log(depth);
    //ahoooooooooojjjjj
    // 2930428
    // 2141830

    }

    List<int> minimax(byte parentColour, int iterdepth, int depth, int maxDepth, byte[,] board, int alpha, int beta, List<(int, int)> startCoords, (int,int) clickedCoords, ulong boardHash, List<int> scores)
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

                    if (parentColour == (board[y,x] & colorMask))
                    {
                        piecesCoords.Add((y,x));
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
            byte pieceName = board[y,x];

            switch (pieceName & nameMask)
            {
                case pawn:
                {
                    toDoMoves = main.pawnClass.getValidMoves((y,x), parentColour, board);
                    break;
                }
                case knight:
                {
                    toDoMoves = main.knightClass.getValidMoves((y,x), parentColour, board);
                    break;
                }
                case rook:
                {
                    toDoMoves = main.rookClass.getValidMoves((y,x), parentColour, board);
                    break;
                }
                case bishop:
                {
                    toDoMoves = main.bishopClass.getValidMoves((y,x), parentColour, board);
                    break;
                }
                case queen:
                {
                    toDoMoves = main.queenClass.getValidMoves((y,x), parentColour, board);
                    break;
                }
                case king:
                {
                    toDoMoves = main.kingClass.getValidMoves((y,x), parentColour, board);
                    break;
                }
                
            }
            

            toDoMoves = toDoMoves.OrderByDescending(move => EvaluateMove(move, (y,x), board, boardHash, pieceName)).ToList();
            

            foreach ((int,int) move in toDoMoves)
            {
                // gettin all the info and making the move
                byte capturedPieceName = board[move.Item1, move.Item2];
                board[move.Item1, move.Item2] = pieceName;
                board[y,x] = 0;
                
                // square tables
                int multiplier = (pieceName >> 4 == 1) ? 1 : -1; 
                int tableY = (pieceName >> 4 == 1) ? (7 - move.Item1) : move.Item1; // flip y coord for black
                if (pieceSquareTables.TryGetValue((byte)(pieceName & nameMask), out int[,] squareTable))
                {
                    piecePositionsValue[pieceName] += multiplier * squareTable[tableY, move.Item2];
                }

                //board hasshing
                
                boardHash ^= zobristTable[pieceName, y, x];
                boardHash ^= zobristTable[pieceName, move.Item1, move.Item2];

                // capturing and updating board hash accordingly
                if (capturedPieceName != 0)
                {
                    boardHash ^= zobristTable[capturedPieceName, move.Item1, move.Item2];
                    pieceCounts[capturedPieceName] --;
                }

                // main logic
                if (depth == 1)
                {
                    score.Add(parentColour == 16 ? minimax(white, iterdepth, depth+1, maxDepth, board, alpha, beta, startCoords, clickedCoords, boardHash,scores).Min() 
                                                : minimax(black, iterdepth,depth+1, maxDepth, board, alpha, beta, startCoords, clickedCoords, boardHash,scores).Max());
                    scoresPos.Add((y,x,pieceName,(move.Item1, move.Item2)));
                }
                else if (maxDepth != depth)
                {
                    score.Add(parentColour == 16 ? minimax(white, iterdepth, depth+1, maxDepth, board, alpha, beta, startCoords, clickedCoords, boardHash,scores).Min() 
                                                : minimax(black, iterdepth,depth+1, maxDepth, board, alpha, beta, startCoords, clickedCoords, boardHash,scores).Max());
                }
                else
                {
                    score.Add(Evaluation());
                }

                // undiong capturing, moves and boadr hashes
                if (capturedPieceName != 0)
                {
                    boardHash ^= zobristTable[capturedPieceName, move.Item1, move.Item2];
                    pieceCounts[capturedPieceName] ++;
                }

                boardHash ^= zobristTable[pieceName, move.Item1, move.Item2];
                boardHash ^= zobristTable[pieceName, y, x];
                
                board[move.Item1, move.Item2] = capturedPieceName;
                board[y,x] = pieceName;

                

                if (pieceSquareTables.TryGetValue((byte)(pieceName & nameMask), out squareTable))
                {
                    piecePositionsValue[pieceName] -= multiplier * squareTable[tableY, move.Item2];
                }
                


                // alpha beta pruning
                if (parentColour == black)
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
        
        transpositionsDic.TryAdd(boardHash, (score, depth));
        return score;
        
    }


    int EvaluateMove((int,int) move, (int,int) movedFrom, byte[,] board, ulong boardHash, byte pieceName)
    {
        int moveScore = 0;
        byte toMovePieceName = board[move.Item1, move.Item2];

        if (toMovePieceName != 0)
            moveScore += piecesWeight[(byte)(toMovePieceName & nameMask)] - piecesWeight[(byte)(pieceName & nameMask)];

        
        if (killerMove.Contains(boardHash))
        {
            moveScore += 1000;
        }
        int tableY = ((pieceName & black) == 0) ? (7 - move.Item1) : move.Item1; // flip y coord for black
        if (pieceSquareTables.TryGetValue((byte)(pieceName & nameMask), out int[,] squareTable))
        {
            moveScore += squareTable[tableY, move.Item2];
        }


        return moveScore;
    }

    int Evaluation()
    {
        main.evals ++;
        int scoreNow = 0;

        foreach (KeyValuePair<byte, int> kvp in pieceCounts)
        {
            byte pieceName = kvp.Key;
            int count = kvp.Value;
            int pieceValue = piecesWeight[(byte)(pieceName & nameMask)];
            scoreNow += (((pieceName & colorMask) == 16) ? pieceValue : -pieceValue) * count + piecePositionsValue[pieceName];  
            

        }
        
        
        return scoreNow;//lalalal blabla blabla hihi haha enter
    }

    void SetCounts(byte[,] board)
    {
        for (int y = 0; y<8; y++)
        {
            for (int x = 0; x<8; x++)
            {
                byte pieceName = board[y,x];
                if (pieceName != 0)
                {
                    pieceCounts[pieceName]++;

                    boardHash ^= zobristTable[pieceName, y, x];
                }
                
            }
        }

    }
}
