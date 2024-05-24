using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductLogic : MonoBehaviour
{
    public RoboHand hand;
    public int stay = 0;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            hand.Punish(-0.01f);
            hand.IAmAFailure();
        }
        if (collision.gameObject.tag == "hand")
        {
            hand.Punish(0.05f);
        }
        

    }
    private void OnTriggerStay(Collider other)
    {
        if (other == hand.pedestal.sc)
        {
            hand.Punish(0.15f);
            stay += 1;
        }
    }
}
