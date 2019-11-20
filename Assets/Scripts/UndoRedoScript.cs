using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoRedoScript : MonoBehaviour {

	/*
	 * This will require a stack for storing information about all children of atomsbonds. It will add to stack as each change is made and go back down as the user undoes changes (and back up while redoing).
	 * If after an undo, user makes a change, everything on top in the stack is removed and new change is added to stack.
	 * There can be a history of 100 changes after which the bottom of the stack is dropped
	 * Each entry in the stack has a list of atom, element number, and location postion, and a list of bonds along with their start, end atoms, and their multiplicity.
	 * If a user creates new atoms and bonds, selects it as the base bond/ active items -> Undo will be prevented (or else those items must be deselected)
	*/

	public GameObject atomPrefab;
    public ContextSelectionScript contextSelectionScript;
    int currIndex, maxIndex, invisOffset;
	int totalSteps, totalUndos; //can be used to measure performance in user test
	float startTime;
	UndoHistoryEntry[] fullHistory = new UndoHistoryEntry[150]; //but there will only be 100 undos available to user


	void Start () {
		currIndex = 0;
		invisOffset = 0;
		totalSteps = 0;
		totalUndos = 0;
		startTime = Time.time;
	}

	public void AddEntry()
	{
		//clear entries with higher index if they exist
		for (int i = currIndex + 1; i <= maxIndex; i++)
			fullHistory [i] = null;
		//recenter the parent
		GetComponent<HandleScript>().ReCenterParent();
		//create new entry
		GameObject[] atoms, bonds;
		atoms = GameObject.FindGameObjectsWithTag("atoms");
		bonds = GameObject.FindGameObjectsWithTag("bonds");
		List<AtomEntry> atomList = new List<AtomEntry>();
		List<BondEntry> bondList = new List<BondEntry> ();

		foreach(GameObject atom in atoms)
		{
			AtomEntry newAtom = new AtomEntry(atom.GetComponent<AtomManagerScript>().atomProperties.AtomicNum, atom.GetComponent<AtomManagerScript>().UniqueId, atom.transform.localPosition, atom.transform.localRotation);
			atomList.Add (newAtom);
		}

		foreach(GameObject bond in bonds)
		{
			BondManagerScript bms = bond.GetComponent<BondManagerScript> ();
			BondEntry newBond = new BondEntry (bms.bondMultiplicity, bms.atomStart.GetComponent<AtomManagerScript>().UniqueId, bms.atomEnd.GetComponent<AtomManagerScript>().UniqueId, bms.uniqueId);
			bondList.Add (newBond);
		}

		currIndex++;
		if (currIndex == 100) //grows the array to 150 but restricts access to only last 100
			invisOffset++;
		if (currIndex == 150) //removes the earlier entries and resets the array
			DropEarlierEntries ();
		UndoHistoryEntry newEntry = new UndoHistoryEntry (currIndex, atomList, bondList, transform.position);
		fullHistory[currIndex] = newEntry; //inserts entry to history
		maxIndex =  currIndex;

		totalSteps++;
	}

	public void OverWriteEntry()
	{
		print ("overwriting entry");
		currIndex--;
		AddEntry ();
	}

	public void Undo()
	{
		currIndex--;
		if (currIndex >= invisOffset) 
		{
			ResetAtomsBonds ();
		}
		else
		{
			print ("no more history");
			currIndex = invisOffset;
		}

		totalUndos++;
	}

	public void Redo()
	{
		currIndex++;
		if (currIndex <= maxIndex) 
		{
			ResetAtomsBonds ();
		}
		else
		{
			print ("no further changes were made");
			currIndex = maxIndex;
		}
	}

	void ResetAtomsBonds()
	{
		//check local positions of selected atoms, bonds if they exist
		int[] selectionIds =  new int[3];
		bool[] selectionActive = new bool[3]{false, false, false};
		if (contextSelectionScript.baseBond)
		{
			selectionIds [0] = contextSelectionScript.baseBond.GetComponent<BondManagerScript> ().uniqueId;
			selectionActive [0] = true;
		}
		if (contextSelectionScript.actionAtom)
		{
			selectionIds[1] = contextSelectionScript.actionAtom.GetComponent<AtomManagerScript>().UniqueId;
			selectionActive [1] = true;
		}			
		if (contextSelectionScript.actionBond)
		{
			selectionIds[2] = contextSelectionScript.actionBond.GetComponent<BondManagerScript> ().uniqueId;
			selectionActive [2] = true;
		}
		
		GameObject selectionBaseBond = null, selectionActionAtom = null, selectionActionBond = null;
		//remove everything
		GetComponentInChildren<ContextSelectionScript>().Deselection();
		foreach(Transform child in transform)
		{
			if(child.GetComponent<AtomManagerScript>() != null || child.GetComponent<BondManagerScript>() != null)
				Destroy(child.gameObject);
		}
		//insert from history entry
		transform.position = fullHistory[currIndex].parentPos;
		Dictionary<int, GameObject> atomRefs = new Dictionary<int, GameObject>();
		if(fullHistory[currIndex] != null)
		{
			foreach (AtomEntry unAtom in fullHistory[currIndex].atomHistoryEntry) 
			{
				GameObject At = Instantiate (atomPrefab, transform);
				At.transform.localPosition = unAtom.lPosition;
				At.transform.localRotation = unAtom.lRotation;
				At.GetComponent<AtomManagerScript> ().SetProperties (unAtom.elementNumber);
				At.GetComponent<AtomManagerScript> ().UniqueId = unAtom.UniqueNumber;
				atomRefs.Add (unAtom.UniqueNumber, At); //overwriting unique id to maintain continuity
				if (selectionActive[1] && unAtom.UniqueNumber == selectionIds [1])
					selectionActionAtom = At;
			}
			foreach(BondEntry unBond in fullHistory[currIndex].bondHistoryEntry)
			{
				GameObject Bo = GetComponent<CreateModeScript> ().CreateBond (atomRefs [unBond.atomStartRef], atomRefs [unBond.atomEndRef], unBond.bondMultiplicity);
				Bo.GetComponent<BondManagerScript>().UpdateBondLength();
				Bo.GetComponent<BondManagerScript> ().uniqueId = unBond.UniqueNumber; //overwriting unique id to maintain continuity
						if (selectionActive[0] && unBond.UniqueNumber == selectionIds [0])
					selectionBaseBond = Bo;
						else if (selectionActive[2] && unBond.UniqueNumber == selectionIds [2])
					selectionActionBond = Bo;
			}
			foreach (Transform atom in gameObject.transform)
			{
				if (atom.GetComponent<AtomManagerScript>() != null)
				{
					atom.GetComponent<AtomManagerScript>().GetBondOrientations();
				}
			}
			//try to reset selections
			GetComponentInChildren<ContextSelectionScript> ().SelectionFromScript (selectionBaseBond, selectionActionAtom, selectionActionBond);
		}
	}

	void DropEarlierEntries()
	{
		for(int a = 0; a < 99; a++)
			fullHistory [a] = fullHistory [a + 51];
		
		invisOffset = 0;
		currIndex -= 99;
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.Z))
			Undo ();
		if (Input.GetKeyDown (KeyCode.X))
			Redo ();
		if(Input.GetKeyDown (KeyCode.Return))
		{
			Debug.Log("Results:: Total Steps: " + totalSteps + ", total undos: " + totalUndos + ", total time taken: " + (Time.time - startTime) + "s");
		}
	}
}

class UndoHistoryEntry
{
	public int historyIndex;
	public List<AtomEntry> atomHistoryEntry;
	public List<BondEntry> bondHistoryEntry;
	public Vector3 parentPos;

	public UndoHistoryEntry(int histInd, List<AtomEntry> atoms, List<BondEntry> bonds, Vector3 parPos)
	{
		historyIndex = histInd;
		atomHistoryEntry = atoms;
		bondHistoryEntry = bonds;
		parentPos = parPos;
	}
}

class AtomEntry
{
	public int elementNumber;
	public int UniqueNumber;
	public Vector3 lPosition;
	public Quaternion lRotation;

	public AtomEntry (int elNu, int uniNu, Vector3 lPos, Quaternion lRot)
	{
		elementNumber = elNu;
		UniqueNumber = uniNu;
		lPosition = lPos;
		lRotation = lRot;
	}
}

class BondEntry
{
	public int bondMultiplicity;
	public int atomStartRef;
	public int atomEndRef;
	public int UniqueNumber;

	public BondEntry(int bondMx, int atStart, int atEnd, int uniNum)
	{
		bondMultiplicity = bondMx;
		atomStartRef = atStart;
		atomEndRef = atEnd;
		UniqueNumber = uniNum;
	}
}
