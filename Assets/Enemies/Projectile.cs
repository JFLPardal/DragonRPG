using UnityEngine;
using UnityEngine.Assertions;

public class Projectile : MonoBehaviour {

	[SerializeField] float damageCaused = 10f;

	void OnTriggerEnter (Collider collider)
	{
		Component damageableComponent = collider.gameObject.GetComponent (typeof(IDamageable));
		Assert.IsNotNull (damageableComponent);
			(damageableComponent as IDamageable).TakeDamage (damageCaused);
	}
}
