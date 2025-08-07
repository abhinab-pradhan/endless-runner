using UnityEngine;

public class carPartHandler : MonoBehaviour
{
    AudioSource bounceAS;
    void Awake()
    {
        bounceAS = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!bounceAS.isPlaying)
        {
            bounceAS.pitch = collision.relativeVelocity.magnitude * .5f;
            bounceAS.pitch = Mathf.Clamp(bounceAS.pitch, .5f, 1);
            bounceAS.Play();
        }
    }
}
