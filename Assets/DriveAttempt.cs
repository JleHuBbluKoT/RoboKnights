using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveAttempt : MonoBehaviour
{
    public Transform parent;
    public Vector3 anchor;

    public Vector3 anchorToParent;
    public Vector3 target;

    public Vector3 Vrotation;
    public bool freeX;
    public Vector2 swingX;
    public bool freeY;
    public Vector2 twistY;
    public bool freeZ;
    public Vector2 swingZ;

    public bool debugLines;

    public Vector3 debugDrive;

    public float weight;
    public Rigidbody mainRB;

    void Start() {
        anchorToParent = this.transform.localPosition + anchor;


    }
    // Эта штука плохо работает с объектами, части вращающейся руки проваливаются сквозь пол
    // Update is called once per frame
    void FixedUpdate() {
        //Vrotation.x =  (Vrotation.x + 0.2f) % 360;
        Debug.DrawLine(parent.position, worldAnchor(), Color.green, 0.1f);
        moveDrive(debugDrive);

        //mainRB.AddForceAtPosition(weight * Vector3.down, this.transform.position);
    }


    public void SetRotation()
    {
        Vrotation = rotationClamp(Vrotation);
        Quaternion rotation = Quaternion.Euler(Vrotation);
        this.transform.localRotation = rotation;
        this.transform.localPosition = anchorToParent + rotation * -anchor;  
        
    }

    /*
    public void moveDrive(Vector3 change) {
        //Quaternion prevRot = Quaternion.Euler(Vrotation);
        Vrotation = rotationClamp(Vrotation + change);
        Quaternion newRot = Quaternion.Euler(Vrotation);
        Vector3 newPos = anchorToParent + newRot * inverseAnchor;
        this.transform.Translate(newPos - this.transform.position, this.transform);
    }*/
    public void moveDrive(Vector3 change)
    {
        Vrotation += change;
        SetRotation();
    }

    public Vector3 rotationClamp(Vector3 rot) {
        float x = freeX ? rot.x : Mathf.Clamp(rot.x, swingX.x, swingX.y);
        float y = freeY ? rot.y : Mathf.Clamp(rot.y, twistY.x, twistY.y);
        float z = freeZ ? rot.z : Mathf.Clamp(rot.z, swingZ.x, swingZ.y);
        rot = new Vector3(x % 360f, y % 360f, z % 360f);
        return rot;
    }

    public Vector3 worldAnchor() {
        return parent.position + parent.rotation * anchorToParent;
    }

}
