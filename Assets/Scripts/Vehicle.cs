using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Vehicle : MonoBehaviour
{
    protected bool moving;
    protected float speed;
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float acceleration;
    protected float rage;
    protected float rageMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (moving) StartMove();
        else EndMove();
    }

    protected void StartMove()
    {
        
    }

    protected void EndMove() 
    {
        
    }
    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
        Debug.Log("Mouse is over GameObject.");
    }

    void OnMouseExit()
    {
        //The mouse is no longer hovering over the GameObject so output this message each frame
        Debug.Log("Mouse is no longer on GameObject.");
    }

    public void Click()
    {
        moving = !moving;
    }
}
