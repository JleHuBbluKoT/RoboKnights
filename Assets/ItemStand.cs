using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStand : MonoBehaviour
{
    public Transform targetPoint;
    public SphereCollider sc;


    public Transform GetTarget()
    {
        return targetPoint;
    }
}
