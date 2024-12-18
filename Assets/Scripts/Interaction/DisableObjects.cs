using UnityEngine;

public class DisableObjects : Interaction
{
    [SerializeField] GameObject _disabledObject;
    [SerializeField] GameObject _enabledObject;

    public override void Interact()
    {
        _disabledObject?.SetActive(false);
        _enabledObject?.SetActive(true);
    }
}
