using UnityEngine;

public class ButtonInteraction : MonoBehaviour
{
    [SerializeField] GameObject hint;

    void Start()
    {
        LeanTween.alpha(hint, 0, 0);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;
        LeanTween.cancel(hint);
        LeanTween.alpha(hint, 1, 0.125f);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;
        LeanTween.cancel(hint);
        LeanTween.alpha(hint, 0, 0.125f);
    }
}
