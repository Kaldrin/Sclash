using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{

    // AUDIO MANAGER
    [Header("Audio manager")]
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager;





    // GAME MANAGER
    [Header("Game manager")]
    [SerializeField] string gameManagerName = "GlobalManager";
    GameManager gameManager;






    // PLAYER'S COMPONENTS
    [Header("Player's components")]
    PlayerAttack playerAttack;
    PlayerAnimations playerAnimation;
    PlayerMovement playerMovements;
    Rigidbody2D rigid;





    // HEALTH
    [Header("Health")]
    [SerializeField] float maxHealth = 0;
    float currentHealth;

    [SerializeField] public bool untouchable = false;
    [HideInInspector] public bool dead = false;





    //STAMINA
    [Header("Stamina")]
    [SerializeField] public Slider staminaSlider = null;
    List<Slider> staminaSliders = new List<Slider>();

    [SerializeField] public float staminaCostForMoves = 1;
    [SerializeField] float
        maxStamina = 3f,
        durationBeforeStaminaRegen = 0.2f,
        staminaGainOverTimeMultiplier = 0.1f,
        idleStaminaGainOverTimeMultiplier = 0.5f,
        backWalkingStaminaGainOverTime = 0.5f;
    [HideInInspector] public float stamina = 0;
    float
        currentTimeBeforeStaminaRegen = 0,
        staminaBarsOpacity = 1;

    bool canRegenStamina = true,
        staminaFull = false;





    // PLAYER IDENTIFICATION
    [HideInInspector] public int playerNum;













    // BASE FUNCTIONS
    void Awake()
    {
        // Get audio manager to use in the script
        audioManager = GameObject.Find(audioManagerName).GetComponent<AudioManager>();



        // Get game manager to use in the script
        gameManager = GameObject.Find(gameManagerName).GetComponent<GameManager>();




        // Get player's components to use in the script
        rigid = GetComponent<Rigidbody2D>();
        playerAttack = GetComponent<PlayerAttack>();
        playerAnimation = GetComponent<PlayerAnimations>();
        playerMovements = GetComponent<PlayerMovement>();



        // Set the stamina slider's max value to the stamina max value
        //staminaSlider.maxValue = maxStamina;

        SetUpStaminaBars();

        // Begin by reseting all the player's values and variable to start fresh

        ResetValues();
    }
    
    // FixedUpdate is called 30 times per second
    void FixedUpdate()
    {
        if (!gameManager.paused)
            StaminaRegen();

        if (gameManager.gameStarted && !playerAttack.enemyDead)
            StaminaBarsOpacity(staminaBarsOpacity);
        else
            StaminaBarsOpacity(0);
    }

    void LateUpdate()
    {
        UpdateStaminaSlider();
    }







    // STAMINA
    // Manage stamina regeneration, executed in FixedUpdate
    void StaminaRegen()
    {
        if (stamina < maxStamina && canRegenStamina)
        {
            // If back walking
            if (rigid.velocity.x * - transform.localScale.x < 0 )
            {
                stamina += Time.deltaTime * backWalkingStaminaGainOverTime;
            }
            else if (Mathf.Abs(rigid.velocity.x) <= 0.5f)
            {
                stamina += Time.deltaTime * idleStaminaGainOverTimeMultiplier;
            }
            else
            {
                stamina += Time.deltaTime * staminaGainOverTimeMultiplier;
                
            }
        }

        // Small duration before the player can regen stamina again after a move
        if (currentTimeBeforeStaminaRegen <= 0 && !canRegenStamina)
        {
            currentTimeBeforeStaminaRegen = 0;
            canRegenStamina = true;
        }
        else if (!canRegenStamina)
        {
            currentTimeBeforeStaminaRegen -= Time.deltaTime;
        }

        /*
        // If the player recovered at least half of his stamina he can parry again
        if (stamina >= maxStamina / 2)
        {
            parryBroke = false;
        }
        */
    }

    // Trigger the stamina regen pause duration
    public void PauseStaminaRegen()
    {
        canRegenStamina = false;
        currentTimeBeforeStaminaRegen = durationBeforeStaminaRegen;
    }

    // Function to decrement to stamina
    public void StaminaCost(float cost)
    {
        stamina -= cost;
        PauseStaminaRegen();

        if (stamina <= 0)
        {
            stamina = 0;
        }
    }

    // Update stamina slider value
    void UpdateStaminaSlider()
    {
        staminaSliders[0].value = Mathf.Clamp(stamina, 0, 1);


        for (int i = 1; i < staminaSliders.Count; i++)
        {
            staminaSliders[i].value = Mathf.Clamp(stamina, i, i +1) - i;
        }
        
        if (stamina >= maxStamina)
        {
            staminaFull = true;
            if (staminaBarsOpacity > 0)
                staminaBarsOpacity -= 0.01f;
        }
        else
        {
            staminaFull = false;
            staminaBarsOpacity = 1;
        }
    }

    // Set up stamina bar system
    void SetUpStaminaBars()
    {
        staminaSliders.Add(staminaSlider);

        for (int i = 0; i < maxStamina - 1; i++)
        {
            staminaSliders.Add(Instantiate(staminaSlider.gameObject, staminaSlider.transform.parent).GetComponent<Slider>());
        }
    }

    // Manages stamina bars opacity
    void StaminaBarsOpacity(float opacity)
    {
        for (int i = 0; i < staminaSliders.Count; i++)
        {
            Color
                fillColor = staminaSliders[i].fillRect.GetComponent<Image>().color,
                backgroundColor = staminaSliders[i].transform.GetChild(0).GetComponent<Image>().color;
            staminaSliders[i].fillRect.GetComponent<Image>().color = new Color(fillColor.r, fillColor.g, fillColor.b, opacity);
            staminaSliders[i].transform.GetChild(0).GetComponent<Image>().color = new Color(backgroundColor.r, backgroundColor.g, backgroundColor.b, opacity);
        }
    }






    // RESET VALUES
    public void ResetValues()
    {
        currentHealth = maxHealth;
        stamina = maxStamina;
        staminaSlider.gameObject.SetActive(true);
        dead = false;

        playerAttack.parrying = false;
        playerAttack.isAttacking = false;
        playerAttack.isAttackRecovering = false;
        playerAttack.kicking = false;
        playerAttack.canKick = true;
        playerAttack.canCharge = true;
        playerAttack.canParry = true;
        playerAttack.canKick = true;
        playerAttack.enemyDead = false;
        playerAttack.playerCollider.isTrigger = false;

        rigid.gravityScale = 1;
        rigid.simulated = true;

        playerAnimation.CancelCharge();
        playerAnimation.ResetAnims();

        playerMovements.Charging(false);
    }









    // RECEIVE AN ATTACK
    public bool TakeDamage(GameObject instigator, int hitStrength = 1)
    {
        bool hit = false;


        if (!dead)
        { 
            // CLASH
            if (playerAttack.activeFrame || playerAttack.clashFrames)
            {
                playerAttack.Clash();
                instigator.GetComponent<PlayerAttack>().Clash();
            }
            // PARRY
            else if (playerAttack.parrying)
            {
                stamina += staminaCostForMoves;
                instigator.GetComponent<PlayerAttack>().Clash();

                // Sound
                audioManager.Parried();
            }
            // UNTOUCHABLE FRAMES
            else if (untouchable)
            {

            }
            // TOUCHED
            else
            {
                hit = true;
                Touched();
            }


            // IS DEAD ?
            if (currentHealth <= 0 && !dead)
            {
                gameManager.Death(instigator.GetComponent<PlayerStats>().playerNum);
                playerAnimation.Dead();
                dead = true;
            }
        }

        return hit;
    }









    // TOUCHED
    void Touched()
    {
        currentHealth -= 1;

        // Sound
        audioManager.SuccessfulAttack();


        playerAttack.playerCollider.isTrigger = true;
        gameManager.SlowMo(gameManager.rounEndSlowMoDuration, gameManager.roundEndSlowMoTimeScale, gameManager.roundEndTimeScaleFadeSpeed);
    }
}




