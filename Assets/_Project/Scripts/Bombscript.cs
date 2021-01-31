using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bombscript : MonoBehaviour
{
    public string PlayerTag = "Player";
    public GameObject ExplosionParticles;
    public GameObject SmokeParticles;
    AudioSource audio;

    GameObject SoundRadius;

    private void Start()
    {
        audio = transform.GetComponent<AudioSource>();
        SoundRadius = this.gameObject.transform.GetChild(0).gameObject;
        ExplosionParticles.SetActive(false);
        SmokeParticles.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PlayerTag))
        {
            audio.Play();
            CharacterControls ps = other.GetComponent<CharacterControls>();
            ps.Die(this.transform.position);
            ExplosionParticles.SetActive(true);
            SmokeParticles.SetActive(true);
        }
    }
}
