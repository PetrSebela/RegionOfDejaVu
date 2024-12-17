using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public float LoopTime;    
    public float LoopTimeCountdown;
    public bool Slowmotion = false;
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        LoopTimeCountdown -= Time.deltaTime;
    }

    void StartNewRun()
    {

    }
}
