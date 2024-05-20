using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductLogic : MonoBehaviour
{
    public RoboHand hand;

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            hand.Punish(-0.01f);
        }
        if (collision.gameObject.tag == "hand")
        {
            hand.Punish(0.1f);
        }
    }
}
