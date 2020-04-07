using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    float horizVal; //Thumbstick H value
    float vertiVal; //Thumbstick V value
    // Update is called once per frame
    void Update()
    {
        //For debugging only
        //Use left thumbstick as direct movement for now
        //horizVal = Input.GetAxis ("Oculus_CrossPlatform_PrimaryThumbstickHorizontal");
        //vertiVal = Input.GetAxis ("Oculus_CrossPlatform_PrimaryThumbstickVertical");
        //transform.Translate (horizVal/10.0f, 0.0f, vertiVal/10.0f);

        //In the final version, only in debug mode should the player move
    }
}
