using System.Collections;
using UnityEngine;

public class endlessLevelHandler : MonoBehaviour
{
    [SerializeField] GameObject[] sectionPrefab;
    GameObject[] sectionPool = new GameObject[20];
    GameObject[] section = new GameObject[10];
    Transform playerCarTransform;
    WaitForSeconds waitFor100ms = new WaitForSeconds(.1f);
    const float sectionLength = 26;
    void Start()
    {
        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;

        int prefabIndex = 0;
        for (int i = 0; i < sectionPool.Length; i++)  //create pool for endless section
        {
            sectionPool[i] = Instantiate(sectionPrefab[prefabIndex]);
            sectionPool[i].SetActive(false);

            prefabIndex++;

            if (prefabIndex > sectionPrefab.Length - 1)  //loop the prefab index if we run out of prefabs
            {
                prefabIndex = 0;
            }
        }

        for (int i = 0; i < section.Length; i++)  //add the first sections to the road
        {
            //get a random section
            GameObject randomSection = getRandomSectionFromPool();

            //move it into position and set it to active
            randomSection.transform.position = new Vector3(sectionPool[i].transform.position.x, -10, i * sectionLength);
            randomSection.SetActive(true);

            //set the section in the array
            section[i] = randomSection;
        }

        StartCoroutine(updateLessOfTen());
    }

    IEnumerator updateLessOfTen()
    {
        while (true)
        {
            updateSectionPosition();
            yield return waitFor100ms;
        }
    }

    void updateSectionPosition()
    {
        for (int i = 0; i < section.Length; i++)
        {
            //check if section is too far behind
            if (section[i].transform.position.z - playerCarTransform.position.z < -sectionLength)
            {
                //store the position of the section and disable it
                Vector3 lastSectionPosition = section[i].transform.position;
                section[i].SetActive(false);

                //get new section & enable it & move it forward
                section[i] = getRandomSectionFromPool();

                //move the new section into place and active it
                section[i].transform.position = new Vector3(lastSectionPosition.x, -10,lastSectionPosition.z+sectionLength*section.Length);
                section[i].SetActive(true);
            }
        }
    }

    GameObject getRandomSectionFromPool()
    {
        //pick a random object and hope that it is available
        int randomIndex = Random.Range(0, sectionPool.Length);

        bool isNewSectionFound = false;
        while (!isNewSectionFound)
        {
            //check if the section is not active
            if (!sectionPool[randomIndex].activeInHierarchy)
            {
                isNewSectionFound = true;
            }
            else
            {
                //if it was active we need to try to find new another one so we increase the index
                randomIndex++;

                //ensure that we loop around if we react the end of the array
                if (randomIndex > sectionPool.Length - 1)
                {
                    randomIndex = 0;
                }
            }
        }

        return sectionPool[randomIndex];
    }
}
