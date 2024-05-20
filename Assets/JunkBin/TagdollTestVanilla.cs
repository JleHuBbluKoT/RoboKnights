using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagdollTestVanilla : MonoBehaviour
{
    public Rigidbody rb;
    public CharacterJoint cj;
    public ArticulationBody ab;
    public Transform target;
    public float degree;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        List<float> targets = new List<float>();
        ab.GetDriveTargets(targets);
        targets[0] = degree;
        ab.SetDriveTargets(targets);*/

        ArticulatingTry();


        //Debug.Log(targets.Count);
    }
    public void ArticulatingTry()
    {
        Vector3 direction = target.position - ab.anchorPosition;
        Quaternion targetRot = Quaternion.LookRotation(direction);
        SetDriveRotation(ab, targetRot);
    }

    public void SetDriveRotation(ArticulationBody body, Quaternion targetLocalRotation)
    {
        Vector3 target = ToTargetRotationInReducedSpace(body, targetLocalRotation);

        // assign to the drive targets...
        ArticulationDrive xDrive = body.xDrive;
        xDrive.target = target.x;
        body.xDrive = xDrive;

        ArticulationDrive yDrive = body.yDrive;
        yDrive.target = target.y;
        body.yDrive = yDrive;

        ArticulationDrive zDrive = body.zDrive;
        zDrive.target = target.z;
        body.zDrive = zDrive;
        Debug.Log(target);
    }

    public Vector3 ToTargetRotationInReducedSpace(ArticulationBody body, Quaternion targetLocalRotation)
    {
        if (body.isRoot)
            return Vector3.zero;
        Vector3 axis;
        float angle;


        //Convert rotation to angle-axis representation (angles in degrees)
        targetLocalRotation.ToAngleAxis(out angle, out axis);

        // Converts into reduced coordinates and combines rotations (anchor rotation and target rotation)
        Vector3 rotInReducedSpace = Quaternion.Inverse(body.anchorRotation) * axis * angle;

        return rotInReducedSpace;
    }

}
