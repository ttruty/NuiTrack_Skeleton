using UnityEngine;
using System.Collections;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] fallingObjectsPrefabs;

    [Range(0.5f, 2f)]
    [SerializeField]
    float minTimeInterval = 1;

    [Range(2f, 4f)]
    [SerializeField]
    float maxTimeInterval = 2;

    float halfWidth;

    public void StartSpawn(float widthImage)
    {
        halfWidth = widthImage / 2;
        StartCoroutine(SpawnObject(0f));
    }

    IEnumerator SpawnObject(float waitingTime)
    {
        yield return new WaitForSeconds(waitingTime);

        float randX = Random.Range(-halfWidth, halfWidth);
        Vector3 localSpawnPosition = new Vector3(randX, 0, 0);

        GameObject currentObject = Instantiate(fallingObjectsPrefabs[Random.Range(0, fallingObjectsPrefabs.Length)]);

        currentObject.transform.SetParent(gameObject.transform, true);
        currentObject.transform.localPosition = localSpawnPosition;

        StartCoroutine(SpawnObject(Random.Range(minTimeInterval, maxTimeInterval)));
    }  
}
