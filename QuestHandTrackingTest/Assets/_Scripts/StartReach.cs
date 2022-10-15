using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartReach : MonoBehaviour
{
    public bool inStartPosition = true;

    private void OnTriggerEnter(Collider other)
    {
        inStartPosition = true;
    }

    private void OnTriggerExit(Collider other)
    {
        inStartPosition = false;
    }
}
