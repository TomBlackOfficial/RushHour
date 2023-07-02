using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using PathCreation;
using EPOOutline;

public class Vehicle : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color colorMoving;
    [SerializeField] private Color colorStopped;
    [SerializeField] private Color colorWaiting;
    [SerializeField] private Color colorHover;

    [SerializeField] private bool emergency;
    private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    private float rage;
    private float rageMultiplier;

    public PathCreator path;
    private float distanceTravelled;
    private EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop;

    private Outlinable outline;

    private bool moving;
    private bool canMove;
    private bool hovering;
    private bool behindCar;
    private bool waiting;
    
    void OnPathChanged()
    {
        distanceTravelled = path.path.GetClosestDistanceAlongPath(transform.position);
    }

    void Start()
    {
        outline = GetComponent<Outlinable>();

        if (path != null)
        {
            path.pathUpdated += OnPathChanged;
        }
        
        speed = maxSpeed;
        moving = true;
        canMove = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && hovering)
        {
            Click();
        }

        Move();

        if (canMove && !behindCar)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        if (moving)
        {
            speed = Mathf.Lerp(speed, maxSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            speed = Mathf.Lerp(speed, 0, deceleration * Time.deltaTime);
        }

        if (speed <= 0.3f)
        {
            waiting = true;
        }
        else
        {
            waiting = false;
        }

        if (!hovering)
        {
            if (waiting && canMove)
                outline.OutlineParameters.Color = colorWaiting;
            else if (canMove && !waiting)
                outline.OutlineParameters.Color = colorMoving;
            else
                outline.OutlineParameters.Color = colorStopped;
        }

        Debug.DrawRay(transform.position + Vector3.up * 0.5f + transform.forward * 3, transform.forward * 5f, Color.red);
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f + transform.forward * 3, transform.forward, out hit, 5f))
        {
            behindCar = true;
        }
        else
        {
            behindCar = false;
        }

        if (distanceTravelled >= path.path.length)
        {
            Destroy(gameObject);
        }
    }

    void Move()
    {
        if (path != null)
        {
            distanceTravelled += speed * Time.deltaTime;
            transform.position = path.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
            transform.rotation = path.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
        }
    }

    void OnMouseEnter()
    {
        hovering = true;
        outline.OutlineParameters.Color = colorHover;
    }

    void OnMouseExit()
    {
        hovering = false;
        if (canMove)
            outline.OutlineParameters.Color = colorMoving;
        else
            outline.OutlineParameters.Color = colorStopped;
    }

    void Click()
    {
        canMove = !canMove;
    }
}
