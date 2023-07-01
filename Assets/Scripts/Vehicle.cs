using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Vehicle : MonoBehaviour
{
    [SerializeField] protected GameObject prefab;
    [SerializeField] protected bool moving;
    [SerializeField] protected float speed;
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float acceleration;
    [SerializeField] protected float rage;
    [SerializeField] protected float rageMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void StartMove()
    {
        
    }

    public void EndMove() 
    {
        
    }
}
