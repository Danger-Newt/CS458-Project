using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dummyAI : MonoBehaviour
{
    public float health;    //HP of enemy
    public int iFrames;     //How long the enemy is immune
    public int enemyState;  //What the enemy is doing
    //State = 1 -> Enemy is idle
    //State = 2 -> Enemy is attacking
    //State = 3 -> Enemy is blocking
    //State = 4 -> Enemy is stunned
    public float reactionTime;      //Time from idle to block
    public enum fightStyle{        //Fight style of enemy
        FourDirStrikes,
        EightDirStrikes,
        FigureEight
    };
    public fightStyle enemyStyle;
    public float attackCooldown = 600.0f;   //Cooldown for each state change
    
    //This should later call a child object
    public GameObject swordHand;
    public GameObject offHand;
    public GameObject player;       //Player character
    int reactionCounter = 0;        //Time left from reaction to block state change
    bool prepareToBlock = false;    //If the reaction counter has been triggered
    float blockSpeed = 1.0f;        //Speed of block movement
    float attackTimer = 0.0f;       //Time left from idle to attack state change
    int attackStateTimer = 0;       //Time to hold attack state
    int stunTimer = 120;            //Time to stay stunned
    Vector3 defHandPos;             //Default hand position (for calculating block)

    Animator swordAnim;
    Animator shieldAnim;
    void Start(){
        //Get default hand position
        defHandPos = swordHand.transform.position;
        //Get animators for enemy hands
        swordAnim = swordHand.GetComponent<Animator>();
        shieldAnim = offHand.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get animators for enemy hands
        Animator swordAnim = swordHand.GetComponent<Animator>();
        Animator shieldAnim = offHand.GetComponent<Animator>();
        Debug.Log("Enemy State - " + enemyState);   //Show state for debugging

        //Check HP stat and die if it is less than zero
        if (health <= 0.0f){
            //Do a proper die sequence later.  For now, just destroy
            //Destroy(this.gameObject);
        }

        //If immune, reduce iFrames
        if (iFrames > 0){
            iFrames --;
        }

        if (enemyState == 4){   //If stunned (highest priority state)
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
                shieldAnim.SetTrigger("endBlock");
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
                shieldAnim.SetTrigger("startBlock");    //Engage block animation
                attackTimer = 0;        //Reset attack timer
            }

            if (attackTimer >= attackCooldown){     //Time to attack
                enemyState = 2;         //Change state
                reactionCounter = 0;    //Reset block reaction
                attackTimer = 0;

                //Attack in a random direction
                int attackDir = Random.Range(1, 4);
                switch (attackDir){
                    case 1: swordAnim.SetTrigger("SwingDown"); break;    //Down Swing
                    case 2: swordAnim.SetTrigger("SwingLeft"); break;    //Left Sweep
                    case 3: swordAnim.SetTrigger("SwingRight"); break;   //Right Sweep
                    //case x: anim.SetTrigger("SwingDir"); break;   //Attack in Dir
                }
            } 
            else if (enemyState == 1){          //Progress attack timer
                attackTimer += 1.0f;
            }

        }
    
    }
}
