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

    public int mapSize;
    public int roomCount;

    private int[,] levelMap;

    private Node _root = null;
    private List<Node> nodesList = new();

    public int seed;

    private List<Color> colorList = new();

    // Start is called before the first frame update
    void Start()
    {
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

        AddRooms();

        FillMap();

        //DebugNodesList();
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
                    newObj.transform.localPosition = new Vector3(i + 0.5f, 1f, j + 0.5f);
                }
                else if(levelMap[i, j] == 2)
                {
                    Debug.Log("Player should spawn");
                    GameObject newObj = Instantiate(player);
                    newObj.transform.SetParent(transform);
                    newObj.transform.localPosition = new Vector3(i + 0.5f, 1f, j + 0.5f);
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
                int xabsDiv = (nodesList[i].x.max - nodesList[i].x.min) / 20;
                int zabsDiv = (nodesList[i].z.max - nodesList[i].z.min) / 20;
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
                    Debug.Log("2 has been placed at: " + midx + " " + midz);
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