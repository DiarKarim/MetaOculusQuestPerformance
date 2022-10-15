using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignFingertip : MonoBehaviour
{
    public Transform virtualFingerTip;
    public Transform MarkerTip;
    Vector3 offset = new Vector3();
    float mag; 

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            offset = MarkerTip.position - virtualFingerTip.position;
            mag = offset.sqrMagnitude;

            //Vector3.MoveTowards(transform.position, offset, 1f);
            transform.position = transform.position + offset;
        }


    }
}
