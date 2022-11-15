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

        if(offset.x == -1)
        {
            newObj.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }
        else if(offset.z == -1)
        {
            newObj.transform.localRotation = Quaternion.Euler(0, 90, 0);
        }
        else if (offset.z == 1)
        {
            newObj.transform.localRotation = Quaternion.Euler(0, 270, 0);
        }
    }
}
