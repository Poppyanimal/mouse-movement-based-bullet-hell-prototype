using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerPuppet : MonoBehaviour
{
    public Rigidbody2D leash;
    float clearCircleRadius = 1.5f;
    float baseSpeed = 4f;
    Rigidbody2D puppet;
    ContactFilter2D bulletsFil;
    bool currentlyInIframes = false;

    void Start()
    {
        puppet = gameObject.GetComponent<Rigidbody2D>();
        bulletsFil = KiroLib.getBulletFilter();
    }

    void FixedUpdate()
    {
        if((leash.position - puppet.position).magnitude > getDeadzone())
            puppet.velocity = (leash.position - puppet.position).normalized * baseSpeed;
        else puppet.velocity = Vector2.zero;

        if (!currentlyInIframes) HitDetection();
    }

    float getDeadzone()
    {
        return baseSpeed * 0.02f;
    }

    void HitDetection()
    {
        Collider2D[] detectedBullets = new Collider2D[16];
        int results = gameObject.GetComponent<Collider2D>().OverlapCollider(bulletsFil, detectedBullets);
        if(results > 0)
        {
            takeDamage();
            for(int i = 0; i < results; i++)
            {
               Destroy(detectedBullets[i].gameObject);
            }
            RaycastHit2D[] cClearResults = Physics2D.CircleCastAll(puppet.position, clearCircleRadius, Vector2.zero, clearCircleRadius*2, bulletsFil.layerMask.value);
            for(int i = 0; i < cClearResults.Length; i++)
            {
                Destroy(cClearResults[i].collider.gameObject);
            }
        }
    }

    int health = 4;
    void takeDamage()
    {
        health--;
        StartCoroutine(doIframes());
        Debug.Log("Damage Taken! "+health+ " health left!");
        if(health <= 0)
        {
            Debug.Log("You died.");
            //Destroy(gameObject);
        }
    }

    IEnumerator doIframes()
    {
        float iFrames = 1f;
        currentlyInIframes = true;
        yield return new WaitForSeconds(iFrames);
        currentlyInIframes = false;
    }
}
