using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticCameraHover : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 3;
    void FollowTarget(float d)
    { //Function that makes the camera follow the player
        float speed = d * followSpeed; //Set speed regardless of fps
        Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed); //Bring the camera closer to the player interpolating with the velocity(0.5 half, 1 everything)
        transform.position = targetPosition; //Update the camera position
    }

    private void FixedUpdate()
    {
        if(target.GetComponent<CharacterControls>().isAlive) FollowTarget(Time.deltaTime);
    }
}
