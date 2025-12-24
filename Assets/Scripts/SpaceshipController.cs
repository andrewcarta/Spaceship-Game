using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
//! example
//? example
//x example
//!? example
//TODO example

//This script will be attatched to the ship parent node
//? The pilot will stay a child of the ship but be stored as the pilot var in the private GameObj


public class ShipController : MonoBehaviour
{
    //parts of the player's unity components
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D cdBase;
    [SerializeField] private Collider2D cdTop;
    [SerializeField] private SpriteRenderer spBase;
    [SerializeField] private SpriteRenderer spTop; 
    [SerializeField] private GameObject shipControls;
    private PlayerInput pilotInput;

    //All ship stats and specs we can give an int value to
    private enum stats : int
    {
        speed = 10
    }
    private bool piloted;
    private Vector2 move;
    private Transform pilotSeat;
    private GameObject pilot;
    private List<Transform> mainEngineSystem;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //TODO It would be amazing to have smth to set all SerializeFields at game start code
        mainEngineSystem = new List<Transform>();
        move = Vector2.zero;
        foreach (Transform child in transform) {
            if (child.name.Contains("ShipControls")) {
                shipControls = child.gameObject;
            }
            if (child.name.Contains("PilotSeat")) {
                pilotSeat = child.transform;
            }
            //All ships will have a main engine system so I can use them to list off the engine points
            if (child.name.Contains("MainEngineSystem")){
                foreach (Transform enginePoint in transform) { 
                mainEngineSystem.Add(enginePoint);
                }
                
            }
        }


        print("<color=green> " + this.name + " spawned at " + gameObject.transform.position.x + " " + gameObject.transform.position.y);
        if (!piloted)
        {
            print("<color=red> No initial pilot, disabling SpaceshipController");
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        lockPilotPos();
        if (piloted)
        {
            applyMovement();   
        }
    }


    //I do need to rework the rotation and movement for smoother movement for spaceship
    //! This method will be reworked and some engine sorter will be made to allow for easy
    private void applyMovement()
    {
        move = pilotInput.actions["PlayerMove"].ReadValue<Vector2>();
        if (move.y > 0) { }
        if (move.y < 0) { }
        if (move.x > 0) { }
        if (move.x < 0) { }
        rb.linearVelocity = move * (int)(stats.speed);
    }
    
    private void targetRaycasts()
    {

    }

    private void ShipActionA(InputAction.CallbackContext context)
    {
        print("<color=orange> Ship Input A");
    }
    private void ShipActionB(InputAction.CallbackContext context)
    {
        print("<color=orange> Ship Input B");
    }
    private void ShipActionC(InputAction.CallbackContext context)
    {
        print("<color=orange> Ship Input C");
        //! This press will unpilot the ship and disable this script while enabling the playerController ship
        if (piloted)
        {
            //enable player script
            pilot.GetComponent<PlayerController>().enabled = true;
            //disable this script
            unpilotShip();
        }
    }

    private void lockPilotPos() {
        if (pilotSeat != null && pilot != null)
        {
            pilot.transform.position = pilotSeat.position;
        }
    }

    //? orders the ship's engines from the -+ quad to -- quad to -+ and ++


    //? This method will be called to let the ship know who the pilot is when the ship starts to piloted
    private void pilotShip(GameObject plt) {
        enabled = true;
        pilot = plt;
        piloted = true;
        pilotInput = plt.GetComponent<PlayerInput>();
        print("Ship being piloted by " + plt.name);
        pilotInput.actions["PlayerActionA"].started += ShipActionA;
        pilotInput.actions["PlayerActionB"].started += ShipActionB;
        pilotInput.actions["PlayerActionC"].started += ShipActionC;
    }
    private void unpilotShip() {
        print("Ship unpiloted by " + pilot.name);
        pilot = null;
        piloted = false;
        rb.linearVelocity = rb.linearVelocity / 100;
        pilotInput.actions["PlayerActionA"].started -= ShipActionA;
        pilotInput.actions["PlayerActionB"].started -= ShipActionB;
        pilotInput.actions["PlayerActionC"].started -= ShipActionC;
        enabled = false;
    }

    private void OnEnable()
    {
        PlayerController.OnShipPiloted += pilotShip;
        print("<color=red> SpaceshipController enabled");
    }
    private void OnDisable()
    {
        PlayerController.OnShipPiloted -= pilotShip;
        print("<color=red> Spaceship controller disabled");
    }
}
