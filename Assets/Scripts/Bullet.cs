using System;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(this.gameObject, 8f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (other.gameObject.GetComponent<Bullet>() != null)
            Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), other.gameObject.GetComponent<Collider>());
    }
}