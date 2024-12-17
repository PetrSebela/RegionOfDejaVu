using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private Vector3 _offset;
    [SerializeField] private Transform _followedObject;
    [SerializeField] private float _smoothingFactor;

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Slerp(transform.position, _followedObject.position + _offset, _smoothingFactor * Time.deltaTime);
    }
}
