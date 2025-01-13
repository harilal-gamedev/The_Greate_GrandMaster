using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    //ART FIELD
    [Header("Art Stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;


    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs; 
    [SerializeField] private Material[] teamMaterials;


    //LOGIC
    private ChessPiece[,] chessPieces;
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;

    private void Awake()
    {
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        // here we casting a ray from the mouse pointer and it only gonna interact with the objects layer named "Tile"
        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray , out info , 100 , LayerMask.GetMask("Tile")))
        {
            // get the indexes of the tile i've hited
            Vector2Int hitPosition = LookUpTileIndex(info.transform.gameObject);

            //If We're hovering a tile after not hovering
            if(currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover"); 
            }

            //If we were already hovering a tile , change the prevous one 
            //(if we are already hovering a tile and change the hover to a new tile then this if called)
            if(currentHover != hitPosition)
            {

                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile"); // the previous hovered layer have the layer name Hovring we want to change it back to Tile.
                currentHover = hitPosition; // update the currentHover here . 
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover"); // change the new hovered layer name to Hover.
            }
        }
        else
        {
            // if we are hovering now then go out of the board then this will exicute.
            if(currentHover != -Vector2Int.one)
            {
                // 
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");// the previous hovered layer have the layer name Hovring we want to change it back to Tile.
                currentHover = -Vector2Int.one;
            }
        }
    }


    //GENERATE THE BOARD


    // this method is responsible for to create all tiles on the screen .
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y; // this helps to lift our board ( help to adjust the hight)
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter; // this is for to get the board center . 

        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }

    // this method is responsible for to create single tiles on the screen.
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0} , Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh(); // this mesh provide a geometry for the tile(vertices , edges , faces) .
        tileObject.AddComponent<MeshFilter>().mesh = mesh; // the geometry is add to the "tileobject" s meshfilter.
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial; // addding a mesh renderer and a materilas to the tileobject;


        // the vertices is for to create the geometry . this vector3 array create 4 vertices that represents the corners of the square.
        Vector3[] vertices = new Vector3[4];

        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) -bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) -bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) -bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 }; // this helps to get the geometry of the shape for the mesh filter.

        mesh.vertices = vertices; // this assagin the vertices that created to the mesh.
        mesh.triangles = tris;  // this assagin the triangles that we created to the mesh.

        tileObject.layer = LayerMask.NameToLayer("Tile"); // this gonna put a layer named Tile to the tile object
        tileObject.AddComponent<BoxCollider>();

        mesh.RecalculateNormals();

        return tileObject;
    }


    //Spawning of the pieces .
    
    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam =0 , blackTeam =1 ;

        //White team 
        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);

                    // this is for the pawn
        for(int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i,1] = SpawnSinglePiece(ChessPieceType.Pawn , whiteTeam);
        }


        //Black team 
        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);

        // this is for the pawn
        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
        }
    }
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
    {
        ChessPiece cp = Instantiate(prefabs[(int)type -1] , transform).GetComponent<ChessPiece>();

        cp.type = type;
        cp.team = team;

       

        // Reset position and ensure rotation is explicitly set
        cp.transform.localRotation = Quaternion.Euler(0, 0, 0);
       

        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];

        return cp;
    }

    //Positioning
    private void PositionAllPieces()
    {
        for(int x = 0;x<TILE_COUNT_X;x++)
        {
            for(int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x,y] != null)
                {
                    PositionSinglePiece(x,y , true);
                }
            }
        }
    }
    private void PositionSinglePiece(int x , int y , bool force = false)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x,y].currentY = y;


        chessPieces[x, y].transform.position = new Vector3(x * tileSize, yOffset, y * tileSize);
    }


    //Operations
    // this method is for to get the ray hited tile index .
    private Vector2Int LookUpTileIndex(GameObject hitInfo)
    {
        // this is gonna iterate through the tiles array and when the hitInfo is in the array then it return the indexes of the tile
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one;
    }
}
