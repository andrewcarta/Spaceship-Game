using System;
using System.Collections.Generic;
using System.Drawing;
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
    [SerializeField] private int shipScale;
    [SerializeField] private int shipSpeed;
    [SerializeField] private int shipTurnSpeed;
    [SerializeField] private int shipArmor;
    [SerializeField] private int shipHealth;
    private PlayerInput pilotInput;

    //All ship stats and specs we can give an int value to
    private bool piloted;
    private bool boostersActive;
    private int boostBonus = 1;
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
            print("<color=magenta> No initial pilot, disabling SpaceshipController");
            enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        
        if (piloted)
        {
            applyMovement();
            lockPilotPos();
        }
        checkForCollsions();
    }


    //I do need to rework the rotation and movement for smoother movement for spaceship
    //! This method will be reworked and some engine sorter will be made to allow for easy
    private void applyMovement()
    {
        //TODO Make the engines work slower at the start and faster later on
        move = pilotInput.actions["PlayerMove"].ReadValue<Vector2>();
        // Rotates the ship left and right(balanced by ship size to slow turning)
        //TODO Make a method to assign values for ship size and possibly speed by default
        //? Possibly change rotating to angularVelocity
        transform.Rotate(new Vector3(0,0,-1 * move.x * (int)(shipTurnSpeed)/shipScale));
        //applies a velocity on the ship pointing in the direction it is facing
        if (move.y != 0)
        {
            if (pilotInput.actions["BoostMovement"].IsPressed()) { print("Boosyed"); boostBonus = 2; boostersActive = true; } else { boostBonus = 1; boostersActive = false; }
            if (move.y > 0) { rb.linearVelocity = transform.up * (int)(shipSpeed*boostBonus) / shipScale; }
            if (move.y < 0) { rb.linearVelocity = transform.up * -1 * (int)(shipSpeed*boostBonus)/(3*shipScale); }
        }
    }

    //? This method will check for collisions with other ships and deal damage based on that
    private void checkForCollsions() { 
    
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

    
    private void OnBoostedMovement(InputAction.CallbackContext context) {
        boostersActive = pilotInput.actions["BoostMovement"].triggered;
    }
    
    private void lockPilotPos() {
        if (pilotSeat != null && pilot != null)
        {
            pilot.transform.position = pilotSeat.position;
            pilot.transform.rotation = this.transform.rotation;
        }
    }

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
        pilotInput.actions["BoostMovement"].started += OnBoostedMovement;
    }
    private void unpilotShip() {
        print("Ship unpiloted by " + pilot.name);
        pilot = null;
        piloted = false;
        rb.linearVelocity = rb.linearVelocity / 100;
        pilotInput.actions["PlayerActionA"].started -= ShipActionA;
        pilotInput.actions["PlayerActionB"].started -= ShipActionB;
        pilotInput.actions["PlayerActionC"].started -= ShipActionC;
        pilotInput.actions["BoostMovement"].started -= OnBoostedMovement;
        enabled = false;
    }

    private void OnEnable()
    {
        PlayerController.OnShipPiloted += pilotShip;
        print("<color=magenta> SpaceshipController enabled");
    }
    private void OnDisable()
    {
        PlayerController.OnShipPiloted -= pilotShip;
        print("<color=magenta> Spaceship controller disabled");
    }
}
