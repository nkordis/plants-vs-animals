using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

    [SerializeField] float health = 100f;
    [SerializeField] GameObject deathVFX;
	[SerializeField] AudioClip deathSound;



	public void DealDamage(float damage)
    {
        health -= damage;
        if (health <= 0) {
			TriggerDeathVFX();
			PlayDeathSound();
			Destroy(gameObject);
		}
	}

	private void PlayDeathSound() {
		AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, 0.2f);
	}

	private void TriggerDeathVFX()
    {
        if(!deathVFX) { return; }
        GameObject deathVFXObject = Instantiate(deathVFX, transform.position, transform.rotation);
        Destroy(deathVFXObject, 1f);
    }

}
