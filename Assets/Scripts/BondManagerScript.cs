using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BondManagerScript : MonoBehaviour {

	public GameObject atomStart, atomEnd;
	public Vector3 bondVector;
	public float bondLength;

	public int bondMultiplicity;
	public int uniqueId;
	void Awake()
	{
		uniqueId = GetInstanceID ();
	}
	//this works as a constructor for instantiated atoms - called from the pinch action script
	public void UpdateBond(GameObject atomStart, GameObject atomEnd)		//Bond "vector" start at atomStart and ends at atomEnd
	{
		this.atomStart = atomStart;
		this.atomEnd = atomEnd;

		//add bond to atom managers
		this.atomStart.GetComponent<AtomManagerScript>().AddBond(gameObject, this.atomEnd);
		this.atomEnd.GetComponent<AtomManagerScript>().AddBond(gameObject, this.atomStart);

		UpdateBond ();
	}

	public void ResetBonds(GameObject oldAtom, GameObject newAtom)
	{
		if (oldAtom == atomStart)
			atomStart = newAtom;
		else if (oldAtom == atomEnd)
			atomEnd = newAtom;
	}

	public void GiveBondAtoms(out GameObject one, out GameObject two)
	{
		one = atomStart;
		two = atomEnd;
	}

	public void UpdateBond()
	{
		bondVector = atomEnd.transform.position - atomStart.transform.position;
		transform.position = atomStart.transform.position + bondVector / 2;
		transform.up = bondVector;
		transform.localScale = new Vector3 (transform.localScale.x, bondVector.magnitude / 2, transform.localScale.z);
		bondLength = bondVector.magnitude;
	}

<<<<<<< HEAD
	public IEnumerator InheritAtoms(GameObject _replacedBond)
=======
	public Vector3 GetBondVector(GameObject fromAtom)
	{
		int bondDirection = 0;
		if (fromAtom == atomStart)
			bondDirection = 1;
		else if (fromAtom == atomEnd)
			bondDirection = -1;
		//else
		//	 ;	//Throw exception

		return bondVector * bondDirection;
	}

	public void InheritAtoms(GameObject _replacedBond)
>>>>>>> 3fa6523... All atom animations should work independently
	{
		_replacedBond.GetComponent<BondManagerScript> ().GiveBondAtoms (out atomStart, out atomEnd);
		UpdateBond ();
<<<<<<< HEAD
		_atomStart.GetComponent<AtomManagerScript> ().ResetBond (_replacedBond, gameObject);
		_atomEnd.GetComponent<AtomManagerScript> ().ResetBond (_replacedBond, gameObject);
<<<<<<< HEAD
<<<<<<< HEAD
		yield return StartCoroutine (SetBondLength(_atomStart)); //is this the best way to do this? Make another recalculate function for only the changing bond?
=======
		StartCoroutine (SetBondLength(_atomStart));
>>>>>>> 02f5469... .mol file coordinate system fixed
=======
>>>>>>> 3fa6523... All atom animations should work independently
=======
		atomStart.GetComponent<AtomManagerScript> ().ReplaceBond (_replacedBond, gameObject);
		atomEnd.GetComponent<AtomManagerScript> ().ReplaceBond (_replacedBond, gameObject);
>>>>>>> 68934d2... Added preliminary support for live optimization
		//UpdateBond ();
	}

	public void UpdateBondLength()
	{
		int atom1Rad = 0, atom2Rad = 0;
		switch (bondMultiplicity)
		{
			case 3:
				Debug.Log("triple");
				if (atomStart.GetComponent<AtomManagerScript>().atomProperties.covRadiusTriple != null)
				{
					atom1Rad = (int) atomStart.GetComponent<AtomManagerScript>().atomProperties.covRadiusTriple;
				}
				if (atomEnd.GetComponent<AtomManagerScript>().atomProperties.covRadiusTriple != null)
				{
					atom2Rad = (int)atomEnd.GetComponent<AtomManagerScript>().atomProperties.covRadiusTriple;
				}
				if (atom1Rad == 0 || atom2Rad == 0) {
					Debug.Log("Missing atleast 1 value for triple");
					goto case 2;
				}
				break;
			case 2:
				Debug.Log("double");
				if(atom1Rad == 0 && atomStart.GetComponent<AtomManagerScript>().atomProperties.covRadiusDouble != null)
				{
					atom1Rad = (int)atomStart.GetComponent<AtomManagerScript>().atomProperties.covRadiusDouble;
				}
				if (atom2Rad == 0 && atomEnd.GetComponent<AtomManagerScript>().atomProperties.covRadiusDouble != null)
				{
					atom2Rad = (int)atomEnd.GetComponent<AtomManagerScript>().atomProperties.covRadiusDouble;
				}
				if (atom1Rad == 0 || atom2Rad == 0)
				{
					Debug.Log("Missing atleast 1 value for double");
					goto default;
				}
				break;
			default:
				//Debug.Log("default");
				atom1Rad = (int)atomStart.GetComponent<AtomManagerScript>().atomProperties.covRadiusSingle;
				atom2Rad = (int)atomEnd.GetComponent<AtomManagerScript>().atomProperties.covRadiusSingle;
				break;
		}
		bondLength = (float)(atom1Rad + atom2Rad) / 1000F;
	}

	public IEnumerator SetBondLength(GameObject staticAtom)
	{
		GameObject rootObj = Instantiate(GameObject.Find("HierarchyRoot"), staticAtom.GetComponent<AtomManagerScript>().atomsBonds.transform);

		UpdateBond();
		UpdateBondLength();

		IEnumerator coroutine = null;

		if (staticAtom == atomEnd)
		{
			HierarchyBuilder(rootObj, atomStart);
			rootObj.transform.position = atomStart.transform.position;
			coroutine = staticAtom.GetComponent<AtomManagerScript>().moveAtomGroup(rootObj, gameObject, -bondVector.normalized * bondLength - (atomStart.transform.position - atomEnd.transform.position), Time.time, 1F);
		}
		else if (staticAtom == atomStart)
		{
			HierarchyBuilder(rootObj, atomEnd);
			rootObj.transform.position = atomEnd.transform.position;
			coroutine = staticAtom.GetComponent<AtomManagerScript>().moveAtomGroup(rootObj, gameObject, bondVector.normalized * bondLength - (atomEnd.transform.position - atomStart.transform.position), Time.time, 1F);
		}
		if (coroutine != null)
		{
			Debug.Log("Moving group");
			yield return StartCoroutine(coroutine);
			Debug.Log("Group moved");
		}
	}


	public void DeleteBond()
	{
		atomStart.GetComponent<AtomManagerScript> ().RemoveBond (gameObject, atomEnd);
		atomEnd.GetComponent<AtomManagerScript> ().RemoveBond (gameObject, atomStart);
		Destroy (gameObject);
	}

	public void DeleteBond(GameObject deletedAtom)
	{
		if (deletedAtom == atomStart) {
			atomEnd.GetComponent<AtomManagerScript> ().RemoveBond (gameObject, deletedAtom);
		} 
		else if (deletedAtom == atomEnd) 
		{
			atomStart.GetComponent<AtomManagerScript> ().RemoveBond (gameObject, deletedAtom);
		}
		Destroy (gameObject);
	}

	private float rotationTimeStamp;

	public bool HierarchyBuilder(GameObject root, GameObject rotatingAtom)
	{
		//call on atom a, and atom b
		float timeStamp = Time.time;
		if (rotatingAtom == atomStart) 
		{
			root.transform.position = atomStart.transform.position;
			return atomStart.GetComponent<AtomManagerScript> ().HierarchyChild (gameObject, root, atomEnd, timeStamp);
		}
		else
		{
			root.transform.position = atomEnd.transform.position;
			return atomEnd.GetComponent<AtomManagerScript> ().HierarchyChild (gameObject, root, atomStart, timeStamp);
		}
	}

	public bool HierarchyBuilderOnBond(GameObject root, GameObject nonRotatingAtomParent)
	{
		float timeStamp = Time.time;
		root.transform.position = nonRotatingAtomParent.transform.position;
		return HierarchyChild (nonRotatingAtomParent, root, nonRotatingAtomParent, timeStamp);
	}

	public bool HierarchyChild(GameObject parent, GameObject root, GameObject unrelated, float timestamp)
	{
		if (rotationTimeStamp != timestamp)
		{
			rotationTimeStamp = timestamp;
			transform.parent = root.transform;

			if (atomStart == parent)
			{
				return atomEnd.GetComponent<AtomManagerScript>().HierarchyChild(gameObject, root, unrelated, timestamp);
			}
			else
			{
				return atomStart.GetComponent<AtomManagerScript>().HierarchyChild(gameObject, root, unrelated, timestamp);
			}
		}
		else if (atomStart == unrelated || atomEnd == unrelated)
		{
			Debug.Log("Ring in bondManger");
			return false;
		}
		else
			return true;
	}
}
