using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EditModeScript : MonoBehaviour
{
    public ContextSelectionScript contextSelectionScript;
    /*void OnDisable()
	{
		LeapInteractionModeScript.rgStart -= StartMove;
		LeapInteractionModeScript.rgEnd -= EndMove;
	}

	void ModeListener(int _mode)
	{
		if (_mode == thisMode)
		{
			LeapInteractionModeScript.rgStart += StartMove;
			LeapInteractionModeScript.rgEnd += EndMove;
			activeMode = true;
		}
		else 
		{
			LeapInteractionModeScript.rgStart -= StartMove;
			LeapInteractionModeScript.rgEnd -= EndMove;
			activeMode = false;
		}
	}*/

    /*----------------------------------------------------------------*/
    public GameObject measureLine, measureText;
	private MeasureLineRenderScript actionArc, rotateArc;
	private GameObject rotateArcText;

	/*------------------------------------------------------------------------------------------------------------------------------*/
	//VR version

	private GameObject grabPoint, grabTarget;
	private bool grabbing, initialOutlineState;
	private int initialOutlineColor;
	private Vector3 startVector, startPos, perpVector; //some are used by mouse version as well
	private Quaternion startRot, perpQuat;

	public void StartMove(GameObject _rightGrabPoint, GameObject _rightGrabTarget)
	{
		transform.parent.gameObject.GetComponent<LiveOptimizeScript>().midCreation = true;

		grabbing = true;
		grabPoint = _rightGrabPoint; grabTarget = _rightGrabTarget;

		if (grabTarget != null && (grabTarget == contextSelectionScript.relatedAtom1 || grabTarget == contextSelectionScript.relatedAtom2) && contextSelectionScript.actionAtom == null && contextSelectionScript.actionBond == null)
		{
			//stretch
			StartCoroutine(StretchUpdate());
		}
		else if (contextSelectionScript.actionAtom != null && contextSelectionScript.noRingDetection)
		{
			//rotate atom
			StartCoroutine(RotateAtomUpdate());
		}
		else if (contextSelectionScript.actionBond != null && contextSelectionScript.noRingDetection)
		{
			//rotate bond
			StartCoroutine(RotateBondUpdate());
		}
		else if (grabTarget != null && grabTarget.GetComponent<AtomManagerScript>() != null && !grabTarget.transform.GetChild(0).gameObject.activeSelf) //cannot move child meant for rotation
		{
			//regular move
			StartCoroutine(RegularMoveUpdate());
		}
	}

	public void EndMove(GameObject caller_unused, GameObject target_unused)
	{
		grabbing = false;
	}

	IEnumerator RegularMoveUpdate()
	{
		grabTarget.transform.position = grabPoint.transform.position;
		grabTarget.GetComponent<Rigidbody>().isKinematic = false;
		grabTarget.GetComponent<FixedJoint>().connectedBody = grabPoint.GetComponent<Rigidbody>(); //print ("picking up atom");
		grabTarget.transform.GetChild(0).gameObject.SetActive(true);
        foreach (GameObject bond in grabTarget.GetComponent<AtomManagerScript>().bonds)
			Instantiate(measureLine, bond.transform);

		while (grabbing && grabTarget)
		{
			grabTarget.GetComponent<AtomManagerScript>().UpdateBonds();
			yield return null;
		}

		if (grabTarget && grabTarget.activeInHierarchy && grabTarget.GetComponent<FixedJoint>().connectedBody == grabPoint.GetComponent<Rigidbody>())
		{
			foreach (GameObject bond in grabTarget.GetComponent<AtomManagerScript>().bonds)
				Destroy(bond.GetComponentInChildren<MeasureLineRenderScript>().gameObject);
			grabTarget.GetComponent<Rigidbody>().isKinematic = true;
			grabTarget.GetComponent<FixedJoint>().connectedBody = null;
			grabTarget.transform.GetChild(0).gameObject.SetActive(false);
            grabTarget.GetComponent<AtomManagerScript>().UpdateBonds();
		}
		grabTarget = null;
		transform.parent.GetComponent<UndoRedoScript>().AddEntry();
		transform.parent.gameObject.GetComponent<LiveOptimizeScript>().moleculeChanged = true; //TODO REMOVE
		transform.parent.gameObject.GetComponent<LiveOptimizeScript>().midCreation = false; //TODO REMOVE
	}

	IEnumerator StretchUpdate()
	{
		contextSelectionScript.noRingDetection = contextSelectionScript.baseBond.GetComponent<BondManagerScript>().HierarchyBuilder(gameObject, grabTarget);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
        }

        /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline>())
		{
			outline.color = 2;
			outline.enabled = true;
		}*/

        startVector = grabPoint.transform.position;
		startPos = transform.position;
		while (grabbing && grabTarget && contextSelectionScript.noRingDetection)
		{
			transform.position = startPos + contextSelectionScript.baseBond.transform.up * Vector3.Dot(contextSelectionScript.baseBond.transform.up, grabPoint.transform.position - startVector);
			contextSelectionScript.baseBond.GetComponent<BondManagerScript>().UpdateBond();
			yield return null;
		}

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject != contextSelectionScript.relatedAtom1 && transform.GetChild(i).gameObject != contextSelectionScript.relatedAtom2)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }

        /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline>())
		{
			if (outline.gameObject != contextSelectionScript.relatedAtom1 && outline.gameObject != contextSelectionScript.relatedAtom2)
				outline.enabled = false;
		}*/
        DetachChildrenToParent();
		transform.parent.GetComponent<UndoRedoScript>().AddEntry();
		transform.parent.gameObject.GetComponent<LiveOptimizeScript>().moleculeChanged = true; //TODO REMOVE
		transform.parent.gameObject.GetComponent<LiveOptimizeScript>().midCreation = false; //TODO REMOVE
	}


	private int snappingIncrement = 15;

	IEnumerator RotateAtomUpdate()
	{
		contextSelectionScript.noRingDetection = contextSelectionScript.baseBond.GetComponent<BondManagerScript>().HierarchyBuilder(gameObject, contextSelectionScript.actionAtom);
		startVector = Vector3.Cross(contextSelectionScript.baseBond.transform.up, grabPoint.transform.position - contextSelectionScript.actionAtom.transform.position).normalized;
		startRot = transform.rotation;

		rotateArc = Instantiate(measureLine, contextSelectionScript.actionAtom.transform).GetComponent<MeasureLineRenderScript>();
		rotateArcText = Instantiate(measureText, contextSelectionScript.actionAtom.transform);

		while (grabbing && contextSelectionScript.noRingDetection)
		{
			Vector3 currentVector = Vector3.Cross(contextSelectionScript.baseBond.transform.up, grabPoint.transform.position - contextSelectionScript.actionAtom.transform.position).normalized;
			transform.rotation = Quaternion.FromToRotation(startVector, currentVector) * startRot;

			Quaternion localI = Quaternion.Inverse(contextSelectionScript.baseBond.transform.rotation) * Quaternion.FromToRotation(startVector, startVector);
			Quaternion localR = Quaternion.Inverse(contextSelectionScript.baseBond.transform.rotation) * Quaternion.FromToRotation(startVector, currentVector);
			float angle = localR.eulerAngles.y - localI.eulerAngles.y;
			rotateArc.ShowAngleArc(angle % 360, contextSelectionScript.baseBond.transform.up);
			if (UIManagerScript.rotationSnapping)
			{
				float snappedAngle = ((int)((angle + (snappingIncrement / 2)) / snappingIncrement)) * snappingIncrement;
				transform.rotation = Quaternion.AngleAxis(snappedAngle, contextSelectionScript.baseBond.transform.up) * startRot; //startRot * Quaternion.Euler(0, snappedAngle, 0); 
				rotateArc.ShowAngleArc(snappedAngle % 360, contextSelectionScript.baseBond.transform.up);
			}
			yield return null;
		}
		Destroy(rotateArc.gameObject);
		Destroy(rotateArcText);
		DetachChildrenToParent();
		transform.parent.GetComponent<UndoRedoScript>().AddEntry();
		transform.parent.gameObject.GetComponent<LiveOptimizeScript>().moleculeChanged = true; //TODO REMOVE
		transform.parent.gameObject.GetComponent<LiveOptimizeScript>().midCreation = false; //TODO REMOVE
	}

	IEnumerator RotateBondUpdate()
	{

		if (contextSelectionScript.relatedAtom1.GetComponent<AtomManagerScript>().bonds.Contains(contextSelectionScript.actionBond))
		{
			contextSelectionScript.noRingDetection = contextSelectionScript.actionBond.GetComponent<BondManagerScript>().HierarchyBuilderOnBond(gameObject, contextSelectionScript.relatedAtom1);
			actionArc = contextSelectionScript.relatedAtom1.GetComponentInChildren<MeasureLineRenderScript>();
		}
		else
		{
			contextSelectionScript.noRingDetection = contextSelectionScript.actionBond.GetComponent<BondManagerScript>().HierarchyBuilderOnBond(gameObject, contextSelectionScript.relatedAtom2);
			actionArc = contextSelectionScript.relatedAtom2.GetComponentInChildren<MeasureLineRenderScript>();
		}

		perpVector = Vector3.Cross(contextSelectionScript.baseBond.transform.up, contextSelectionScript.actionBond.transform.up);
		perpQuat = Quaternion.LookRotation(contextSelectionScript.baseBond.transform.up, perpVector);
		startVector = Vector3.Cross(perpVector, grabPoint.transform.position - transform.position).normalized;
		startRot = transform.rotation;

		while (grabbing && contextSelectionScript.noRingDetection)
		{
			Vector3 currentVector = Vector3.Cross(perpVector, grabPoint.transform.position - transform.position);
			transform.rotation = Quaternion.FromToRotation(startVector, currentVector) * startRot;
			actionArc.ShowAngleArc(contextSelectionScript.baseBond.transform.position, contextSelectionScript.actionBond.transform.position);
			Quaternion localI = Quaternion.Inverse(perpQuat) * Quaternion.FromToRotation(startVector, startVector);
			Quaternion localR = Quaternion.Inverse(perpQuat) * Quaternion.FromToRotation(startVector, currentVector);
			float angle = localR.eulerAngles.y - localI.eulerAngles.y;

			if (UIManagerScript.rotationSnapping)
			{
				float snappedAngle = ((int)((angle + (snappingIncrement / 2)) / snappingIncrement)) * snappingIncrement;
				transform.rotation = Quaternion.AngleAxis(snappedAngle, perpVector) * startRot;
                actionArc.ShowAngleArc(contextSelectionScript.baseBond.transform.position, contextSelectionScript.actionBond.transform.position);
            }

            yield return null;
		}
		DetachChildrenToParent();
		transform.parent.GetComponent<UndoRedoScript>().AddEntry();
		transform.parent.gameObject.GetComponent<LiveOptimizeScript>().moleculeChanged = true; //TODO REMOVE
		transform.parent.gameObject.GetComponent<LiveOptimizeScript>().midCreation = false; //TODO REMOVE
	}

	/*-----------------------------------------------------------------------------------------------------------------------------------------------------------------*/
	//mouse keyboard version

	/*private bool mouseMoving, mouseRegularMoving, mouseStretching, mouseRotatingAtom, mouseRotatingBond; 
	private RaycastHit mouseRayHit;
	private LayerMask mouseMask;
	private GameObject mouseTarget;
	private Vector3 screenAxis, worldAxis;

	void Start()
	{
		mouseMask = LayerMask.GetMask("atoms");
		//activeMode = true;
	}

	void Update()
	{
		if(Input.GetKey(KeyCode.V) && Input.GetMouseButtonDown(0) && activeMode)
		{
			//start move
			mouseMoving = true;
			Ray mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (mouseRay, out mouseRayHit, 10, mouseMask))
			{
				mouseTarget = mouseRayHit.collider.gameObject;
				if((mouseTarget == contextSelectionScript.relatedAtom1 || mouseTarget == contextSelectionScript.relatedAtom2) && contextSelectionScript.actionAtom == null && contextSelectionScript.actionBond == null )
				{ 
					//build hierarchy
					contextSelectionScript.noRingDetection = contextSelectionScript.baseBond.GetComponent<BondManagerScript>().HierarchyBuilder(gameObject, mouseTarget);
					//stretch
					if(contextSelectionScript.noRingDetection)
					{
						startVector = Input.mousePosition;
						startPos = transform.position;
						worldAxis = (contextSelectionScript.baseBond.transform.position - transform.position).normalized;
						screenAxis = ScreenAxis (contextSelectionScript.baseBond);
						mouseStretching = true;
						foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
						{
							outline.color = 2;
							outline.enabled = true;
						}
					}
					else
					{
						print ("bond is part of ring, no stretching allowed");
					}
				}
				else if(!mouseTarget.GetComponent<cakeslice.Outline>().enabled) //shouldn't move child in hierarchy individually
				{
					//regular move
					mouseTarget.GetComponent<cakeslice.Outline> ().color = 1;
					mouseTarget.GetComponent<cakeslice.Outline> ().enabled = true;
					foreach(GameObject bond in mouseTarget.GetComponent<AtomManagerScript>().bonds)
						Instantiate (measureLine, bond.transform);
					mouseRegularMoving = true;

				}
			}

			if(contextSelectionScript.actionAtom != null)
			{
				//rotate atom
				contextSelectionScript.noRingDetection = contextSelectionScript.baseBond.GetComponent<BondManagerScript> ().HierarchyBuilder (gameObject, contextSelectionScript.actionAtom);

				Debug.Log("NO DETECTION: " + contextSelectionScript.noRingDetection);
				if(contextSelectionScript.noRingDetection)
				{
					startVector = Input.mousePosition;
					startRot = transform.rotation;
					rotateArc = Instantiate(measureLine, contextSelectionScript.actionAtom.transform).GetComponent<MeasureLineRenderScript>();
					rotateArcText = Instantiate (measureText, contextSelectionScript.actionAtom.transform);
					mouseRotatingAtom = true;
				}
				else
				{
					print ("bond is part of ring, no stretching allowed");
				}
			}
			else if(contextSelectionScript.actionBond != null)
			{
				//rotate bond
				if (contextSelectionScript.relatedAtom1.GetComponent<AtomManagerScript> ().bonds.Contains (contextSelectionScript.actionBond))
				{
					contextSelectionScript.noRingDetection = contextSelectionScript.actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, contextSelectionScript.relatedAtom1);
					actionArc = contextSelectionScript.relatedAtom1.GetComponentInChildren<MeasureLineRenderScript> ();
				}
				else
				{
					contextSelectionScript.noRingDetection = contextSelectionScript.actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, contextSelectionScript.relatedAtom2);
					actionArc = contextSelectionScript.relatedAtom2.GetComponentInChildren<MeasureLineRenderScript> ();
				}

				if(contextSelectionScript.noRingDetection)
				{
					perpVector = Vector3.Cross(contextSelectionScript.baseBond.transform.up, contextSelectionScript.actionBond.transform.up);
					startVector =  Input.mousePosition;
					startRot = transform.rotation;
					mouseRotatingBond = true;
				}
				else
				{
					print ("bond is part of ring, no stretching allowed");
				}
			}
		}

		if(mouseMoving)
		{
			if(mouseStretching)
			{
				transform.position = startPos + worldAxis * Vector3.Dot(screenAxis, Input.mousePosition - startVector) / 500; //magic number for mouse to world 
				contextSelectionScript.baseBond.GetComponent<BondManagerScript> ().UpdateBond ();
			}
			else if(mouseRegularMoving)
			{
				mouseTarget.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint (mouseTarget.transform.position).z));
				mouseTarget.GetComponent<AtomManagerScript> ().UpdateBonds ();
			}
			else if (mouseRotatingAtom)
			{
				float rotAngle = Input.mousePosition.x - startVector.x;

				if (UIManagerScript.rotationSnapping)
				{
					rotAngle = (float)(((int)((rotAngle + (snappingIncrement/2))/snappingIncrement)) * snappingIncrement);
				}
				rotateArc.ShowAngleArc (rotAngle % 360, contextSelectionScript.baseBond.transform.up); // (contextSelectionScript.actionAtom.transform.position - contextSelectionScript.baseBond.transform.position).normalized);
				Quaternion rotVec = Quaternion.AngleAxis (rotAngle, contextSelectionScript.baseBond.transform.up);
				transform.rotation = rotVec * startRot;

			}
			else if(mouseRotatingBond)
			{
				actionArc.ShowAngleArc (contextSelectionScript.baseBond.transform.position, contextSelectionScript.actionBond.transform.position);
				float rotAngle = Input.mousePosition.x - startVector.x;
				if (UIManagerScript.rotationSnapping)
				{
					rotAngle = (float)(((int)((rotAngle + (snappingIncrement/2))/snappingIncrement)) * snappingIncrement);
				}

				Quaternion rotVec = Quaternion.AngleAxis (rotAngle, perpVector);
				transform.rotation = rotVec * startRot;
			}
		}

		if(mouseMoving && Input.GetMouseButtonUp(0))
		{
			//end move
			if(mouseStretching)
			{
				foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
				{
					if (outline.gameObject != contextSelectionScript.relatedAtom1 && outline.gameObject != contextSelectionScript.relatedAtom2)
						outline.enabled = false;
				}
				mouseStretching = false;
				transform.parent.GetComponent<UndoRedoScript>().AddEntry();
			}
			else if(mouseRegularMoving)
			{
				mouseTarget.GetComponent<cakeslice.Outline> ().enabled = false;
				foreach (GameObject bond in mouseTarget.GetComponent<AtomManagerScript>().bonds)
					Destroy (bond.GetComponentInChildren<MeasureLineRenderScript> ().gameObject);
				mouseRegularMoving = false;
				transform.parent.GetComponent<UndoRedoScript>().AddEntry();
			}
			else if (mouseRotatingAtom)
			{
				Destroy (rotateArc.gameObject);
				Destroy (rotateArcText);
				mouseRotatingAtom = false;
				transform.parent.GetComponent<UndoRedoScript>().AddEntry();
			}
			else if(mouseRotatingBond)
			{
				mouseRotatingBond = false;
				transform.parent.GetComponent<UndoRedoScript>().AddEntry();
			}
			mouseMoving = false;
			DetachChildrenToParent ();
		}
	}*/

	Vector3 ScreenAxis(GameObject _bond)
	{
		Vector3 ScreenPoint1 = Camera.main.WorldToScreenPoint(transform.position);
		Vector3 ScreenPoint2 = Camera.main.WorldToScreenPoint(_bond.transform.position);
		return (ScreenPoint2 - ScreenPoint1).normalized;
	}

	void DetachChildrenToParent()
	{
		List<Transform> childrenToFree = new List<Transform>();
		foreach (Transform child in transform)
			childrenToFree.Add(child);
		foreach (Transform child in childrenToFree)
			child.transform.parent = transform.parent;
	}

}