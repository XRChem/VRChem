using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContextSelectionScript : MonoBehaviour {

	public LeapEventDelegatorScript leapEventDelegatorScript;

    private Color selectionColorGreen = new Color(0f, 1f, 0.1725f);
    private Color selectionColorOrange = new Color(1f, 0.65f, 0f);

	//for context selection in BUILD mode
	//private int thisMode = 1;
	//private bool activeMode;

		//SelectionDelegate and SelectionMethod are used to allow the use on a single listener and still call the correct selection method, by switching the method SelectionMethod points to
	delegate void SelectionDelegate(GameObject pointedObject); //switching between the different selection states
	SelectionDelegate SelectionMethod;

	//activate listeners
	void OnEnable()
	{
		//UIManagerScript.modeChange += ModeListener;
		SelectionMethod = SetFirstSelection;
	}
	void OnDisable()
	{
		//UIManagerScript.modeChange -= ModeListener;
		//interactionModeScript.riTarget -= DelegateWrapper;
		//interactionModeScript.rtDown -= Deselection;
		SelectionMethod = null;
	}

	/*void ModeListener(int _mode)
	{
		if (_mode == thisMode)
		{
			interactionModeScript.riTarget += DelegateWrapper;
			interactionModeScript.rtDown += Deselection;
			SelectionMethod = SetFirstSelection;
			activeMode = true;
		}
		else 
		{
			interactionModeScript.riTarget -= DelegateWrapper;
			interactionModeScript.rtDown -= Deselection;
			Deselection (null, null);
			SelectionMethod = null;
			activeMode = false;
		}
	}*/

	/*----------------------------------------------------------------------------------*/
	//for Leap callback
	public void DelegateWrapper(GameObject caller_unused, GameObject _pointedObject) //using one listener and calling the correct selection function each time
	{
		SelectionMethod(_pointedObject);
	}

	/*----------------------------------------------------------------------------------*/
	//for keyboard mouse input
	/*private Ray mouseRay; private RaycastHit mouseHit; private LayerMask mouseMask;
	void Start()
	{
		//activeMode = true;
		mouseMask = LayerMask.GetMask ("atoms", "bonds");
		instructionText.text = "Hold 'c' and left click to create or replace. Hold 'v' and left click and drag to move. Hold 'd' and left click to delete.\nLeft click to select a first bond.";
		instructionTextVR.text = "Pinch(index and thumb) and release to create or replace. Grab(all fingers) to move. Touch with thumb to delete.\nTouch with index finger to select a first bond.";
	}
	void Update()
	{
		//poiting - selection
		if(Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.C) && !Input.GetKey(KeyCode.V) && !Input.GetKey(KeyCode.D)  && SelectionMethod != null)
		{
			mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (mouseRay, out mouseHit, 10, mouseMask))
			{
				SelectionMethod (mouseHit.collider.gameObject);
			}
		}
	}*/

	/*----------------------------------------------------------------------------------*/

	//for selection in order to stretch or rotate : works like a state system between first selection, second selection, and reset selection
	public GameObject baseBond, relatedAtom1, relatedAtom2, actionAtom, actionBond;
	private List<GameObject> relatedBonds = new List<GameObject>();
	public bool noRingDetection = false;
	public GameObject MeasureLine, MeasureText;
	public Text instructionText, instructionTextVR;
	private GameObject baseline, baseLengthText, actionline, actionLengthText, arcBetweenBonds, arcAngleText;

    private void SetHighlightColor (GameObject bond, Color color)
    {
        foreach (SpriteRenderer renderer in bond.GetComponentsInChildren<SpriteRenderer>(true))
        {
            renderer.color = color;
        }
    }

	private void SetFirstSelection(GameObject _indexTarget1)
	{
        Debug.Log("asd");
		if (_indexTarget1.GetComponent<BondManagerScript> () != null) 
		{
			baseBond = _indexTarget1;
            SetHighlightColor (baseBond, selectionColorOrange);
			baseBond.transform.GetChild(0).gameObject.SetActive(true);
            baseBond.GetComponent<BondManagerScript> ().GiveBondAtoms (out relatedAtom1, out relatedAtom2);
            relatedAtom1.transform.GetChild(0).gameObject.SetActive(true);
			relatedAtom2.transform.GetChild(0).gameObject.SetActive(true);
			relatedBonds.AddRange (relatedAtom1.GetComponent<AtomManagerScript> ().bonds);
			relatedBonds.AddRange (relatedAtom2.GetComponent<AtomManagerScript> ().bonds);
			baseline = Instantiate (MeasureLine, baseBond.transform);
			baseLengthText = Instantiate (MeasureText, baseBond.transform);
			SelectionMethod = SetSecondSelection;
			instructionText.text = "Move('v' + left-click + drag) adjacent atoms to stretch first bond. Select adjacent atom to rotate it around first bond axis. Select adjacent bond to rotate it towards/away from first bond. Left-click bond to deselect it or left-click non-adjacent bond to select that instead.";
			instructionTextVR.text = "Move(grab with all fingers) adjacent atoms to stretch first bond. Select adjacent atom to rotate it around first bond axis. Select adjacent bond to rotate it towards/away from first bond. Retouch bond to deselect it or touch non-adjacent bond to select that instead.";
		}
	}

	private void SetSecondSelection(GameObject _indexTarget2)
	{
		if(_indexTarget2 == baseBond)
		{
			//deselect the base bond
			baseBond.transform.GetChild(0).gameObject.SetActive(false);
            SetHighlightColor(baseBond, selectionColorGreen);
            relatedAtom1.transform.GetChild(0).gameObject.SetActive(false);
            relatedAtom2.transform.GetChild(0).gameObject.SetActive(false);
            Destroy (baseline);
			Destroy (baseLengthText);
			baseBond = null; relatedAtom1 = null; relatedAtom2 = null; relatedBonds.Clear ();
			SelectionMethod = SetFirstSelection;
			instructionText.text = "Hold 'c' and left click to create or replace. Hold 'v' and left click and drag to move. Hold 'd' and left click to delete.\nLeft click to select a first bond.";
			instructionTextVR.text = "Pinch(index and thumb) and release to create or replace. Grab(all fingers) to move. Touch with thumb to delete.\nTouch with index finger to select a first bond.";
		}
		else if(_indexTarget2 == relatedAtom1 || _indexTarget2 == relatedAtom2)
		{
			//set action atom as either of the related atoms
			relatedAtom1.transform.GetChild(0).gameObject.SetActive(false);
            relatedAtom2.transform.GetChild(0).gameObject.SetActive(false);
            actionAtom = _indexTarget2;
            SetHighlightColor(actionAtom, selectionColorOrange);
            actionAtom.transform.GetChild(0).gameObject.SetActive(true);
            SelectionMethod = ResetSelection;
			//build hierarchy and start listening for atom rotation
			noRingDetection = baseBond.GetComponent<BondManagerScript>().HierarchyBuilder(gameObject, actionAtom);
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
            }
			/*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
			{
				outline.color = 2;
				outline.enabled = true;
			}*/
			DetachChildrenToParent (); //incase the user wants to make changes while something is selected, the hierarchy should not get messed up - remake the hierarchy during move mode
			instructionText.text = "Move('v' + left click + drag) to rotate atom around first bond's axis. Reselect atom to deselect. Select other adjacent atom to switch which atom rotates.";
			instructionTextVR.text = "Move(grab and drg) to rotate atom around first bond's axis. Reselect atom to deselect. Select other adjacent atom to switch which atom rotates.";
		}
		else if(relatedBonds.Contains(_indexTarget2))
		{
			//set one of the related bonds as action bond
			actionBond = _indexTarget2;
            SetHighlightColor(actionBond, selectionColorOrange);
            relatedAtom1.transform.GetChild(0).gameObject.SetActive(false);
            relatedAtom2.transform.GetChild(0).gameObject.SetActive(false);
			//build hierarchy and outline
			if (relatedAtom1.GetComponent<AtomManagerScript> ().bonds.Contains (actionBond))
			{
				noRingDetection = actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, relatedAtom1);
				arcBetweenBonds = Instantiate (MeasureLine, relatedAtom1.transform);
				arcAngleText = Instantiate (MeasureText, relatedAtom1.transform);
			}
			else
			{
				noRingDetection = actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, relatedAtom2);
				arcBetweenBonds = Instantiate (MeasureLine, relatedAtom2.transform);
				arcAngleText = Instantiate (MeasureText, relatedAtom2.transform);
			}
			arcBetweenBonds.GetComponent<MeasureLineRenderScript> ().ShowAngleArc (baseBond.transform.position, actionBond.transform.position);

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
            }

            /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
			{
				outline.color = 2;
				outline.enabled = true;
			}*/
            actionline = Instantiate (MeasureLine, actionBond.transform);
			actionLengthText = Instantiate (MeasureText, actionBond.transform);
			DetachChildrenToParent ();
			SelectionMethod = ResetSelection;
			instructionText.text = "Move('v' + left click + drag) to rotate adjaent bond towards/away from first bond. Reselect adjacent bond to deselect. Select other adjacent bond to switch which bond rotates.";
			instructionTextVR.text = "Move(grab and drg) to rotate adjacent bond around first bond. Reselect adjacent bond to deselect. Select other adjacent bond to switch which bond rotates.";
		}
		else if(_indexTarget2.GetComponent<BondManagerScript>() != null)
		{
			//reset base bond, no need to change listeners
			relatedAtom1.transform.GetChild(0).gameObject.SetActive(false);
            relatedAtom2.transform.GetChild(0).gameObject.SetActive(false);
            baseBond.transform.GetChild(0).gameObject.SetActive(false);
            SetHighlightColor(baseBond, selectionColorGreen);
            Destroy(baseline); Destroy (baseLengthText);
			baseBond = _indexTarget2;
            SetHighlightColor(baseBond, selectionColorOrange);
            baseBond.transform.GetChild(0).gameObject.SetActive(true);
            baseBond.GetComponent<BondManagerScript> ().GiveBondAtoms (out relatedAtom1, out relatedAtom2);
			relatedAtom1.transform.GetChild(0).gameObject.SetActive(true);
			relatedAtom2.transform.GetChild(0).gameObject.SetActive(true);
            baseline = Instantiate (MeasureLine, baseBond.transform);
			baseLengthText = Instantiate (MeasureText, baseBond.transform);
			relatedBonds.Clear ();
			relatedBonds.AddRange (relatedAtom1.GetComponent<AtomManagerScript> ().bonds);
			relatedBonds.AddRange (relatedAtom2.GetComponent<AtomManagerScript> ().bonds);
		}
	}

	private void ResetSelection(GameObject _indexTarget3)
	{
		//if selected bond is base bond, deselect it, if there is action bond make it the base bond, remove action atoms and bonds
		if(_indexTarget3 == baseBond)
		{
			baseBond.transform.GetChild(0).gameObject.SetActive(false);
            SetHighlightColor(baseBond, selectionColorGreen);
            relatedAtom1.transform.GetChild(0).gameObject.SetActive(false);
            relatedAtom2.transform.GetChild(0).gameObject.SetActive(false);
            Destroy (baseline); Destroy (baseLengthText);

			if(actionBond != null)
			{
				Destroy (actionline); Destroy (actionLengthText); Destroy (arcBetweenBonds); Destroy (arcAngleText);
				if (relatedAtom1.GetComponent<AtomManagerScript> ().bonds.Contains (actionBond))
					noRingDetection = actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, relatedAtom1);
				else
					noRingDetection = actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, relatedAtom2);

                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                }

                /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
				{
					outline.enabled = false;
				}*/
                DetachChildrenToParent();

				baseBond = actionBond;
                actionBond = null;
				baseBond.transform.GetChild(0).gameObject.SetActive(true);
                baseBond.GetComponent<BondManagerScript> ().GiveBondAtoms (out relatedAtom1, out relatedAtom2);
				relatedAtom1.transform.GetChild(0).gameObject.SetActive(true);
				relatedAtom2.transform.GetChild(0).gameObject.SetActive(true);
                baseline = Instantiate (MeasureLine, baseBond.transform);
				baseLengthText = Instantiate (MeasureText, baseBond.transform);
				relatedBonds.Clear ();
				relatedBonds.AddRange (relatedAtom1.GetComponent<AtomManagerScript> ().bonds);
				relatedBonds.AddRange (relatedAtom2.GetComponent<AtomManagerScript> ().bonds);
				SelectionMethod = SetSecondSelection;
				instructionText.text = "Move('v' + left-click + drag) adjacent atoms to stretch first bond. Select adjacent atom to rotate it around first bond axis. Select adjacent bond to rotate it towards/away from first bond. Left-click bond to deselect it or left-click non-adjacent bond to select that instead.";
				instructionTextVR.text = "Move(grab with all fingers) adjacent atoms to stretch first bond. Select adjacent atom to rotate it around first bond axis. Select adjacent bond to rotate it towards/away from first bond. Retouch bond to deselect it or touch non-adjacent bond to select that instead.";
			}
			else if(actionAtom != null)
			{
				noRingDetection = baseBond.GetComponent<BondManagerScript>().HierarchyBuilder(gameObject, actionAtom);
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
                }

                /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
				{
					outline.enabled = false;
				}*/
                DetachChildrenToParent();
				Destroy (baseline);
                SetHighlightColor(actionAtom, selectionColorGreen);
                actionAtom = null; baseBond = null; relatedAtom1 = null; relatedAtom2 = null; relatedBonds.Clear ();
				SelectionMethod = SetFirstSelection;
				instructionText.text = "Hold 'c' and left click to create or replace. Hold 'v' and left click and drag to move. Hold 'd' and left click to delete.\nLeft click to select a first bond.";
				instructionTextVR.text = "Pinch(index and thumb) and release to create or replace. Grab(all fingers) to move. Touch with thumb to delete.\nTouch with index finger to select a first bond.";
			}
		}

		//if there is action atom, & if selected atom is the other related atom, build that hierarchy and wait for rotation
		else if((_indexTarget3 == relatedAtom1 && actionAtom == relatedAtom2) || (_indexTarget3== relatedAtom2 && actionAtom == relatedAtom1))
		{
			noRingDetection = baseBond.GetComponent<BondManagerScript>().HierarchyBuilder(gameObject, actionAtom);
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }

            /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
			{
				outline.enabled = false;
			}*/
			DetachChildrenToParent ();
            SetHighlightColor(actionAtom, selectionColorGreen);
            actionAtom = _indexTarget3;
            SetHighlightColor(actionAtom, selectionColorOrange);
            //build hierarchy and wait for atom rotation
            noRingDetection = baseBond.GetComponent<BondManagerScript>().HierarchyBuilder(gameObject, actionAtom);
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
            }

            /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
			{
				outline.color = 2;
				outline.enabled = true;
			}*/
            DetachChildrenToParent();
		}

		//if action atom/bond reselected, remove action atom/bond, keep base bond
		else if(_indexTarget3 == actionAtom)
		{
			noRingDetection = baseBond.GetComponent<BondManagerScript>().HierarchyBuilder(gameObject, actionAtom);
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }

            /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
			{
				outline.enabled = false;
			}*/
            DetachChildrenToParent();
            SetHighlightColor(actionAtom, selectionColorGreen);
            actionAtom = null;
			relatedAtom1.transform.GetChild(0).gameObject.SetActive(true);
			relatedAtom2.transform.GetChild(0).gameObject.SetActive(true);
			SelectionMethod = SetSecondSelection;
			instructionText.text = "Move('v' + left-click + drag) adjacent atoms to stretch first bond. Select adjacent atom to rotate it around first bond axis. Select adjacent bond to rotate it towards/away from first bond. Left-click bond to deselect it or left-click non-adjacent bond to select that instead.";
			instructionTextVR.text = "Move(grab with all fingers) adjacent atoms to stretch first bond. Select adjacent atom to rotate it around first bond axis. Select adjacent bond to rotate it towards/away from first bond. Retouch bond to deselect it or touch non-adjacent bond to select that instead.";
		}
		else if(_indexTarget3 == actionBond)
		{
			Destroy (actionline);
			Destroy (arcBetweenBonds);
			Destroy (actionLengthText);
			Destroy (arcAngleText);
			if (relatedAtom1.GetComponent<AtomManagerScript> ().bonds.Contains (actionBond))
				noRingDetection = actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, relatedAtom1);
			else
				noRingDetection = actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, relatedAtom2);
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }

            /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
			{
				outline.enabled = false;
			}*/
            DetachChildrenToParent();
            SetHighlightColor(actionBond, selectionColorGreen);
            actionBond = null;
			relatedAtom1.transform.GetChild(0).gameObject.SetActive(true);
			relatedAtom2.transform.GetChild(0).gameObject.SetActive(true);
            SelectionMethod = SetSecondSelection;
			instructionText.text = "Move('v' + left-click + drag) adjacent atoms to stretch first bond. Select adjacent atom to rotate it around first bond axis. Select adjacent bond to rotate it towards/away from first bond. Left-click bond to deselect it or left-click non-adjacent bond to select that instead.";
			instructionTextVR.text = "Move(grab with all fingers) adjacent atoms to stretch first bond. Select adjacent atom to rotate it around first bond axis. Select adjacent bond to rotate it towards/away from first bond. Retouch bond to deselect it or touch non-adjacent bond to select that instead.";
		}

		//if there is ation bond & if selected bond is another related bond, update action bond
		else if(relatedBonds.Contains(_indexTarget3) && _indexTarget3 != actionBond && actionBond != null)
		{
			if (relatedAtom1.GetComponent<AtomManagerScript> ().bonds.Contains (actionBond))
				noRingDetection = actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, relatedAtom1);
			else
				noRingDetection = actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, relatedAtom2);
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }

            /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
			{
				outline.enabled = false;
			}*/
            Destroy(actionline);
			Destroy (arcBetweenBonds);
			Destroy (actionLengthText); Destroy (arcAngleText);
			DetachChildrenToParent ();

            SetHighlightColor (actionBond, selectionColorGreen);
            actionBond = _indexTarget3;
            SetHighlightColor(actionBond, selectionColorOrange);
            actionline = Instantiate (MeasureLine, actionBond.transform);
			actionLengthText = Instantiate (MeasureText, actionBond.transform);
			//build hierarchy and wait for bond rotation
			if (relatedAtom1.GetComponent<AtomManagerScript> ().bonds.Contains (actionBond))
			{
				noRingDetection = actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, relatedAtom1);
				arcBetweenBonds = Instantiate (MeasureLine, relatedAtom1.transform);
				arcAngleText = Instantiate (MeasureText, relatedAtom1.transform);
			}
			else
			{
				noRingDetection = actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, relatedAtom2);
				arcBetweenBonds = Instantiate (MeasureLine, relatedAtom2.transform);
				arcAngleText = Instantiate (MeasureText, relatedAtom2.transform);
			}
			arcBetweenBonds.GetComponent<MeasureLineRenderScript> ().ShowAngleArc (baseBond.transform.position, actionBond.transform.position);
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
            }
            /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
			{
				outline.color = 2;
				outline.enabled = true;
			}*/
            DetachChildrenToParent ();
		}

		//if there is action atom & any non-base bond or unrelated atom is selected, nothing happens
		//if there is action bond & atom selected or unrelated bond, nothing happens
	}

	//thumbs down action should activate this deselction
	public void Deselection()
	{
		if(actionBond != null)
		{
			if (relatedAtom1.GetComponent<AtomManagerScript> ().bonds.Contains (actionBond))
				noRingDetection = actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, relatedAtom1);
			else
				noRingDetection = actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, relatedAtom2);
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
            SetHighlightColor(actionBond, selectionColorGreen);
            /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
			{
				outline.enabled = false;
			}*/
            Destroy (actionline);
			Destroy (arcBetweenBonds);
			Destroy (actionLengthText); Destroy (arcAngleText);
		}
		else if(actionAtom != null)
		{
			noRingDetection = baseBond.GetComponent<BondManagerScript>().HierarchyBuilder(gameObject, actionAtom);
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
            SetHighlightColor(actionAtom, selectionColorGreen);
            /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
			{
				outline.enabled = false;
			}*/
        }
		DetachChildrenToParent ();

		if(relatedAtom1)
			relatedAtom1.transform.GetChild(0).gameObject.SetActive(false);
        if (relatedAtom2)
			relatedAtom2.transform.GetChild(0).gameObject.SetActive(false);
        relatedAtom1 = null; relatedAtom2 = null;
		if(baseBond)
		{
			baseBond.transform.GetChild(0).gameObject.SetActive(false);
            SetHighlightColor(baseBond, selectionColorGreen);
            Destroy (baseline);
			Destroy (baseLengthText);
		}
		baseBond = null; actionAtom = null; actionBond = null;
		relatedBonds.Clear ();

		SelectionMethod = SetFirstSelection;
		instructionText.text = "Hold 'c' and left click to create or replace. Hold 'v' and left click and drag to move. Hold 'd' and left click to delete.\nLeft click to select a first bond.";
		instructionTextVR.text = "Pinch(index and thumb) and release to create or replace. Grab(all fingers) to move. Touch with thumb to delete.\nTouch with index finger to select a first bond.";
	}

	public void SelectionFromScript(GameObject baseB, GameObject ActionA, GameObject actionB)
	{
		SelectionMethod = SetFirstSelection;
		instructionText.text = "Hold 'c' and left click to create or replace. Hold 'v' and left click and drag to move. Hold 'd' and left click to delete.\nLeft click to select a first bond.";
		instructionTextVR.text = "Pinch(index and thumb) and release to create or replace. Grab(all fingers) to move. Touch with thumb to delete.\nTouch with index finger to select a first bond.";
		if (baseB) 
		{
			baseBond = baseB;
			baseBond.transform.GetChild(0).gameObject.SetActive(true);
            baseBond.GetComponent<BondManagerScript> ().GiveBondAtoms (out relatedAtom1, out relatedAtom2);
			relatedBonds.AddRange (relatedAtom1.GetComponent<AtomManagerScript> ().bonds);
			relatedBonds.AddRange (relatedAtom2.GetComponent<AtomManagerScript> ().bonds);
			baseline = Instantiate (MeasureLine, baseBond.transform);
			baseLengthText = Instantiate (MeasureText, baseBond.transform);
			if(ActionA)
			{
				actionAtom = ActionA;
				actionAtom.transform.GetChild(0).gameObject.SetActive(true);
                //build hierarchy and start listening for atom rotation
                noRingDetection = baseBond.GetComponent<BondManagerScript>().HierarchyBuilder(gameObject, actionAtom);
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                }

                /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
				{
					outline.color = 2;
					outline.enabled = true;
				}*/
                DetachChildrenToParent();
				SelectionMethod = ResetSelection;
				instructionText.text = "Move('v' + left click + drag) to rotate atom around first bond's axis. Reselect atom to deselect. Select other adjacent atom to switch which atom rotates.";
				instructionTextVR.text = "Move(grab and drg) to rotate atom around first bond's axis. Reselect atom to deselect. Select other adjacent atom to switch which atom rotates.";
			}
			else if(actionB)
			{
				actionBond = actionB;
				//build hierarchy and outline
				if (relatedAtom1.GetComponent<AtomManagerScript> ().bonds.Contains (actionBond))
				{
					noRingDetection = actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, relatedAtom1);
					arcBetweenBonds = Instantiate (MeasureLine, relatedAtom1.transform);
					arcAngleText = Instantiate (MeasureText, relatedAtom1.transform);
				}
				else
				{
					noRingDetection = actionBond.GetComponent<BondManagerScript> ().HierarchyBuilderOnBond (gameObject, relatedAtom2);
					arcBetweenBonds = Instantiate (MeasureLine, relatedAtom2.transform);
					arcAngleText = Instantiate (MeasureText, relatedAtom2.transform);
				}
				arcBetweenBonds.GetComponent<MeasureLineRenderScript> ().ShowAngleArc (baseBond.transform.position, actionBond.transform.position);

                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
                }

                /*foreach (cakeslice.Outline outline in GetComponentsInChildren<cakeslice.Outline> ())
				{
					outline.color = 2;
					outline.enabled = true;
				}*/
                actionline = Instantiate (MeasureLine, actionBond.transform);
				actionLengthText = Instantiate (MeasureText, actionBond.transform);
				DetachChildrenToParent ();
				SelectionMethod = ResetSelection;
				instructionText.text = "Move('v' + left click + drag) to rotate adjaent bond towards/away from first bond. Reselect adjacent bond to deselect. Select other adjacent bond to switch which bond rotates.";
				instructionTextVR.text = "Move(grab and drg) to rotate adjacent bond around first bond. Reselect adjacent bond to deselect. Select other adjacent bond to switch which bond rotates.";
			}
			else
			{
				relatedAtom1.transform.GetChild(0).gameObject.SetActive(true);
				relatedAtom2.transform.GetChild(0).gameObject.SetActive(true);
                SelectionMethod = SetSecondSelection;
				instructionText.text = "Move('v' + left-click + drag) adjacent atoms to stretch first bond. Select adjacent atom to rotate it around first bond axis. Select adjacent bond to rotate it towards/away from first bond. Left-click bond to deselect it or left-click non-adjacent bond to select that instead.";
				instructionTextVR.text = "Move(grab with all fingers) adjacent atoms to stretch first bond. Select adjacent atom to rotate it around first bond axis. Select adjacent bond to rotate it towards/away from first bond. Retouch bond to deselect it or touch non-adjacent bond to select that instead.";
			}
		}
	}


	void DetachChildrenToParent()
	{
		List<Transform> childrenToFree =  new List<Transform>();
		foreach (Transform child in transform)
			childrenToFree.Add (child);
		foreach (Transform child in childrenToFree)
			child.transform.parent = transform.parent;
	}
}
