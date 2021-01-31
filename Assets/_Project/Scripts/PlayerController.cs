using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// a prototype script that didn't end up being used
public class PlayerController : MonoBehaviour
{
    public float speed = 10.0f;
    public Vector3 movedir;
    Vector3 watchDir;
    Vector2 move;
    Rigidbody rb;
    PlayerInputController controls;
    // Start is called before the first frame update

    public void Awake()
    {
        controls = new PlayerInputController();
    }

    public void OnEnable()
    {
        controls.Enable();
    }

    public void OnDisable()
    {
        controls.Disable();
    }
    void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 targetVelocity = movedir;
        targetVelocity *= speed;
        rb.velocity = targetVelocity;
        // rb.AddForce(targetVelocity);
    }
    // Update is called once per frame
    void Update()
    {
        Vector2 temp = controls.Player.Move.ReadValue<Vector2>();
        movedir = new Vector3(temp.x, 0, temp.y);
        // if movinng, rotate the model as well.
        if (movedir.magnitude != 0)
        {
            this.transform.rotation = Quaternion.LookRotation(movedir);
        }
    }

    public void Die()
    {
        Debug.Log("Oh no i am a dead bunny");
    }
}
