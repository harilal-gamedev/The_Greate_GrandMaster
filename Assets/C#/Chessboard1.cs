using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chessboard1 : MonoBehaviour
{
    //ART FIELD
    [SerializeField] private Material tileMaterial;


    //LOGIC
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;

    private void Awake()
    {
        GenerateAllTiles(1, TILE_COUNT_X, TILE_COUNT_Y);
    }


    //GENERATE THE BOARD


    // this method is responsible for to create all tiles on the screen .
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
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

        vertices[0] = new Vector3(x * tileSize, 0, y * tileSize);
        vertices[1] = new Vector3(x * tileSize, 0, (y + 1) * tileSize);
        vertices[2] = new Vector3((x + 1) * tileSize, 0, y * tileSize);
        vertices[3] = new Vector3((x + 1) * tileSize, 0, (y + 1) * tileSize);

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 }; // this helps to get the geometry of the shape for the mesh filter.

        mesh.vertices = vertices; // this assagin the vertices that created to the mesh.
        mesh.triangles = tris;  // this assagin the triangles that we created to the mesh.

        tileObject.AddComponent<BoxCollider>();

        mesh.RecalculateNormals();

        return tileObject;
    }
}
