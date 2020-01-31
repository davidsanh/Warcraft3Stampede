using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StampedeObjectScript : MonoBehaviour
{
    public bool isDead = false, debug;
    public float movementSpeed, radius, deadTimer, travelDistance;

    [HideInInspector]
    public float yOffset;
    [HideInInspector]
    public LayerMask layerMask;
    private Vector3 spawnLoc;
    private float height;
    [HideInInspector]
    public BoxCollider spawnCol;
    private ParticleSystem impactFX, loopFX;
    private MeshRenderer rend;

    private void Start()
    {
        //The sphere mesh renderer
        rend = GetComponent<MeshRenderer>();
        //Reference both of our particle systems
        impactFX = transform.Find("ImpactFX").GetComponent<ParticleSystem>();
        loopFX = transform.Find("LoopFX").GetComponent<ParticleSystem>();
        //Store the spawn location to calculate travel distance
        spawnLoc = transform.position;
        //Set the height of our raycast
        height = spawnCol.size.y + 1;
    }

    private void Update()
    {
        if (!isDead)
        {
            //position and movement
            RaycastHit hit;
            var curPosition = transform.position;
            //Split it half using the height of the collider
            curPosition.y += height / 2;
            //Begin raycast
            if (Physics.Raycast(curPosition, Vector3.down, out hit, height, layerMask))
            {
                //Add an offset so it is not inside the ground
                var yPos = hit.point.y + yOffset;
                //Set our position accordingly
                transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
                //Move this object forward using the movement speed
                transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
            }
            else
            {
                //We were not able to find a floor so we destroy ourselves
                Destroy(gameObject);
            }

            var distanceFromSpawn = Vector3.Distance(transform.position, spawnLoc);
            if (distanceFromSpawn > travelDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy" && !isDead)
        {
            Impact();
        }
    }

    void Impact()
    {
        impactFX.Play();
        loopFX.Stop();

        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in objectsInRange)
        {
            if (col.tag == "Enemy")
            {
                Destroy(col.gameObject);
            }
        }

        rend.enabled = false;
        Destroy(gameObject, deadTimer);
        isDead = true;
    }

    private void OnDrawGizmos()
    {
        if (debug && isDead)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, radius);
        }
    }
}
