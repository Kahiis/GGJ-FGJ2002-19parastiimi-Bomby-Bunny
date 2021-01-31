using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillPlaneScript : MonoBehaviour
{
    public string PlayerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            other.GetComponent<CharacterControls>().Die();
        }
    }
}
