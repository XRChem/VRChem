using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleScript : MonoBehaviour {

	private bool rotating, panning;
	private Vector3 sum, startVector, startPos;
	private Quaternion startRot;
	private GameObject grabPoint;
	Renderer[] childRends;
	List<Transform> children = new List<Transform> ();


	public bool ReCenterParent()
	{
		childRends = GetComponentsInChildren<Renderer> ();

		if(childRends.Length > 0)
		{
			sum = Vector3.zero;
			for (int i = 0; i < childRends.Length; i++)
			{
				sum += childRends [i].bounds.center;
			}

			//unparent
			foreach (Transform child in transform)
			{
				children.Add (child);
			}
			foreach (Transform child in children)
			{
				child.parent = null;
			}

			//center the parent
			transform.position = sum / childRends.Length;

			//reparent
			foreach (Transform child in children)
			{
				child.parent = transform;
			}
			children.Clear ();
			return true;
		}
		else
			return false; //if there are no atoms in the scene, it will return false and no rotation will take place.
	}

	/*-------------------------------------------------------------------------------------------------------*/
	//VR rotation

	public void StartRotate(GameObject _grabPoint, GameObject target_unused)
	{
		if (ReCenterParent ())
		{
			rotating = true;
			grabPoint = _grabPoint;
			startVector = grabPoint.transform.position - transform.position;
			startRot = transform.rotation;
			StartCoroutine (RotateUpdate ());
		}
	}

	public void EndRotate(GameObject caller_unused, GameObject target_unused)
	{
		rotating = false;
	}

	IEnumerator RotateUpdate()
	{
		while (rotating) 
		{
			Quaternion rotVec = Quaternion.FromToRotation(startVector, grabPoint.transform.position - transform.position);
			//rotate the parent
			transform.rotation = rotVec * startRot;
			//there is an issue when the rotation angle is around 180 degrees so I reset the start rotation every frame and the rotation delta will remain small
			startVector = grabPoint.transform.position - transform.position;
			startRot = transform.rotation;
			yield return null;
		}
	}
	//there is no panning and zooming  - somewhat handled by head movement

	/*--------------------------------------------------------------------------------------------------------------------------------------*/
	//for mouse rotation, panning, and zooming - it moves the molecule, not the cameras

	private bool mouseRotating, mousePanning;
	void Update()
	{
		if(Input.GetMouseButtonDown(1)) //right click - rotation
		{
			if(ReCenterParent())
			{
				mouseRotating = true;
				startVector = Input.mousePosition;
				startRot = transform.rotation;
			}
		}
		if(mouseRotating && Input.GetMouseButtonUp(1))
			mouseRotating = false;

		if(mouseRotating)
		{
			Quaternion rotVec = Quaternion.Euler (new Vector3(Input.mousePosition.y - startVector.y, startVector.x - Input.mousePosition.x, 0));
			transform.rotation = rotVec * startRot;
			startVector = Input.mousePosition;
			startRot = transform.rotation;
		}

		if(Input.GetMouseButtonDown(2)) //middle click for panning
		{
			mousePanning = true;
			startVector = Input.mousePosition;
			startPos = transform.position;
		}
		if (Input.GetMouseButtonUp (2))
			mousePanning = false;

		if(mousePanning)
		{
			transform.position = startPos + ((Input.mousePosition - startVector)/500); // 500 magic number to reduce the screenspace numbers to unity world spaces
		}

		if((Input.mouseScrollDelta.y > 0 && Camera.main.transform.InverseTransformPoint(transform.position).z > 0.3f) || Input.mouseScrollDelta.y < 0) // scroll for zooming
		{
			Vector3 centerDirection = Camera.main.ScreenPointToRay (Input.mousePosition).direction;
			transform.Translate (-centerDirection * Input.mouseScrollDelta.y / 20, Space.World); // 20 magic number for controlling zoom sensitivity
		}
	}
}
