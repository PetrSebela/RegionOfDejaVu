using UnityEngine;

public class MainCoreShutdown : Interaction
{
    public override void Interact()
    {
        GameManager.Instance.MainCoreShutdown();
        effect?.Play();
    }
}
