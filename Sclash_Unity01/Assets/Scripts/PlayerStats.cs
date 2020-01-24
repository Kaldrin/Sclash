using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    # region MANAGERS
    // MANAGERS
    [Header("MANAGERS")]
    // Audio manager
    [Tooltip("The name of the object holding the AudioManager script component in the scene to find its reference")]
    [SerializeField] string audioManagerName = "GlobalManager";
    AudioManager audioManager;

    // Game manager
    [Tooltip("The name of the object holding the GameManager script component in the scene to find its reference")]
    [SerializeField] string gameManagerName = "GlobalManager";
    GameManager gameManager;
    # endregion




    # region PLAYER'S COMPONENTS
    // PLAYER'S COMPONENTS
    [Header("PLAYER'S COMPONENTS")]
    [Tooltip("The reference to the PlayerAttack script component attached the player")]
    [SerializeField] PlayerAttack playerAttack;
    [Tooltip("The reference to the PlayerAnimations script component attached the player")]
    [SerializeField] PlayerAnimations playerAnimation;
    [Tooltip("The reference to the PlayerMovement script component attached the player")]
    [SerializeField] PlayerMovement playerMovements;
    [Tooltip("The reference to the Rigidbody2D component attached the player")]
    [SerializeField] Rigidbody2D rigid;
    # endregion




    # region HEALTH
    // HEALTH
    [Header("HEALTH")]
    [Tooltip("The maximum health of the player")]
    [SerializeField] float maxHealth = 1;
    float currentHealth;

    [Tooltip("Can the player be hit in the current frames ?")]
    [SerializeField] public bool untouchable = false;
    [HideInInspector] public bool dead = false;
    # endregion




    # region STAMINA
    //STAMINA
    [Header("STAMINA")]
    [Tooltip("The reference to the base stamina slider attached to the player to create the other sliders")]
    [SerializeField] public Slider staminaSlider = null;
    List<Slider> staminaSliders = new List<Slider>();

    [Tooltip("The amount of stamina each move will cost when executed")]
    [SerializeField] public float staminaCostForMoves = 1;
    [Tooltip("The maximum amount of stamina one player can have")]
    [SerializeField] public float maxStamina = 4f;
    [Tooltip("Stamina parameters")]
    [SerializeField] float
        durationBeforeStaminaRegen = 0.8f,
        staminaGainOverTimeMultiplier = 0.4f,
        idleStaminaGainOverTimeMultiplier = 0.8f,
        backWalkingStaminaGainOverTime = 0.4f,
        staminaBarBaseOpacity = 0.8f;
    [HideInInspector] public float stamina = 0;
    float
        currentTimeBeforeStaminaRegen = 0,
        staminaBarsOpacity = 1,
        oldStamina = 0;

    [HideInInspector] public bool canRegenStamina = true;

    [Tooltip("Stamina colors depending on how much there is left")]
    [SerializeField] Color
        staminaBaseColor = Color.green,
        staminaLowColor = Color.yellow,
        staminaDeadColor = Color.red;
    # endregion




    # region FX
    // FX
    [Header("FX")]
    [Tooltip("The reference to the game object holding the stamina loss FX")]
    [SerializeField] GameObject staminaLossFX = null;
    [Tooltip("The references to the game objects holding the different FXs")]
    [SerializeField] GameObject
        clashFXPrefabRef = null,
        staminaGainFX = null,
        deathBloodFX = null;

    [Tooltip("The amount to rotate the death blood FX's object because for some reason it takes another rotation when it plays :/")]
    [SerializeField] float deathBloodFXRotationForDirectionChange = 240;

    Vector3 deathFXbaseAngles = new Vector3(0, 0, 0);
    # endregion




    # region PLAYERS IDENTIFICATION
    // PLAYERS IDENTIFICATION
    [HideInInspector] public int playerNum = 0;
    int otherPlayerNum = 0;
    # endregion




    # region SOUND
    // SOUND
    [Header("SOUND")]
    [Tooltip("The reference to the stamina charged audio FX AudioSource")]
    [SerializeField] AudioSource staminaBarChargedAudioEffectSource = null;
    # endregion



















    /*
    # region BASE FUNCTIONS
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
        // staminaSlider.maxValue = maxStamina;

        SetUpStaminaBars();

        // Begin by reseting all the player's values and variable to start fresh

        deathFXbaseAngles = deathBloodFX.transform.localEulerAngles;
        ResetValues();
    }

    void Start()
    {
        StartCoroutine(GetOtherPlayerNum());
    }

    // FixedUpdate is called 50 times per second
    void FixedUpdate()
    {
        if (!gameManager.paused)
            StaminaRegen();

        if (gameManager.gameStarted && !playerAttack.enemyDead && !dead)
            StaminaBarsOpacity(staminaBarsOpacity);
        else
            StaminaBarsOpacity(0);


        if (transform.localScale.x < 0)
            deathBloodFX.transform.localEulerAngles = new Vector3(deathFXbaseAngles.x, deathFXbaseAngles.y, deathBloodFXRotationForDirectionChange);
        else
            deathBloodFX.transform.localEulerAngles = new Vector3(deathFXbaseAngles.x, deathFXbaseAngles.y, deathFXbaseAngles.z);


        if (!dead)
            UpdateStaminaSlidersValue();


        UpdateStaminaColor();
    }

    // LateUpdate is called last at each frame
    void LateUpdate()
    {
    }
    # endregion


    IEnumerator GetOtherPlayerNum()
    {
        yield return new WaitForSeconds(0.2f);


        for (int i = 0; i < gameManager.playersList.Count; i++)
        {
            if (i + 1 != playerNum)
            {
                otherPlayerNum = i + 1;
            }
        }
    }








    # region STAMINA
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

        staminaLossFX.SetActive(false);
        staminaLossFX.SetActive(true);
        staminaLossFX.GetComponent<ParticleSystem>().Play();

        if (stamina <= 0)
        {
            stamina = 0;
        }
    }

    // Update stamina slider value
    void UpdateStaminaSlidersValue()
    {
        // DETECT STAMINA CHARGE UP
        if (Mathf.FloorToInt(oldStamina) < Mathf.FloorToInt(stamina))
        {
            if (!gameManager.playerDead && gameManager.gameStarted)
            {
                staminaBarChargedAudioEffectSource.Play();

                staminaGainFX.SetActive(false);
                staminaGainFX.SetActive(true);
                staminaGainFX.GetComponent<ParticleSystem>().Play();
            }
                
        }


        oldStamina = stamina;


        staminaSliders[0].value = Mathf.Clamp(stamina, 0, 1);
        staminaLossFX.gameObject.transform.position = staminaSliders[(int)Mathf.Clamp((int)(stamina + 0.5f), 0, maxStamina - 1)].transform.position;
        staminaGainFX.gameObject.transform.position = staminaSliders[(int)Mathf.Clamp((int)(stamina - 0.5f), 0, maxStamina - 1)].transform.position + new Vector3(0.2f, 0, 0) * Mathf.Sign(transform.localScale.x);


        for (int i = 1; i < staminaSliders.Count; i++)
        {
            staminaSliders[i].value = Mathf.Clamp(stamina, i, i + 1) - i;
        }
        

        if (stamina >= maxStamina)
        {
            if (staminaBarsOpacity > 0)
                staminaBarsOpacity -= 0.05f;
        }
        else if (staminaBarsOpacity != staminaBarBaseOpacity)
        {
            staminaBarsOpacity = staminaBarBaseOpacity;
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

    void UpdateStaminaColor()
    {
        if (stamina < staminaCostForMoves)
        {
            SetStaminaColor(staminaDeadColor);
        }
        else if (stamina < staminaCostForMoves * 2)
        {
            SetStaminaColor(staminaLowColor);
        }
        else
        {
            SetStaminaColor(staminaBaseColor);
        }
    }

    void SetStaminaColor(Color color)
    {
        for (int i = 0; i < staminaSliders.Count; i++)
        {
            staminaSliders[i].fillRect.gameObject.GetComponent<Image>().color = Color.Lerp(staminaSliders[i].fillRect.gameObject.GetComponent<Image>().color, color, Time.deltaTime * 10);
        }
    }
    # endregion








    # region RESET VALUES
    // RESET VALUES
    public void ResetValues()
    {
        currentHealth = maxHealth;
        stamina = maxStamina;
        staminaSlider.gameObject.SetActive(true);
        dead = false;
        canRegenStamina = true;

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
        for (int i = 0; i < playerAttack.playerColliders.Length; i++)
        {
            playerAttack.playerColliders[i].isTrigger = false;
        }

        rigid.gravityScale = 1;
        rigid.simulated = true;

        playerAnimation.CancelCharge();
        playerAnimation.ResetAnims();

        playerMovements.Charging(false);
    }
    # endregion








    # region RECEIVE AN ATTACK
    // RECEIVE AN ATTACK
    public bool TakeDamage(GameObject instigator, int hitStrength = 1)
    {
        bool hit = false;


        if (playerAttack.kicking)
        {
            playerAttack.StopAllCoroutines();
        }


        if (!dead)
        { 
            // CLASH
            if (playerAttack.activeFrame || playerAttack.clashFrames)
            {
                playerAttack.Clash();
                instigator.GetComponent<PlayerAttack>().Clash();


                // FX
                Vector3 fxPos = new Vector3((gameManager.playersList[0].transform.position.x + gameManager.playersList[1].transform.position.x) / 2, playerAttack.clash.transform.position.y, playerAttack.clash.transform.position.z);
                Instantiate(clashFXPrefabRef, fxPos, playerAttack.clash.transform.rotation, null).GetComponent<ParticleSystem>().Play();
            }
            // PARRY
            else if (playerAttack.parrying)
            {
                stamina += staminaCostForMoves;
                instigator.GetComponent<PlayerAttack>().Clash();


                // FX
                playerAttack.clash.GetComponent<ParticleSystem>().Play();


                // Sound
                audioManager.Parried();
            }
            // UNTOUCHABLE FRAMES
            else if (untouchable)
            {
                audioManager.BattleEventIncreaseIntensity();
                gameManager.SlowMo(gameManager.clashSlowMoDuration, gameManager.clashSlowMoTimeScale, gameManager.clashTimeScaleFadeSpeed);
            }
            // TOUCHED
            else
            {
                hit = true;
                Touched();
                audioManager.BattleEventIncreaseIntensity();
            }


            // IS DEAD ?
            if (currentHealth <= 0 && !dead)
            {
                gameManager.Death(instigator.GetComponent<PlayerStats>().playerNum);
                playerAnimation.Dead();
                dead = true;
                playerAttack.chargeFX.SetActive(false);
                

                StaminaBarsOpacity(0);
            }
        }

        return hit;
    }
    # endregion








    # region TOUCHED
    // TOUCHED
    void Touched()
    {
        currentHealth -= 1;

        // Sound
        audioManager.SuccessfulAttack();
        audioManager.BattleEventIncreaseIntensity();


        playerAttack.playerCollider.isTrigger = true;


        for (int i = 0; i < playerAttack.playerColliders.Length; i++)
        {
            playerAttack.playerColliders[i].isTrigger = true;
        }

        //Debug.Log(gameManager.score[otherPlayerNum]);
        Debug.Log(playerNum);
        if (gameManager.score[otherPlayerNum - 1] + 1 >= gameManager.scoreToWin)
            gameManager.StartDeathVFXCoroutine();


        gameManager.cameraShake.shakeDuration = gameManager.deathCameraShakeDuration;
        gameManager.SlowMo(gameManager.rounEndSlowMoDuration, gameManager.roundEndSlowMoTimeScale, gameManager.roundEndTimeScaleFadeSpeed);
    }
    # endregion
*/
}




