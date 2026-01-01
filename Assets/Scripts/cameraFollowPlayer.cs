using UnityEngine;

public class cameraFollowPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject playerToFollow;
    private PlayerController playerFollowingScript;
    
    void Start()
    {
        
        playerToFollow = this.transform.parent.gameObject;
        print("<color=red>" + playerToFollow.name);
        print("<color=red>"+ playerToFollow.GetComponent<MonoBehaviour>());
        playerFollowingScript = (PlayerController)playerToFollow.GetComponent<MonoBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        
    }
    private void LateUpdate()
    {
        this.transform.position = new Vector3(playerToFollow.transform.position.x,playerToFollow.transform.position.y,-10);
        this.transform.rotation = Quaternion.identity;
        //? Should scale out the camera based upon the ship scale
        if (playerFollowingScript != null && playerFollowingScript.getCurrentShip() != null)
        {
            ShipController shipScript = (ShipController)playerFollowingScript.getCurrentShip().GetComponent<MonoBehaviour>();
            this.GetComponent<Camera>().orthographicSize = 25 * shipScript.getScale();
        }
        else { this.GetComponent<Camera>().orthographicSize = 30; }
    }
}
