using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;

public class Zobrist
{
    Dictionary<(string,(int,int)), ulong> zobristDic = new Dictionary<(string, (int, int)), ulong>();
    List<string> pieces = new List<string>();
    Random random = new Random();

    public Zobrist()
    {
        pieces.AddRange(new List<string> {"wPawn", "bPawn", "wKnight", "bKnight", "wRook", "bRook", "wBishop", "bBishop", "wQueen", "bQueen", "wKing", "bKing"}.ToList());
    }

    public Dictionary<(string,(int,int)), ulong> ZobristHashing()
    {
        foreach (string piece in  pieces)
        {
            for(int y = 0; y < 8; y ++)
            {
                for(int x = 0; x < 8; x ++)
                {


                    zobristDic.Add((piece,(y,x)), ((ulong)random.Next() << 32) | (ulong)random.Next());

                }
            }
            
        }
        return zobristDic;
    }




}