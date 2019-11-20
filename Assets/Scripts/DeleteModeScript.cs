using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeleteModeScript : MonoBehaviour {

	public LeapEventDelegatorScript leapEventDelegatorScript;
    public ContextSelectionScript contextSelectionScript;

	//for creation in BUILD mode
	//private int thisMode = 1;
	//private bool activeMode;
	//activate listeners
	/*void OnEnable()
	{
		UIManagerScript.modeChange += ModeListener;
	}
	void OnDisable()
	{
		UIManagerScript.modeChange -= ModeListener;
		interactionModeScript.rtTarget -= DeleteObject;
	}

	void ModeListener(int _mode)
	{
		if (_mode == thisMode)
		{
			interactionModeScript.rtTarget += DeleteObject;
			activeMode = true;
		}
		else 
		{
			interactionModeScript.rtTarget -= DeleteObject;
			activeMode = false;
		}
	}*/

	//for deleteing - triggered by leap event and by mouse input
	public void DeleteObject(GameObject caller_unused, GameObject deleteTarget)
	{
		//if deleting an atom //not delete something if it is action atom, action bond, base bond, related atoms
		if (deleteTarget.GetComponent<AtomManagerScript> () != null && deleteTarget != contextSelectionScript.actionAtom && deleteTarget != contextSelectionScript.relatedAtom1 && deleteTarget != contextSelectionScript.relatedAtom2) 
		{
			deleteTarget.GetComponent<AtomManagerScript> ().DeleteAtom ();
		}
		//if deleting a bond
		else if (deleteTarget.GetComponent<BondManagerScript> () != null && deleteTarget != contextSelectionScript.actionBond && deleteTarget != contextSelectionScript.baseBond)
		{
			deleteTarget.GetComponent<BondManagerScript> ().DeleteBond ();
		}
		GetComponentInParent<UndoRedoScript> ().AddEntry ();

		transform.parent.gameObject.GetComponent<LiveOptimizeScript>().moleculeChanged = true;
	}

	/*---------------------------------------------------------------------------------*/
	//keyboard mouse version
	RaycastHit mouseHit; LayerMask mouseMask;
	Rect delRect, invisDelRect; //depends on size of del slider
	bool deleting;
	GameObject[] atomsList, bondsList;
	GameObject tempDelHiglight;

	void Start()
	{
		mouseMask = LayerMask.GetMask ("atoms", "bonds");
		//activeMode = true;
		invisDelRect.size = Vector2.zero;
		delRect.size = Vector2.zero;
	}
	void Update()
	{
		//thumbing - deletion
		if (Input.GetKey (KeyCode.D)) 
		{
			deleting = true;
			delRect.center = new Vector2 (Input.mousePosition.x, Screen.height - Input.mousePosition.y);
			invisDelRect.center = new Vector2 (Input.mousePosition.x, Input.mousePosition.y);

			if(invisDelRect.size != Vector2.zero && Input.GetMouseButtonDown (0))
			{
				atomsList = GameObject.FindGameObjectsWithTag ("atoms");
				bondsList = GameObject.FindGameObjectsWithTag ("bonds");
				foreach(GameObject delObj in atomsList)
				{
					if (invisDelRect.Contains (Camera.main.WorldToScreenPoint (delObj.transform.position)))
						DeleteObject (null, delObj);
				}
				foreach(GameObject delObj in bondsList)
				{
					if (invisDelRect.Contains (Camera.main.WorldToScreenPoint (delObj.transform.position)))
						DeleteObject (null, delObj);
				}
			}
			else
			{
				Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (mouseRay, out mouseHit, 10, mouseMask))
				{
					if (mouseHit.collider.gameObject != tempDelHiglight)
					{
						if(tempDelHiglight)
							tempDelHiglight.GetComponent<cakeslice.Outline> ().enabled = false;
						tempDelHiglight = mouseHit.collider.gameObject;
						tempDelHiglight.GetComponent<cakeslice.Outline> ().color = 0; //highlights the atom or bond you are about to delete in red
						tempDelHiglight.GetComponent<cakeslice.Outline> ().enabled = true;
					}
					if(Input.GetMouseButtonDown(0))
						DeleteObject (null, tempDelHiglight);
				}
				else if(tempDelHiglight)
				{
					tempDelHiglight.GetComponent<cakeslice.Outline> ().enabled = false;
					tempDelHiglight = null;
				}
			}
		} 
		else
		{
			if(tempDelHiglight)
			{
				tempDelHiglight.GetComponent<cakeslice.Outline> ().enabled = false;
				tempDelHiglight = null;
			}
			deleting = false;
		}
	}

	public void DelColliderResize(UnityEngine.UI.Slider _delSlider)
	{
		print ("changing del size");
		delRect.size = new Vector2 (_delSlider.value * 500, _delSlider.value * 500);
		invisDelRect.size = new Vector2 (_delSlider.value * 500, _delSlider.value * 500);
	}

	void OnGUI()
	{
		if (deleting && invisDelRect.size != Vector2.zero) 
		{
			GUI.Box(delRect, "delete area");
		}
	}
}
