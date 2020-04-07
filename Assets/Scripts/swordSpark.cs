using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swordSpark : MonoBehaviour
{
  public float lifeTime;    //Time for effect to live
  public AudioClip sparkSound;  //Sound to play at point

    void Start(){
        //Play sound at position
        AudioSource.PlayClipAtPoint(sparkSound, this.transform.position);
    }

    void Update()
    {
        if (lifeTime > 0.0f){
            lifeTime -= 1.0f;
        }
        else{
            Destroy(this.gameObject);  //If lifetime is over, destroy self
        }
    }
}
