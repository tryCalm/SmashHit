using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DestructibleObject : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;
    }

    private void OnCollisionEnter()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        StartCoroutine(DestroyCoroutine());
    }
    
    public IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(5);
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}