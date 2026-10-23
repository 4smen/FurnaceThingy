using UnityEngine;

public class HandRotation : MonoBehaviour
{
    [SerializeField] private float positionSmoothTime = 0.08f;
    [SerializeField] private float rotationSmoothTime = 0.1f;

    [SerializeField] private float inertiaAmount = 0.5f;

    void LateUpdate()
    {
        Quaternion targetRotation = Camera.main.transform.rotation;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmoothTime * Time.deltaTime * 15f
        );
    }
}