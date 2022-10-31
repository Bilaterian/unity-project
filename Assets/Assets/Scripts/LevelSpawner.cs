using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    public GameObject wall;

    public GameObject player;

    public GameObject enemy;

    public int mapSize;

    public int roomCount;

    private int[,] levelMap;
    // Start is called before the first frame update
    void Start()
    {
        levelMap = new int[mapSize, mapSize];
        for(int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                levelMap[i, j] = 1;
            }
        }

        AddRooms();

        FillMap();
    }

    void FillMap()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if(levelMap[i, j] == 1)
                {
                    GameObject newObj = Instantiate(wall);
                    newObj.transform.SetParent(transform);
                    newObj.transform.localPosition = new Vector3(i * 2, 0.5f, j * 2);
                }
            }
        }
    }

    void AddRooms()
    {
        int numRooms = 0;
        while (numRooms < roomCount)
        {
            //find suitable room
        }
    }
}
