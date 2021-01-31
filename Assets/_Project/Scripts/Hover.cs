using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour
{
    public float hoverForce = 12f;

    private void OnTriggerStay (Collider other)
    {
        other.GetComponent<Rigidbody>().AddForce(Vector3.up * hoverForce, ForceMode.Acceleration);
    }

}
