using UnityEngine;

public class LaunchExperiment : Interaction
{
    public override void Interact()
    {
       GameManager.Instance.LaunchExperiment();
    }
}
