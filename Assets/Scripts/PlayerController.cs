using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static bool IsLive = true;
    public static int Score = 0;

    public float moveSpeed = 5;
    public float sprintSpeed = 1.5f;
    public float rotateSpeed = 2;
    public float rotateYSpeed = 2;
    public float jumpForce = 10;

    public Vector2 rotateClamp;
    public Vector2 lookClamp;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float checkRadiuss = 0.5f;

    public Animator jammoAnim;

    [Header("Canvas")]
    public GameObject menuPanel;
    public TMP_Text scoreText;
    public TMP_InputField nickField;
    public TMP_Text topText;

    float rotateY = 0;
    float rotateX = 0;
    Rigidbody rigi;
    Vector3 input = Vector3.zero;
    bool isSprint = false;

    Animator anim;
    float scoreTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        rigi = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        menuPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;

        ReadScores();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLive)
        {
            scoreText.text = " ";
            input = Vector3.zero;
            return;
        }
        scoreTime += Time.deltaTime;
        Score = (int)scoreTime;

        scoreText.text = Score.ToString();

        bool isGround = IsGround();

        if (isGround)
        {
            if (Input.GetButtonDown("Jump"))
            {
                rigi.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
                jammoAnim.SetBool("Jump", true);
            } 
            else
            {
                if (rigi.velocity.y <= 0)
                {
                    jammoAnim.SetBool("Jump", false);
                }
            }
        }

        anim.SetBool("IsSlide", Input.GetKey(KeyCode.S) && isGround);
        jammoAnim.SetBool("Slide", Input.GetKey(KeyCode.S) && isGround);

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
        pos.y = 3.46f;
        Camera.main.transform.position = pos;
    }
    bool IsGround()
    {
        Vector3 pos = groundCheck.position;

        return Physics.OverlapSphere(pos, checkRadiuss, groundLayer).Length > 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(groundCheck.position, checkRadiuss);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            Score += 2;
            Destroy(collision.gameObject,0.7f);
            ParticleSystem particle = collision.gameObject.GetComponentInChildren<ParticleSystem>(true);
            if (particle)
                particle.gameObject.SetActive(true);

            return;
        }

        if (!collision.gameObject.CompareTag("Obstacle"))
            return;

        IsLive = false;
        scoreTime = 0;
        menuPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

     }

    public void RestartLevel()
    {
        IsLive = true;
        SceneManager.LoadScene(0);
    }

    class SaveClass
    {
        public string name;
        public int score;
    }

    List<SaveClass> saves = new List<SaveClass>();
    void ReadScores()
    {
        saves.Clear();
        int count = PlayerPrefs.GetInt("Count", 0);
        if (count < 1)
            return;

        for (int i = 0; i < count; i++)
        {
            string name = PlayerPrefs.GetString(i.ToString() + "_name");
            int score = PlayerPrefs.GetInt(i.ToString() + "_score");

            saves.Add(new SaveClass() { name = name, score = score });
        }

        saves.Sort((score1, score) => score.score.CompareTo(score1.score));

        ShowTop();
    }

    void SaveScore()
    {
        if (saves.Count < 10)
        {
            saves.Add(new SaveClass() { name = nickField.text, score = Score });
        }
        else
        {
            int scr = Score;
            SaveClass sve = null;
            foreach (SaveClass save in saves)
            {
                if (save.score < scr)
                {
                    sve = save;
                    scr = save.score;
                }
            }

            if (sve == null)
                return;

            sve.name = nickField.text;
            sve.score = Score;
        }

        saves.Sort((score1, score) => score.score.CompareTo(score1.score));


        PlayerPrefs.DeleteAll();

        for (int i = 0; i < saves.Count; i++)
        {
            SaveClass sve = saves[i];
            PlayerPrefs.SetString(i.ToString() + "_name", sve.name);
            PlayerPrefs.SetInt(i.ToString() + "_score", sve.score);
        }

        PlayerPrefs.SetInt("Count", saves.Count);

        ShowTop();
    }

    public void ButtonSave()
    {
        SaveScore();
    }

    void ShowTop()
    {
        topText.text = "";
        for (int i = 0; i < saves.Count; i++)
        {
            SaveClass save = saves[i];

            string txt = string.Format("{0}. {1}: {2} \n", i + 1, save.name, save.score);

            topText.text = topText.text + txt;
        }
    }
}
