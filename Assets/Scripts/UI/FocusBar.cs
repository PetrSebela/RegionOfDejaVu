using UnityEngine;
using UnityEngine.UI;

public class FocusBar : MonoBehaviour
{
    [SerializeField] Slider[] _sliders;
    [SerializeField] TimeJump _jumpDrive;

    void Update()
    {
        foreach (Slider s in _sliders)
            s.value = _jumpDrive.GetFocusPercent();
    }
}
