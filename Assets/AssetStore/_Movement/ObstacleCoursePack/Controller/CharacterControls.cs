using UnityEngine;
using System.Collections;
using System;
using System.Security.Cryptography;

[RequireComponent (typeof (Rigidbody))]
[RequireComponent (typeof (CapsuleCollider))]

// A character controller from the asset store with some added modifications
public class CharacterControls : MonoBehaviour {
	
	public float speed = 10.0f;
	public float airVelocity = 8f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public float jumpHeight = 2.0f;
	public float maxFallSpeed = 20.0f;
	public float rotateSpeed = 25f; //Speed the player rotate
	private Vector3 moveDir;
	public GameObject cam;
	private Rigidbody rb;

	private float distToGround;

	private bool canMove = true; //If player is not hitted
	private bool isStuned = false;
	private bool wasStuned = false; //If player was stunned before get stunned another time
	private float pushForce;
	private Vector3 pushDir;

	public Vector3 checkPoint;
	private bool slide = false;

	private PlayerInputController controls;

	// ---------------------------------
	// debugging things
	// ---------------------------------
	LineRenderer LineRenderer;
	// For testing purposes only
	[SerializeField]
	Vector3 MousePosInWorld;
	public float maxViewAngle = 45f;
	public Transform HeadTransform;
	public float ExplosionForce = 100f;

	public AudioSource GameMusic;
	public AudioSource DeathSound;
	public AudioSource LoseSound;
	public AudioSource VictorySound;
	public AudioSource VictoryMusic;

	[HideInInspector]
	public bool isAlive;

	void OnEnable()
	{
		controls.Enable();
	}

	private void OnDisable()
	{
		controls.Disable();
	}
	void  Start (){

		// get the distance to ground
		// Ei varmaan meidän keississä tarvita mutta jos poistan tämän niin eiköhän jokin mene rikki :D
		distToGround = GetComponent<Collider>().bounds.extents.y;

		isAlive = true;

		GameMusic.Play();
		GameMusic.loop = true;
		// ---------------------------------
		// debugging things
		// ---------------------------------
		// LineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
		
	}
	
	bool IsGrounded (){
		return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
	}
	
	void Awake () {
		rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
		rb.useGravity = false;
		controls = new PlayerInputController();

		checkPoint = transform.position;
		Cursor.visible = true;
	}

	void FixedUpdate () {
		if (!isAlive) return;

		if (canMove)
		{
			if (moveDir.x != 0 || moveDir.z != 0)
			{
				Vector3 targetDir = moveDir; //Direction of the character

				targetDir.y = 0;
				if (targetDir == Vector3.zero)
					targetDir = transform.forward;
				Quaternion tr = Quaternion.LookRotation(targetDir); //Rotation of the character to where it moves
				Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, Time.deltaTime * rotateSpeed); //Rotate the character little by little
				transform.rotation = targetRotation;
			}

			if (IsGrounded())
			{
			 // Calculate how fast we should be moving
				Vector3 targetVelocity = moveDir;
				targetVelocity *= speed;

				// Apply a force that attempts to reach our target velocity
				Vector3 velocity = rb.velocity;
				if (targetVelocity.magnitude < velocity.magnitude) //If I'm slowing down the character
				{
					targetVelocity = velocity;
					rb.velocity /= 1.1f;
				}
				Vector3 velocityChange = (targetVelocity - velocity);
				velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
				velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				velocityChange.y = 0;
				if (!slide)
				{
					if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
						rb.AddForce(velocityChange, ForceMode.VelocityChange);
				}
				else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
				{
					rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
					//Debug.Log(rb.velocity.magnitude);
				}

				// Jump
				if (IsGrounded() && Input.GetButton("Jump"))
				{
					rb.velocity = new Vector3(velocity.x, CalculateJumpVerticalSpeed(), velocity.z);
				}
			}
			else
			{
				if (!slide)
				{
					Vector3 targetVelocity = new Vector3(moveDir.x * airVelocity, rb.velocity.y, moveDir.z * airVelocity);
					Vector3 velocity = rb.velocity;
					Vector3 velocityChange = (targetVelocity - velocity);
					velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
					velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
					rb.AddForce(velocityChange, ForceMode.VelocityChange);
					if (velocity.y < -maxFallSpeed)
						rb.velocity = new Vector3(velocity.x, -maxFallSpeed, velocity.z);
				}
				else if (Mathf.Abs(rb.velocity.magnitude) < speed * 1.0f)
				{
					rb.AddForce(moveDir * 0.15f, ForceMode.VelocityChange);
				}
			}
		}
		else
		{
			rb.velocity = pushDir * pushForce;
		}
		// We apply gravity manually for more tuning control
		rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0));
	}

	private void Update()
	{
		if (isAlive)
		{
			// float h = Input.GetAxis("Horizontal");
			// float v = Input.GetAxis("Vertical");
			// Vector3 temp = new Vector3(h * horizontalOffset , 0, v * verticaloffset);

			// Vector3 v2 = v * cam.transform.forward; //Vertical axis to which I want to move with respect to the camera
			// Vector3 h2 = h * cam.transform.right; //Horizontal axis to which I want to move with respect to the camera

			// En jaksa kameran kulmia säätää, sillä se on vähän lukkoon lyöty, joten täällä virittelen,
			// että oikeasti liikkuu vasemmalle, oikealle, jne kun pelaaja painaa vasemmalle, oikealle, jne

			Vector2 temp = controls.Player.Move.ReadValue<Vector2>();
			moveDir = Quaternion.AngleAxis(-45, Vector3.up) * new Vector3(temp.x, 0, temp.y);

			// Haetaan hiiren sijainti 
			Plane plane = new Plane(Vector3.up, 0);
			float distance;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (plane.Raycast(ray, out distance))
			{
				MousePosInWorld = ray.GetPoint(distance);
				HandleHeadAngle();
			}
			// moveDir = temp.normalized; //Global position to which I want to move in magnitude 1
			// TestRenderLine();

			RaycastHit hit;
			if (Physics.Raycast(transform.position, -Vector3.up, out hit, distToGround + 0.1f))
			{
				if (hit.transform.tag == "Slide")
				{
					slide = true;
				}
				else
				{
					slide = false;
				}
			}
		}
	}

	private void HandleHeadAngle()
	{
		HeadTransform.LookAt(MousePosInWorld);
		float angle = Quaternion.Angle(HeadTransform.rotation, transform.rotation);
		if (Mathf.Abs(angle) >= maxViewAngle)
		{
			// TODO, päähahmo on pupu, eikö pöllö, joten jotenkin tekisi mieli rajoittaa pään katselukulmia
			// Debug.Log("MY HEAAAAAAD");
		}
	}

	float CalculateJumpVerticalSpeed () {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * jumpHeight * gravity);
	}

	public void HitPlayer(Vector3 velocityF, float time)
	{
		rb.velocity = velocityF;

		pushForce = velocityF.magnitude;
		pushDir = Vector3.Normalize(velocityF);
		StartCoroutine(Decrease(velocityF.magnitude, time));
	}

	// kuolee ja lähtee lentämään
	public void Die(Vector3 BombPos)
	{
		isAlive = false;
		controls.Disable();
		rb.freezeRotation = false;
		rb.useGravity = true;
		// Näiden pitäisi tehdä paukusta vähän hallitumpi
		rb.mass = 300;
		rb.drag = 0.5f;

		Vector3 torq = new Vector3(UnityEngine.Random.Range(-2000, 2000),
								   UnityEngine.Random.Range(-2000, 2000),
								   UnityEngine.Random.Range(-2000, 2000));
		rb.AddTorque(torq);

		// Pistetään pommi heittämään pelaaja pommista poispäin
		Vector3 ExplosionDirection = this.transform.position - BombPos;
		rb.velocity = ExplosionDirection * ExplosionForce;

		DeathSound.Play();

		Debug.Log("Oh noes i am a dead bunny");
		StartCoroutine(RespawnTimer(5));
	}

	public void Die()
	{
		isAlive = false;
		controls.Disable();
		rb.freezeRotation = false;
		rb.useGravity = true;

		DeathSound.Play();

		Debug.Log("Oh noes i have fallen and can't get up");
		StartCoroutine(RespawnTimer(5));
	}

	public void LoseGame()
	{
		isAlive = false;
		controls.Disable();
		rb.freezeRotation = false;
		rb.useGravity = true;

		Vector3 torq = new Vector3(UnityEngine.Random.Range(-2000, 2000),
								   UnityEngine.Random.Range(-2000, 2000),
								   UnityEngine.Random.Range(-2000, 2000));
		rb.AddTorque(torq);
		// Soita väävää ääni
		Debug.Log("playing lose sound effect");
		LoseSound.Play();
	}

	public void LoadCheckPoint()
	{
		isAlive = true;
		controls.Enable();
		rb.freezeRotation = true;
		rb.useGravity = false;
		rb.mass = 60;
		rb.drag = 0f;

		transform.position = checkPoint;
	}

	IEnumerator RespawnTimer(float time) {
		Debug.Log("respawn timer beginning");
		yield return new WaitForSeconds(time);
		// Lisää tyhmiä muutoksia näin kolmelta aamuyöstä
		if (!isAlive)
		{
			FindObjectOfType<GameTimer>().ResetGameTime();
			LoadCheckPoint();
		}
	}

	private IEnumerator Decrease(float value, float duration)
	{
		if (isStuned)
			wasStuned = true;
		isStuned = true;
		canMove = false;

		float delta = 0;
		delta = value / duration;

		for (float t = 0; t < duration; t += Time.deltaTime)
		{
			yield return null;
			if (!slide) //Reduce the force if the ground isnt slide
			{
				pushForce = pushForce - Time.deltaTime * delta;
				pushForce = pushForce < 0 ? 0 : pushForce;
				//Debug.Log(pushForce);
			}
			rb.AddForce(new Vector3(0, -gravity * GetComponent<Rigidbody>().mass, 0)); //Add gravity
		}

		if (wasStuned)
		{
			wasStuned = false;
		}
		else
		{
			isStuned = false;
			canMove = true;
		}
	}

	// voitetaan, disabloidaan kontrollit ja sanotaan käyttöliittymälle että tee juttuja!
	public void Win()
	{
		controls.Disable();
		// ja tehdään muita juttuja hiemam tyhmästi.
		// kuten kutsua UI:ta pelaajasta!
		FindObjectOfType<GameTimer>().GameWin();
		GameMusic.loop = false;
		GameMusic.Stop();
		VictorySound.Play();
		VictoryMusic.Play();
		


	}

	// Unityn foorumeilta löytyneitä snippettejä
	void TestRenderLine()
	{
		LineRenderer.startColor = Color.black;
		LineRenderer.endColor = Color.black;
		LineRenderer.startWidth = 0.01f;
		LineRenderer.endWidth = 0.01f;
		LineRenderer.positionCount = 2;
		LineRenderer.useWorldSpace = true;

		LineRenderer.SetPosition(0, this.transform.position); //x,y and z position of the starting point of the line
		LineRenderer.SetPosition(1, MousePosInWorld); //x,y and z position of the starting point of the line
	}
}
