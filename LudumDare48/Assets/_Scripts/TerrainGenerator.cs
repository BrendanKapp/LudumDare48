using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{

    [SerializeField]
    private string terrainObjectName = "Terrain";
    [SerializeField]
    private int terrainSize = 10;
    [SerializeField]
    private TerrainObject[,,] map = new TerrainObject[3, 3, 3];
    [SerializeField]
    private Vector3 mapOffset = new Vector3(0, 0, 0); //offset is measured in world position

    [SerializeField]
    private float timeUpdateThreshold = 0.5f;
    private float lastUpdate = 0;
    [SerializeField]
    private Transform player;

    private void Awake()
    {
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
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    // clear map
                    //map[x, y, z] = null;
                    // if position has two or more ones, add a square
                    int ones = (x == 1 ? 1 : 0) + (y == 1 ? 1 : 0) + (z == 1 ? 1 : 0);
                    if (ones > 1) AddSquareIfNull(new Vector3(x, y, z));
                }
            }
        }
    }
    public void UpdateTerrain(Vector3 playerPosition)
    {
        Vector3 check = ConvertWorldToMap(playerPosition);
        //print("checking... " + check);
        float upperBound = 1.5f;
        float lowerBound = 0.5f;
        if (check.x > upperBound)
        {
            print("right");
            // transition terrain right
            TerrainTransition(Vector3.right);
        } else if (check.x < lowerBound)
        {
            print("left");
            // transition terrain left
            TerrainTransition(Vector3.left);
        }
        if (check.z > upperBound)
        {
            print("forward");
            // transition terrain forward
            TerrainTransition(Vector3.forward);
        } else if (check.z < lowerBound)
        {
            print("back");
            // transition terrain back
            TerrainTransition(Vector3.back);
        }
        if (check.y > upperBound)
        {
            print("up");
            // transition terrain up
            TerrainTransition(Vector3.up);
        } else if (check.y < lowerBound)
        {
            print("down");
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
        print("Terrain Transition " + newPosition);
        Vector3 offset = -newPosition; // create offset vector
        CleanHoldMap();
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    MoveSquare(x, y, z, (int)offset.x + x, (int)offset.y + y, (int)offset.z + z);
                }
            }
        }
        HoldMapToMap();
        // add new squares in at new positions
        AddSquareIfNull(Vector3.one + new Vector3(1, 0, 0));
        AddSquareIfNull(Vector3.one + new Vector3(-1, 0, 0));
        AddSquareIfNull(Vector3.one + new Vector3(0, 1, 0));
        AddSquareIfNull(Vector3.one + new Vector3(0, -1, 0));
        AddSquareIfNull(Vector3.one + new Vector3(0, 0, 1));
        AddSquareIfNull(Vector3.one + new Vector3(0, 0, -1));
    }
    private void AddSquareIfNull(Vector3 position)
    {
        if (map[(int)position.x, (int)position.y, (int)position.z] == null)
        {
            // add a terrain object
            TerrainObject terrain = ObjectPooler.PoolObject(terrainObjectName).GetComponent<TerrainObject>();
            map[(int)position.x, (int)position.y, (int)position.z] = terrain;
            terrain.transform.position = ConvertMapToWorld((int)position.x, (int)position.y, (int)position.z);
            terrain.gameObject.SetActive(true);
            print("Adding sqare " + position);
        }
    }
    private void RemoveSquareIfNotNull(int x, int y, int z)
    {
        if (map[x, y, z] != null)
        {
            print("Square removed " + new Vector3(x, y, z));
            map[x, y, z].gameObject.SetActive(false);
            map[x, y, z] = null;
        }
    }
    private TerrainObject[,,] holdMap = new TerrainObject[3, 3, 3];
    // private void MapToHoldMap()
    // {
    //     for (int x = 0; x < 3; x++)
    //     {
    //         for (int y = 0; y < 3; y++)
    //         {
    //             for (int z = 0; z < 3; z++)
    //             {
    //                 holdMap[x, y, z] = map[x, y, z];
    //             }
    //         }
    //     }
    // }
    private void CleanHoldMap()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    holdMap[x, y, z] = null;
                }
            }
        }
    }
    private void HoldMapToMap()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    map[x, y, z] = holdMap[x, y, z];
                }
            }
        }
    }
    private void UpdateGridPosition()
    {
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                for (int z = 0; z < 3; z++)
                {
                    if(map[x, y, z] != null) map[x, y, z].gridPosition = new Vector3(x, y, z);
                }
            }
        }
    }
    private void MoveSquare(int x0, int y0, int z0, int x1, int y1, int z1)
    {
        if (x1 > 2 || x1 < 0 || y1 > 2 || y1 < 0 || z1 > 2 || z1 < 0)
        {
            RemoveSquareIfNotNull(x0, y0, z0);
        } else {
            // move square
            print("Moving square (" + x0 + ", " + y0 + ", " + z0 + ") (" + x1 + ", " + y1 + ", " + z1 + ")");
            holdMap[x1, y1, z1] = map[x0, y0, z0];
        }
    }
    private Vector3 ConvertWorldToMap(Vector3 position)
    {
        Vector3 converted = (position - mapOffset) / terrainSize;
        //print("Converted W2M " + position + " to " + converted);
        return converted;
    }
    private Vector3 ConvertMapToWorld(int x, int y, int z)
    {
        Vector3 converted = new Vector3(x, y, z) * terrainSize + mapOffset;
        //print("Converted M2W " + x + ", " + y + ", " + z + " to " + converted);
        return converted;
    }
}

