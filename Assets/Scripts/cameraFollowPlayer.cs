using UnityEngine;

public class cameraFollowPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject playerToFollow;
    
    void Start()
    {
        
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
    }
}
