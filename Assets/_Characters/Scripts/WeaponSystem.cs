using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace RPG.Characters
{ 
    public class WeaponSystem : MonoBehaviour {
        
        [SerializeField] float baseDamage = 10f;
        [SerializeField] WeaponConfig currentWeaponConfig;
        
        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        GameObject target;
        GameObject weaponObject;
        Animator animator;
        Character character;

        float lastHitTime = 0;

        void Start()
        {
            animator = GetComponent<Animator> ();
            character = GetComponent<Character>();

            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();
        }

        void Update()
        {
            bool isTargetDead;
            bool isTargetOutOfRange;

            if(target == null)
            {
                isTargetDead = false;
                isTargetOutOfRange = false;
            }
            else
            {
                float targetHealth = target.GetComponent<HealthSystem>().healthAsPercentage;
                isTargetDead = targetHealth <= Mathf.Epsilon;

                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                isTargetOutOfRange  = distanceToTarget > currentWeaponConfig.GetAttackRange();
            }

            float characterHealth = GetComponent<HealthSystem>().healthAsPercentage;
            bool isCharacterDead = (characterHealth <= Mathf.Epsilon);

            if(isCharacterDead || isTargetOutOfRange || isTargetDead)
            {
                StopAllCoroutines();
            }
        }

        public void PutWeaponInHand(WeaponConfig weaponConfig)
        {
            currentWeaponConfig = weaponConfig;
            var weaponPrefab = weaponConfig.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            Destroy(weaponObject); //empty hands
            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        }

        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;
            StartCoroutine(AttackTargetRepeatedly());
        }

        IEnumerator AttackTargetRepeatedly()
        {
            bool isAttackerAlive = GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
            bool isTargetAlive = target.GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;

            while(isAttackerAlive && isTargetAlive)
            {
                float timeBetweenAttacksInSeconds = currentWeaponConfig.GetMinTimeBetweenHitsInSeconds();
                float timeToWait = timeBetweenAttacksInSeconds * character.GetAnimationSpeedMultiplier();

                bool isTimeToAttackAgain = Time.time - lastHitTime > timeToWait;
                if(isTimeToAttackAgain)
                {
                    AttackTargetOnce();
                    lastHitTime = Time.time;
                }
                yield return new WaitForSeconds(timeToWait);
            }
        }

        private void AttackTargetOnce()
        {
            transform.LookAt(target.transform);
            animator.SetTrigger(ATTACK_TRIGGER);
            float damageDelay = 1f; // todo get from weapon
            SetAttackAnimation();
            StartCoroutine(DamageAfterDelay(damageDelay));
        }

        IEnumerator DamageAfterDelay(float delayInSeconds)
        {
            yield return new WaitForSecondsRealtime(delayInSeconds);
            target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
        }

        private float CalculateDamage()
        {
            return baseDamage + currentWeaponConfig.GetAdditionalDamage();
        }

        private void SetAttackAnimation()
        {
            if(!character.GetOverrideController())
            {
                Debug.Break();
                Debug.LogAssertion("Provide " + gameObject + " with an animator override controller.");
            }
            else
            {
                var animatorOverrideController = character.GetOverrideController();
                animator.runtimeAnimatorController = animatorOverrideController;
                animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAnimClip();
            }
        }

        private GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantsHands = dominantHands.Length;
            Assert.AreNotEqual(numberOfDominantsHands, 0, "No Dominant Hands Found");
            Assert.IsFalse(numberOfDominantsHands > 1, "Multiple dominant hand scripts on player, remove one");
            return dominantHands[0].gameObject;
        }

    }
}
