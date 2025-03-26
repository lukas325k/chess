using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Pawn
{
    private Main main;

    public Pawn(Main caller, int y, int x, bool toCreate, string parentColour)
    {
        this.main = caller;
        
        if (toCreate)
        {
            create(y,x, parentColour);
        }
        
    }

    public void create(int y, int x, string parentColour)
    {
        GameObject obj = new GameObject(parentColour == "white" ? $"wPawn{y}{x}" : $"bPawn{y}{x}");
        obj.transform.position = new Vector3((x - main.moveLeft) * main.sqareSize,(-y + main.moveUp)*main.sqareSize, 0f);
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        obj.tag = "piece";

        // Convert Texture2D to Sprite
        renderer.sprite = Sprite.Create(
            parentColour == "white" ? main.wPawnTexture : main.bPawnTexture,
            new Rect(0, 0, main.wPawnTexture.width, main.wPawnTexture.height), // doesnt mater if w or b coause the dimensions are the same
            new Vector2(0.5f, 0.5f) // Pivot
    );
    }

    public List<(int, int)> getValidMoves((int, int) startPos, string parentColour, string[,] board)
    {
        bool isAtStart = parentColour == "white" ? startPos.Item1 == 6 ? true : false : startPos.Item1 == 1 ? true : false;
        int dir = parentColour == "white" ? -1 : 1;
        List<(int,int)> validMoves = new List<(int, int)>();
        if (0 <= startPos.Item1 + dir && startPos.Item1 + dir < 8)
        {
            if (0 <= startPos.Item2 && startPos.Item2 < 8)
            {

                if (board[startPos.Item1 + dir, startPos.Item2] == null)
                {
                    validMoves.Add((startPos.Item1 + dir, startPos.Item2));

                    if (isAtStart && board[startPos.Item1 + dir*2, startPos.Item2] == null)
                    {
                        validMoves.Add((startPos.Item1 + dir*2, startPos.Item2));
                    }
                }

                foreach (int i in new int[] {-1,1})
                {
                    if (0 <= startPos.Item2 + i && startPos.Item2 + i <8)
                    {
                        if (board[startPos.Item1 + dir, startPos.Item2 + i] != null)
                        {
                            if (board[startPos.Item1 + dir, startPos.Item2 + i][0] == (parentColour == "white" ? 'b' : 'w'))
                            {
                                validMoves.Add((startPos.Item1 + dir, startPos.Item2 + i));
                            }
                        }
                    }
                }
            }
        }
        
        

        return validMoves;
    }


    


}