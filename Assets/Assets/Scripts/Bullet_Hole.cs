using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Hole : MonoBehaviour
{
    [SerializeField]
    private float lifetime;

    private float readyToDie = 0f;
    void Start()
    {
        lifetime = lifetime + Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(readyToDie >= lifetime)
        {
            kill();
        }

        readyToDie = Time.time + 1f / lifetime;
    }

    private void kill()
    {
        Destroy(gameObject);
    }
}
