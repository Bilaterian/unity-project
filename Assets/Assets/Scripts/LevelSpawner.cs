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
    
    private int choice = 0;

    public int seed;

    private List<Color> colorList = new List<Color>();

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
                levelMap[i, j] = 0;
            }
        }

        AddRooms();

        //FillMap();

        DebugNodesList();
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
            }
        }
    }

    void AddRooms()
    {
        int treeDepth = (int)Mathf.Ceil(Mathf.Log(roomCount, 2));
        Insert(0, treeDepth);

        int counter = 0;
        for (int i = 0; i < nodesList.Count; i++)
        {
            if (nodesList[i].isLeaf == true && counter < roomCount)
            {
                counter++;
                /*for(int j = nodesList[i].x.min; j < nodesList[i].x.max; j++)
                {
                    for(int k = nodesList[i].z.min; k < nodesList[i].z.max; k++)
                    {
                        levelMap[j, k] = 0;
                    }
                }*/
                for(int j = nodesList[i].x.min; j < nodesList[i].x.max; j++)
                {
                    levelMap[j, nodesList[i].z.min] = 1;
                    levelMap[j, nodesList[i].z.max - 1] = 1;

                    GameObject newObj = Instantiate(wall);
                    newObj.transform.SetParent(transform);
                    newObj.transform.localPosition = new Vector3(j + 0.5f, 1f, nodesList[i].z.min + 0.5f);
                    newObj.GetComponent<Renderer>().material.SetColor("_Color", colorList[counter % colorList.Count]);

                    GameObject newObj1 = Instantiate(wall);
                    newObj1.transform.SetParent(transform);
                    newObj1.transform.localPosition = new Vector3(j + 0.5f, 1f, nodesList[i].z.max - 1 + 0.5f);
                    newObj1.GetComponent<Renderer>().material.SetColor("_Color", colorList[counter % colorList.Count]);
                }
                for(int k = nodesList[i].z.min; k < nodesList[i].z.max; k++)
                {
                    levelMap[nodesList[i].x.min, k] = 1;
                    levelMap[nodesList[i].x.max - 1, k] = 1;

                    GameObject newObj = Instantiate(wall);
                    newObj.transform.SetParent(transform);
                    newObj.transform.localPosition = new Vector3(nodesList[i].x.min + 0.5f, 1f, k + 0.5f);
                    newObj.GetComponent<Renderer>().material.SetColor("_Color", colorList[counter % colorList.Count]);

                    GameObject newObj1 = Instantiate(wall);
                    newObj1.transform.SetParent(transform);
                    newObj1.transform.localPosition = new Vector3(nodesList[i].x.max - 1 + 0.5f, 1f, k + 0.5f);
                    newObj1.GetComponent<Renderer>().material.SetColor("_Color", colorList[counter % colorList.Count]);
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

        if (choice == 0)
        {
            choice = 1;
            //split horizontally
            int newxmax = ((xmax - xmin) / 2) + xmin + Random.Range(targetDepth * (-1), targetDepth);

            root.isLeaf = false;
            root.leftNode = new Node(new Coordinates(xmin, newxmax), new Coordinates(zmin, zmax));
            nodesList.Add(root.leftNode);
            root.rightNode = new Node(new Coordinates(newxmax + 1, xmax), new Coordinates(zmin, zmax));
            nodesList.Add(root.rightNode);
        }
        else
        {
            choice = 0;
            //split vertically
            int newzmax = ((zmax - zmin) / 2) + zmin + Random.Range(targetDepth * (-1), targetDepth); ;

            root.isLeaf = false;
            root.leftNode = new Node(new Coordinates(xmin, xmax), new Coordinates(zmin, newzmax));
            nodesList.Add(root.leftNode);
            root.rightNode = new Node(new Coordinates(xmin, xmax), new Coordinates(newzmax + 1, zmax));
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