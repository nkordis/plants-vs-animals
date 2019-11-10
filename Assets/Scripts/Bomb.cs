using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
	[SerializeField] float damage = 5000f;

	public void BombNow() 
	{
		
		AttackerSpawner[] spawners = FindObjectsOfType<AttackerSpawner>();
		foreach (AttackerSpawner spawner in spawners) {

			for (int i = 0; i < spawner.transform.childCount; i++) {
				if (Math.Abs(spawner.transform.GetChild(i).gameObject.transform.position.x - transform.position.x) <= 2f  && Math.Abs(spawner.transform.GetChild(i).gameObject.transform.position.y - transform.position.y) <= 2f)
				{
					spawner.transform.GetChild(i).gameObject.GetComponent<Health>().DealDamage(damage);
					Debug.Log("Oleeee");
				}
			}

		}
		
	}

	public void BombDestroy() 
	{
		Destroy(gameObject);
	}
}
