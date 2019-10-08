using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        GameObject otherObject = otherCollider.gameObject;

        if (otherObject.GetComponent<Defender>() && otherObject.transform.position.x < transform.position.x)
        {
            GetComponent<Attacker>().Attack(otherObject);
        }
    }

}
