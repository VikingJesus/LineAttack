using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

	#region Movement Veribles
	[SerializeField] float moveSpeed = 20;
	float currentMoveSpeed;
	[SerializeField] float rotateSpeed = 2;
	[SerializeField] float zoomSpeed = 10;

	[SerializeField] float maxPosX;
	[SerializeField] float minPosX;

	[SerializeField] float maxPosZ;
	[SerializeField] float minPosZ;

	[SerializeField] float maxPosY = 40;
	[SerializeField] float minPosY = 25;

	#endregion

	private void Start()
	{
		Cursor.lockState = CursorLockMode.None;
	}

	private void Update()
	{
		MovePlayer(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		RotatePlayer(Input.GetAxis("Rotation"));
		ZoomPlayer(-Input.GetAxis("Mouse ScrollWheel")* zoomSpeed);
		MouseControll();
	}

	#region PlayerMovement
	//Moves the player on the horizontal and Lateral axies, clamps the players movement to the grid size pluss a small amount.
	private void MovePlayer(float hozValm, float latval)
	{
		if (ShiftTest() == true)
			currentMoveSpeed = moveSpeed * 2;
		else
			currentMoveSpeed = moveSpeed;

		gameObject.transform.Translate(Vector3.right * (hozValm * currentMoveSpeed) * Time.deltaTime);
		gameObject.transform.Translate(Vector3.forward * (latval * currentMoveSpeed) * Time.deltaTime);


		Vector3 pos = transform.position;
		float posX = Mathf.Clamp(pos.x, minPosX, maxPosX);
		float posZ = Mathf.Clamp(pos.z, minPosZ, maxPosZ);
		Vector3 newPos = new Vector3(posX, pos.y, posZ);
		gameObject.transform.position = newPos;
	}

	//Rottates around the vertical axies.
	private void RotatePlayer(float rotVal)
	{
		gameObject.transform.RotateAround(gameObject.transform.position, Vector3.up, (-rotVal * rotateSpeed));
	}

	//Pans the camara in and out and clamps it between a min and max set in the veriables.
	public void ZoomPlayer(float zoomVal)
	{

		if (ShiftTest() == true)
			currentMoveSpeed = moveSpeed * 2;
		else
			currentMoveSpeed = moveSpeed;

		gameObject.transform.Translate(Vector3.up * (zoomVal * currentMoveSpeed) * Time.deltaTime);

		Vector3 pos = transform.position;
		float posY = Mathf.Clamp(pos.y, minPosY, maxPosY);
		Vector3 newPos = new Vector3(pos.x, posY, pos.z);
		gameObject.transform.position = newPos;

	}

	//test if the shift axis (as seen/set on the input manager) is active.
	bool ShiftTest()
	{
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			return true;
		else
			return false;
	}
	#endregion

	#region Mouse Input

	//Handles all the mouse 
	private void MouseControll()
	{
		//int hozSpeed = 0;
		//int latSpeed = 0;

		//if (Input.mousePosition.y >= Screen.height * 0.99)
		//	latSpeed = 1;
		//else if (Input.mousePosition.y <= Screen.height * 0.01)
		//	latSpeed = -1;

		//if (Input.mousePosition.x >= Screen.width * 0.99)
		//	hozSpeed = 1;
		//else if (Input.mousePosition.x <= Screen.width * 0.01)
		//	hozSpeed = -1;

		//MovePlayer(hozSpeed, latSpeed);
		ZoomPlayer(Mathf.RoundToInt(Input.GetAxis("Mouse ScrollWheel")));
	}

	#endregion
}
