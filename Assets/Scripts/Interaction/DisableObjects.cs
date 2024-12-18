using UnityEngine;

public class DisableObjects : Interaction
{
    [SerializeField] GameObject[] _disabledObject;
    [SerializeField] GameObject[] _enabledObject;

    public override void Interact()
    {
        foreach(GameObject go in _disabledObject)
            go?.SetActive(false);
    
        foreach(GameObject go in _enabledObject)
            go?.SetActive(true);
    }
}
