using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{ 
    [ExecuteInEditMode]
    public class WeaponPickupPoint : MonoBehaviour
    {
        [SerializeField] WeaponConfig weaponConfig;
        [SerializeField] AudioClip pickUpSFX;

        AudioSource audioSource = null;

	    void Start ()
        {
            audioSource = GetComponent<AudioSource>();
	    }

        void Update ()
        {
            if(!Application.isPlaying)
            { 
                DestroyChildren();
                InstantiateWeapon();
            }
        }

        private void DestroyChildren()
        {
            foreach(Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        void InstantiateWeapon()
        {
            var weapon = weaponConfig.GetWeaponPrefab();
            weapon.transform.position = Vector3.zero;
            Instantiate(weapon, gameObject.transform);
        }

        void OnTriggerEnter(Collider other)
        {
            FindObjectOfType<PlayerControl>().GetComponent<WeaponSystem>().PutWeaponInHand(weaponConfig);
            audioSource.PlayOneShot(pickUpSFX);
        }
    }
}