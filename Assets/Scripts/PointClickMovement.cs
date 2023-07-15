using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]

public class PointClickMovement : MonoBehaviour {
	public float moveSpeed = 6.0f;
	public float rotSpeed = 15.0f;
	public float jumpSpeed = 15.0f;
	public float gravity = -9.8f;
	public float terminalVelocity = -20.0f;
	public float minFall = -1.5f;
	public float pushForce = 3.0f;
    public float deceleration = 25.9f;
    public float targetBuffer = 1.5f;
	private float _vertSpeed;
	private ControllerColliderHit _contact;
	private CharacterController _charController;
	private Animator _animator;
    private float _curSpeed = 0f;
    private Vector3 _targetPos = Vector3.one;

	// Use this for initialization
	void Start() {
		_vertSpeed = minFall;
		_charController = GetComponent<CharacterController>();
		_animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update() {

		// start with zero and add movement components progressively
		Vector3 movement = Vector3.zero;

        //Set target position when mouse clicks
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject()) {
            //Raycast at the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mouseHit;
            if (Physics.Raycast(ray, out mouseHit)) {
                //check if hitObject is on "ground" layer
				GameObject hitObject = mouseHit.transform.gameObject;
				if (hitObject.layer == LayerMask.NameToLayer("Ground")) {
					_targetPos = mouseHit.point;
                	_curSpeed = moveSpeed;
				}
            }
        }

        //Move if target position (mouse click) is set
        if (_targetPos != Vector3.one) {
            //Rotate toward target while moving quickly
            if (_curSpeed > moveSpeed * .5f) {
                //create targetRot which tells how much player needs to rotate (could also use LookAt)
                //rotate smoothly to face the target using slerp
				Vector3 adjustedPos = new Vector3(_targetPos.x, transform.position.y, _targetPos.z);
                Quaternion targetRot = Quaternion.LookRotation(adjustedPos - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotSpeed * Time.deltaTime);
            }
        }

        movement = _curSpeed * Vector3.forward;
        //transform from local to global
        movement = transform.TransformDirection(movement);

        //Decelerate to 0 when close to target
        if (Vector3.Distance(_targetPos, transform.position) < targetBuffer) {
            _curSpeed -= deceleration * Time.deltaTime;
            if (_curSpeed <= 0) {
                _targetPos = Vector3.one;
            }
        }

		_animator.SetFloat("Speed", movement.sqrMagnitude);

		// raycast down to address steep slopes and dropoff edge
		bool hitGround = false;
		RaycastHit hit;
		if (_vertSpeed < 0 && Physics.Raycast(transform.position, Vector3.down, out hit)) {
			float check = (_charController.height + _charController.radius) / 1.9f;
			hitGround = hit.distance <= check;	// to be sure check slightly beyond bottom of capsule
		}

		// y movement: possibly jump impulse up, always accel down
		// could _charController.isGrounded instead, but then cannot workaround dropoff edge
		if (hitGround) {
			/*
			if (Input.GetButtonDown("Jump")) {
				_vertSpeed = jumpSpeed;
			} else {
				_vertSpeed = minFall;
				_animator.SetBool("Jumping", false);
			}
			*/
			_vertSpeed = minFall;
			_animator.SetBool("Jumping", false);
		} else {
			_vertSpeed += gravity * 5 * Time.deltaTime;
			if (_vertSpeed < terminalVelocity) {
				_vertSpeed = terminalVelocity;
			}
			if (_contact != null ) {	// not right at level start
				_animator.SetBool("Jumping", true);
			}

			// workaround for standing on dropoff edge
			if (_charController.isGrounded) {
				if (Vector3.Dot(movement, _contact.normal) < 0) {
					movement = _contact.normal * moveSpeed;
				} else {
					movement += _contact.normal * moveSpeed;
				}
			}
		}
		movement.y = _vertSpeed;

		movement *= Time.deltaTime;
		_charController.Move(movement);
	}

	// store collision to use in Update
	void OnControllerColliderHit(ControllerColliderHit hit) {
		_contact = hit;

		Rigidbody body = hit.collider.attachedRigidbody;
		if (body != null && !body.isKinematic) {
			body.velocity = hit.moveDirection * pushForce;
		}
	}
}
