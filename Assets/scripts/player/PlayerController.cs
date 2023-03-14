using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    

    [Header("Max Values")]
    public int maxHp;
    public int maxJumps;
    public float move_speed;
    public float slow_time;
    public float max_chAttkDmg;



    [Header("Cur Values")]
    public int curHp;
    public int curJumps;
    public int score;
    public float curMoveInput;
    public bool isSlowed;
    public float time_hit;
    public bool isCharging;

    [Header("Mods")]
    public float cur_speed;
    public float jump_force;

    [Header("Audio Clips")]
    //jump 0
    //hit ground 1
    //taunt giggle 2
    //death sound 3
    //shoot snd 4

    public AudioClip[] playerfx;


    [Header("Attacking")]
    [SerializeField]
    private PlayerController curAttacker;
    public float attackRate;
    public float lastAttackTime;
    public float attackSpeed;
    public float attackDmg;
    public float chAttkDmg;
    public float chargeRate;
    public GameObject[] attackPrefabs;


    



    [Header("Components")]
    [SerializeField]
    private Rigidbody2D rig;
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private AudioSource audio;
    [SerializeField]
    private Transform muzzle;
    public PlayerContainerUI uiContainer;
    private GameManager gameManager;
    public GameObject deathEfectprefab;


    //unity life cycle methods
    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        muzzle = GetComponentInChildren<Muzzle>().GetComponent<Transform>();
      
    }
    // Start is called before the first frame update
    void Start()
    {
        curHp = maxHp;
        cur_speed = move_speed;
        curJumps = maxJumps;
        score = 0;
        uiContainer.updateHealthBar(curHp, maxHp);
    }
    private void FixedUpdate()
    {
        move();
    }

    // Update is called once per frame
    void Update()
    {
        // kill player if they fall off the screen or their health is 0 or below
        if(transform.position.y < -10||curHp <= 0)
        {
            die();
        }

        if (isSlowed)
        {
            if(Time.time - time_hit > slow_time)
            {
                isSlowed= false;
                cur_speed= move_speed;
            }
        }
        if (isCharging)
        {
            
            chAttkDmg += chargeRate;
            if (chAttkDmg > max_chAttkDmg)
            {
                chAttkDmg= max_chAttkDmg;
            }
            uiContainer.updateChargeBar(chAttkDmg, max_chAttkDmg);
        }
        else
        {
            uiContainer.updateChargeBar(chAttkDmg, max_chAttkDmg);
        }

        
       
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        //reseting curJumps when I hit the ground
        foreach(ContactPoint2D x  in collision.contacts){
            if (x.collider.CompareTag("Ground"))
            {
                if(x.point.y < transform.position.y)
                {
                    audio.PlayOneShot(playerfx[1]);
                    curJumps = maxJumps;

                }
                // add an extra jump if i hit the side of a wall 
                if((x.point.x > transform.position.x || x.point.x < transform.position.x) && (x.point.y < transform.position.y))
                {
                    if(curJumps < maxJumps)
                    {
                        curJumps++;
                    }
                }
            }
        }
  
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        
        
    }





    //action methods below

    private void jump()
    {
        //set cur y velocity to 0
        rig.velocity = new Vector2(rig.velocity.x,0 );
        //play sound
        audio.PlayOneShot(playerfx[0]);
        //add force up
        rig.AddForce(Vector2.up*jump_force, ForceMode2D.Impulse);
    }
     
    private void move()
    {
        //sets the velocity on rig x to what ever the cur move input is and mutiply by move speed
        rig.velocity = new Vector2(curMoveInput*cur_speed,rig.velocity.y);

        // flip player in the correct direction
        if(curMoveInput != 0.0f)
        {
            transform.localScale = new Vector3(curMoveInput > 0 ? 1 : -1, 1, 1);
        }
    }

    public void die()
    {
        // add partical efect to our death
        Destroy(Instantiate(deathEfectprefab, transform.position, Quaternion.identity),1f);
        //play death sound
        audio.PlayOneShot(playerfx[3]);
        

        // stop any movment 
        rig.velocity = Vector2.zero;
        // check for a curent attacker 
        if(curAttacker != null)
        {
            // rewoard the player who killed us
            curAttacker.addScore();
        }
        // if we kill self take away score
        else
        {
            score--;
            if (score < 0)
            {
                score = 0;
            }
            uiContainer.updateScoreText(score);

        }
        // respawn
        respawn();
    }
    public void drop_out()
    {
        

        Destroy(uiContainer.gameObject);
        Destroy(gameObject);
    }

    public void addScore()
    {
        // ups our score when we get a kill
        score++;
        uiContainer.updateScoreText(score);

    }

    public void takeDamage(int ammount,PlayerController attacker)
    {
        // handels us taking dmg
        curHp -= ammount;
        // need to set the curent attacker
        curAttacker= attacker;
        chAttkDmg = 0;
        if (isCharging)
        {
            chAttkDmg = chAttkDmg / 2;
            uiContainer.updateChargeBar(chAttkDmg, max_chAttkDmg);

        }
        uiContainer.updateHealthBar(curHp, maxHp);
    }
    //over load method to take float 
    public void takeDamage(float ammount, PlayerController attacker)
    {
        // handels us taking dmg
        curHp -= (int)ammount;
        // need to set the curent attacker
        curAttacker = attacker;
        chAttkDmg = 0;
        if (isCharging)
        {
            chAttkDmg = chAttkDmg / 2;
            uiContainer.updateChargeBar(chAttkDmg, max_chAttkDmg);

        }
        uiContainer.updateHealthBar(curHp, maxHp);
    }
    public void takeIceDamage(float ammount, PlayerController attacker)
    {
        time_hit = Time.time;
        // handels us taking dmg
        curHp -= (int)ammount;
        // need to set the curent attacker
        curAttacker = attacker;
        isSlowed= true;
        cur_speed /= 2;
        chAttkDmg = 0;
        if (isCharging)
        {
            chAttkDmg = chAttkDmg/2;
            uiContainer.updateChargeBar(chAttkDmg, max_chAttkDmg);

        }
        uiContainer.updateHealthBar(curHp, maxHp);
    }

    private void respawn()
    {
        // Respawns the player at a random spawn point and resets starting values
        curHp = maxHp;
        curJumps = maxJumps;
        cur_speed = move_speed;
        curAttacker = null;
        transform.position = gameManager.spawn_points[Random.Range(0,gameManager.spawn_points.Length)].position;
        uiContainer.updateHealthBar(curHp, maxHp);

    }


    
    // spawning methods
    
    public void spanwnStdFireball()
    {
        audio.PlayOneShot(playerfx[4]);
        GameObject fireball = Instantiate(attackPrefabs[0], muzzle.position, Quaternion.identity);
        fireball.GetComponent<Projectile>().onSpawn( attackDmg, attackSpeed,this,transform.localScale.x);
  


    }

    public void spawnIceAttack()
    {
        audio.PlayOneShot(playerfx[5]);
        GameObject iceBall = Instantiate(attackPrefabs[1], muzzle.position, Quaternion.identity);
        iceBall.GetComponent<Projectile>().onSpawn(attackDmg, attackSpeed, this, transform.localScale.x);
    }

    public void spawnChargAttk()
    {
        audio.PlayOneShot(playerfx[6]);
        GameObject chargeBall = Instantiate(attackPrefabs[2], muzzle.position, Quaternion.identity);
        chargeBall.GetComponent<Projectile>().onSpawn(chAttkDmg, attackSpeed, this, transform.localScale.x);
        chAttkDmg = 1;
    }






    //input system methods below 

    //move input methods
    public void onMoveInput(InputAction.CallbackContext context)
    {
        
        float x = context.ReadValue<float>();
        if (x > 0)
        {
            curMoveInput = 1;
        }
        else if (x < 0)
        {
            curMoveInput = -1;
        }
        else
        {
            curMoveInput = 0;
        }
    }

    //jump input method
    public void onJumpInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed jump button");
            if(curJumps > 0)
            {
                curJumps--;
                jump();
            }
        }
    }

    //block input methods
    public void onBlockInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed block button");
        }
    }

    //attack input methods
    public void onStdAtachInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate)
        {
            lastAttackTime = Time.time;
            spanwnStdFireball();
            if (isCharging)
            {
                chAttkDmg = 0;
                isCharging= false;
                uiContainer.updateChargeBar(chAttkDmg, max_chAttkDmg);
            }
        }
        
    }

    public void onChrAtachInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            isCharging= true;
        }
        if(context.phase == InputActionPhase.Canceled)
        {
            isCharging= false;
            spawnChargAttk();
        }
    }

    public void onIceAtachInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && Time.time - lastAttackTime > attackRate*3)
        {
            lastAttackTime = Time.time;
            spawnIceAttack();
            if (isCharging)
            {
                chAttkDmg = 0;
                isCharging = false;
                uiContainer.updateChargeBar(chAttkDmg, max_chAttkDmg);
            }
        }
    }

    public void setUiContainer(PlayerContainerUI containerUI)
    {
        uiContainer = containerUI;
    }


    // taunt input methods

    public void onTaunt_1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed Taunt_1 button");
            audio.PlayOneShot(playerfx[2]);
        }
    }

    public void onTaunt_2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed Taunt_2 button");
        }
    }

    public void onTaunt_3(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed Taunt_3 button");
        }
    }

    public void onTaunt_4(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed Taunt_4 button");
        }
    }

    //paues input method
    public void onPauseInput(InputAction.CallbackContext context)
    {

        if (context.phase == InputActionPhase.Performed)
        {
            print("pressed pause button");
        }
    }
}
