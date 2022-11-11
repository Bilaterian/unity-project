using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLight : MonoBehaviour
{
    public GameObject toSpawn;
    public void SpawnObject(Vector3 offset)
    {
        GameObject newObj = Instantiate(toSpawn);
        newObj.transform.SetParent(transform);
        newObj.transform.localPosition = offset;
    }
}
