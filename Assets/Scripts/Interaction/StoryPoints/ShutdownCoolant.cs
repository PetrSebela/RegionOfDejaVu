using UnityEngine;

public class ShutdownCoolant : Interaction
{
    public override void Interact()
    {
       GameManager.Instance.ShutdownCryoCoolant();
    }
}
