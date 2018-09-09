using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent(typeof(WeaponSystem))]
	public class EnemyAI : MonoBehaviour
    {
	    [SerializeField] float chaseRadius = 7f;

	    bool isAttacking = false; // todo richer state , patrol, idle...
	    PlayerMovement player;
        float currentWeaponRange;

        void Start()
	    {
		    player = FindObjectOfType<PlayerMovement>();
        }

	    void Update()
	    {
		    float distanceToPlayer = Vector3.Distance (transform.position, player.transform.position);
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetAttackRange();
            
        }
        
	    void OnDrawGizmos()
	    {
		    //chase radius
		    Gizmos.color = Color.blue;
		    Gizmos.DrawWireSphere (transform.position, chaseRadius);

		    //attack radius
		    Gizmos.color = Color.red;
		    Gizmos.DrawWireSphere (transform.position, currentWeaponRange);
	    }
    }
}