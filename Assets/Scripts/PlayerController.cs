using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static bool IsLive = true;

    public float moveSpeed = 5;
    public float sprintSpeed = 1.5f;
    public float rotateSpeed = 2;
    public float rotateYSpeed = 2;
    public float jumpForce = 10;

    public Vector2 rotateClamp;
    public Vector2 lookClamp;
    public LayerMask groundLayer;

    public GameObject menuPanel;

    float rotateY = 0;
    float rotateX = 0;
    Rigidbody rigi;
    Vector3 input = Vector3.zero;
    bool isJumping = false;
    bool isSprint = false;

    // Start is called before the first frame update
    void Start()
    {
        rigi = GetComponent<Rigidbody>();

        menuPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLive)
        {
            input = Vector3.zero;
            return;
        }

        if (isJumping && IsGround())
        {
            isJumping = false;
        }

        if (!isJumping && Input.GetButtonDown("Jump"))
        {
            rigi.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
            isJumping = true;
        }

        input.x = Input.GetAxis("Horizontal");
        //input.z = Input.GetAxis("Vertical");
        isSprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        //rotateX += Input.GetAxis("Mouse X") * rotateSpeed;
        rotateY += Input.GetAxis("Mouse Y") * rotateYSpeed;


        rotateY = Mathf.Clamp(rotateY, rotateClamp.x, rotateClamp.y);
        rotateX = Mathf.Clamp(rotateX, lookClamp.x, lookClamp.y);

        Camera.main.transform.localEulerAngles = new Vector3(-rotateY, 0, 0);



        transform.localEulerAngles = new Vector3(0, rotateX, 0);

    }
    void FixedUpdate()
    {
        Vector3 velocity = input * moveSpeed * (isSprint ? sprintSpeed : 1);
        velocity.y = rigi.velocity.y;
        rigi.velocity = velocity;
    }

    private void LateUpdate()
    {
        Vector3 pos = Camera.main.transform.position;
        pos.y = 3;
        Camera.main.transform.position = pos;
    }
    bool IsGround()
    {
        Vector3 pos = transform.position;
        pos.y += 0.5f;

        Ray ray = new Ray(pos, new Vector3(0, -1, 0));

        return Physics.SphereCast(ray, 0.5f, 0.01f, groundLayer);

    }

    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        pos.y += 0.5f;
        Gizmos.DrawSphere(pos, 0.5f);
        pos.y -= 0.05f;
        Gizmos.DrawSphere(pos, 0.5f);

    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Obstacle"))
            return;

        IsLive = false;
        menuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void RestartLevel()
    {
        IsLive = true;
        SceneManager.LoadScene(0);
    }
}
