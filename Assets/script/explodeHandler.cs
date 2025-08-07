using UnityEngine;

public class explodeHandler : MonoBehaviour
{
    [SerializeField] GameObject originalGameobject;
    [SerializeField] GameObject model;
    Rigidbody[] rigidBody;

    void Awake()
    {
        rigidBody = model.GetComponentsInChildren<Rigidbody>(true);
    }
    void Start()
    {
        //explode(Vector3.forward);
    }

    public void explode(Vector3 externalForce)
    {
        originalGameobject.SetActive(false);

        foreach (Rigidbody rb in rigidBody)
        {
            rb.transform.parent = null;
            rb.GetComponent<MeshCollider>().enabled = true;
            rb.gameObject.SetActive(true);
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            rb.AddForce(Vector3.up * 200 + externalForce, ForceMode.Force);
            rb.AddTorque(Random.insideUnitSphere * .5f, ForceMode.Impulse);

            //change the tag so other objects can explode after being hit by a carpart
            rb.gameObject.tag = "carPart";
        }
    }

}
