using UnityEngine;

public class randomizedObject : MonoBehaviour
{
    [SerializeField] Vector3 localRotationMin = Vector3.zero;
    [SerializeField] Vector3 loaclRotationMax = Vector3.zero;
    [SerializeField] float localScaleMultiplierMin = .8f;
    [SerializeField] float localScaleMultiplierMax = 1.5f;

    Vector3 localScaleOriginal = Vector3.one;
    void Start()
    {
        localScaleOriginal = transform.localScale;
    }

    void OnEnable()
    {
        transform.localRotation = Quaternion.Euler(Random.Range(localRotationMin.x, loaclRotationMax.x), Random.Range(localRotationMin.y, loaclRotationMax.y), Random.Range(localRotationMin.z, loaclRotationMax.z));
        transform.localScale = localScaleOriginal * Random.Range(localScaleMultiplierMin, localScaleMultiplierMax);
    }



}
