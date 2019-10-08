using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goat : MonoBehaviour {

	private float damage = 2000f;

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        GameObject otherObject = otherCollider.gameObject;

		if (otherObject.GetComponent<Mushroom>() && otherObject.transform.position.x < transform.position.x) {
			GetComponent<Attacker>().Attack(otherObject);
			GetComponent<Attacker>().StrikeCurrentTarget(damage);
			GetComponent<Health>().DealDamage(damage);
		}

        else if (otherObject.GetComponent<Defender>() && otherObject.transform.position.x < transform.position.x)
        {
            GetComponent<Attacker>().Attack(otherObject);
			GetComponent<Attacker>().StrikeCurrentTarget(damage);
        }
    }

}
