using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager _instance;

    [SerializeField] private PathCreator[] paths;
    [SerializeField] private Material[] materialOptions;
    [SerializeField] private GameObject[] normalVehicles;
    [SerializeField] private GameObject[] emergencyVehicles;
    [SerializeField] private int timeBeforeSpawn = 5;
    [SerializeField] private float timeBetweenSpawn = 3;

    private int lastPath = 100;

    private void Awake()
    {
        _instance = this;
    }

    public void StartSpawning()
    {
        InvokeRepeating("RandomSpawn", timeBeforeSpawn, timeBetweenSpawn);
    }

    public void StopSpawning()
    {
        CancelInvoke("RandomSpawn");
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
        int randomPath = Random.Range(0, paths.Length);

        while (randomPath == lastPath)
        {
            randomPath = Random.Range(0, paths.Length);
        }

        if (emergency)
        {
            GameObject newVehicle = Instantiate(emergencyVehicles[Random.Range(0, emergencyVehicles.Length)], transform.position + Vector3.right * 100, Quaternion.identity);
            newVehicle.GetComponent<Vehicle>().path = paths[randomPath];
        }
        else
        {
            GameObject newVehicle = Instantiate(normalVehicles[Random.Range(0, normalVehicles.Length)], transform.position + Vector3.right * 100, Quaternion.identity);
            newVehicle.GetComponent<Vehicle>().path = paths[randomPath];
            if (newVehicle.name != "Taxi")
                newVehicle.GetComponent<MeshRenderer>().material = materialOptions[Random.Range(0, materialOptions.Length)];
        }
    }
}
