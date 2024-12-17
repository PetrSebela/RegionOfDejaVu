using UnityEngine;

public class Paralax : MonoBehaviour
{
    [SerializeField] Transform trackedObject;
    [SerializeField] float strength = 0.5f;

    // Update is called once per frame
    void Update()
    {
        transform.position = trackedObject.position * strength;
    }
}
