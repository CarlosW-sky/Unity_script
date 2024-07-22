using UnityEngine;



[RequireComponent(typeof(UnityEngine.CharacterController))]

public class PlayerManager : MonoBehaviour
{   
    // 玩家相机 和 步行；
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f; 
    public float jumpPower = 7f; 
    public float gravity = 10f;
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;
    // 行走声音v 跳跃声音
    public AudioClip walkSound; 
    public AudioClip jumpSound;
    // 缩放速度 最小视野范围 最大视野范围 第一人称视野 第三人称视野
    public float zoomSpeed = 2f;
    public float minZoom = 20f;
    public float maxZoom = 60f; 
    public float firstPersonFOV = 60f;
    public float thirdPersonFOV = 90f;
    // 音频源 移动方向 视角旋转X轴角度 相机偏移
    private AudioSource audioSource; 
    private Vector3 moveDirection = Vector3.zero; 
    private float rotationX = 0; 
    private Vector3 cameraOffset;

    public bool canMove = true; // 是否可以移动
    private UnityEngine.CharacterController characterController; // 角色控制器
    // 启动时调用的方法
    void Start()
    {   // MY WORLD 1
        characterController = GetComponent<UnityEngine.CharacterController>(); // 获取角色控制器组件
        audioSource = gameObject.AddComponent<AudioSource>(); // 添加音频源组件
        Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标
        Cursor.visible = false; // 隐藏鼠标光标
        cameraOffset = playerCamera.transform.position - transform.position; // 计算相机偏移

        //  MY WORLD 2
        // 输出 "Hello, World!" 到控制台
        Debug.Log("Player1 Success");
        Debug.Log(" 中文简体:》》 玩家1 成功加载");
        /*
        Console.WriteLine("hello");
        Console.WriteLine("helloworld");
        Console.Error.WriteLine("hello");
        Console.Error.WriteLine("hellowrld");   */
        //输入-输出，脚本 Player2;      
    }   

    void Update() // 每帧调用一次的方法
    {
        HandleMovement(); // 调用处理移动方法
        HandleJumping(); // 调用处理跳跃方法
        HandleRotation(); // 调用处理旋转方法
        HandleZoom(); // 调用处理缩放方法
    }
    // 处理角色移动
    private void HandleMovement()
    {
        Vector3 forward = playerCamera.transform.TransformDirection(Vector3.forward); // 前方向（相机）
        Vector3 right = playerCamera.transform.TransformDirection(Vector3.right); // 右方向（相机）

        forward.y = 0; // 保持在水平面上
        right.y = 0; // 保持在水平面上
          //接触地面
        forward.Normalize();
        right.Normalize();

        bool isRunning = Input.GetKey(KeyCode.LeftShift); // 检测跑步
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
