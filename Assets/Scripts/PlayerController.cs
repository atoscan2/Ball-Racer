using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour 
{
	private GameObject player;
	public GameObject camera;
	
	public Vector3 front;
	public Vector3 gravity;
	
	public float speed = 10.0f;
	private Vector3 currPos;
	private Vector3 newPos;
	
	private float controlThreshold = 0.10f;
	private Vector3 positionOffset = new Vector3(0.00001f, 0.0f, 0.00001f);
	 
	public float maxFlipTime = 0.5f;
	public float maxFlipAngle = 0.5f;
	private float flippedTime;
	private Vector3 oldFront;
	private bool isFlipped;
	
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		
		currPos = new Vector3 (player.transform.position.x, 0, player.transform.position.z);

		gravity = Physics.gravity;
		gravity.Normalize();
		
		isFlipped = false;
		flippedTime = 0.0f;
	}
	
	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		//helps controls on a camera flip
		if (isFlipped) 
		{
			moveVertical = -moveVertical;
		}

		//fixes special case where camera flips out
		if (isNegative(moveVertical) && !isZero(moveHorizontal)) 
		{
			moveVertical = 0.0f;
		}

		//get the correct direction based on where the player is facing
		Vector3 moveDirection = getRelativeDirection (front, moveVertical, moveHorizontal);

		//move the player
		rigidbody.AddForce (moveDirection * speed * Time.deltaTime);
		
		//calculate the "front" based on the player's movement
		newPos = new Vector3 (player.transform.position.x, 0, player.transform.position.z);
		front = newPos - currPos + positionOffset;
		currPos = newPos;
			
		front.Normalize();

		//an attempt at handling camera flips
		float angle = Mathf.Acos (Vector3.Dot (-front, oldFront));
		
		if (angle < maxFlipAngle)
		{
			isFlipped = true;
		}
		
		if (isFlipped) 
		{
			flippedTime += Time.deltaTime;

			if (flippedTime > maxFlipTime || moveVertical == 0)
			{
				isFlipped = false;
				flippedTime = 0.0f;
			}
		}
		
		oldFront = front;

		//drawing front and gravity ray
		Debug.DrawRay (player.transform.position, front * 5, Color.cyan);
		Debug.DrawRay (player.transform.position, gravity * 5, Color.red);
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
}
