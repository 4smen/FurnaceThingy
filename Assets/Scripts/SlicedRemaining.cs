using UnityEngine;

public class SlicedRemaining : MonoBehaviour
{
    public void Decay()
    {
        Destroy(gameObject, 5f);
    }
}
