using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
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
    private int numLeafs = 0;

    private List<Node> nodesList = new();
    private int choice = 0;

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
                    newObj.transform.localPosition = new Vector3(i * 2 + 0.5f, 1f, j * 2 + 0.5f);
                }
            }
        }
    }

    void AddRooms()
    {
        while (numLeafs < roomCount)
        {
            int data = Random.Range(0,100);

            Insert(data);
            numLeafs = CountLeafs();
        }

        for(int i = 0; i < nodesList.Count; i++)
        {
            if (nodesList[i].isLeaf == true)
            {
                for(int j = nodesList[i].x.min; j < nodesList[i].x.max; j++)
                {
                    for(int k = nodesList[i].z.min; k < nodesList[i].z.max; k++)
                    {
                        levelMap[j, k] = 0;
                    }
                }
            }
        }
    }

    void Insert(int data)
    {
        if(_root == null)
        {
            _root = new Node(data, new Coordinates(0, mapSize), new Coordinates(0, mapSize));
            nodesList.Add(_root);
        }
        else
        {
            Recursion(_root, data);
        }
    }

    void Recursion(Node root, int data)
    {
        //decide how to split first
        
        if(choice == 0)
        {
            choice = 1;
        }
        else
        {
            choice = 0;
        }

        int xmin = root.x.min;
        int xmax = root.x.max; 
        int zmin = root.z.min;
        int zmax = root.z.max;


        if (data < root.data)
        {
            if (root.leftNode == null)
            {
                if (choice == 0)
                {
                    //split horizontally
                    xmax = ((xmax - xmin) / 2) + xmin;
                }
                else
                {
                    //split vertically
                    zmax = ((zmax - zmin) / 2) + zmin;
                }
                root.isLeaf = false;
                root.leftNode = new Node(data, new Coordinates(xmin, xmax), new Coordinates(zmin, zmax));
                nodesList.Add(root.leftNode);
            }
            else
            {
                Recursion(root.leftNode, data);
            }
        }
        else
        {
            if (root.rightNode == null)
            {
                if (choice == 0)
                {
                    //split horizontally
                    xmin = ((xmax - xmin) / 2) + xmin;
                }
                else
                {
                    //split vertically
                    zmin = ((zmax - zmin) / 2) + zmin;
                }
                root.isLeaf = false;
                root.rightNode = new Node(data, new Coordinates(xmin, xmax), new Coordinates(zmin, zmax));
                nodesList.Add(root.rightNode);
            }
            else
            {
                Recursion(root.rightNode, data);
            }
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
            Debug.Log(nodesList[i].data + " " + nodesList[i].isLeaf + " " + nodesList[i].x.min + " " +
                nodesList[i].x.max + " " + nodesList[i].z.min + " " + nodesList[i].z.max);
        }
    }
}

public class Node
{
    public Node leftNode;
    public Node rightNode;
    public int data;
    public Coordinates x;
    public Coordinates z;
    public bool isLeaf = true;
    public Node(int data, Coordinates x, Coordinates z){
        leftNode = null;
        rightNode = null;
        this.data = data;

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