using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletConfig : MonoBehaviour
{
    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        Destroy(gameObject);
    }
}
