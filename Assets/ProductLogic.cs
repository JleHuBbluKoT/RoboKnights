using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductLogic : MonoBehaviour
{
    public RoboHand hand;
    public int stay = 0;
    public int pedestal = 0;
    public int inhand = 0;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            hand.Punish(-0.01f);
            hand.IAmAFailure();
        }
        //if (collision.gameObject.tag == "hand") hand.Punish(0.001f);

        

    }
    private void OnTriggerStay(Collider other)
    {
        
        
        if (other == hand.pedestal.sc)
        {
            pedestal = 1;
            stay += 1;
        } else
        {
            pedestal = 0;
        }
        if (other.tag == "Finishing" & stay == 0)
        {
            inhand = 1;
        } else
        {
            inhand = 0;
        }
    }
}
