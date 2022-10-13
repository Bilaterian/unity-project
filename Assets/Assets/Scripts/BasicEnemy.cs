using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicEnemy : MonoBehaviour
{
    [Serializable]
    public struct PlayerStats
    {
        public int hitPoints;
    }

    [Serializable]
    public struct MovementStats
    {
        public float forwardSpeed;
        public float backSpeed;
        public float strafeSpeed;
        public float jumpHeight;

        public float sprintFactor;
        public float gravity;
    }

    [SerializeField]
    private PlayerStats stats;

    [SerializeField]
    private MovementStats moveStats;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
