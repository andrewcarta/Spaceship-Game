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
    [SerializeField] private GameObject shipControls;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //TODO It would be amazing to have smth to set all SerializeFields at game start code
        move = Vector2.zero;
        foreach (Transform child in transform) {
            if (child.name.Contains("ShipControls")) {
                shipControls = child.gameObject;
            }
            if (child.name.Contains("PilotSeat")) {
                pilotSeat = child.transform;
            }
        }


        print("<color=green> " + this.name + " spawned at " + gameObject.transform.position.x + " " + gameObject.transform.position.y);
        print("Spaceship controller initialized, now disabling till piloted");
        if (!piloted)
        {
            print("<color=red> Couldn't find pilot, disabling SpaceshipController");
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
            move = pilotInput.actions["PlayerMove"].ReadValue<Vector2>();
        }

        rb.linearVelocity = move * (int)(stats.speed);
    }


    //I do need to rework the rotation and movement for smoother movement for spaceship
    private void rotateSprite()
    {
        Vector2 thisPos = transform.position;
        Vector2 targetPos = thisPos + move;
        float angle = Mathf.Atan2(move.y, move.x) * Mathf.Rad2Deg - 90;
        if (move != Vector2.zero)
        {
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    
    private void targetRaycasts()
    {

    }

    //ALL methods linked to the inputMap and are called when the input happens (public so the methods can be used in other classes?)
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
        enabled = false;
        pilotInput.actions["PlayerActionA"].started -= ShipActionA;
        pilotInput.actions["PlayerActionB"].started -= ShipActionB;
        pilotInput.actions["PlayerActionC"].started -= ShipActionC;
    }

    private void OnEnable()
    {
        PlayerController.OnShipPiloted += pilotShip;
        print("Spaceship controller enabled");
    }
    private void OnDisable()
    {
        PlayerController.OnShipPiloted -= pilotShip;
        print("Spaceship controller disabled");
    }
}
