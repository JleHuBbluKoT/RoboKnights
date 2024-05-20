using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurableJointTest : MonoBehaviour
{

    public ConfigurableJoint cj;
    public Transform observer;
    public Transform target;
    public int qy;
    public int qx;
    public int qz;
    // Start is called before the first frame update
    void Start()
    {
        //observer.localPosition = cj.anchor + this.transform.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate() {
        observer.LookAt(target);
        //i = (i + 1) % 720;
        //cj.SetTargetRotationLocal(Quaternion.Euler(0, i, 0), cj.transform.localRotation);

        //cj.targetRotation = observer.rotation; 

        
        Vector3 eulers = observer.rotation.eulerAngles;
        cj.targetRotation = (Quaternion.Euler(eulers));

        //cj.targetRotation = (Quaternion.Euler(qx, qy, qz));

    }
}
