using System.Collections.Generic;
using UnityEngine;


public class King
{
    private Main main;
    private (int,int)[] directions = {(1,1), (1,-1), (-1,1), (-1,-1), (0,1), (0,-1), (1,0), (-1,0)};

    public King(Main caller, int y, int x, bool toCreate, string parentColour)
    {
        this.main = caller;
        
        if (toCreate)
        {
            create(y,x, parentColour);
        }
        
        
    }

    public void create(int y, int x, string parentColour)
    {
        GameObject obj = new GameObject(parentColour == "white" ? $"wKing{y}{x}" : $"bKing{y}{x}");
        obj.transform.position = new Vector3((x - main.moveLeft) * main.sqareSize,(-y + main.moveUp)*main.sqareSize, 0f);
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        obj.tag = "piece";

        // Convert Texture2D to Sprite
        renderer.sprite = Sprite.Create(
            parentColour == "white" ? main.wKingTexture : main.bKingTexture,
            new Rect(0, 0, main.wKingTexture.width, main.wKingTexture.height), // doesnt mater if w or b coause the dimensions are the same
            new Vector2(0.5f, 0.5f) // Pivot
    );
    }

    public List<(int, int)> getValidMoves((int, int) startPos, byte parentColour, byte[,] board)
    {
        int i = 1;
        List<(int,int)> validMoves = new List<(int, int)>();
        foreach ((int,int) dir in directions)
        {
            if (0 <= startPos.Item1 + dir.Item1*i && startPos.Item1 + dir.Item1*i < 8)
            {
                if (0 <= startPos.Item2 + dir.Item2*i && startPos.Item2 + dir.Item2*i < 8)
                {
                    if (board[startPos.Item1 + dir.Item1*i, startPos.Item2 + dir.Item2*i] == 0)
                    {
                        validMoves.Add((startPos.Item1 + dir.Item1*i, startPos.Item2 + dir.Item2*i));
                    }
                    else if ((board[startPos.Item1 + dir.Item1*i, startPos.Item2 + dir.Item2*i] & parentColour) == 0)
                    {
                        validMoves.Add((startPos.Item1 + dir.Item1*i, startPos.Item2 + dir.Item2*i));
                    }
                }
            }
        }

        return validMoves;
    }


    


}