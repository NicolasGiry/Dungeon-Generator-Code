using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    Vector2 look;
    public float mouseSensitivity = 3f;
    public float movementSpeed = 2f;
    public Camera cam;
    public Vector3 playerPos;
    public RoomManager roomManager;
    bool canMove;
    public CharacterController characterController;
    public GameObject pin;
    public RawImage miniMap;

    void Update()
    {
        if (canMove)
        {
            UpdateMovement();
            UpdateLook();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitExploration();
            }
        }
    }

    void UpdateMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 input = new Vector3();
        input += transform.right * x;
        input += transform.forward * y;
        input = Vector3.ClampMagnitude(input, 1f);

        characterController.Move(input * movementSpeed * Time.deltaTime);
    }

    void UpdateLook()
    {
        look.x += Input.GetAxis("Mouse X") * mouseSensitivity;
        look.y += Input.GetAxis("Mouse Y") * mouseSensitivity;

        look.y = Mathf.Clamp(look.y, -89f, 89f);

        transform.localRotation = Quaternion.Euler(0, look.x, 0);
        cam.transform.localRotation = Quaternion.Euler(-look.y, 0, 0);
    }

    public void ExploreDungeon()
    {
        Cursor.lockState = CursorLockMode.Locked;
        pin.SetActive(true);
        miniMap.enabled = true;

        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            playerPos = roomManager.rooms[0].GetCenter();
        }

        characterController.Move(playerPos - new Vector3(0, 0.5f, 0) - transform.position);
        transform.position = playerPos;
        cam.transform.position = playerPos - new Vector3(0, 0.5f, 0);
        canMove = true;
    }

    void QuitExploration()
    {
        miniMap.enabled = false;
        pin.SetActive(false);
        canMove = false;
        playerPos = new Vector3(0, 1, 0);
        characterController.Move(playerPos - transform.position);
        cam.transform.position = new Vector3(60, 150, 50);
        cam.transform.rotation = Quaternion.Euler (90, 0, 90);
        Cursor.lockState= CursorLockMode.None;
    }
}
