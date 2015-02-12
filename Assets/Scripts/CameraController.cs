using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour 
{
	public GameObject player;
	private PlayerController playerController;

	private Vector3 front;
	private Vector3 gravity;
	private Vector3 target;

	public float upDist;
	public float awayDist;
	public float smooth;

	public Vector3 direction;

	private float maxHeight = 2;

	void Start ()
	{
		playerController = player.GetComponent <PlayerController> ();
	}

	void Update () 
	{
		front = playerController.front;
		gravity = playerController.gravity;
	}

	void FixedUpdate()
	{
		target = player.transform.position + (-gravity * upDist) + (-front * awayDist);

		transform.position = Vector3.Slerp (transform.position, target, Time.deltaTime * smooth);

		transform.LookAt (player.transform);

		direction = target - transform.position;
	}
}