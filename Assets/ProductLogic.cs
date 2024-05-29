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

    private void FixedUpdate()
    {
        if (pedestal  == 1)
        {
            stay += 1;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finishing" & stay == 0)
        {
            inhand = 1;
        }
        if (other == hand.pedestal.sc)
        {
            pedestal = 1;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Finishing" & stay == 0)
        {
            inhand = 0;
        }
        if (other == hand.pedestal.sc)
        {
            pedestal = 0;

        }
    }

}
