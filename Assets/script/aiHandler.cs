using System.Collections;
using UnityEngine;

public class aiHandler : MonoBehaviour
{
    [SerializeField] carHandler carHandler;
    [SerializeField] LayerMask otherCarLayerMask;
    [SerializeField] MeshCollider meshCollider;

    [SerializeField] AudioSource horn;
    RaycastHit[] raycastHits = new RaycastHit[1];  //collision detection
    bool isCarAhead = false;
    float carAheadDistance = 0;

    int drivingInLane = 0;  //lanes

    WaitForSeconds wait = new WaitForSeconds(.2f);
    void Awake()
    {
        if (CompareTag("Player"))
        {
            Destroy(this);
            return;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(updateLessOften());
    }

    // Update is called once per frame
    void Update()
    {
        float accelerationInput = 1;
        float steerInput = 0;

        if (isCarAhead)
        {
            accelerationInput = -1;
            if (carAheadDistance < 10 && !horn.isPlaying)
            {
                horn.pitch = Random.Range(.5f, 1.1f);
                horn.Play();
            }
        }

        float desiredPositionX = utils.CarLane[drivingInLane];
        float difference = desiredPositionX - transform.position.x;

        if (Mathf.Abs(difference) > .05f)
        {
            steerInput = 1 * difference;
        }
        steerInput = Mathf.Clamp(steerInput, -1, 1);
        carHandler.setInput(new Vector2(steerInput, accelerationInput));
    }

    IEnumerator updateLessOften()
    {
        while (true)
        {
            isCarAhead=checkIfOtherCarsIsAhead();
            yield return wait;
        }
    }

    bool checkIfOtherCarsIsAhead()
    {
        meshCollider.enabled = false;
        int numberOfHits = Physics.BoxCastNonAlloc(transform.position, Vector3.one * .25f, transform.forward, raycastHits, Quaternion.identity, 2, otherCarLayerMask);
        meshCollider.enabled = true;

        if (numberOfHits > 0)
        {
            carAheadDistance = (transform.position - raycastHits[0].point).magnitude;
            return true;
        }
        return false;
    }

    void OnEnable()
    {
        carHandler.setMaxSpeed(Random.Range(2, 4));   //set a random speed

        //set a random lane
        drivingInLane = Random.Range(0, utils.CarLane.Length);
    }
}
