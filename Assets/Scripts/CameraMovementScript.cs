using UnityEngine;
using System.Collections;

public class CameraMovementScript : MonoBehaviour {

	public GameObject cameraObject, eyeObject;

	private GameObject grabPoint;
	private bool panning = false;
	private Vector3 cameraAnchor, panAnchor;

	public void StartPan(GameObject _anchor)
	{
		panning = true;
		grabPoint = _anchor;
		cameraAnchor = cameraObject.transform.position;
		panAnchor = grabPoint.transform.position - cameraAnchor;
		StartCoroutine (PanUpdate());
	}

	public void EndPan()
	{
		panning = false;
	}

	IEnumerator PanUpdate()
	{
		while (panning) 
		{
			cameraObject.transform.position = cameraAnchor + ((grabPoint.transform.position - (cameraObject.transform.position + panAnchor)) * 4);
			yield return null;
		}
	}

	//-------------------------------------------------------------------------------

	private bool selecting;
	private GameObject pivotObject;

	public void SetPivotObject(GameObject _targetPivot)
	{
		if (selecting)
		{
			pivotObject = _targetPivot;
			print (pivotObject);
		}
	}

	public void ClearPivotObject()
	{
		//pivotObject = null;
	}

	public void SelectingPivotObject(bool _selecting)
	{
		selecting = _selecting;
	}

	//--------------------------------------------------------------------------------

	private GameObject pinchPoint;
	private bool pivoting = false;
	private Vector3 pivotAnchor, pivotRadius;

	public void StartPivot(GameObject _anchor)
	{
		pivoting = true;
		pinchPoint = _anchor;
		pivotAnchor = cameraObject.transform.InverseTransformVector(pinchPoint.transform.position - cameraObject.transform.position);
		StartCoroutine (PivotUpdate ());
	}

	public void EndPivot()
	{
		pivoting = false;
	}

	IEnumerator PivotUpdate()
	{
		while (pivoting) 
		{
			Vector3 pivotVector = pinchPoint.transform.position - (cameraObject.transform.position + cameraObject.transform.TransformVector(pivotAnchor));
			pivotVector = eyeObject.transform.InverseTransformVector (pivotVector);
			pivotAnchor = cameraObject.transform.InverseTransformVector(pinchPoint.transform.position - cameraObject.transform.position);
			print (pivotVector);
			if (pivotObject != null)
			{
				cameraObject.transform.RotateAround (pivotObject.transform.position, Vector3.up, pivotVector.x * -500);
				cameraObject.transform.RotateAround (pivotObject.transform.position, cameraObject.transform.right, pivotVector.y * 500);
			} 
			else 
			{
				cameraObject.transform.RotateAround (Vector3.zero, Vector3.up, pivotVector.x * -500);
				cameraObject.transform.RotateAround (Vector3.zero, cameraObject.transform.right, pivotVector.y * 500);
			}
			yield return null;
		}
	}
}