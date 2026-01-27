using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform pilotSeat;
    private List<GameObject> preloadedBullets;
    //the current plan is to hide and reuse bullets that hit other ships to reduce lag
    private float turnSpeed;
    private Transform ship;
    private ShipController shipScript;
    private Transform controller;
    private Rigidbody2D controllerRB;
    private Rigidbody2D rb;

    void Start()
    {
        if(transform.parent != null && transform.parent.CompareTag("Ship"))
        {
        ship = transform.parent;
        shipScript = ship.GetComponent<ShipController>();
        rb = GetComponent<Rigidbody2D>();
        }
    }

    void FixedUpdate()
    {
        if(ship != null)
        {
        rb.linearVelocity = shipScript.getShipVelocity();
        rb.angularVelocity = shipScript.getShipAngVelocity();
        }
        if(controller != null)
        {
            lockController();
            rotateTurret();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void shoot()
    {
        if(bulletPrefab != null)
        {
            

        }else{Debug.LogError("No bullet prefab assigned to "+name);}

    }

    private void lockController()
    {
        
        controllerRB.linearVelocity = Vector2.zero;
        controllerRB.position = pilotSeat.position;
        controller.position = pilotSeat.position;
    }

    private void rotateTurret()
    {
        transform.rotation = controller.transform.rotation;
        
    }

    public void controlTurret(Transform cntrl)
    {
        controller = cntrl;
        controllerRB = controller.GetComponent<Rigidbody2D>();
        print(cntrl+" is now controlling a turret");
    } 
    public void dismountTurret()
    {
        controller = null;
        controllerRB = null;
    }
}
