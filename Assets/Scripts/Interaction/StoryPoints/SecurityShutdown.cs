using UnityEngine;

public class SecurityShutdown : Interaction
{
    public override void Interact()
    {
        GameManager.Instance.SecurityShutdown();
        effect?.Play();
    }
}
