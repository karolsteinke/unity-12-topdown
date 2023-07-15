using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceOperator : MonoBehaviour
{
    public float radius = 1.5f;
    void Update() {
        if (Input.GetButtonDown("Fire3")) {
            //Send message to all nearby objects and try to open one
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider hitCollider in hitColliders) {
                Vector3 direction = hitCollider.transform.position - transform.position;
                if (Vector3.Dot(transform.forward, direction) > .5f) {
                    hitCollider.SendMessage("Operate", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
