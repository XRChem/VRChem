using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BondLengthModeScript : MonoBehaviour {

	//script for changing the bond length

	public GameObject stretchingRoot;
	public GameObject MeasureLine, MeasureText;
	public GameObject textCam;

	private GameObject stretchingBond, stretchingAtom, grabObject;
	private bool pointing, stretching, stretchingAllowed = false, settingBond = true;
	private Vector3 rootStartPos, startVector;

	public void SetStretchingObject(GameObject _object)
	{
		//first sets the target bond
		if (settingBond && _object.GetComponent<BondManagerScript>() != null && pointing) 
		{
			stretchingBond = _object;
			stretchingAtom = null;
			settingBond = false;
		}
		//then sets the atom that will move in order to stretch that bond (similar to rotation)
		else if(!settingBond && _object.GetComponent<AtomManagerScript>() != null && pointing)
		{
			stretchingAtom = _object;
			stretchingRoot.transform.DetachChildren ();
			stretchingAllowed = stretchingBond.GetComponent<BondManagerScript> ().HierarchyBuilder (stretchingRoot, stretchingAtom); //attaches all related atoms to also move
			settingBond = true;
		}
	}

	public void Pointing(bool _pointing)
	{
		pointing = _pointing; //checks if the gesture is correct (pointing gesture)
	}

	//after bond and atom are set, grab gesture anywhere and move hand
	public void StartStretch(GameObject grabPointObject)
	{
		if (stretchingAllowed && stretchingAtom != null && stretchingBond != null) 
		{
			stretching = true;
			grabObject = grabPointObject;
			startVector = grabObject.transform.position;
			rootStartPos = stretchingRoot.transform.position;
			StartCoroutine (StretchUpdate ()); //updates every frame while stretching bond, see below
		}
	}

	public void EndStretch()
	{
		stretching = false;
	}

	IEnumerator StretchUpdate()
	{
		GameObject line = Instantiate (MeasureLine);
		GameObject text = Instantiate (MeasureText);
		//text.GetComponent<TextFaceCameraScript> ().GetCamera (textCam);
		while (stretching && stretchingAllowed) 
		{
			//moves the atom in based on hand movmement along bond vector
			stretchingRoot.transform.position = rootStartPos + stretchingBond.transform.up * Vector3.Dot(stretchingBond.transform.up, grabObject.transform.position - startVector);
			stretchingBond.GetComponent<BondManagerScript> ().UpdateBond ();
			line.GetComponent<MeasureLineRenderScript> ().ShowDistanceSegment ();
			text.transform.position = stretchingBond.transform.position;
			text.GetComponent<Text> ().text = (stretchingBond.transform.localScale.y * 20).ToString("F2") + " Å";
			yield return null;
		}
		Destroy (line);
		Destroy (text);
	}
}
