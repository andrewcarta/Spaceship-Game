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

public class ShipController : MonoBehaviour
{
    //parts of the player's unity components
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D cd;
    [SerializeField] private SpriteRenderer sp;

    //All ship stats and specs we can give an int value to
    private enum stats : int
    {
        speed = 10
    }
    private bool piloted;
    private Vector2 move;
    private Transform pilotSeat;
    [SerializeField] private Transform pilot;


    //Input Actions
    [SerializeField] private InputActionReference moveShip;
    [SerializeField] private InputActionReference actionAShip;
    [SerializeField] private InputActionReference actionBShip;
    [SerializeField] private InputActionReference actionCShip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
            move = Vector2.zero;
        print("<color=green> " + this.name + " Spawned at " + gameObject.transform.position.x + " " + gameObject.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        lockPilotPos();
        move = moveShip.action.ReadValue<Vector2>();
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
        //enable player script
        foreach (Transform child in transform) {
            //checks if is player AND if player is pilot
            if (child.Equals(pilot)) {  }
        }
        //disable this script

    }

    private void lockPilotPos() {
        pilotSeat = this.transform;
        foreach (Transform child in this.transform)
        {
            if (child.name.Contains("PilotSeat")) { pilotSeat = child; }
        }

    }


    private void OnEnable()
    {
        //do the code: 'buttonName'.action.started += On+'buttonName'; to make a method instantiator for input method
        actionAShip.action.started += ShipActionA;
        actionBShip.action.started += ShipActionB;
        actionCShip.action.started += ShipActionC;

    }
    private void OnDisable()
    {
        //need this disable code or else if code is enabled twice the obj will run the method twice
        actionAShip.action.started -= ShipActionA;
        actionBShip.action.started -= ShipActionB;
        actionCShip.action.started -= ShipActionC;
    }
}
