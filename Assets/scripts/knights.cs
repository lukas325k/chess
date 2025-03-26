using System.Collections.Generic;
using UnityEngine;


public class Knight
{
    private Main main;

    private (int,int)[] directions = {(-2,-1), (-1,-2), (1,-2), (2,-1), (2,1), (1,2), (-1,2), (-2,1)};

    public Knight(Main caller, int y, int x, bool toCreate, string parentColour)
    {
        this.main = caller;
        
        if (toCreate)
        {
            create(y,x, parentColour);
        }
        
        
    }

    public void create(int y, int x, string parentColour)
    {
        GameObject obj = new GameObject(parentColour == "white" ? $"wKnight{y}{x}" : $"bKnight{y}{x}");
        obj.transform.position = new Vector3((x - main.moveLeft) * main.sqareSize,(-y + main.moveUp)*main.sqareSize, 0f);
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        obj.tag = "piece";

        // Convert Texture2D to Sprite
        renderer.sprite = Sprite.Create(
            parentColour == "white" ? main.wKnightTexture : main.bKnightTexture,
            new Rect(0, 0, main.wKnightTexture.width, main.wKnightTexture.height), // doesnt mater if w or b coause the dimensions are the same
            new Vector2(0.5f, 0.5f) // Pivot
    );
    }

    public (List<(int, int)>,List<(int, int)>) getValidMoves((int, int) startPos, string parentColour, string[,] board)
    {
        List<(int,int)> validMoves = new List<(int, int)>();
        List<(int,int)> capturedMoves = new List<(int, int)>();
        foreach ((int,int) dir in directions)
        {
            int yPos = startPos.Item1 + dir.Item1;
            int xPos = startPos.Item2 + dir.Item2;
            if (0 <= yPos && yPos < 8 && 0 <= xPos && xPos < 8)
            {
                if (board[yPos,xPos] == null)
                    validMoves.Add((yPos,xPos));
                else if (board[yPos,xPos][0] == (parentColour == "white" ? 'b' : 'w'))
                    capturedMoves.Add((yPos,xPos));
                    
                
            }
        }

        return (validMoves, capturedMoves);
    }


    


}