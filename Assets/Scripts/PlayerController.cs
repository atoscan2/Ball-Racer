using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	private GameObject player;
	public GameObject camera;
	
	public Vector3 front;
	public Vector3 gravity;
	
	public float speed = 10.0f;
	private Vector3 currentPos;
	private Vector3 newPos;
	
	private float controlThreshold = 0.10f;
	private Vector3 positionOffset = new Vector3(0.0001f, 0.0f, 0.0001f);
	 
	private Vector3 oldFront;
	private float minAngle = 0.5f;
	private bool isFlipped;
	private float flippedTime;
	public float maxFlipTime = 0.5f;

	bool reverse;
	
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		
		currentPos = new Vector3 (player.transform.position.x, 0, player.transform.position.z);
		
		front = new Vector3 (1, 0, 1);
		gravity = Physics.gravity;
		gravity.Normalize();

		oldFront = front;
		
		isFlipped = false;
		flippedTime = 0.0f;

		reverse = false;
	}
	
	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		//helps with camera flip controls
		if (reverse) 
		{
			moveVertical = -moveVertical;
		}

		//special case where camera flips out
		if (isNegative(moveVertical) && !isZero(moveHorizontal)) 
		{
			moveVertical = 0.0f;
		}

		Debug.Log ("moveVertical" + moveVertical);

		//get the correct direction based on where the player is facing
		Vector3 moveDirection = getRelativeDirection (front, moveVertical, moveHorizontal);

		//move the player
		rigidbody.AddForce (moveDirection * speed * Time.deltaTime);
		
		//calculate the "front" based on the player's movement
		newPos = new Vector3 (player.transform.position.x, 0, player.transform.position.z);
		front = newPos - currentPos + positionOffset;
		currentPos = newPos;

		front.Normalize();

		//attempt at handling camera flips
		float angle = Mathf.Acos (Vector3.Dot (-front, oldFront));
		
		if (angle < minAngle)
		{
			isFlipped = true;
		}
		
		if (isFlipped) 
		{
			reverse = true;

			flippedTime += Time.deltaTime;

			if (flippedTime > maxFlipTime && moveVertical == 0)
			{
				isFlipped = false;
				flippedTime = 0.0f;

				reverse = false;
			}
		}
		
		oldFront = front;

		//drawing front and gravity ray
		Debug.DrawRay (player.transform.position, front * 5, Color.cyan);
		Debug.DrawRay (player.transform.position, gravity * 5, Color.red);
	}
	
	bool isZero(float val)
	{
		return val > -controlThreshold && val <= controlThreshold;
	}
	
	bool isPositive(float val)
	{
		return val > controlThreshold;
	}
	
	bool isNegative(float val)
	{
		return val <= -controlThreshold;
	}
	
	Vector3 getRelativeDirection(Vector3 front, float vertical, float horizontal)
	{
		Vector3 left = Quaternion.Euler(0, -90, 0) * front;
		Vector3 right = Quaternion.Euler(0, 90, 0) * front;
		
		Vector3 direction = new Vector3 (0, 0, 0);
		
		if (isPositive (vertical)) 
		{
			direction += front;
		}
		
		if (isNegative (vertical)) 
		{
			direction -= front;
		}
		
		if (isPositive (horizontal)) 
		{
			direction += right;
		}
		
		if (isNegative (horizontal)) 
		{
			direction += left;
		}
		
		direction.Normalize();
		
		Debug.DrawRay (player.transform.position, left * 5, Color.green);
		Debug.DrawRay (player.transform.position, right * 5, Color.blue);
		
		return direction;
	}
}
