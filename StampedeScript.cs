using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampedeScript : MonoBehaviour
{
    //Duration is the lifetime of the stampede, spawnRate is how often they will spawn and SpawnYOffset is the offset on the Y axis upon spawn
    public float duration, spawnRate, spawnYOffset;
    //Set this to our stampede obj prefab
    public GameObject stampedeOBJ;
    //What will our raycast interact with(ground)
    public LayerMask layerMask;

    //Private variables, no need to touch these
    [HideInInspector]
    public BoxCollider col;
    private float spawnRateTimer, colliderHeight;

    private void Start()
    {
        //Set the spawn timer
        spawnRateTimer = spawnRate;
        //Reference the start box collider
        col = transform.Find("Start").GetComponent<BoxCollider>();
    }

    private void Update()
    {
        //If our duration is bigger than zero
        if(duration > 0)
        {
            if (spawnRateTimer > 0)
            {
                spawnRateTimer -= Time.deltaTime;
            }
            else { SpawnObject(); }

            duration -= Time.deltaTime;
        }
        else
        {
            //Get all our children that contain a transform
            Transform[] allChildren = GetComponentsInChildren<Transform>();
            foreach (Transform child in allChildren)
            {
                //Make sure we are destroying the correct children by checking their name
                if(child.name == "StampedeOBJ")
                {
                    child.SetParent(null);
                }
            }
            //Destroy our stampede
            Destroy(gameObject);
        }
    }

    void SpawnObject()
    {
        //Set timer
        spawnRateTimer = spawnRate;

        //Getting spawn location
        RaycastHit hit;
        colliderHeight = col.size.y + 1;
        var randomPosition = RandomPointInsideBox(col.bounds);
        randomPosition.y = col.transform.position.y + (colliderHeight / 2);

        if (Physics.Raycast(randomPosition, Vector3.down, out hit, colliderHeight, layerMask))
        {
            var yPos = hit.point.y + spawnYOffset;
            Vector3 spawnPosition = new Vector3(randomPosition.x, yPos, randomPosition.z);

            var obj = Instantiate(stampedeOBJ, spawnPosition, Quaternion.identity);
            obj.transform.SetParent(transform);
            obj.name = "StampedeOBJ";

            var stampedeScript = obj.GetComponent<StampedeObjectScript>();
            stampedeScript.spawnCol = col;
            stampedeScript.layerMask = layerMask;
            stampedeScript.yOffset = spawnYOffset;
        }
    }

    //This function gives you a random location within some bounds
    public static Vector3 RandomPointInsideBox(Bounds bounds)
    {
        return new Vector3
        (
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
