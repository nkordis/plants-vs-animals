using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        GameObject otherObject = otherCollider.gameObject;

		if (otherObject.GetComponent<Thorn>() && otherObject.transform.position.x < transform.position.x) {
			GetComponent<Animator>().SetTrigger("jumpTrigger");
		} else if (otherObject.GetComponent<Defender>() && otherObject.transform.position.x < transform.position.x)
        {
            GetComponent<Attacker>().Attack(otherObject);
        }
    }

}
