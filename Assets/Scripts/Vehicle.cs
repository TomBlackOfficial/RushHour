using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using PathCreation;
using EPOOutline;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Vehicle : MonoBehaviour
{
    [SerializeField] private Sprite leftTurnSprite;
    [SerializeField] private Sprite rightTurnSprite;
    [SerializeField] private Sprite straightSprite;
    [SerializeField] private Image directionSpriteRenderer;

    [Header("Particles")]
    [SerializeField] private GameObject explosionVFX;
    [SerializeField] private GameObject smokeVFX;

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
    private bool speedBoost;

    private float speedBoostSpeed;
    private float normalMoveSpeed;

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

        if (path.name == "Straight Right Lane" || path.name == "Straight Left Lane")
        {
            directionSpriteRenderer.sprite = straightSprite;
            directionSpriteRenderer.transform.GetChild(0).GetComponent<Image>().sprite = straightSprite;
        }
        else if (path.name == "Right")
        {
            directionSpriteRenderer.sprite = rightTurnSprite;
            directionSpriteRenderer.transform.GetChild(0).GetComponent<Image>().sprite = rightTurnSprite;
        }
        else if (path.name == "Left")
        {
            directionSpriteRenderer.sprite = leftTurnSprite;
            directionSpriteRenderer.transform.GetChild(0).GetComponent<Image>().sprite = leftTurnSprite;
        }

        speed = maxSpeed;
        moving = true;
        canMove = true;

        normalMoveSpeed = maxSpeed;
        speedBoostSpeed = maxSpeed * 1.5f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && hovering)
        {
            LeftClick();
        }
        if (Input.GetMouseButtonDown(1) && hovering)
        {
            RightClick();
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

        if (speedBoost)
        {
            maxSpeed = speedBoostSpeed;
        }
        else
        {
            maxSpeed = normalMoveSpeed;
        }

        if (moving || enraged)
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

            if (Physics.Raycast(transform.position + Vector3.up * 0.5f + transform.forward * 3, transform.forward, out hit, 5f))
            {
                if (!hit.transform.GetComponent<Vehicle>().moving)
                    behindCar = true;
            }
            else
            {
                behindCar = false;
            }

            if (speed <= 1f)
            {
                waiting = true;
                rage = Mathf.Clamp01(rage + 0.00075f);

                outline.OutlineParameters.FillPass.SetColor("_PublicColor" , new Color(200, 0, 0, rage * 0.6f));
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

                if (rage != 1)
                {
                    rage = Mathf.Clamp01(rage - 0.0003f);
                    outline.OutlineParameters.FillPass.SetColor("_PublicColor", new Color(200, 0, 0, rage * 0.6f));
                }
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

        if (path != null)
        {
            if (distanceTravelled >= path.path.length)
            {
                GameManager._instance.AddScore(score - (int)(rage * 100));
                Destroy(gameObject);
            }
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

    void LeftClick()
    {
        if (enraged || emergency || GameManager._instance.currentState == GameManager.GameStates.Paused)
            return;

        canMove = !canMove;
    }

    void RightClick()
    {
        if (emergency || GameManager._instance.currentState == GameManager.GameStates.Paused)
            return;

        if (!canMove)
            LeftClick();

        speedBoost = !speedBoost;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CompareTag(other.tag))
        {
            GameManager._instance.SpawnVFX(explosionVFX, Between(transform.position, other.transform.position, 0.5f), Quaternion.identity);
            GameManager._instance.Die();

            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }

    Vector3 Between(Vector3 v1, Vector3 v2, float percentage)
    {
        return (v2 - v1) * percentage + v1;
    }
}
