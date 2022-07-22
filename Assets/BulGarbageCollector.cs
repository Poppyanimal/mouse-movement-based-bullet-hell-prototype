using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulGarbageCollector : MonoBehaviour
{
    //attach to a block to remove all bullets that overlap it's hitbox, a large body is recommended

    ContactFilter2D bulletsFil;
    void Start()
    {
        bulletsFil = KiroLib.getBulletFilter();
    }

    void Update()
    {
        Collider2D[] bulletsToDestroy = new Collider2D[16];
        int results = gameObject.GetComponent<Collider2D>().OverlapCollider(bulletsFil, bulletsToDestroy);
        for(int i = 0; i < results; i++)
        {
            Destroy(bulletsToDestroy[i].gameObject);
        }
    }
}
