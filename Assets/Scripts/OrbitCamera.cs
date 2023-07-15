using UnityEngine;
using System.Collections;

// maintains position offset while orbiting around target

public class OrbitCamera : MonoBehaviour {
	[SerializeField] private Transform target;

	public float rotSpeed = 1.5f;

	private float _rotY;
	private Vector3 _offset;

	// Use this for initialization
	void Start() {
		_rotY = transform.eulerAngles.y;
		_offset = target.position - transform.position;
		//Hide cursor
		//Cursor.lockState = CursorLockMode.Locked;
		//Cursor.visible = false;
	}
	
	// Update is called once per frame
	void LateUpdate() {
		_rotY -= Input.GetAxis("Horizontal") * rotSpeed;
		//"rotation * _offset" rotates _offset by given angles (euler)
		Quaternion rotation = Quaternion.Euler(0, _rotY, 0);
		transform.position = target.position - (rotation * _offset);
		transform.LookAt(target);
	}
}
