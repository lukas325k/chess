using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;

public class Zobrist
{
    ulong[,,] zobristTable = new ulong[31, 8, 8];
    List<string> pieces = new List<string>();
    Random random = new Random();

    public Zobrist()
    {
        pieces.AddRange(new List<string> {"wPawn", "bPawn", "wKnight", "bKnight", "wRook", "bRook", "wBishop", "bBishop", "wQueen", "bQueen", "wKing", "bKing"}.ToList());
    }

    public ulong[,,] ZobristHashing()
    {
        for (int piece = 0; piece < 31; piece++)
        {
            for(int y = 0; y < 8; y ++)
            {
                for(int x = 0; x < 8; x ++)
                {


                    zobristTable[piece, y, x] = ((ulong)random.Next() << 32) | (ulong)random.Next();

                }
            }
            
        }
        return zobristTable;
    }




}