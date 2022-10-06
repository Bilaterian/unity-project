using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targetable : MonoBehaviour
{
    [SerializeField]
    private int health;

    public void getHit(int damageValue)
    {
        health = health - damageValue;

        if(health <= 0)
        {
            kill();
        }
    }

    private void kill()
    {
        Destroy(gameObject);
    }
}
