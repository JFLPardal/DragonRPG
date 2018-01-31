using UnityEngine;

public class Projectile : MonoBehaviour {

	public float projectileSpeed; //other classes can set this

	float damageCaused;

	public void SetDamage(float damage)
	{
		damageCaused = damage;
	}

	void OnTriggerEnter (Collider collider)
	{
		Component damageableComponent = collider.gameObject.GetComponent (typeof(IDamageable));
		if(damageableComponent)
			(damageableComponent as IDamageable).TakeDamage (damageCaused);
	}
}
