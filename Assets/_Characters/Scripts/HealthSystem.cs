using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Characters
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] Image healthBar;
        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;
        [SerializeField] float enemyVanishesAfterSeconds = 2.0f;

        const string DEATH_TRIGGER = "Death";

        float currentHealthPoints;
        Animator animator;
        AudioSource audioSource;
        Character characterMovement;

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        void Start()
        {
            animator = GetComponent<Animator>();   
            audioSource = GetComponent<AudioSource>();   
            characterMovement= GetComponent<Character>();

            currentHealthPoints = maxHealthPoints;
        }

        void Update()
        {
            UpdateHealthBar();    
        }

        private void UpdateHealthBar()
        {
            if(healthBar) //enemies may not have an healthbar to update
            {
                healthBar.fillAmount = healthAsPercentage;
            }
        }
        
        public void TakeDamage(float changeAmount)
        {
            bool characterDies = (currentHealthPoints - changeAmount <= 0);
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - changeAmount, 0f, maxHealthPoints);
            var clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            audioSource.PlayOneShot(clip);
            if (characterDies)
            {
                StartCoroutine(KillCharacter());
            }
        }
        
        IEnumerator KillCharacter()
        {
            characterMovement.Kill();
            animator.SetTrigger(DEATH_TRIGGER);

            audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
            audioSource.Play();
            yield return new WaitForSecondsRealtime(audioSource.clip.length);

            var playerComponent = GetComponent<PlayerControl>();
            if(playerComponent && playerComponent.isActiveAndEnabled)
            {
                SceneManager.LoadScene(0);
            }
            else // for now we assume these are enemies, but NPCs are in this 'else' as well 
            {
                Destroy(gameObject, enemyVanishesAfterSeconds);
            }
        }

        public void Heal(float healAmountInPoints)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + healAmountInPoints, 0f, maxHealthPoints);
        }
    }
}