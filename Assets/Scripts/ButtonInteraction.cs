using System;
using UnityEngine;

public class ButtonInteraction : MonoBehaviour
{
    [SerializeField] CanvasGroup _hint;
    [SerializeField] Interaction _action;
    private Action<float> _alphaCallback;

    void Start()
    {
        _alphaCallback += ChangeCanvasAlpha;
        _hint.alpha = 0;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(!enabled)
            return;
            
        if(col.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        col.gameObject.GetComponent<PlayerController>().CurrentInteraction = _action;
        LeanTween.cancel(this.gameObject);
        LeanTween.value(this.gameObject, _alphaCallback, _hint.alpha, 1, 0.125f);
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if(!enabled)
            return;

        if(col.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        col.gameObject.GetComponent<PlayerController>().CurrentInteraction = null;
        LeanTween.cancel(this.gameObject);
        LeanTween.value(this.gameObject, _alphaCallback, _hint.alpha, 0, 0.125f);
    }

    void ChangeCanvasAlpha(float alpha)
    {
        _hint.alpha = alpha;
    }
}
