using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : MonoBehaviour
{
	[Range(0f, 5f)]
	float currentSpeed = 1f;
	[SerializeField] float damage = 50;

	AttackerSpawner myLaneSpawner;
	Animator animator;
	GameObject currentTarget;

	void Start()
    {
		SetLaneSpawner();
		animator = GetComponent<Animator>();
	}

	private void SetLaneSpawner() {
		AttackerSpawner[] spawners = FindObjectsOfType<AttackerSpawner>();
		
		foreach (AttackerSpawner spawner in spawners) {
			bool IsCloseEnough =
				(Mathf.Abs(spawner.transform.position.y - transform.position.y)
				<= Mathf.Epsilon);
			if (IsCloseEnough) {
				myLaneSpawner = spawner;
			}
		}
	}

	private bool IsAttackerInLane() {
		if (myLaneSpawner.transform.childCount <= 0) {
			return false;
		} else {
			return true;
		}
	}


	void Update()
    {
		transform.Translate(Vector2.right * currentSpeed * Time.deltaTime);
		if (IsAttackerInLane()) {
			animator.SetBool("isAttacking", true);
		} else {
			//animator.SetBool("isAttacking", false);
		}
	}

	private void UpdateAnimationState() {
		if (!currentTarget) {
			GetComponent<Animator>().SetBool("isAttacking", false);
		}
	}

	public void SetMovementSpeed(float speed) {
		currentSpeed = speed;
	}

	public void Attack(GameObject target) {
		GetComponent<Animator>().SetBool("isAttacking", true);
		currentTarget = target;
	}

	public void StrikeCurrentTarget(float damage) {
		if (!currentTarget) { return; }
		Health health = currentTarget.GetComponent<Health>();
		if (health) {
			health.DealDamage(damage);
		}
	}


	private void OnTriggerEnter2D(Collider2D otherCollider) {

		if (otherCollider.gameObject.GetComponent<Attacker>() != null && otherCollider.gameObject.transform.position.x >= gameObject.transform.position.x
			) 
		{
			Attack(otherCollider.gameObject);
			StrikeCurrentTarget(damage);
		}
		

	}
}
