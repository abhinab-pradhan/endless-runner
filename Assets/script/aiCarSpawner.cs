using System.Collections;
using UnityEngine;

public class aiCarSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] carAiPrefab;
    [SerializeField] LayerMask otherCarLayermask;
    Collider[] overlappedCheckCollider = new Collider[1];
    GameObject[] carAiPool = new GameObject[20];

    Transform playerCarTransform;
    //timing
    WaitForSeconds wait = new WaitForSeconds(.5f);
    float timeLastCarSpawned = 0;
    void Start()
    {

        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;


        int prefabIndex = 0;
        for (int i = 0; i < carAiPool.Length; i++)
        {
            carAiPool[i] = Instantiate(carAiPrefab[prefabIndex]);
            carAiPool[i].SetActive(false);

            prefabIndex++;

            //loop the prefab index if we run out of prefab
            if (prefabIndex > carAiPrefab.Length - 1)
            {
                prefabIndex = 0;
            }
        }
        StartCoroutine(updateLessOfTen());
    }

    IEnumerator updateLessOfTen()
    {
        while (true)
        {
            cleanUpBeyondiew();
            spawnCars();
            yield return wait;
        }
    }


    void spawnCars()
    {
        if (Time.time - timeLastCarSpawned < 2)
        {
            return;
        }

        GameObject carToSpawn = null;

        foreach (GameObject aiCar in carAiPool)
        {
            if (aiCar.activeInHierarchy)   //skip active car
            {
                continue;
            }

            carToSpawn = aiCar;
            break;
        }

        //no car available to spawn
        if (carToSpawn == null)
        {
            return;
        }

        Vector3 spawnPosition = new Vector3(0, 0, playerCarTransform.transform.position.z + 100);

        if (Physics.OverlapBoxNonAlloc(spawnPosition, Vector3.one * 2, overlappedCheckCollider, Quaternion.identity, otherCarLayermask) > 0)
        {
            return;
        }
        carToSpawn.transform.position = spawnPosition;
        carToSpawn.SetActive(true);

        timeLastCarSpawned = Time.time;
    }

    void cleanUpBeyondiew()
    {
        foreach (GameObject aiCar in carAiPool)
        {
            //skip inactive car
            if (!aiCar.activeInHierarchy)
            
                continue;
            
            if (aiCar.transform.position.z - playerCarTransform.position.z > 200)  //check if ai car is too far
            
                aiCar.SetActive(false);
            

            if (aiCar.transform.position.z - playerCarTransform.position.z < -50)   //check if ai car is too far behind
            
                aiCar.SetActive(false);
            
        }
    }
}
