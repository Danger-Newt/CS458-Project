using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerSwordCollide : MonoBehaviour
{
    private float swingSpeed = 0.0f;    //Swing speed of player sword
    public int swingState = 0;        //State based on swing speed
     //Last position of player sword
    private Vector3 lastPos = new Vector3(0.0f, 0.0f, 0.0f); 

    public float minSwingSpeed = 1.0f;  //Swing speed required to be in swinging state
    public float damageModif = 1.0f;    //Damage multiplier

    public GameObject sparkPrefab;
    public GameObject bloodPrefab;
    public AudioClip swingSound;
    public AudioClip parrySound;

    void Update(){
        //Get swing speed and update swing state
        swingSpeed = Vector3.Distance(transform.position, lastPos);

        if ((swingSpeed >= minSwingSpeed) && (swingState == 0)){
            swingState = 1; //swinging
            AudioSource.PlayClipAtPoint(swingSound, transform.position);
        }
        else if ((swingSpeed < minSwingSpeed) && (swingState == 1)){
            swingState = 0; //not swinging
        }

        lastPos = transform.position;   //Update last position
    }

    void OnCollisionEnter(Collision colli){
        //Get point of collision (this is copied from unity's docs)
        ContactPoint contact = colli.contacts[0];
        Quaternion rotation = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 position = contact.point;

        //Sword with Sword collision
        if (colli.gameObject.tag == "EnemyWep"){
            //First get the enemyAI script
            vikingAI enemyHit = colli.gameObject.GetComponentInParent<vikingAI>();
            //Change this to general enemyAI later
            Debug.Log("Enemy State (from Sword) - " + enemyHit.enemyState);
            //Next, handle the collision is one of three ways:
            //Sword moving & enemy attacking -> Parry
            if (enemyHit.enemyState == 2 && swingState == 1){
                enemyHit.enemyState = 4;                                      //Change enemy to stun state
                AudioSource.PlayClipAtPoint(parrySound, transform.position);  //Play parry sound effect
            }
            //Player sword moving, enemy sword stopped -> enemy blocked
           else if (swingState == 1){
               //Do nothing for now
            }
            //Player sword stopped, enemy sword moving -> player blocked
            else if (swingState == 0){
                //Do nothing for now
                //Should make player immune for a little while, just in case they move into
                //enemy weapon after blocking
            }
            //Spawn spark effect at point of collision
            Instantiate(sparkPrefab, position, rotation);
        }
        //Sword with enemy body collision
        else if (colli.gameObject.tag == "EnemyBody"){
            //First get the enemyAI script
            //vikingAI enemyHit = colli.gameObject.GetComponentInParent<vikingAI>();
            //Get grandparent of collision object
            vikingAI enemyHit = colli.gameObject.transform.parent.gameObject.GetComponentInParent<vikingAI>();

            //Next, see if enemy is immune or not
            if (enemyHit.iFrames <= 0){
                //Deal damage to enemy
                float damageToDo = Mathf.Sqrt(swingSpeed) * damageModif;      //Calculate damage
                if (enemyHit.enemyState == 4){          //If enemy is stunned, deal critical damage
                    enemyHit.health -= damageToDo * 2;  //Deal critical damage to enemy health
                }else{
                    enemyHit.health -= damageToDo;      //Deal regular damage to enemy health
                }
                enemyHit.iFrames = 15;                      //Give the enemy some iFrames
                if (enemyHit.enemyState != 5){
                    enemyHit.animController.SetTrigger("Hit");  //Player hit animation only if enemy is not dying
                }
            }

            //Spawn blood effect at point of collision
            Instantiate(bloodPrefab, position, rotation);
        }

    }
}
