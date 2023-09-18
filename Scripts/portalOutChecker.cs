using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portalOutChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.isOutsidePortal = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.isOutsidePortal = false;
        }
    }
}
