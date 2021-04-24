using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    [SerializeField]
    private string[] terrainObjectName;
    [SerializeField]
    private int terrainSize = 10;
    private int xSize = 5;
    private int ySize = 3;
    private int zSize = 5;
    private TerrainObject[,,] map;
    private TerrainObject[,,] holdMap;
    private Vector3 mapOffset = new Vector3(0, 0, 0); //offset is measured in world position

    [SerializeField]
    private float timeUpdateThreshold = 0.5f;
    private float lastUpdate = 0;
    [SerializeField]
    private Transform player;

    private bool debug = false;

    private void Awake()
    {
        map = new TerrainObject[xSize, ySize, zSize];
        holdMap = new TerrainObject[xSize, ySize, zSize];
        mapOffset = new Vector3((xSize - 1) / 2, (ySize - 1) / 2, (zSize - 1) / 2) * -terrainSize;
        print("Map Offset " + mapOffset);
        Setup();
    }
    private void Update()
    {
        if (Time.time - lastUpdate > timeUpdateThreshold)
        {
            lastUpdate = Time.time;
            UpdateTerrain(player.position);
        }
    }
    private void Setup()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    // clear map
                    //map[x, y, z] = null;
                    // if position has a or more mid, add a square
                    int mids = (x == ((xSize - 1) / 2) ? 1 : 0) + (y == ((ySize - 1) / 2) ? 1 : 0) + (z == ((zSize - 1) / 2) ? 1 : 0);
                    if (mids > 1) AddSquareIfNull(new Vector3(x, y, z));
                }
            }
        }
    }
    public void UpdateTerrain(Vector3 playerPosition)
    {
        Vector3 check = ConvertWorldToMap(playerPosition);
        //print("checking... " + check);
        float upperBound = 0.5f;
        float lowerBound = -0.5f;
        if (check.x > upperBound + (xSize - 1) / 2)
        {
            // transition terrain right
            TerrainTransition(Vector3.right);
        } else if (check.x < lowerBound + (xSize - 1) / 2)
        {
            // transition terrain left
            TerrainTransition(Vector3.left);
        }
        if (check.z > upperBound + (zSize - 1) / 2)
        {
            // transition terrain forward
            TerrainTransition(Vector3.forward);
        } else if (check.z < lowerBound + (zSize - 1) / 2)
        {
            // transition terrain back
            TerrainTransition(Vector3.back);
        }
        if (check.y > upperBound + (ySize - 1) / 2)
        {
            // transition terrain up
            TerrainTransition(Vector3.up);
        } else if (check.y < lowerBound + (ySize - 1) / 2)
        {
            // transition terrain down
            TerrainTransition(Vector3.down);
        }
        UpdateGridPosition();
    }
    private void TerrainTransition(Vector3 newPosition)
    {
        // move old squares in memory to new positions
        // delete old squares if out of bounds
        mapOffset += newPosition * terrainSize;
        if (debug) print("Terrain Transition " + newPosition);
        Vector3 offset = -newPosition; // create offset vector
        CleanHoldMap();
        for (int x = 0; x < zSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    MoveSquare(x, y, z, (int)offset.x + x, (int)offset.y + y, (int)offset.z + z);
                }
            }
        }
        HoldMapToMap();
        // add new squares in at new positions
        Vector3 center = new Vector3((xSize - 1) / 2, (ySize - 1) / 2, (zSize - 1) / 2);
        AddSquareIfNull(center + new Vector3(1, 0, 0));
        AddSquareIfNull(center + new Vector3(-1, 0, 0));
        AddSquareIfNull(center + new Vector3(0, 1, 0));
        AddSquareIfNull(center + new Vector3(0, -1, 0));
        AddSquareIfNull(center + new Vector3(0, 0, 1));
        AddSquareIfNull(center + new Vector3(0, 0, -1));
        AddSquareIfNull(center + new Vector3(0, 0, 2));
        AddSquareIfNull(center + new Vector3(0, 0, -2));
        AddSquareIfNull(center + new Vector3(2, 0, 0));
        AddSquareIfNull(center + new Vector3(-2, 0, 0));
        AddSquareIfNull(center + new Vector3(1, 0, 1));
        AddSquareIfNull(center + new Vector3(-1, 0, 1));
        AddSquareIfNull(center + new Vector3(1, 0, -1));
        AddSquareIfNull(center + new Vector3(-1, 0, -1));
    }
    private void AddSquareIfNull(Vector3 position)
    {
        print("Square Position " + position);
        if (map[(int)position.x, (int)position.y, (int)position.z] == null)
        {
            // add a terrain object
            TerrainObject terrain = null;
            while(terrain == null)
            {
                terrain = ObjectPooler.PoolObject(terrainObjectName[Random.Range(0, terrainObjectName.Length - 1)]).GetComponent<TerrainObject>();
            }
            map[(int)position.x, (int)position.y, (int)position.z] = terrain;
            terrain.transform.position = ConvertMapToWorld((int)position.x, (int)position.y, (int)position.z);
            terrain.transform.rotation = Quaternion.Euler(0, 90 * Random.Range(0, 3), 0);
            terrain.gameObject.SetActive(true);
            if(debug) print("Adding sqare " + position);
        }
    }
    private void RemoveSquareIfNotNull(int x, int y, int z)
    {
        if (map[x, y, z] != null)
        {
            if(debug) print("Square removed " + new Vector3(x, y, z));
            map[x, y, z].gameObject.SetActive(false);
            map[x, y, z] = null;
        }
    }
    private void CleanHoldMap()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    holdMap[x, y, z] = null;
                }
            }
        }
    }
    private void HoldMapToMap()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    map[x, y, z] = holdMap[x, y, z];
                }
            }
        }
    }
    private void UpdateGridPosition()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int z = 0; z < zSize; z++)
                {
                    if(map[x, y, z] != null) map[x, y, z].gridPosition = new Vector3(x, y, z);
                }
            }
        }
    }
    private void MoveSquare(int x0, int y0, int z0, int x1, int y1, int z1)
    {
        if (x1 > xSize - 1 || x1 < 0 || y1 > ySize - 1 || y1 < 0 || z1 > zSize - 1 || z1 < 0)
        {
            RemoveSquareIfNotNull(x0, y0, z0);
        } else {
            // move square
            if(debug) print("Moving square (" + x0 + ", " + y0 + ", " + z0 + ") (" + x1 + ", " + y1 + ", " + z1 + ")");
            holdMap[x1, y1, z1] = map[x0, y0, z0];
        }
    }
    private Vector3 ConvertWorldToMap(Vector3 position)
    {
        Vector3 converted = (position - mapOffset) / terrainSize;
        return converted;
    }
    private Vector3 ConvertMapToWorld(int x, int y, int z)
    {
        Vector3 converted = new Vector3(x, y, z) * terrainSize + mapOffset;
        return converted;
    }
}

