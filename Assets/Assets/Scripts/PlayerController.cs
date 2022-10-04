using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Serializable]
    public struct PlayerStats
    {
        public int hitPoints;
    }

    [Serializable]
    public struct CameraMovement
    {
        public float mouseSensitivity;
        public float minClamp;
        public float maxClamp;
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
    private CameraMovement playerCam;

    [SerializeField]
    private MovementStats moveStats;

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

    // Start is called before the first frame update
    void Start(){
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update(){
        float cameraX = Input.GetAxis("Mouse X") * playerCam.mouseSensitivity * Time.deltaTime;
        float cameraY = Input.GetAxis("Mouse Y") * playerCam.mouseSensitivity * Time.deltaTime;

        xRotation -= cameraY;
        xRotation = Mathf.Clamp(xRotation, playerCam.minClamp, playerCam.maxClamp);

        this.gameObject.transform.GetChild(0).localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        this.gameObject.transform.Rotate(new Vector3(0, cameraX, 0));

        float positionX = Input.GetAxis("Horizontal") * Time.deltaTime;
        float positionZ = Input.GetAxis("Vertical") * Time.deltaTime * moveStats.strafeSpeed;

        if(positionX > 0.5)
        {
            positionX = positionX * moveStats.forwardSpeed;
        }
        else if(positionX < 0.5)
        {
            positionX = positionX * moveStats.backSpeed;
        }

        Vector3 move = transform.right * positionX + transform.forward * positionZ;
        controller.Move(move);

        isGrounded = Physics.CheckSphere(groundCheck.position, checkSphereSize, groundMask);

        if(isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(moveStats.jumpHeight * -2 * moveStats.gravity);
        }

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y = velocity.y + moveStats.gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}

