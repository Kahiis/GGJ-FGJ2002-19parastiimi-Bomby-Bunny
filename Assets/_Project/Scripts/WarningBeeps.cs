using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WarningBeeps : MonoBehaviour
{
    public string PlayerTag = "Player";
    public string SweeperName = "coil_4";
    AudioSource audio;
    public float PitchMax = 1f;
    public float PitchMin = 0.2f;
    public float VolMax = 1f;
    public float VolMin = 0f;
    // public bool isInsideTriggerZone;
    // float DistanceToPlayer = 1f;

    private void Start()
    {
        audio = this.GetComponent<AudioSource>();
        audio.volume = 0f;
        audio.pitch = PitchMin;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == SweeperName)
        {
            // isInsideTriggerZone = false;
            audio.volume = 0;
            audio.loop = false;
            // audio.Pause();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == SweeperName)
        {
            audio.Play();
            audio.volume = 1;
            audio.loop = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name == SweeperName)
        {
            Vector3 parentPosition = this.transform.GetComponentInParent<Transform>().position;
            float Distance = Vector3.Distance(parentPosition, other.transform.position);
            float HitBoxRadius = this.transform.GetComponent<SphereCollider>().radius;

            // aivot ei toimi joten teen tämä tulee tehtyä aika tyhämällä tavalla :D
            float delta = HitBoxRadius - Distance;
            float percentage = delta / HitBoxRadius;
            float StupidPercentageVolume = percentage / 100;
            audio.volume = VolMin + (StupidPercentageVolume * (percentage * 100));
            
            float StupidDelta = PitchMax - PitchMin;
            float StupidPercentagePitch = StupidDelta / 100;
            audio.pitch = PitchMin + (StupidPercentagePitch * (percentage * 100));
        }
    }

    private void Update()
    {
        /*
        if (audio.isPlaying)
        {
            Debug.Log("objekti nimeltä: " + this.GetComponentInParent<Transform>().name + " is playing");
        }
        */
    }

}
