using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameMaster : MonoBehaviour
{
    public GameObject northSpawn;   //North spawn block
    public GameObject southSpawn;   //South spawn block
    public GameObject eastSpawn;    //East spawn block
    public GameObject westSpawn;    //West spawn block

    public GameObject enemyPrefab;  //EnemyPrefab to spawn

    //public int enemyKillLimit;      //Number of enemies to kill to end the level
    //Removed to make the game endless for now

    private int spawnCoolDown;     //Count down until next enemy spawns
    public int enemiesKilled;      //Number of enemies player has killed
    public int enemiesAlive;       //Number of current enemies in game world

    public bool[] freeSpaces = new bool[4];    //Array of states to track if the selected path is free or not

    // Start is called before the first frame update
    void Start()
    {
        spawnCoolDown = 120;    //Wait two seconds before first spawn
        enemiesAlive = 0;       //No enemies alive yet
        enemiesKilled = 0;      //No enemies killed yet

        freeSpaces[0] = true;
        freeSpaces[1] = true;
        freeSpaces[2] = true;
        freeSpaces[3] = true;
    }

    // Update is called once per frame
    void Update()
    {
        spawnCoolDown --;

        if (((spawnCoolDown <= 0) || (enemiesAlive == 0)) && (enemiesAlive < 3)){    //If spawn cool down is done or no enemies alive, AND less that 3 enemies alive
         //&& (enemiesKilled + enemiesAlive < enemyKillLimit)){                       //AND enemies are below the kill limit
         //Removed to make the game endless
            spawnCoolDown = 2700;   //Wait 45 seconds until next spawn
            //Spawn the enemy
            GameObject newEnemy;                                //Make a new game object
            int spawnLocal;                                     //Spawn direction
            
            do{                                             //while spawn local is not free:
                spawnLocal = (int)Random.Range(0.0f, 3.0f); //Get a random spawn location
            }while(freeSpaces[spawnLocal] == false);        //check if spawn local is free
            

            switch (spawnLocal){
                case 0:{    //Spawn to the North
                    newEnemy = Instantiate(enemyPrefab, northSpawn.transform.position, northSpawn.transform.rotation);
                    newEnemy.GetComponent<vikingAI>().spawnDir = spawnLocal;    //Assign enemy a direction
                    newEnemy.GetComponent<vikingAI>().player = GameObject.FindGameObjectWithTag("Player");  //Give enemy the player ref
                    //newEnemy.GetComponent<vikingAI>().gmScript = this.gameObject.GetComponent<gameMaster>();  //Give enemy the gmScript ref
                    newEnemy.GetComponent<vikingAI>().gmObj = this.gameObject;  //Give the enemy the gm object ref
                    break;
                }
                case 1:{    //Spawn to the East
                    newEnemy = Instantiate(enemyPrefab, eastSpawn.transform.position, eastSpawn.transform.rotation);
                    newEnemy.GetComponent<vikingAI>().spawnDir = spawnLocal;    //Assign enemy a direction
                    newEnemy.GetComponent<vikingAI>().player = GameObject.FindGameObjectWithTag("Player");  //Give enemy the player ref
                    //newEnemy.GetComponent<vikingAI>().gmScript = this.gameObject.GetComponent<gameMaster>();  //Give enemy the gmScript ref
                    newEnemy.GetComponent<vikingAI>().gmObj = this.gameObject;  //Give the enemy the gm object ref
                    break;
                }
                case 2:{    //Spawn to the South
                    newEnemy = Instantiate(enemyPrefab, southSpawn.transform.position, southSpawn.transform.rotation);
                    newEnemy.GetComponent<vikingAI>().spawnDir = spawnLocal;    //Assign enemy a direction
                    newEnemy.GetComponent<vikingAI>().player = GameObject.FindGameObjectWithTag("Player");  //Give enemy the player ref
                    //newEnemy.GetComponent<vikingAI>().gmScript = this.gameObject.GetComponent<gameMaster>();  //Give enemy the gmScript ref
                    newEnemy.GetComponent<vikingAI>().gmObj = this.gameObject;  //Give the enemy the gm object ref
                    break;
                }
                case 3:{    //Spawn to the West
                    newEnemy = Instantiate(enemyPrefab, westSpawn.transform.position, westSpawn.transform.rotation);
                    newEnemy.GetComponent<vikingAI>().spawnDir = spawnLocal;    //Assign enemy a direction
                    newEnemy.GetComponent<vikingAI>().player = GameObject.FindGameObjectWithTag("Player");  //Give enemy the player ref
                    //newEnemy.GetComponent<vikingAI>().gmScript = this.gameObject.GetComponent<gameMaster>();  //Give enemy the gmScript ref
                    newEnemy.GetComponent<vikingAI>().gmObj = this.gameObject;  //Give the enemy the gm object ref
                    break;
                }
            }
            freeSpaces[spawnLocal] = false;   //Mark the chosen path as taken
            enemiesAlive ++;
        }

        /*
        if (enemiesKilled >= enemyKillLimit){
            //End the level here
        }   //Removed to make the game endless
        */
    }
}
