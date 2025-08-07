using UnityEngine;
using UnityEngine.SceneManagement;

public class inputHandler : MonoBehaviour
{
    [SerializeField]
    carHandler carHandler;

    void Awake()
    {
        if (!CompareTag("Player"))
        {
            Destroy(this);
            return;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = Vector2.zero;
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");
        carHandler.setInput(input);

        if (Input.GetKeyDown(KeyCode.R))
        {
            //restore time scale
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
