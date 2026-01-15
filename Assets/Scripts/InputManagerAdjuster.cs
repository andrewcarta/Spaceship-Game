using UnityEngine;
using UnityEngine.InputSystem;

public class InputManagerAdjuster : MonoBehaviour
{
    [SerializeField] private PlayerInputManager playerInputManager;
    int activePlayers = 0;
    int lastActivePlayers = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        activePlayers = playerInputManager.playerCount;
        if(activePlayers!=1 && activePlayers != lastActivePlayers)
        {
            
        }
        lastActivePlayers = activePlayers;
    }

    
}
