using System.Media;
using UnityEngine;


public class CameraController : MonoBehaviour
{
  

    [SerializeField] Transform followTarget;
                         // 旋转速度和 距离
    [SerializeField] float rotationSpeed = 2f;
    [SerializeField] float distance = 5;
                             // 缩放速度 和 缩小距离 和 缩大距离 
    [SerializeField] float zoomSpeed = 2f; // 缩放速度
    [SerializeField] float minDistance = 2f; // 最小距离
    [SerializeField] float maxDistance = 10f; // 最大距离
             // 垂直角 小和大
    [SerializeField] float minVerticalAngle = -45f;
    [SerializeField] float maxVerticalAngle = 45;

    [SerializeField] Vector2 framingOffset;

    [SerializeField] bool invertX;
    [SerializeField] bool invertY;
    
    float rotationX;
    float rotationY;

    float invertXal;
    float invertYal;
    private int invertXVal;
    private int invertYVal;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }



    private void Update()
    {
        invertXVal = (invertX) ? -1 : 1;
        invertYVal = (invertY) ? -1 : 1;

        rotationX += Input.GetAxis("Mouse Y") * invertYVal * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        rotationY += Input.GetAxis("Mouse X") * invertXVal * rotationSpeed;

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        var focusPostion = followTarget.position + new Vector3(framingOffset.x,framingOffset.y);

        
        // 获取鼠标滚轮输入并调整距离
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        distance -= scrollInput * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);


        transform.position = focusPostion - targetRotation * new Vector3(0, 0, distance);
        transform.rotation = targetRotation;

    }
        public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);
}
