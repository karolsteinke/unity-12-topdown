using UnityEngine;
using System.Collections;

public class WanderingAI : MonoBehaviour {
	public float speed = 3.0f;
	public float obstacleRange = 2.0f;
	
	[SerializeField] private GameObject fireballPrefab;
	private GameObject _player;
	private GameObject _fireball;
	
	private bool _alive;
	
	void Start() {
		_alive = true;
		_player = GameObject.FindGameObjectWithTag("Player");
	}
	
	void Update() {
		if (_alive) {
			transform.Translate(0, 0, speed * Time.deltaTime);
			
			Ray ray = new Ray(transform.position, transform.forward);
			RaycastHit hit;
			if (Physics.SphereCast(ray, 0.75f, out hit)) {
				GameObject hitObject = hit.transform.gameObject;
				//it checks for some close obstacles and if there are any - it rotates
				if (hit.distance < obstacleRange) {
					float angle = Random.Range(-110, 110);
					transform.Rotate(0, angle, 0);
				}
			}

			//check if player is nearby and facing
			Vector3 playerPos = _player.transform.position;
			Vector3 dir = playerPos - transform.position;
			if (Vector3.SqrMagnitude(dir) < 100.0f && Vector3.Dot(transform.forward, dir.normalized) > .6f) {
				transform.LookAt(new Vector3(playerPos.x, transform.position.y, playerPos.z));
				//if player is close enough - shot a fireball
				if (Vector3.SqrMagnitude(dir) < 25.0f && _fireball == null) {
					_fireball = Instantiate(fireballPrefab) as GameObject;
					_fireball.transform.position = transform.TransformPoint(Vector3.forward * 1.5f);
					_fireball.transform.rotation = transform.rotation;
				}
			}
		}
	}

	public void SetAlive(bool alive) {
		_alive = alive;
	}
}
