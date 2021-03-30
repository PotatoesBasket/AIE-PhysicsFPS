using UnityEngine;

public class PlayerTargeter : MonoBehaviour
{
    public delegate void TargetFound();
    public event TargetFound OnTargetFound;

    public delegate void TargetLost();
    public event TargetLost OnTargetLost;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            OnTargetFound?.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            OnTargetLost?.Invoke();
    }
}