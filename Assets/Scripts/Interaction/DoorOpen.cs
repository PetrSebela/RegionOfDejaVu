using UnityEngine;

public class DoorOpen : Interaction
{
    [SerializeField] GameObject doors;
    [SerializeField] float _height;
    private bool _doorState = false;
    private float _defaultHeight;

    void Start()
    {
        _defaultHeight = doors.transform.position.y;
        
    } 

    public override void Interact()
    {           
        _doorState = !_doorState;
        
        if(_doorState)
            LeanTween.moveLocalY(doors, _defaultHeight, 0.25f).setEaseInOutQuad();
        else
            LeanTween.moveLocalY(doors, _defaultHeight - _height, 0.25f).setEaseInOutQuad();
    }
}
