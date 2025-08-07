using UnityEngine;

public class lockCameraRotation : MonoBehaviour
{
    public Vector3 fixedEulerAngles = new Vector3(20, 0, 0);
    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(fixedEulerAngles);
    }
}
