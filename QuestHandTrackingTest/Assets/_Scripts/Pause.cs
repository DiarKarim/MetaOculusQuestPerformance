using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public bool paused = false;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(PauseSequence());
    }

    IEnumerator PauseSequence()
    {
        if (paused)
            paused = false;
        if (!paused)
            paused = true;

        yield return new WaitForSecondsRealtime(2f);
    }
}
