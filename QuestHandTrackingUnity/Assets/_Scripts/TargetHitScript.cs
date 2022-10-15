using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHitScript : MonoBehaviour
{
    public bool isHit;
    MeshRenderer targetRend;

    private void Start()
    {
        targetRend = GetComponent<MeshRenderer>();
        targetRend.material.color = new Color(0f, 1f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        targetRend.material.color = new Color(0f, 0f, 1f);
        isHit = true;
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    targetRend.material.color = new Color(1f, 0f, 0f);
    //    isHit = false;
    //}
}
