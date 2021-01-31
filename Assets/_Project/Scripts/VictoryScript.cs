using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryScript : MonoBehaviour
{
    public string PlayerTag = "Player";
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(PlayerTag))
        {
            other.GetComponent<CharacterControls>().Win();
        }
    }
}
