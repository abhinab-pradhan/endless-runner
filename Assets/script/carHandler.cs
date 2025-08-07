//using System.Numerics;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class carHandler : MonoBehaviour
{

    [Header("sfx")]
    [SerializeField] AudioSource carEngine;
    [SerializeField] AudioSource carSkid;
    [SerializeField] AudioSource carCrash;
    [SerializeField] AnimationCurve carPitchAnimationCurve;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform gameModel;
    [SerializeField] explodeHandler explodeHandler;
    bool isExploded = false;
    bool isPlayer = true;

    float carMaxSpeed=0;

    float startPositionZ;
    float distanceTravelled = 0;
    public float DistanceTravelled => distanceTravelled;

    //events
    public event Action<carHandler> onPlayerCrashed;


    float accelerationMultiplier = 3;
    float breakMultiplier = 15;
    float steeringMutiplier = 5;

    float maxSteerVelocity = 2;
    float maxForwardVelocity = 30;
    Vector2 input = Vector2.zero;
    void Start()
    {
        isPlayer = CompareTag("Player");

        if (isPlayer)
        {
            carEngine.Play();
        }

        startPositionZ = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        if (isExploded)
        {
            fadeOutCarAudio();
            return;
        }
        //rotate model while turning
        gameModel.transform.rotation = Quaternion.Euler(0, rb.linearVelocity.x * 5, 0);

        updateCarAudio();

        //Update distanceTravelled
        distanceTravelled = transform.position.z - startPositionZ;
    }
    void FixedUpdate()
    {

        //is explode
        if (isExploded)
        {
            //apply drag
            rb.linearDamping = rb.linearVelocity.z * .1f;
            rb.linearDamping = Mathf.Clamp(rb.linearDamping, 1.5f, 10);

            //move towards after the car ahs exploded
            rb.MovePosition(Vector3.Lerp(transform.position, new Vector3(0, 0, transform.position.z), Time.deltaTime * .5f));
            return;
        }
        //apply acceleration
        if (input.y > 0)
        {
            Accelerate();
        }
        else
        {
            rb.linearDamping = 0.2f;
        }

        //apply breaks
        if (input.y < 0)
        {
            Break();
        }

        steer();

        //force the car not to go backward
        if (rb.linearVelocity.z <= 0)
            rb.linearVelocity = Vector3.zero;
    }

    void Accelerate()
    {
        rb.linearDamping = 0;

        //stay within the velocity
        if (rb.linearVelocity.z >= maxForwardVelocity)
            return;


        rb.AddForce(rb.transform.forward * 10 * accelerationMultiplier * input.y);
    }

    void Break()
    {
        if (rb.linearVelocity.z < 0)
        {
            return;
        }
        rb.AddForce(rb.transform.forward * breakMultiplier * input.y);
    }

    void steer()
    {
        if (Mathf.Abs(input.x) > 0)
        {
            //move car sideways
            float speedBaseSteerLimit = rb.linearVelocity.z / 5;
            speedBaseSteerLimit = Mathf.Clamp01(speedBaseSteerLimit);
            rb.AddForce(rb.transform.right * steeringMutiplier * input.x * speedBaseSteerLimit);

            //normalize the x velocity
            float normalizedX = rb.linearVelocity.x / maxSteerVelocity;

            //Ensure that we dont alow it to get bigger than 1 in magnitude
            normalizedX = Mathf.Clamp(normalizedX, -1, 1);
        }
        else
        {
            //auto center car
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, new Vector3(0, 0, rb.linearVelocity.z), Time.fixedDeltaTime * 3);
        }
    }

    void updateCarAudio()
    {
        if (!isPlayer)
            return;

         carMaxSpeed = rb.linearVelocity.z / maxForwardVelocity;
        carEngine.pitch = carPitchAnimationCurve.Evaluate(carMaxSpeed);

        if (input.y < 0 && carMaxSpeed > .2f)
        {
            if (!carSkid.isPlaying)
            {
                carSkid.Play();
            }
            carSkid.volume = Mathf.Lerp(carSkid.volume, 1, Time.deltaTime * 10);
        }
        else
        {
            carSkid.volume = Mathf.Lerp(carSkid.volume, 0, Time.deltaTime * 30);
        }
    }

    void fadeOutCarAudio()
    {
        if (!isPlayer)
            return;

        carEngine.volume = Mathf.Lerp(carEngine.volume, 0, Time.deltaTime * 10);
        carSkid.volume = Mathf.Lerp(carSkid.volume, 0, Time.deltaTime * 10);
    }

    public void setInput(Vector2 inputVector)
    {
        inputVector.Normalize();
        input = inputVector;
    }

    public void setMaxSpeed(float newMaxSpeed)
    {
        maxForwardVelocity = newMaxSpeed;
    }

    IEnumerator slowDownTime()
    {
        while (Time.timeScale > .2f)
        {
            Time.timeScale -= Time.deltaTime * 2;
            yield return null;
        }
        yield return new WaitForSeconds(.5f);

        while (Time.timeScale <= 1)
        {
            Time.timeScale += Time.deltaTime;
            yield return null;
        }
        Time.timeScale = 1;
    }

    void OnCollisionEnter(Collision collision)
    {

        if (!isPlayer)  //ai car will explode only when it collide with player or a car part
        {
            if (collision.transform.root.CompareTag("Untagged"))
            {
                return;
            }
            if (collision.transform.root.CompareTag("carAI"))
            {
                return;
            }
        }
        //Debug.Log("hit " + collision.collider.name);
        Vector3 velocity = rb.linearVelocity;
        explodeHandler.explode(velocity * 40);
        isExploded = true;

        carCrash.volume = carMaxSpeed;
        carCrash.volume = Mathf.Clamp(carCrash.volume, .25f, 1);

        carCrash.pitch = carMaxSpeed;
        carCrash.pitch = Mathf.Clamp(carCrash.pitch, .3f,1);
        carCrash.Play();

        onPlayerCrashed?.Invoke(this);



        StartCoroutine(slowDownTime());
    }
}
