
using System;
using System.Collections.Generic;
using UnityEngine;



public class Bishop
{
    private Main main;
    private (int,int)[] directions = {(1,1), (1,-1), (-1,1), (-1,-1)};

    public Bishop(Main caller, int y, int x, bool toCreate, string parentColour)
    {
        this.main = caller;
        
        if (toCreate)
        {
            create(y,x, parentColour);
        }
        
        
    }

    public void create(int y, int x, string parentColour)
    {
        GameObject obj = new GameObject(parentColour == "white" ? $"wBishop{y}{x}" : $"bBishop{y}{x}");
        obj.transform.position = new Vector3((x - main.moveLeft) * main.sqareSize,(-y + main.moveUp)*main.sqareSize, 0f);
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        obj.tag = "piece";

        // Convert Texture2D to Sprite
        renderer.sprite = Sprite.Create(
            parentColour == "white" ? main.wBishopTexture : main.bBishopTexture,
            new Rect(0, 0, main.wBishopTexture.width, main.wBishopTexture.height), // doesnt mater if w or b coause the dimensions are the same
            new Vector2(0.5f, 0.5f) // Pivot
    );
    }

    public List<(int, int)> getValidMoves((int, int) startPos, byte parentColour, byte[,] board)
    {
        List<(int,int)> validMoves = new List<(int, int)>();
        foreach ((int dx, int dy) in directions)
        {
            for (int i = 1; i < 8; i++)
            {
                int newX = startPos.Item1 + dx * i;
                int newY = startPos.Item2 + dy * i;

                if (newX < 0 || newX >= 8 || newY < 0 || newY >= 8)
                    break; 

                byte piece = board[newX, newY];

                if (piece == 0)
                {
                    validMoves.Add((newX, newY));
                }
                else
                {
                    if ((byte)(piece & parentColour) == 0)
                        validMoves.Add((newX, newY));
                    
                    break; 
                }
            }
        }

        return validMoves;
    }


    


}