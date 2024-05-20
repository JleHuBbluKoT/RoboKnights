using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sillycopycat : MonoBehaviour
{
    public Transform target;

    private void Update()
    {
        this.transform.rotation = target.rotation;
    }
}
