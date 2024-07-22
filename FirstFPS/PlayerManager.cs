using OpenCover.Framework.Model;
using System;
using UnityEngine;

//cameraOffset相偏移, walk步行, 启动时调用的方法Method called at startup, processing处理
//
[RequireComponent(typeof(UnityEngine.CharacterController))]
public class PlayerManager : MonoBehaviour
{   
    //player walk 
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f; 
    public float jumpPower = 7f; 
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    //Walk,jump Audio
    public AudioClip walkSound; 
    public AudioClip jumpSound;
    //Zoom Camera
    public float zoomSpeed = 2f;
    public float minZoom = 20f;
    public float maxZoom = 60f; 
    public float firstPersonFOV = 60f;
    public float thirdPersonFOV = 90f;
    //Audio source Movement direction View rotation X-axis angle Camera offset
    private AudioSource audioSource; 
    private Vector3 moveDirection = Vector3.zero; 
    private float rotationX = 0; 
    private Vector3 cameraOffset;

    public bool canMove = true; // Can it be moved
    private UnityEngine.CharacterController characterController; // Character Controller
    //Method called at startup
    void Start()
    {   
        //Get character controller component and add Audio Component 
        characterController = GetComponent<UnityEngine.CharacterController>(); 
        audioSource = gameObject.AddComponent<AudioSource>();
        //Lock mouse, visible cursor, Calculating camera offset 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraOffset = playerCamera.transform.position - transform.position;

        Debug.Log("Player1 Success");
        Debug.Log("玩家1 成功加载");        
    }   

    void Update()
    {
        //processing ( )method
        HandleMovement(); 
        HandleJumping(); 
        HandleRotation();
        HandleZoom(); 
    }
    private void HandleMovement()
    {
        //forward direction and Right direction ( Camera )
        Vector3 forward = playerCamera.transform.TransformDirection(Vector3.forward); 
        Vector3 right = playerCamera.transform.TransformDirection(Vector3.right);
        //Keep on level surface保持水平面上
        forward.y = 0;
        right.y = 0;
        //Contact the ground
        forward.Normalize();
        right.Normalize();

        bool isRunning = Input.GetKey(KeyCode.LeftShift); //Detect running
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0; // 当前X轴速度
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0; // 当前Y轴速度
        float movementDirectionY = moveDirection.y; // 移动方向Y轴
        moveDirection = (forward * curSpeedX) + (right * curSpeedY); // 计算移动方向

        // 计算行走声音
        if ((curSpeedX != 0 || curSpeedY != 0) && characterController.isGrounded) // 如果移动且在地面上
        {
            if (!audioSource.isPlaying) // 如果音频源未播放
            {
                audioSource.clip = walkSound; // 设置音频剪辑为行走声音
                audioSource.loop = true; // 设置音频循环播放
                audioSource.Play(); // 播放音频
            }
        }
        else // 如果未移动或不在地面上
        {
            if (audioSource.isPlaying && audioSource.clip == walkSound) // 如果音频源正在播放且音频剪辑为行走声音
            {
                audioSource.Stop(); // 停止播放音频
            }
        }

        moveDirection.y = movementDirectionY; // 保持Y轴方向
        characterController.Move(moveDirection * Time.deltaTime); // 移动角色
    }

    // 处理跳跃
    private void HandleJumping()
    {
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded) // 如果按下跳跃键且可移动且在地面上
        {
            moveDirection.y = jumpPower; // 设置跳跃力度
            PlayJumpSound(); // 播放跳跃声音
        }
        else if (!characterController.isGrounded) // 如果不在地面上
        {
            moveDirection.y -= gravity * Time.deltaTime; // 应用重力
        }
    }

    // 播放跳跃声音
    private void PlayJumpSound()
    {
        if (audioSource.isPlaying) // 如果音频源正在播放
        {
            audioSource.Stop(); // 停止播放音频
        }
        audioSource.clip = jumpSound; // 设置音频剪辑为跳跃声音
        audioSource.loop = false; // 设置音频不循环播放
        audioSource.Play(); // 播放音频
    }

    // 处理视角旋转
    private void HandleRotation()
    {
        if (canMove) // 如果可移动
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed; // 计算X轴旋转角度
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit); // 限制X轴旋转角度
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0); // 设置相机本地旋转
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0); // 设置角色旋转
        }

        // 更新相机位置
        Vector3 newCameraPosition = transform.position + cameraOffset;
        playerCamera.transform.position = Vector3.Slerp(playerCamera.transform.position, newCameraPosition, 0.1f);
    }

    // 处理视角缩放
    private void HandleZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel"); // 获取鼠标滚轮输入
        if (scrollInput != 0) // 如果有滚轮输入
        {
            float newFieldOfView = playerCamera.fieldOfView - scrollInput * zoomSpeed; // 计算新的视野范围
            playerCamera.fieldOfView = Mathf.Clamp(newFieldOfView, firstPersonFOV, thirdPersonFOV); // 限制视野范围
        }
    }
}
