using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    Vector3 startPos;
    Quaternion startRot;
    public float speed;
    public float turnSpeed;

    public int canMove;
    public bool BallMode;
    public Transform product;

    public int camDist;
    float ballRotX; float ballRotY;

    public LayerMask toIgnore;

    void Start()
    {
        startPos = this.transform.position;
        startRot = this.transform.rotation;
        canMove = 1;
        //camDist = 3;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (BallMode == false)
        {
            this.transform.position = this.transform.position + this.transform.forward * speed * Input.GetAxis("Vertical") * canMove;
            this.transform.position = this.transform.position + this.transform.right * speed * Input.GetAxis("Horizontal") * canMove;
            //Debug.Log(Input.GetAxis("Horizontal"));

            if (Input.GetKey(KeyCode.Mouse0))
            {
                float y = Input.GetAxis("Mouse X");
                float x = Input.GetAxis("Mouse Y");
                //Debug.Log(x + ":" + y);
                Vector3 rotateValue = new Vector3(x, y * -1, 0) * turnSpeed;
                transform.eulerAngles = transform.eulerAngles - rotateValue * canMove;
            }
        }
        else
        {
            Vector3 targetpos;

            if (Input.GetKey(KeyCode.Mouse0))
            {
                ballRotX += Input.GetAxis("Mouse X") * turnSpeed;
                ballRotY += Input.GetAxis("Mouse Y") * turnSpeed;
            }
            

            ballRotY = Mathf.Clamp(ballRotY, -50, 50);

            Vector3 Direction = new Vector3(0, 0, -camDist);
            Quaternion rotation = Quaternion.Euler(ballRotY, -ballRotX, 0);
            targetpos = product.position + rotation * Direction;

            /*
            RaycastHit hit;
            if (Physics.Raycast( product.position, new Vector3(ballRotY, -ballRotX, 0), out hit, camDist,  ~toIgnore) )
            {
                if (Vector3.Distance(hit.point, product.position) < camDist)
                {
                    targetpos = hit.point;
                }  
                
            }*/
            this.transform.position = targetpos;
            this.transform.LookAt(product.position, Vector3.up);

        }
        
    }
    public void ResetPos()
    {
        this.transform.position = startPos;
        this.transform.rotation = startRot;
    }
}
