using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    public GameObject wall;
    public GameObject player;
    public GameObject enemy;
    public GameObject crate;
    public GameObject caveWall;
    public GameObject floor;
    public GameObject caveFloor;
    public GameObject water;

    public int mapSize;
    public int roomCount;
    public int floorFill;

    public int octaves;
    public float persistance;
    public float lacunarity;
    public Vector2 offset;

    private int[,] levelMap;
    private int[,] caveMap;

    private Node _root = null;
    private List<Node> nodesList = new();

    public int neighborThreshold;
    public int seed;

    private List<Color> colorList = new();

    private float minHeight = float.MaxValue;
    private float maxHeight = float.MinValue;

    // Start is called before the first frame update
    void Start()
    {
        if (seed == 0) {
            seed = (int)Time.time;
        }

        Random.InitState(seed);

        InitColorList();

        levelMap = new int[mapSize, mapSize];
        for(int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                levelMap[i, j] = -1;
            }
        }

        caveMap = new int[mapSize, mapSize];
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if(i == 0 || i == mapSize - 1 || j == 0 || j == mapSize - 1)
                {
                    caveMap[i, j] = 4;
                }
                else
                {
                    float perlinHeight = 0f;
                    float amplitude = 1f;
                    float frequency = 1f;
                    for(int k = 0; k < octaves; k++)
                    {
                        float sampleX = i * ((float)floorFill / 100f) * frequency;
                        float sampleZ = j * ((float)floorFill / 100f) * frequency;
                        float noiseValue = Mathf.PerlinNoise(sampleX, sampleZ) * 2 - 1;
                        perlinHeight += noiseValue * amplitude;
                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }
                    if(perlinHeight > maxHeight)
                    {
                        maxHeight = perlinHeight;
                    }
                    else if(perlinHeight < minHeight)
                    {
                        minHeight = perlinHeight;
                    }

                    perlinHeight = Mathf.InverseLerp(minHeight, maxHeight, perlinHeight) * 2 - 1;
                    
                    if((int)(perlinHeight * 100) > 0)
                    {
                        caveMap[i, j] = 4;
                    }
                    else if((int)(perlinHeight * 100) > (floorFill * -1))
                    {
                        caveMap[i, j] = -1;
                    }
                    else
                    {
                        caveMap[i, j] = -2;
                    }
                }
            }
        }

        AddRooms();
        AddCorridors();
        SmoothCaveMap(4);
        SmoothCaveMap(-2);

        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (levelMap[i, j] == -1)
                {
                    levelMap[i, j] = caveMap[i, j];
                }
            }
        }

        FillMap();

        //DebugNodesList();
    }

    void FillMap()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (levelMap[i, j] == -2)
                {
                    GameObject newObj = Instantiate(water);
                    newObj.transform.SetParent(transform);
                    newObj.transform.localPosition = new Vector3(i * 2 + 0.5f, -0.8f, j * 2 + 0.5f);

                    GameObject newObj1 = Instantiate(caveFloor);
                    newObj1.transform.SetParent(transform);
                    newObj1.transform.localPosition = new Vector3(i * 2 + 0.5f, -2.5f, j * 2 + 0.5f);
                }
                else if (levelMap[i, j] == -1)
                {
                    GameObject newObj = Instantiate(caveFloor);
                    newObj.transform.SetParent(transform);
                    newObj.transform.localPosition = new Vector3(i * 2 + 0.5f, -0.5f, j * 2 + 0.5f);
                }
                else if (levelMap[i, j] == 0)
                {
                    GameObject newObj = Instantiate(floor);
                    newObj.transform.SetParent(transform);
                    newObj.transform.localPosition = new Vector3(i * 2 + 0.5f, -0.5f, j * 2 + 0.5f);
                }
                else if(levelMap[i, j] == 1)
                {
                    GameObject newObj = Instantiate(wall);
                    newObj.transform.SetParent(transform);
                    newObj.transform.localPosition = new Vector3(i * 2 + 0.5f, 2f, j * 2 + 0.5f);

                    GameObject newObj1 = Instantiate(floor);
                    newObj1.transform.SetParent(transform);
                    newObj1.transform.localPosition = new Vector3(i * 2 + 0.5f, -0.5f, j * 2 + 0.5f);
                }
                else if(levelMap[i, j] == 2)
                {
                    GameObject newObj = Instantiate(player);
                    newObj.transform.SetParent(transform);
                    newObj.transform.localPosition = new Vector3(i * 2 + 0.5f, 1f, j * 2 + 0.5f);

                    GameObject newObj1 = Instantiate(floor);
                    newObj1.transform.SetParent(transform);
                    newObj1.transform.localPosition = new Vector3(i * 2 + 0.5f, -0.5f, j * 2 + 0.5f);
                }
                else if (levelMap[i, j] == 3)
                {
                    GameObject newObj = Instantiate(crate);
                    newObj.transform.SetParent(transform);
                    newObj.transform.localPosition = new Vector3(i * 2 + 0.5f, 1f, j + 0.5f);

                    GameObject newObj1 = Instantiate(floor);
                    newObj1.transform.SetParent(transform);
                    newObj1.transform.localPosition = new Vector3(i * 2 + 0.5f, -0.5f, j * 2 + 0.5f);
                }
                else if (levelMap[i, j] == 4)
                {
                    GameObject newObj = Instantiate(caveWall);
                    newObj.transform.SetParent(transform);
                    newObj.transform.localPosition = new Vector3(i * 2 + 0.5f, 2f, j * 2 + 0.5f);

                    GameObject newObj1 = Instantiate(caveFloor);
                    newObj1.transform.SetParent(transform);
                    newObj1.transform.localPosition = new Vector3(i * 2 + 0.5f, -0.5f, j * 2 + 0.5f);
                }
            }
        }
    }

    void AddRooms()
    {
        int treeDepth = (int)Mathf.Ceil(Mathf.Log(roomCount, 2));
        Insert(0, treeDepth);

        int toRemove = (int)Mathf.Pow(2, treeDepth) - roomCount;
        if(toRemove > 0){
            int i = 0;
            while(toRemove > 0)
            {
                if (nodesList[i % nodesList.Count].isLeaf == true && Random.Range(0,2) == 0)
                {
                    nodesList[i % nodesList.Count].isLeaf = false;
                }
                toRemove = CountLeafs() - roomCount;
                i++;
            }
        }

        int counter = 0;
        for (int i = 0; i < nodesList.Count; i++)
        {
            if (nodesList[i].isLeaf == true && counter < roomCount)
            {
                counter++;
                int xabsDiv = nodesList[i].x.max / 20;
                int zabsDiv = nodesList[i].z.max / 20;
                nodesList[i].x.min = nodesList[i].x.min + Random.Range(1, xabsDiv);
                nodesList[i].x.max = nodesList[i].x.max - Random.Range(1, xabsDiv);
                nodesList[i].z.min = nodesList[i].z.min + Random.Range(1, zabsDiv);
                nodesList[i].z.max = nodesList[i].z.max - Random.Range(1, zabsDiv);

                for(int j = nodesList[i].x.min; j < nodesList[i].x.max; j++)
                {
                    for(int k = nodesList[i].z.min; k < nodesList[i].z.max; k++)
                    {
                        levelMap[j, k] = 0;
                    }
                }

                if(counter == 1)
                {
                    int midx = (nodesList[i].x.max - nodesList[i].x.min) / 2 + nodesList[i].x.min;
                    int midz = (nodesList[i].z.max - nodesList[i].z.min) / 2 + nodesList[i].z.min;
                    levelMap[midx , midz] = 2;
                }

                for (int j = nodesList[i].x.min; j < nodesList[i].x.max; j++)
                {
                    levelMap[j, nodesList[i].z.min] = 1;
                    levelMap[j, nodesList[i].z.max - 1] = 1;

                    /*GameObject newObj = Instantiate(wall);
                    newObj.transform.SetParent(transform);
                    newObj.transform.localPosition = new Vector3(j + 0.5f, 1f, nodesList[i].z.min + 0.5f);
                    newObj.GetComponent<Renderer>().material.SetColor("_Color", colorList[counter % colorList.Count]);

                    GameObject newObj1 = Instantiate(wall);
                    newObj1.transform.SetParent(transform);
                    newObj1.transform.localPosition = new Vector3(j + 0.5f, 1f, nodesList[i].z.max - 1 + 0.5f);
                    newObj1.GetComponent<Renderer>().material.SetColor("_Color", colorList[counter % colorList.Count]);*/
                }
                for(int k = nodesList[i].z.min; k < nodesList[i].z.max; k++)
                {
                    levelMap[nodesList[i].x.min, k] = 1;
                    levelMap[nodesList[i].x.max - 1, k] = 1;

                    /*GameObject newObj = Instantiate(wall);
                    newObj.transform.SetParent(transform);
                    newObj.transform.localPosition = new Vector3(nodesList[i].x.min + 0.5f, 1f, k + 0.5f);
                    newObj.GetComponent<Renderer>().material.SetColor("_Color", colorList[counter % colorList.Count]);

                    GameObject newObj1 = Instantiate(wall);
                    newObj1.transform.SetParent(transform);
                    newObj1.transform.localPosition = new Vector3(nodesList[i].x.max - 1 + 0.5f, 1f, k + 0.5f);
                    newObj1.GetComponent<Renderer>().material.SetColor("_Color", colorList[counter % colorList.Count]);*/
                }
            }
        }
    }

    void Insert(int depth, int targetDepth)
    {
        _root = new Node(new Coordinates(0, mapSize), new Coordinates(0, mapSize));
        nodesList.Add(_root);
        Recursion(_root, depth + 1, targetDepth);

    }

    void Recursion(Node root, int depth, int targetDepth)
    {
        int xmin = root.x.min;
        int xmax = root.x.max; 
        int zmin = root.z.min;
        int zmax = root.z.max;

        if (xmax - xmin > zmax - zmin)
        {
            //split horizontally
            int newxmax = ((xmax - xmin) / 2) + xmin + Random.Range((xmax - xmin) / 4 * (-1), (xmax - xmin) / 4);

            root.isLeaf = false;
            root.leftNode = new Node(new Coordinates(xmin, newxmax), new Coordinates(zmin, zmax));
            nodesList.Add(root.leftNode);
            root.rightNode = new Node(new Coordinates(newxmax, xmax), new Coordinates(zmin, zmax));
            nodesList.Add(root.rightNode);
        }
        else
        {
            //split vertically
            int newzmax = ((zmax - zmin) / 2) + zmin + Random.Range((zmax - zmin) / 4 * (-1), (zmax - zmin) / 4); ;

            root.isLeaf = false;
            root.leftNode = new Node(new Coordinates(xmin, xmax), new Coordinates(zmin, newzmax));
            nodesList.Add(root.leftNode);
            root.rightNode = new Node(new Coordinates(xmin, xmax), new Coordinates(newzmax, zmax));
            nodesList.Add(root.rightNode);
        }

        if(depth < targetDepth)
        {
            Recursion(root.leftNode, depth + 1, targetDepth);
            Recursion(root.rightNode, depth + 1, targetDepth);
        }
        
    }

    int CountLeafs()
    {
        int counter = 0;
        for(int i = 0; i < nodesList.Count; i++)
        {
            if (nodesList[i].isLeaf == true)
            {
                counter++;
            }
        }

        return counter;
    }

    void DebugLevelMap()
    {
        for(int i = 0; i < mapSize; i++)
        {
            for(int j = 0; j < mapSize; j++)
            {
                Debug.Log(levelMap[i , j]);
            }
        }
    }

    void DebugNodesList()
    {
        for(int i = 0; i < nodesList.Count; i++)
        {
            Debug.Log(nodesList[i].isLeaf + " " + nodesList[i].x.min + " " +
                nodesList[i].x.max + " " + nodesList[i].z.min + " " + nodesList[i].z.max);
        }
    }

    void InitColorList()
    {
        colorList = new List<Color>()
        {
            new Color( 1f, 0f, 0f, 0.2f),
            new Color( 0f, 1f, 0f, 0.2f),
            new Color( 0f, 0f, 1f, 0.2f),
            new Color( 1f, 1f, 0f, 0.2f),
            new Color( 1f, 0f, 1f, 0.2f),
            new Color( 0f, 1f, 1f, 0.2f),
            new Color( 1f, 1f, 1f, 0.2f),
            new Color( 0f, 0f, 0f, 0.2f),
        };
    }

    void AddCorridors()
    {
        for (int i = nodesList.Count - 1; i > 1; i -= 2)
        {
            Node room1 = nodesList[i];
            Node room2 = nodesList[i - 1];

            int room1x = (room1.x.max - room1.x.min) / 2 + room1.x.min;
            int room2x = (room2.x.max - room2.x.min) / 2 + room2.x.min;
            int room1z = (room1.z.max - room1.z.min) / 2 + room1.z.min;
            int room2z = (room2.z.max - room2.z.min) / 2 + room2.z.min;

            int xdis = Mathf.Abs(room1x - room2x);
            int vdis = Mathf.Abs(room1z - room2z);
            
            if(xdis > vdis)
            {
                for (int j = room2x; j < room1x; j++)
                {
                    if (levelMap[j, room2z] == 1 || levelMap[j, room2z] == -1)
                    {
                        levelMap[j, room2z] = 0;
                    }
                    if (levelMap[j, room2z - 1] == -1)
                    {
                        levelMap[j, room2z - 1] = 1;
                    }
                    if (levelMap[j, room2z + 1] == -1)
                    {
                        levelMap[j, room2z + 1] = 1;
                    }
                }
                if (levelMap[room2x - 1, room1z] == -1)
                {
                    levelMap[room2x - 1, room1z] = 1;
                    levelMap[room2x - 1, room1z + 1] = 1;
                    levelMap[room2x - 1, room1z - 1] = 1;
                    levelMap[room2x, room1z] = 3; 
                }
                if (levelMap[room1x, room1z] == -1)
                {
                    levelMap[room1x, room1z] = 1;
                    levelMap[room1x, room1z + 1] = 1;
                    levelMap[room1x, room1z - 1] = 1;
                    levelMap[room1x - 1, room1z] = 3;
                }
            }
            else
            {
                for (int j = room2z; j < room1z; j++)
                {
                    if (levelMap[room1x, j] == 1 || levelMap[room1x, j] == -1)
                    {
                        levelMap[room1x, j] = 0;
                    }
                    if (levelMap[room1x - 1, j] == -1)
                    {
                        levelMap[room1x - 1, j] = 1;
                    }
                    if (levelMap[room1x + 1, j] == -1)
                    {
                        levelMap[room1x + 1, j] = 1;
                    }
                }
                if (levelMap[room1x, room2z - 1] == -1)
                {
                    levelMap[room1x, room2z - 1] = 1;
                    levelMap[room1x + 1, room2z - 1] = 1;
                    levelMap[room1x - 1, room2z - 1] = 1;
                    levelMap[room1x, room2z] = 3;
                }
                if (levelMap[room1x, room1z] == -1)
                {
                    levelMap[room1x, room1z] = 1;
                    levelMap[room1x - 1, room1z] = 1;
                    levelMap[room1x + 1, room1z] = 1;
                    levelMap[room1x, room1z - 1] = 3;
                }
            }
        }
    }

    void SmoothCaveMap(int target)
    {
        int[,] copyMap = new int[mapSize, mapSize];
        for(int i = 1; i < mapSize - 1; i++)
        {
            for(int j = 1; j < mapSize - 1; j++)
            {
                int numNeighbors = 0;
                if (caveMap[i - 1, j - 1] == target)
                {
                    numNeighbors++;
                }
                if (caveMap[i - 1, j] == target)
                {
                    numNeighbors++;
                }
                if (caveMap[i - 1, j + 1] == target)
                {
                    numNeighbors++;
                }
                if (caveMap[i, j - 1] == target)
                {
                    numNeighbors++;
                }
                if (caveMap[i, j + 1] == target)
                {
                    numNeighbors++;
                }
                if (caveMap[i + 1, j - 1] == target)
                {
                    numNeighbors++;
                }
                if (caveMap[i + 1, j] == target)
                {
                    numNeighbors++;
                }
                if (caveMap[i + 1, j + 1] == target)
                {
                    numNeighbors++;
                }

                if (numNeighbors > neighborThreshold)
                {
                    copyMap[i, j] = target;
                }
                else
                {
                    copyMap[i, j] = -1;
                }
            }
        }

        for(int i = 1; i < mapSize - 1; i++)
        {
            for(int j = 1; j < mapSize -1; j++)
            {
                if (caveMap[i, j] == target)
                {
                    caveMap[i, j] = copyMap[i, j];
                }
            }
        }
    }
}

public class Node
{
    public Node leftNode;
    public Node rightNode;
    public Coordinates x;
    public Coordinates z;
    public bool isLeaf = true;
    public Node( Coordinates x, Coordinates z){
        leftNode = null;
        rightNode = null;

        this.x = x;
        this.z = z;
    }
}

public class Coordinates
{
    public int min;
    public int max;

    public Coordinates(int min, int max)
    {
        this.min = min;
        this.max = max;
    }

}