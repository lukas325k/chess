using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject tileLight;
    public GameObject tileDark;
    public GameObject clickedPiece = null;

    public Texture2D wPawnTexture;
    public Texture2D bPawnTexture;
    public Texture2D wKnightTexture;
    public Texture2D bKnightTexture;
    public Texture2D wRookTexture;
    public Texture2D bRookTexture;
    public Texture2D wBishopTexture;
    public Texture2D bBishopTexture;
    public Texture2D wQueenTexture;
    public Texture2D bQueenTexture;
    public Texture2D wKingTexture;
    public Texture2D bKingTexture;

    public float sqareSize = 1.25f;
    public float moveUp = 3.5f;
    public float moveLeft = 3.5f;
    public string[,] board = new string[8,8];

    public Dictionary<(int,int), GameObject> pawnsDic= new Dictionary<(int, int), GameObject>();

    public (int, int)[] whitePawnsPos = {(6,0),(6,1),(6,2),(6,3),(6,4),(6,5),(6,6),(6,7)};
    public (int, int)[] blackPawnsPos = {(1,0),(1,1),(1,2),(1,3),(1,4),(1,5),(1,6),(1,7)};
    public (int, int)[] whiteKnightsPos = {(7,1), (7,6)};
    public (int, int)[] blackKnightsPos = {(0,1), (0,6)};
    public (int, int)[] whiteRooksPos = {(7,0), (7,7)};
    public (int, int)[] blackRooksPos = {(0,0), (0,7)};
    public (int, int)[] whiteBishopsPos = {(7,2), (7,5)};
    public (int, int)[] blackBishopsPos = {(0,2), (0,5)};
    public (int, int)[] whiteQueenPos = {(7,3)};
    public (int, int)[] blackQueenPos = {(0,3)};
    public (int, int)[] whiteKingPos = {(7,4)};
    public (int, int)[] blackKingPos = {(0,4)};
     
    public Pawn pawnClass;
    public Knight knightClass;
    public Rook rookClass;
    public Bishop bishopClass;
    public Queen queenClass;
    public King kingClass;


    public List<(int, int)[]> piecesPos = new List<(int, int)[]>();
    public List<string> piecesName = new List<string>();    

    (int, int) clickedCoords = (0,0);
    (int, int) unclickedCoords = (0,0);
    string clickedName;

    public int evals = 0;
    void Start()
    {
        pawnClass = new Pawn(this, 0, 0, false, "white"); // the colour here doesnt matter i just want to crete an instance so i can acces the valid moves I pass the color there
        knightClass = new Knight(this, 0, 0, false, "white"); 
        rookClass = new Rook(this, 0, 0, false, "white"); 
        bishopClass = new Bishop(this, 0,0, false, "white");
        queenClass = new Queen(this, 0,0, false, "white");
        kingClass = new King(this , 0,0, false, "white");

        piecesPos = new List<(int, int)[]> {whitePawnsPos,blackPawnsPos, whiteKnightsPos, blackKnightsPos, whiteRooksPos, blackRooksPos, whiteBishopsPos, blackBishopsPos, whiteQueenPos, blackQueenPos, whiteKingPos, blackKingPos};
        piecesName.AddRange(new List<string> {"wPawn", "bPawn", "wKnight", "bKnight", "wRook", "bRook", "wBishop", "bBishop", "wQueen", "bQueen", "wKing", "bKing"});

        CreateBoard();
        PlacePieces();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickedCoords = GetClickedCoords(Input.mousePosition.y, Input.mousePosition.x);
            clickedPiece = FindGameObjectBySuffix(clickedCoords.ToString());
            clickedName = board[clickedCoords.Item1, clickedCoords.Item2];
            
        }

        if (Input.GetMouseButton(0) && clickedPiece != null)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickedPiece.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
        }

        if (Input.GetMouseButtonUp(0))
        {
            unclickedCoords = GetClickedCoords(Input.mousePosition.y, Input.mousePosition.x);
            clickedPiece = null;

            switch (clickedName)
            {
                case "wPawn":
                    if (pawnClass.getValidMoves(clickedCoords, "white", board).Contains(unclickedCoords))
                    {
                        board[clickedCoords.Item1, clickedCoords.Item2] = null;
                        board[unclickedCoords.Item1, unclickedCoords.Item2] = "wPawn";
                    }
                    break;
                case "wKnight":
                    if (knightClass.getValidMoves(clickedCoords, "white", board).Contains(unclickedCoords))
                    {
                        board[clickedCoords.Item1, clickedCoords.Item2] = null;
                        board[unclickedCoords.Item1, unclickedCoords.Item2] = "wKnight";
                    }
                    break;
                case "wRook":
                    if (rookClass.getValidMoves(clickedCoords, "white", board).Contains(unclickedCoords))
                    {
                        board[clickedCoords.Item1, clickedCoords.Item2] = null;
                        board[unclickedCoords.Item1, unclickedCoords.Item2] = "wRook";
                    }
                    break;
                case "wBishop":
                    if (bishopClass.getValidMoves(clickedCoords, "white", board).Contains(unclickedCoords))
                    {
                        board[clickedCoords.Item1, clickedCoords.Item2] = null;
                        board[unclickedCoords.Item1, unclickedCoords.Item2] = "wBishop";
                    }
                    break;
                case "wQueen":
                    if (queenClass.getValidMoves(clickedCoords, "white", board).Contains(unclickedCoords))
                    {
                        board[clickedCoords.Item1, clickedCoords.Item2] = null;
                        board[unclickedCoords.Item1, unclickedCoords.Item2] = "wQueen";
                    }
                    break;
                case "wKing":
                    if (kingClass.getValidMoves(clickedCoords, "white", board).Contains(unclickedCoords))
                    {
                        board[clickedCoords.Item1, clickedCoords.Item2] = null;
                        board[unclickedCoords.Item1, unclickedCoords.Item2] = "wKing";
                    }
                    break;
                
            }
            if (clickedName[0] == 'b')
            {
                    new Ai(this);
                    Debug.Log(evals);
                    evals = 0;
            }
                    

            UpdatePieces();
        }

    }

    public void DeleteCaptured((int,int) coords)
    {
        int indexToDeleteFrom = 0;
        int indexToDelete = 0;
        bool toDelete = false;
        foreach ((int,int)[] piecepos in piecesPos)
        {
            
            if (piecepos.Contains(coords))
            {
                indexToDeleteFrom = piecesPos.IndexOf(piecepos);            
                indexToDelete = Array.IndexOf(piecepos, coords);
                toDelete = true;
                break;
            }
            
        }
        if (toDelete)
        {
            piecesPos[indexToDeleteFrom] = piecesPos[indexToDeleteFrom].Where((val, idx) => idx != indexToDelete).ToArray();
        }
    }

    void CreateBoard()
    {
        for (int y = 0; y < 8; y ++)
        {
            for (int x = 0; x < 8; x ++)
            {
                if ((y+x)%2 == 0)
                {
                    Instantiate(tileLight, new Vector2((x - moveLeft) * sqareSize,(-y + moveUp)*sqareSize), Quaternion.identity);
                }
                else
                {
                    Instantiate(tileDark, new Vector2((x - moveLeft) * sqareSize,(-y + moveUp)*sqareSize), Quaternion.identity);
                }
            }
            

        }

    }

    public void PlacePieces()
    {   
        DestroyObjectsWithTag("piece");
        board = new string[8,8];
        piecesPos = new List<(int, int)[]> {whitePawnsPos, blackPawnsPos, whiteKnightsPos, blackKnightsPos, whiteRooksPos, blackRooksPos, whiteBishopsPos, blackBishopsPos, whiteQueenPos, blackQueenPos, whiteKingPos, blackKingPos};

        for (int y = 0; y < 8; y ++)
        {
            for (int x = 0; x < 8; x ++)
            {
                foreach ((int, int)[] piecePos in piecesPos)
                {
                    int index = piecesPos.IndexOf(piecePos);

                    if (piecePos.Contains((y,x)))
                    {
                        board[y,x] = piecesName[index];
                        switch (piecesName[index][1..])
                        {
                            case "Pawn":
                                new Pawn(this, y, x, true, piecesName[index][0] == 'w' ? "white" : "black");
                                break;

                            case "Knight":
                                new Knight(this, y, x, true, piecesName[index][0] == 'w' ? "white" : "black");
                                break;

                            case "Rook":
                                new Rook(this, y, x, true, piecesName[index][0] == 'w' ? "white" : "black");
                                break;

                            case "Bishop":
                                new Bishop(this, y, x, true, piecesName[index][0] == 'w' ? "white" : "black");
                                break;
                                
                            case "Queen":
                                new Queen(this, y, x, true, piecesName[index][0] == 'w' ? "white" : "black");
                                break;

                            case "King":
                                new King(this, y, x, true, piecesName[index][0] == 'w' ? "white" : "black");
                                break;

                        }
                    }
                }
            }
        }
    }

    public void UpdatePieces()
    {
        DestroyObjectsWithTag("piece");
        for (int y = 0; y < 8; y ++)
        {
            for (int x = 0; x < 8; x ++)
            {
                if (board[y,x] != null)
                {
                    switch(board[y,x][1..])
                    {
                        case "Pawn":
                            new Pawn(this, y, x, true, board[y,x][0]  == 'w' ? "white" : "black");
                            break;

                        case "Knight":
                            new Knight(this, y, x, true, board[y,x][0] == 'w' ? "white" : "black");
                            break;

                        case "Rook":
                            new Rook(this, y, x, true, board[y,x][0] == 'w' ? "white" : "black");
                            break;

                        case "Bishop":
                            new Bishop(this, y, x, true, board[y,x][0] == 'w' ? "white" : "black");
                            break;
                            
                        case "Queen":
                            new Queen(this, y, x, true, board[y,x][0] == 'w' ? "white" : "black");
                            break;

                        case "King":
                            new King(this, y, x, true, board[y,x][0] == 'w' ? "white" : "black");
                            break;
                    }
                }
            }
        }

    }
    void DestroyObjectsWithTag(string tag)
    {
        GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag(tag);
        

        foreach (GameObject obj in objectsToDestroy)
        {
            Destroy(obj);
        }
    }

    (int,int) GetClickedCoords(float clickedY, float clickedX)
    {

        return (7-(int)math.floor(clickedY/187.5f),(int)math.floor(clickedX/187.5f));
    }

    GameObject FindGameObjectBySuffix(string suffix)
    {
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            string name = obj.name;
            if (name[^2] == suffix[1] && name[^1] == suffix[4])
            {
                return obj; // Return the first match
            }
        }

        return null; // No match found
    }
}
