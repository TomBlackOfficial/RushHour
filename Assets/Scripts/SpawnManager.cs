using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] normalVehicles;
    [SerializeField] private GameObject[] emergencyVehicles;
    [SerializeField] private int timeBeforeSpawn = 5;
    [SerializeField] private float timeBetweenSpawn = 3;

    private void Start()
    {
        InvokeRepeating("RandomSpawn", timeBeforeSpawn, timeBetweenSpawn);
    }

    private void RandomSpawn()
    {
        int randomNum = Random.Range(0, 100);

        if (randomNum <= 96)
        {
            Spawn(false);
        }
        else
        {
            Spawn(true);
        }
    }

    private void Spawn(bool emergency)
    {
        if (emergency)
        {
            Instantiate(emergencyVehicles[Random.Range(0, emergencyVehicles.Length)], spawnPoints[Random.Range(0, spawnPoints.Length)]);
        }
        else
        {
            Instantiate(normalVehicles[Random.Range(0, normalVehicles.Length)], spawnPoints[Random.Range(0, spawnPoints.Length)]);
        }
    }
}
