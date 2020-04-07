using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vikingAI : MonoBehaviour
{
    public float health;    //HP of enemy
    public int iFrames;     //How long the enemy is immune
    public int enemyState;  //What the enemy is doing
    //State = 0 -> Enemy is moving towards player
    //State = 1 -> Enemy is idle
    //State = 2 -> Enemy is attacking
    //State = 3 -> Enemy is blocking
    //State = 4 -> Enemy is stunned
    //State = 5 -> Enemy is dead or dying
    public float reactionTime;      //Time from idle to block

    public int spawnDir;            //Spawn direction

    public enum fightStyle{         //Fight style of enemy
        FourDirStrikes,
        EightDirStrikes,
        FigureEight
    };
    public fightStyle enemyStyle;
    public float attackCooldown = 600.0f;   //Cooldown for each state change
    
    //This should later call a child object
    public Animator animController; //Enemy Animation Controller
    public GameObject player;       //Player character
    public GameObject gmObj;        //Game state controller object

    private int reactionCounter = 0;        //Time left from reaction to block state change
    private bool prepareToBlock = false;    //If the reaction counter has been triggered
    private float blockSpeed = 1.0f;        //Speed of block movement
    private float attackTimer = 0.0f;       //Time left from idle to attack state change
    private int attackStateTimer = 0;       //Time to hold attack state
    private int stunTimer = 120;            //Time to stay stunned
    private int stepCounter = 360;          //Time to move from spawn to player
    private gameMaster gmScript;             //Game master script

    void Start(){
        enemyState = 0; //Always start by running towards the player
        gmScript = gmObj.GetComponent<gameMaster>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get animators for enemy hands
        Debug.Log("Enemy State - " + enemyState);   //Show state for debugging

        //Check HP stat and die if it is less than zero
        if ((health <= 0.0f) && (enemyState != 5)){
            //Do the die function
            StartCoroutine(Die());
            enemyState = 5;
        }

        //If immune, reduce iFrames
        if (iFrames > 0){
            iFrames --;
        }

        if (enemyState == 0){   //If running, don't start main AI loop yet
            //Move enemy forward towards the player
            transform.position += transform.forward * Time.deltaTime * 5.0f;
            stepCounter --; //Decrement steps left to take
            if (stepCounter <= 0){
                //If out of steps, stop running and enter idle state
                enemyState = 1;
                animController.SetBool("isRunning", false);
            }
        }
        else if (enemyState == 5){   //If dead or dying (highest priority)
            //Do nothing.  The Die coroutine handles it all
        }
        else if (enemyState == 4){   //If stunned
            if (stunTimer > 0){
                stunTimer --;  
            }else{
                stunTimer = 120;    //Reset timer
                enemyState = 1;
                attackStateTimer = 200;   //Reset the interrupted attack timer
            }
        }

        else if (enemyState == 2){  //If attacking (next highest priority state)
            if (attackStateTimer > 0){
                attackStateTimer --;
            }else{
                attackStateTimer = 200; //Reset timer
                enemyState = 1;         //Change to idle state
            }
        }

        else if (enemyState == 3){  //If blocking (next highest priority state)
            //If player is still swinging, add some block time
            if (player.GetComponent<playerSwordCollide>().swingState == 1){
                reactionCounter++;
            }
            //Count down the block timer
            if (reactionCounter > 0){
                reactionCounter --;
            }else{
                //If block counter is done, end the block
                reactionCounter = 0;
                enemyState = 1;
                //Stop block animation
                animController.SetTrigger("EndBlock");
            }
        }

        else{   //If idle (lowest priority state)
            //If player is swinging, or preparing to block, prepare to block
            if (player.GetComponent<playerSwordCollide>().swingState == 1){
                prepareToBlock = true;
            }

            if (prepareToBlock){    //Reaction time counter
                reactionCounter ++;
            }

            if (reactionCounter >= reactionTime){   //Enter block state
                enemyState = 3;         //Change state
                prepareToBlock = false; //Start blocking
                animController.SetTrigger("StartBlock");    //Engage block animation
                attackTimer = 0;        //Reset attack timer
            }

            if (attackTimer >= attackCooldown){     //Time to attack
                enemyState = 2;         //Change state
                reactionCounter = 0;    //Reset block reaction
                attackTimer = 0;

                //Attack in a random direction
                int attackDir = Random.Range(1, 4);
                switch (attackDir){
                    case 1: animController.SetTrigger("SwingDown"); break;    //Down Swing
                    case 2: animController.SetTrigger("SwingLeft"); break;    //Left Sweep
                    case 3: animController.SetTrigger("SwingRight"); break;   //Right Sweep
                    //case x: anim.SetTrigger("SwingDir"); break;   //Attack in Dir
                }
            } 
            else if (enemyState == 1){          //Progress attack timer
                attackTimer += 1.0f;
            }

        }
    
    }

    IEnumerator Die(){
             var notDying = true;
             while (true) {
                if (notDying){
                    animController.SetTrigger("Die");
                    notDying = false;
                }
                yield return new WaitForSeconds(2.08f);
                gmScript.enemiesKilled ++;  //Inc killed enemies in GM script
                gmScript.enemiesAlive --;   //Dec enemies alive in GM script
                gmScript.freeSpaces[spawnDir] = true;   //Open path for new enemy
                Destroy(this.gameObject);
             }
         }
}

