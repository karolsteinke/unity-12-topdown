using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceTrigger : MonoBehaviour
{
    [SerializeField] private GameObject[] targets;
    public bool requireKey;
    
    void OnTriggerEnter(Collider other) {
        //Don't open if key is required but not equipped
        if (requireKey && Managers.Inventory.equippedItem != "key") {
            return;
        }

        //Active
        foreach (GameObject target in targets) {
            target.SendMessage("Active");
        }
    }

    void OnTriggerExit(Collider other) {
        foreach (GameObject target in targets) {
            target.SendMessage("Deactive");
        }
    }

}
