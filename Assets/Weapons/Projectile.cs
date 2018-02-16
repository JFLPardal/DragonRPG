using UnityEngine;

public class Projectile : MonoBehaviour {

	[SerializeField] float projectileSpeed; //other classes can set this
	[SerializeField] GameObject shooter;	//serialized so that we can inspect who shot if game is paused

	const float DESTROY_DELAY = 0.01f;
	float damageCaused;

	public void SetShooter(GameObject shooter)
	{
		this.shooter = shooter;
	}

	public void SetDamage(float damage)
	{
		damageCaused = damage;
	}

	public float GetDefaultLaunchSpeed()
	{
		return projectileSpeed;
	}

	void OnCollisionEnter(Collision collision)
	{
		var layerCollidedWith = collision.gameObject.layer;
		if ( layerCollidedWith != shooter.layer)
		{
			DamageIfDamageable (collision);
		}
	}

	private void DamageIfDamageable (Collision collision)
	{
		Component damageableComponent = collision.gameObject.GetComponent (typeof(IDamageable));
		if (damageableComponent)
			(damageableComponent as IDamageable).TakeDamage (damageCaused);
		Destroy (gameObject, DESTROY_DELAY);
	}

	void OnTriggerEnter (Collider collider)
	{
	}
}
