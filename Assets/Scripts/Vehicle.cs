using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using PathCreation;
using EPOOutline;
using UnityEngine.SceneManagement;

public class Vehicle : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color colorMoving;
    [SerializeField] private Color colorStopped;
    [SerializeField] private Color colorWaiting;
    [SerializeField] private Color colorHover;

    [SerializeField] private bool emergency;

    [Header("Movement")]
    private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    private float rage;
    private float rageMultiplier = 2;

    [Header("Score")]
    [SerializeField] private int score;

    [HideInInspector] public PathCreator path;
    private float distanceTravelled;
    private EndOfPathInstruction endOfPathInstruction = EndOfPathInstruction.Stop;

    private Outlinable outline;

    private bool moving;
    private bool canMove;
    private bool hovering;
    private bool behindCar;
    private bool waiting;
    private bool enraged;

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
            if (enraged)
            {
                speed = Mathf.Lerp(speed, maxSpeed, acceleration * rageMultiplier * Time.deltaTime);
            }
            else
            {
                speed = Mathf.Lerp(speed, maxSpeed, acceleration * Time.deltaTime);
            }
            
        }
        else
        {
            speed = Mathf.Lerp(speed, 0, deceleration * Time.deltaTime);
        }

        if (!enraged)
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f + transform.forward * 3, transform.forward * 5f, Color.red);
            RaycastHit hit;

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f + transform.forward * 3, transform.forward, out hit, 5f) 
                && !hit.transform.gameObject.GetComponent<Vehicle>().moving)
            {
                behindCar = true;
            }
            else
            {
                behindCar = false;
            }

            if (speed <= 1f)
            {
                waiting = true;
                rage = Mathf.Clamp01(rage + 0.002f);
                outline.OutlineParameters.FillPass.SetColor("_PublicColor" , new Color(200, 0, 0, rage * 0.7f));
                if (rage == 1f)
                {
                    enraged = true;
                    canMove = true;
                    waiting = false;
                    behindCar = false;
                }
            }
            else
            {
                waiting = false;
                rage = Mathf.Clamp01(rage - 0.0005f);
            }
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

        if (distanceTravelled >= path.path.length)
        {
            GameManager._instance.AddScore(score);
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

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag(other.tag))
        {
            GameManager._instance.Die();
        }
    }
}
