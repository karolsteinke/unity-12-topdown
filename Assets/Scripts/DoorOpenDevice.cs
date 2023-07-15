using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenDevice : MonoBehaviour
{
    [SerializeField] private Vector3 dPos;
    private bool _open;
    
    void Start() {
        _open = false;
    }
    
    public void Operate() {
        if (_open) {
            transform.position -= dPos;
        }
        else {
            transform.position += dPos;
        }
        _open = !_open;
    }

    public void Active() {
        if (!_open) {
            transform.position += dPos;
        }
        _open = true;
    }

    public void Deactive() {
        if (_open) {
            transform.position -= dPos;
        }
        _open = false;
    }
}
