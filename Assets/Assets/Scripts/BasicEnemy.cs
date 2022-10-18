using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField]
    private CharacterController controller;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, detectionSphereSize, playerMask);

        if (playerInSight)
        {
            //go to player
            GetComponent<NavMeshAgent>().destination = playerTransform.position;
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
        controller.Move(velocity * Time.deltaTime);
    }
}
