using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener_keyboard : MonoBehaviour
{
	public GameObject mousePointerObject, atomsBonds;
	private CreateModeScript createModeScript;

	private int thisMode = 1; //build mode is 1, measure is 2, select is 3
	private bool creationModeActivated; //mouse input checks this instead of checking the event system

	//activate listeners
	/*void OnEnable()
	{
		UIManagerScript.modeChange += ModeListener;
	}
	void OnDisable()
	{
		UIManagerScript.modeChange -= ModeListener;
	}

	//when mode is changed
	void ModeListener(int _mode)
	{
		if (_mode == thisMode)
		{
			creationModeActivated = true;
		}
		else
		{
			creationModeActivated = false;
		}
	}*/

	private GameObject initialAtom, createdAtom, createdBond, bondToReplace;
	private bool mouseButtonDown = false, creatingNew = false, replacingOrBonding = false;
	private LayerMask mouseMask; //atom mask, atoms and bonds mask, bonds mas


	void Start()
	{
		createModeScript = atomsBonds.GetComponent<CreateModeScript>();

		mouseMask = LayerMask.GetMask("atoms", "bonds");
		creationModeActivated = true;
	}

	void Update()
	{
		if (Input.GetKey(KeyCode.C) && Input.GetMouseButtonDown(0) && creationModeActivated)
		{
			//start create
			mouseButtonDown = true;

			Ray mouseRay; RaycastHit mouseHit;
			mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(mouseRay, out mouseHit, 10, mouseMask))			//Mouse cursor hits an object
			{
				mousePointerObject.transform.position = mouseHit.transform.position;
				replacingOrBonding = true;
				createModeScript.StartCreate(mousePointerObject, mouseHit.transform.gameObject);
			}
			else				//Clicked on empty space
			{
				creatingNew = true;
				mousePointerObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(atomsBonds.transform.position).z));

				createModeScript.StartCreate(mousePointerObject, null);
				//Debug.Break();
			}
		}

		if (mouseButtonDown)
		{
			mousePointerObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(atomsBonds.transform.position).z));
		}

		if (mouseButtonDown && Input.GetMouseButtonUp(0))
		{
			//end create
			mouseButtonDown = false;
			mousePointerObject.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(atomsBonds.transform.position).z));

			Ray mouseRay; RaycastHit mouseHit;
			mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			createModeScript.EndCreate(mousePointerObject, Physics.Raycast(mouseRay, out mouseHit, 10, mouseMask) == true ? mouseHit.transform.gameObject : null);
		}

		if (Input.GetKey(KeyCode.M))        //TODO remove lol
			createModeScript.RemoveEverythingWrapper();

		if (Input.GetKeyDown(KeyCode.O))
		{
			StartCoroutine(atomsBonds.GetComponent<OpenBabelScript>().Optimize(20));
		}

	}
}
