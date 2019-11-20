using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RotateModeScript : MonoBehaviour {

	//script to rotate molecular sub structure around bond
	public GameObject rotationRoot, measureLine, measureText, textCamera;

	private GameObject rotationBond, rotationAtom, grabPointObject;
	private bool isSettingTarget, settingBond = true, rotationAllowed;

	//find the bond and which side of the bond to rotate - the relative rotation changes either way but this is for convinience
	public void SetRotateTarget(GameObject _targetObject)
	{
		if (settingBond && _targetObject.GetComponent<BondManagerScript> () != null && isSettingTarget) 
		{
			rotationBond = _targetObject;
			settingBond = false;
			rotationAtom = null;
		} 
		else if (!settingBond && _targetObject.GetComponent<AtomManagerScript> () != null && isSettingTarget)
		{
			rotationAtom = _targetObject;
			rotationRoot.transform.DetachChildren ();
			rotationRoot.transform.rotation = rotationBond.transform.rotation;
			rotationAllowed = rotationBond.GetComponent<BondManagerScript> ().HierarchyBuilder (rotationRoot, rotationAtom);
			settingBond = true;
		}
	}

	//needs a button to clear selection
	public void ClearRotateTargets()
	{
	}

	public void isSettingRotateTarget(bool _pointing)
	{
		isSettingTarget = _pointing;
	}

	private Vector3 startingVector, startingEuler;
	private Quaternion startingRotation;
	private bool rotating, snapping = false;
	private int snappingIncrement = 15;

	public void ToggleSnapping()
	{
		snapping = !snapping;
	}

	//when grab, start rotating
	public void StartRotate(GameObject _grabPoint)
	{
		if (rotationAtom != null & rotationBond != null && rotationAllowed)
		{
			grabPointObject = _grabPoint;
			startingVector = Vector3.Cross(rotationBond.transform.up, grabPointObject.transform.position - rotationAtom.transform.position).normalized;
			startingRotation = rotationRoot.transform.rotation;
			rotating = true;
			StartCoroutine (RotateUpdate ());
		}
	}

	//when release, stop rotating
	public void EndRotate()
	{
		rotating = false;
	}
		
	//runs every frame while grabbing
	IEnumerator RotateUpdate()
	{
		GameObject arc = Instantiate(measureLine, rotationAtom.transform.position, Quaternion.LookRotation(startingVector, rotationBond.transform.up)) as GameObject;
		GameObject text = Instantiate(measureText, rotationAtom.transform.position, Quaternion.identity) as GameObject;
		//text.GetComponent<TextFaceCameraScript> ().GetCamera (textCamera);
		while (rotating) 
		{
			Vector3 currentVector = Vector3.Cross(rotationBond.transform.up, grabPointObject.transform.position - rotationAtom.transform.position).normalized;
			rotationRoot.transform.rotation = Quaternion.FromToRotation(startingVector, currentVector) * startingRotation;

			Quaternion localI = Quaternion.Inverse (startingRotation) * Quaternion.FromToRotation (startingVector, startingVector);
			Quaternion localR = Quaternion.Inverse (startingRotation) * Quaternion.FromToRotation (startingVector, currentVector);
			float angle = localR.eulerAngles.y - localI.eulerAngles.y;

			//arc.GetComponent<MeasureLineRenderScript> ().ShowAngleArc (angle);
			text.GetComponent<Text> ().text = angle.ToString ("F1");

			if(snapping)
			{
				float snappedAngle = ((int)((angle + (snappingIncrement / 2)) / snappingIncrement)) * snappingIncrement;
				rotationRoot.transform.rotation = startingRotation * Quaternion.Euler(0, snappedAngle, 0);
				//arc.GetComponent<MeasureLineRenderScript> ().ShowAngleArc (snappedAngle);
				text.GetComponent<Text> ().text = snappedAngle.ToString ("F1");
			}
			yield return null;
		}
		Destroy (arc);
		Destroy (text);
	}

}