using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class BasicEnemy : MonoBehaviour
{
    [Serializable]
    public struct EnemyStats
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
    private EnemyStats enemyStats;
    [SerializeField]
    private MovementStats enemyMoveStats;

    private float xRotation = 0f;
    private Vector3 velocity;

    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float checkSphereSize;

    public LayerMask groundMask;
    private bool isGrounded;

    [SerializeField]
    private float detectionSphereSize;

    public LayerMask playerMask;
    private bool playerInSight;
    public Transform playerTransform;
    private Transform lastSeenTransform;
    private NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, detectionSphereSize, playerMask);
        
        if (playerInSight)
        {
            //go to player
            agent.destination = playerTransform.position;
        }
        else
        {
            //wander around
        }

        isGrounded = Physics.CheckSphere(groundCheck.position, checkSphereSize, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y = velocity.y + enemyMoveStats.gravity * Time.deltaTime;
        agent.Move(velocity * Time.deltaTime);
    }
}
