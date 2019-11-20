using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AtomManagerScript : MonoBehaviour
{
	public GameObject interactionModeSystem, atomsBonds, HierarchyRoot;
	public ElementDataProviderScript elementDataProviderScript;

	public GameObject satAtom, bondPrefab, doubleBondPrefab, tripleBondPrefab;      //Prefabs for saturation

	public ChemElement atomProperties;                                              //Class containing all element data
	private Color atomColor;

	private List<Vector3> bondOrientations;                    //For storing bond orientations

	public List<GameObject> bonds;
	public List<GameObject> bondedAtoms;

	public int UniqueId { get; set; }

	void Awake()
	{
		interactionModeSystem = GameObject.Find("InteractionModeSystem");
<<<<<<< HEAD
		atomsBonds = GameObject.Find("AtomsBonds");
<<<<<<< HEAD
		satAtom = GameObject.Find("InteractionModeSystem").GetComponent<CreateModeScript>().atomPrefab;		//STUPID WORKAROUND, SCRIPT CANNOT REFERENCE THE PREFAB IT IS TIED TO, HAVE TO GET atomPrefab FROM ANOTHER SCRIPT
=======
		HierarchyRoot = GameObject.Find("HierarchyRoot");
<<<<<<< HEAD
<<<<<<< HEAD
		satAtom = atomsBonds.GetComponent<CreateModeScript>().atomPrefab;		//STUPID WORKAROUND, SCRIPT CANNOT REFERENCE THE PREFAB IT IS TIED TO, HAVE TO GET atomPrefab FROM ANOTHER SCRIPT
<<<<<<< HEAD
>>>>>>> 23d929e... MoveAtom now works while rotating molecule
=======
		uniqueId = GetInstanceID();
>>>>>>> 8bcf0a5... fixed most bugs related to undo redo functions
=======
		satAtom = atomsBonds.GetComponent<CreateModeScript>().atomPrefab;			//STUPID WORKAROUND, SCRIPT CANNOT REFERENCE THE PREFAB IT IS TIED TO, HAVE TO GET atomPrefab FROM ANOTHER SCRIPT
>>>>>>> 3fa6523... All atom animations should work independently
=======
=======
		atomsBonds = transform.parent.gameObject;
		HierarchyRoot = transform.parent.Find("HierarchyRoot").gameObject;
		elementDataProviderScript = transform.parent.GetComponent<ElementDataProviderScript>();
>>>>>>> 68934d2... Added preliminary support for live optimization
		satAtom = atomsBonds.GetComponent<CreateModeScript>().atomPrefab;           //STUPID WORKAROUND, SCRIPT CANNOT REFERENCE THE PREFAB IT IS TIED TO, HAVE TO GET atomPrefab FROM ANOTHER SCRIPT
		UniqueId = GetInstanceID();

		bondOrientations = new List<Vector3>();
		bonds = new List<GameObject>();
		bondedAtoms = new List<GameObject>();
>>>>>>> df9d52d... Create mode control methods rewritten.
	}

	public void SetProperties()
	{
		atomProperties = elementDataProviderScript.GetElementData();

		gameObject.name = atomProperties.Name;
		transform.localScale = new Vector3(((float)atomProperties.covRadiusSingle * 2 * 0.65F) / 1000, ((float)atomProperties.covRadiusSingle * 2 *0.65F) / 1000, ((float)atomProperties.covRadiusSingle * 2 *0.65F) / 1000);	//Atom diameter is now set to 65% of covalent radius. TODO UI setting and progressively smaller radius for larger atoms
		atomColor = Color.magenta;
		if (atomProperties.ColorR != null && atomProperties.ColorG != null && atomProperties.ColorB != null && atomProperties.ColorA != null)
		{
			float colorR = (float)atomProperties.ColorR / 255F;
			float colorG = (float)atomProperties.ColorG / 255F;
			float colorB = (float)atomProperties.ColorB / 255F;
			float colorA = (float)atomProperties.ColorA / 255F;

			Color temp = new Color(colorR, colorG, colorB, colorA);

			bool UpperLimit = (temp.r <= 1 && temp.g <= 1 && temp.b <= 1 && temp.a <= 1);
			bool LowerLimit = (temp.r >= 0 && temp.g >= 0 && temp.b >= 0 && temp.a >= 0);

			if (UpperLimit && LowerLimit)
			{
				atomColor = temp;
			}
		}
		GetComponent<MeshRenderer>().material.color = atomColor;
	}

	public void SetProperties(int atomicNum)
	{
		atomProperties = elementDataProviderScript.GetElementData(atomicNum);

		name = atomProperties.Name;
		transform.localScale = new Vector3(((float)atomProperties.covRadiusSingle * 2 * 0.65F) / 1000, ((float)atomProperties.covRadiusSingle * 2 * 0.65F) / 1000, ((float)atomProperties.covRadiusSingle * 2 * 0.65F) / 1000);
		atomColor = Color.magenta;
		if (atomProperties.ColorR != null && atomProperties.ColorG != null && atomProperties.ColorB != null && atomProperties.ColorA != null)
		{
			float colorR = (float)atomProperties.ColorR / 255F;
			float colorG = (float)atomProperties.ColorG / 255F;
			float colorB = (float)atomProperties.ColorB / 255F;
			float colorA = (float)atomProperties.ColorA / 255F;

			Color temp = new Color(colorR, colorG, colorB, colorA);

			bool UpperLimit = (temp.r <= 1 && temp.g <= 1 && temp.b <= 1 && temp.a <= 1);
			bool LowerLimit = (temp.r >= 0 && temp.g >= 0 && temp.b >= 0 && temp.a >= 0);

			if (UpperLimit && LowerLimit)
			{
				atomColor = temp;
			}
		}
		GetComponent<MeshRenderer>().material.color = atomColor;
	}
	
	/*public void SetProperties(string elementString)
	{
		atomProperties = elementDataProviderScript.GetElementData(elementString);

		name = atomProperties.Name;
		transform.localScale = new Vector3(((float)atomProperties.covRadiusSingle * 2 * 0.65F) / 1000, ((float)atomProperties.covRadiusSingle * 2 * 0.65F) / 1000, ((float)atomProperties.covRadiusSingle * 2 * 0.65F) / 1000);
		atomColor = Color.magenta;
		if (atomProperties.ColorR != null && atomProperties.ColorG != null && atomProperties.ColorB != null && atomProperties.ColorA != null)
		{
			float colorR = (float)atomProperties.ColorR / 255F;
			float colorG = (float)atomProperties.ColorG / 255F;
			float colorB = (float)atomProperties.ColorB / 255F;
			float colorA = (float)atomProperties.ColorA / 255F;

			Color temp = new Color(colorR, colorG, colorB, colorA);

			bool UpperLimit = (temp.r <= 1 && temp.g <= 1 && temp.b <= 1 && temp.a <= 1);
			bool LowerLimit = (temp.r >= 0 && temp.g >= 0 && temp.b >= 0 && temp.a >= 0);

			if (UpperLimit && LowerLimit)
			{
				atomColor = temp;
			}
		}
		GetComponent<MeshRenderer>().material.color = atomColor;
	}
	*/

	public void AddBond(GameObject bond, GameObject bondedAtom)
	{
		//TODO Check for bond amount limits?
		bonds.Add(bond);
		bondedAtoms.Add(bondedAtom);
	}

	public void RemoveBond(GameObject bond, GameObject bondedAtom)
	{
		if (bonds.Contains(bond) && bondedAtoms.Contains(bondedAtom))
		{
			Debug.Log("Bond removed");
			bonds.Remove(bond);
			bondedAtoms.Remove(bondedAtom);
		}
		else
			Debug.Log("Trying to remove non-existent bond");
	}

	public void ReplaceBondedAtom(GameObject oldAtom, GameObject newAtom)
	{
		if (bondedAtoms.Contains(oldAtom))
		{
			bondedAtoms.Remove(oldAtom);
			bondedAtoms.Add(newAtom);
		}
		else
			Debug.Log("Trying to replace non-existent atom");
	}

	public void ReplaceBond(GameObject oldBond, GameObject newBond)
	{
		if (bonds.Contains(oldBond))
		{
			bonds.Remove(oldBond);
			bonds.Add(newBond);
		}
		else
			Debug.Log("Trying to replace non-existent bond");
	}

	public void UpdateBonds()
	{
		foreach (GameObject bond in bonds)
		{
			bond.GetComponent<BondManagerScript>().UpdateBond();
		}
	}

	public void InheritBonds(GameObject replacedAtom)
	{
		bonds = replacedAtom.GetComponent<AtomManagerScript>().bonds;
		bondedAtoms = replacedAtom.GetComponent<AtomManagerScript>().bondedAtoms;

		foreach (GameObject bond in bonds)
		{
			bond.GetComponent<BondManagerScript>().ResetBonds(replacedAtom, gameObject);
		}
		foreach (GameObject bondedAtom in bondedAtoms)
		{
			bondedAtom.GetComponent<AtomManagerScript>().ReplaceBondedAtom(replacedAtom, gameObject);
		}

		if (bonds[0].GetComponent<BondManagerScript>().atomStart == gameObject)
			transform.up = replacedAtom.GetComponent<AtomManagerScript>().bonds[0].transform.up;
		else
			transform.up = -replacedAtom.GetComponent<AtomManagerScript>().bonds[0].transform.up;
	}

	public void DeleteAtom()
	{
		List<GameObject> bondsToRemove = new List<GameObject>(bonds);

		foreach (GameObject bond in bondsToRemove)
		{
			Debug.Log("tests");
			if (bond != null)
				bond.GetComponent<BondManagerScript>().DeleteBond();
		}
		Destroy(gameObject);
	}

	public void GetBondOrientations()
	{
		if (atomProperties.VSEPR_X != null && atomProperties.VSEPR_E != null)
		{
			int electronsInHigherBonds = 0;     //number of electrons in double/triple bonds
			foreach (GameObject bond in bonds)
			{
				electronsInHigherBonds += bond.GetComponent<BondManagerScript>().bondMultiplicity - 1;
			}

			if (atomProperties.VSEPR_X - electronsInHigherBonds == bondOrientations.Count)
				return;
			else
			{
				bondOrientations.Clear();

				switch (atomProperties.VSEPR_X + atomProperties.VSEPR_E - electronsInHigherBonds)
				{
					case 1:     //LINEAR (ONE BOND)
						bondOrientations.Add(Vector3.up);
						break;
					case 2:     //LINEAR (CO2)
						bondOrientations.Add(Vector3.up);
						bondOrientations.Add(-bondOrientations[0]);
						break;
					case 3:     //TRIGONAL PLANAR (BCl3 / SO2)
						bondOrientations.Add(Vector3.up);
						bondOrientations.Add(Quaternion.AngleAxis(120f, Vector3.forward) * bondOrientations[0]);
						bondOrientations.Add(Quaternion.AngleAxis(-120f, Vector3.forward) * bondOrientations[0]);
						break;
					case 4:     //TETRAHEDRAL
						bondOrientations.Add(Vector3.up);
						bondOrientations.Add(Quaternion.AngleAxis(109.5f, Vector3.forward) * bondOrientations[0]);
						bondOrientations.Add(Quaternion.AngleAxis(120f, bondOrientations[0]) * bondOrientations[1]);
						bondOrientations.Add(Quaternion.AngleAxis(-120f, bondOrientations[0]) * bondOrientations[1]);
						break;
					case 5:     //TRIGONAL BIPYRAMIDAL
						bondOrientations.Add(Vector3.up);
						bondOrientations.Add(Quaternion.AngleAxis(180f, Vector3.forward) * bondOrientations[0]);
						bondOrientations.Add(Quaternion.AngleAxis(90f, Vector3.forward) * bondOrientations[0]);
						bondOrientations.Add(Quaternion.AngleAxis(120f, bondOrientations[0]) * bondOrientations[2]);
						bondOrientations.Add(Quaternion.AngleAxis(-120f, bondOrientations[0]) * bondOrientations[2]);
						break;
					case 6:     //OCTAHEDRAL
						bondOrientations.Add(Vector3.up);
						bondOrientations.Add(Quaternion.AngleAxis(90f, Vector3.forward) * bondOrientations[0]);
						bondOrientations.Add(Quaternion.AngleAxis(180f, Vector3.forward) * bondOrientations[0]);
						bondOrientations.Add(Quaternion.AngleAxis(270f, Vector3.forward) * bondOrientations[0]);
						bondOrientations.Add(Quaternion.AngleAxis(90f, bondOrientations[0]) * bondOrientations[1]);
						bondOrientations.Add(Quaternion.AngleAxis(-90f, bondOrientations[0]) * bondOrientations[1]);
						break;
					case 7:     //PENTAGONAL BIPYRAMIDAL
						bondOrientations.Add(Vector3.up);
						bondOrientations.Add(Quaternion.AngleAxis(72f, Vector3.forward) * bondOrientations[0]);
						bondOrientations.Add(Quaternion.AngleAxis(144f, Vector3.forward) * bondOrientations[0]);
						bondOrientations.Add(Quaternion.AngleAxis(216f, Vector3.forward) * bondOrientations[0]);
						bondOrientations.Add(Quaternion.AngleAxis(288f, Vector3.forward) * bondOrientations[0]);
						bondOrientations.Add(Quaternion.AngleAxis(90f, bondOrientations[0]) * bondOrientations[0]);
						bondOrientations.Add(Quaternion.AngleAxis(180f, bondOrientations[0]) * bondOrientations[0]);
						break;
				}
			}
		}
	}

	public IEnumerator CheckSaturation()           //Saturate / desaturate with hydrogen atoms
	{
		int bondNum = bonds.Count;
		int electronsInHigherBonds = 0;

		List<IEnumerator> coroutineList = new List<IEnumerator>();

		if (bonds.Count < 1)		//Atom has no bonds, set local Up to global Up (first bond will point straight up in world space)
			transform.up = Vector3.up;

		foreach (GameObject bond in bonds)
		{
			electronsInHigherBonds += bond.GetComponent<BondManagerScript>().bondMultiplicity - 1;
		}

		if (atomProperties.VSEPR_X != null && atomProperties.VSEPR_X > 0)
		{
			if (bondNum < atomProperties.VSEPR_X - electronsInHigherBonds)				//Too few bonds
			{
				/*Vector3 planeAtom1pos = new Vector3(), planeAtom2pos = new Vector3();

				foreach (GameObject atom in bondedAtoms)
				{
					if (atom.GetComponent<AtomManagerScript>().bondedAtoms.Count > 1)
					{
						planeAtom1pos = atom.transform.position;
						foreach (GameObject atom2 in atom.GetComponent<AtomManagerScript>().bondedAtoms)
						{
							if (atom2 != gameObject)
							{
								planeAtom2pos = atom2.transform.position;
								break;
							}
						}
						break;
					}
				}

				Plane orientationPlane = new Plane(transform.position, planeAtom1pos, planeAtom2pos);
				Debug.DrawLine(transform.position, planeAtom1pos, Color.green, 10);
				Debug.DrawLine(planeAtom1pos, planeAtom2pos, Color.green, 10);
				Debug.DrawLine(planeAtom2pos, transform.position, Color.green, 10);
				Debug.DrawRay(transform.position, orientationPlane.normal, Color.blue, 10);
				transform.rotation = Quaternion.LookRotation(-orientationPlane.normal, transform.up);*/

				GetBondOrientations();

				for (int i = bondNum; i < atomProperties.VSEPR_X - electronsInHigherBonds; i++)
				{
					GameObject saturationAtom = Instantiate(satAtom, transform.position, Quaternion.identity, atomsBonds.transform) as GameObject;
					saturationAtom.GetComponent<AtomManagerScript>().SetProperties(1);

					GameObject saturationBond = Instantiate(bondPrefab, transform.position, Quaternion.identity, atomsBonds.transform) as GameObject;
					saturationBond.GetComponent<BondManagerScript>().UpdateBond(gameObject, saturationAtom);
					saturationBond.GetComponent<BondManagerScript>().UpdateBondLength();

					saturationAtom.transform.up = -(transform.rotation * bondOrientations[i]);
					saturationAtom.GetComponent<AtomManagerScript>().bondOrientations.Add(Vector3.up);

					Vector3 moveVector = -(saturationAtom.transform.up * saturationBond.GetComponent<BondManagerScript>().bondLength);

					IEnumerator coroutine = moveAtom(saturationAtom, moveVector, Time.time, 0.2F);
					//Debug.Log("Start " + coroutine.Current);
					coroutineList.Add(coroutine);
					StartCoroutine(coroutine);
				}
			}
			else if (bondNum > atomProperties.VSEPR_X - electronsInHigherBonds)			//Too many bonds
			{
				for (int i = bondNum - 1; i >= 0; i--)									//Using for-loop to loop through the list backwards
				{
					if (bondNum > atomProperties.VSEPR_X - electronsInHigherBonds)
					{
						if (bonds[i].GetComponent<BondManagerScript>().atomEnd.GetComponent<AtomManagerScript>().atomProperties.AtomicNum == 1)
						{
							bonds[i].GetComponent<BondManagerScript>().atomEnd.GetComponent<AtomManagerScript>().DeleteAtom();
							bondNum--;
						}
						else if (bonds[i].GetComponent<BondManagerScript>().atomStart.GetComponent<AtomManagerScript>().atomProperties.AtomicNum == 1)
						{
							bonds[i].GetComponent<BondManagerScript>().atomStart.GetComponent<AtomManagerScript>().DeleteAtom();
							bondNum--;
						}
					}
				}

				if (bondNum > atomProperties.VSEPR_X - electronsInHigherBonds)
					Debug.Log("Atom has too many bonds to non-hydrogen atoms");
			}

			while (true)    //Wait until all coroutines above are done, then run ReorientBonds()
			{
				bool done = true;
				foreach (IEnumerator coroutine in coroutineList)
				{
					if (coroutine.Current != null)
					{
						done = false;
						//Debug.Log(coroutine.Current);
						yield return new WaitForSeconds(0.1F);
					}
				}
				if (done)
				{
					//Debug.Log("All coroutines done");
					//yield return StartCoroutine(ReorientBonds());
					break;
				}
			}
		}
		yield return null;
	}

	public IEnumerator ReorientBonds()
	{
        if (bonds.Count < 2) yield return null;

		float? minSumOfAngles = null;
		Quaternion minRotation = Quaternion.identity;

		List<int> occupiedOrientations = new List<int>();

		List<IEnumerator> coroutineList = new List<IEnumerator>();

		GetBondOrientations();

		//The orientations obtained from GetBondOrientations() are in a local coordinate system, e.g. orientation[0] is always 0,1,0 i.e. "up". The following code finds a quaternion where quaternion * orientation[i] = correct orientation of bond [i] in world space.
		//The largest ligand is set as the 1st orientation and shouldn't move. The rest are selected so that the sum of angles between the old and new bond vectors is smallest, to minimize the amount of travel of the ligands on screen.

		List<int> atomsInLigand = new List<int>();

		int largestLigandSize = 0, largestLigandIndex = 0;

		for (int i = 0; i < bonds.Count; i++)       //Loop through all ligands and find the largest one. This will be the 1st bond in bondOrientations; set this bond as "up" in the minimum rotation quaternion
		{
			if (bonds[i].GetComponent<BondManagerScript>().atomEnd == gameObject)
			{
				atomsInLigand.Add(gameObject.GetInstanceID());		//Adding gameObject to list to prevent AtomlistBuilder from propagating through gameObject to other ligands
				bonds[i].GetComponent<BondManagerScript>().atomStart.GetComponent<AtomManagerScript>().AtomListBuilder(atomsInLigand);
			}
			else
			{
				atomsInLigand.Add(gameObject.GetInstanceID());
				bonds[i].GetComponent<BondManagerScript>().atomEnd.GetComponent<AtomManagerScript>().AtomListBuilder(atomsInLigand);
			}

			if (atomsInLigand.Count > largestLigandSize)
			{
				largestLigandSize = atomsInLigand.Count;
				largestLigandIndex = i;
			}

			atomsInLigand.Clear();
		}

        Vector3 fwd;
        if (largestLigandIndex == 0)
        {
            fwd = Vector3.ProjectOnPlane(bonds[1].GetComponent<BondManagerScript>().GetBondVector(gameObject), bonds[largestLigandIndex].GetComponent<BondManagerScript>().GetBondVector(gameObject));
        }
        else
        {
            fwd = Vector3.ProjectOnPlane(bonds[0].GetComponent<BondManagerScript>().GetBondVector(gameObject), bonds[largestLigandIndex].GetComponent<BondManagerScript>().GetBondVector(gameObject));
        }
        if (fwd != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(fwd, bonds[largestLigandIndex].GetComponent<BondManagerScript>().GetBondVector(gameObject));
        }

        for (int i = 0; i < bonds.Count; i++)
        {
            if (i == largestLigandIndex)
                continue;

            float minAngle = float.MaxValue;
            int index = -1;
            BondManagerScript bms = bonds[i].GetComponent<BondManagerScript>();

            for (int j = 1; j < bondOrientations.Count; j++)
            {
                if (occupiedOrientations.Contains(j))
                    continue;

                float angle = Vector3.Angle(bms.GetBondVector(gameObject), transform.rotation * bondOrientations[j]);
                if (angle < minAngle)
                {
                    minAngle = angle;
                    index = j;
                }
            }

            occupiedOrientations.Add(index);
            GameObject rotationRoot = new GameObject("rot");
            rotationRoot.transform.parent = atomsBonds.transform;
            rotationRoot.transform.up = -(bms.GetBondVector(gameObject));

            if (gameObject == bms.atomEnd)
                bms.HierarchyBuilder(rotationRoot, bms.atomStart);
            else if (gameObject == bms.atomStart)
                bms.HierarchyBuilder(rotationRoot, bms.atomEnd);

            IEnumerator coroutine = rotateAtomGroup(rotationRoot, bonds[i], gameObject, transform.rotation * bondOrientations[index] * bms.bondLength, Time.time, 0.2F);

            coroutineList.Add(coroutine);
            StartCoroutine(coroutine);

        }

        //Debug.Log("largest " + largestLigandIndex + " " + largestLigandSize);

        /*for (int j = 0; j < bonds.Count; j++)	//"Forward" in minimum rotation quaternion is projected from this bond
		{
			if (j == largestLigandIndex)
			{
				if (j+1 == bonds.Count)		//Special case: atom has only 1 bond
				{
					minRotation = transform.rotation;
				}
				continue;
			}

			Quaternion tempQuaternion = new Quaternion();

			if (Vector3.ProjectOnPlane(bonds[j].GetComponent<BondManagerScript>().GetBondVector(gameObject), bonds[largestLigandIndex].GetComponent<BondManagerScript>().GetBondVector(gameObject)) == Vector3.zero)
			{
				Debug.Log("Bonds are parallel " + largestLigandIndex + " " + j);
				transform.up = bonds[largestLigandIndex].GetComponent<BondManagerScript>().GetBondVector(gameObject);
				tempQuaternion = transform.rotation;
			}
			else
			{
				tempQuaternion = Quaternion.LookRotation(Vector3.ProjectOnPlane(bonds[j].GetComponent<BondManagerScript>().GetBondVector(gameObject), bonds[largestLigandIndex].GetComponent<BondManagerScript>().GetBondVector(gameObject)), bonds[largestLigandIndex].GetComponent<BondManagerScript>().GetBondVector(gameObject));
				//tempQuaternion = Quaternion.AngleAxis(90, bonds[largestLigandIndex].GetComponent<BondManagerScript>().GetBondVector(gameObject)) * tempQuaternion;
				//dummy.transform.rotation = temp;	//TODO delete
			}

			float sumOfAngles = 0;

			for (int k = 0; k < bonds.Count; k++)		//Check sum of angles between existing bonds and ideal orientations
			{
				GameObject closestBond = bonds.Aggregate((x, y) => Vector3.Angle(x.GetComponent<BondManagerScript>().GetBondVector(gameObject), tempQuaternion * bondOrientations[k]) < Vector3.Angle(y.GetComponent<BondManagerScript>().GetBondVector(gameObject), tempQuaternion * bondOrientations[k]) ? x : y);
				Debug.Log(string.Format("angle: {0} (bondIndex {1} orient {2})", Vector3.Angle(closestBond.GetComponent<BondManagerScript>().bondVector, tempQuaternion * bondOrientations[k]), j, k));
                Debug.Break();
				sumOfAngles += Vector3.Angle(closestBond.GetComponent<BondManagerScript>().GetBondVector(gameObject), tempQuaternion * bondOrientations[k]);
			}
			//Debug.Log("sum " + sumOfAngles + " " + largestLigandIndex + " " + j);

			if (minSumOfAngles == null)
			{
				minSumOfAngles = sumOfAngles;
				minRotation = tempQuaternion;
			}

			if (sumOfAngles < 1)	//Break if angle is 0 i.e. no change (or very small. Vector3.angle is inaccurate, accepting a small deviation from 0 degrees)
			{
				minRotation = tempQuaternion;
				minSumOfAngles = 0;
				break;
			}
			else if (sumOfAngles < minSumOfAngles)
			{
				minSumOfAngles = sumOfAngles;
				minRotation = tempQuaternion;
			}
		}

			//if (minSumOfAngles == 0)
			//	break;

		//Debug.Log("FINAL SUM " + minSumOfAngles);

		for (int i = 0; i < bonds.Count; i++)
		{
			var bondManager = bonds[i].GetComponent<BondManagerScript>();

			GameObject rootObj = Instantiate(HierarchyRoot, atomsBonds.transform);
			rootObj.transform.up = bondManager.GetBondVector(gameObject);

			occupiedOrientations.Add(int.MinValue);	//Add some value at index i

			float minAngle = 180F;
			Vector3 minVector = Vector3.zero;

			for (int j = 0; j < bondOrientations.Count; j++)
			{
				float angle = 0;

				if (occupiedOrientations.Contains(j))
				{
					//Debug.Log(string.Format("{0}: {1} is already occupied!", i, j));
					continue;
				}

				//Debug.Log("test " + bonds[i].GetComponent<BondManagerScript>().GetBondVector(gameObject) + " " + minRotation * bondOrientations[j]);
				angle = Vector3.Angle(bonds[i].GetComponent<BondManagerScript>().GetBondVector(gameObject), minRotation * bondOrientations[j]);

				//Debug.Log(angle + " " + minAngle + " " + j);

				if (angle < 1)
				{
					minVector = minRotation * bondOrientations[j];
					occupiedOrientations[i] = j;
					break;
				}

				if (angle < minAngle)
				{
					minAngle = angle;
					minVector = minRotation * bondOrientations[j];
					occupiedOrientations[i] = j;
					continue;
				}
			}

			//Debug.Log("End: " + i + " " + bonds[i].transform.up +  " " + minVector);

			//rootObj.transform.up = -bondManager.GetBondVector(gameObject);

			if (gameObject == bondManager.atomEnd)
				bondManager.HierarchyBuilder(rootObj, bondManager.atomStart);
			else if (gameObject == bondManager.atomStart)
				bondManager.HierarchyBuilder(rootObj, bondManager.atomEnd);

			IEnumerator coroutine = rotateAtomGroup(rootObj, bonds[i], gameObject, minVector * bondManager.bondLength, Time.time, 0.2F);

			//Debug.Log("rotateAtomG" + bondManager.bondLength);
			
			//Debug.Log("rotating group" + minVector);
			//yield return new WaitForSeconds(2F);
			coroutineList.Add(coroutine);
			StartCoroutine(coroutine);
		}*/

        while (true)
		{
			bool done = true;
			foreach (IEnumerator coroutine in coroutineList)
			{
				if (coroutine.Current != null)
				{
					done = false;
					//Debug.Log(coroutine.Current);
					yield return coroutine;
				}
			}
			if (done)
			{
				//Debug.Log("All coroutines done");
				break;
			}
		}

		yield return null;
	}

	public IEnumerator RecalculateBondLengths()
	{
		foreach (GameObject bond in bonds)
		{
			yield return StartCoroutine (bond.GetComponent<BondManagerScript>().SetBondLength(gameObject));
		}
	}

	private float rotationTimeStamp;

	public bool HierarchyChild(GameObject parent, GameObject root, GameObject unrelated, float timestamp)
	{
		bool returnval = true;

		if (rotationTimeStamp == timestamp)		//Atom has been added to root already, do not propagate HierarchyBuilder
		{
			return returnval;
		}

		if (gameObject != unrelated && rotationTimeStamp != timestamp)
		{
			rotationTimeStamp = timestamp;
			transform.parent = root.transform;
			foreach (GameObject childbond in bonds)
			{
				if (childbond != parent)
				{
					returnval = returnval && childbond.GetComponent<BondManagerScript>().HierarchyChild(gameObject, root, unrelated, timestamp);
				}
				//else do nothing
			}
			return returnval;
		}
		else
		{
			//Prevent rotation, return error that bond is part of ring
			print("bond is part of ring");
			return false;
		}
	}

	public void AtomListBuilder (List<int> atomsList)
	{
		foreach (GameObject atom in bondedAtoms)
		{
			if (!atomsList.Contains(atom.GetInstanceID()))
			{
				atomsList.Add(atom.GetInstanceID());
				atom.GetComponent<AtomManagerScript>().AtomListBuilder(atomsList);
			}
		}
	}

	public IEnumerator moveAtom(GameObject movingAtom, Vector3 moveVector, float startTime, float speed)
	{
		float targetDistance = moveVector.magnitude;

		GameObject dummyStart = new GameObject("dummy_startpos");		//Have to use dummy objects for animation to work while rotating. World coordinates fail when rotating, local coordinates fail when parent
		dummyStart.transform.SetParent(transform.parent);				//	object's position changes (it is always set at center of molecule, so when more atoms are added, the center shifts)
		dummyStart.transform.position = movingAtom.transform.position;

		GameObject dummyEnd = new GameObject("dummy_endpos");
		dummyEnd.transform.SetParent(transform.parent);
		dummyEnd.transform.position = movingAtom.transform.position + moveVector;

		while (movingAtom.transform.localPosition != dummyEnd.transform.localPosition)
		{
			float distCovered = (Time.time - startTime) * speed;
			movingAtom.transform.localPosition = Vector3.Lerp(dummyStart.transform.localPosition, dummyEnd.transform.localPosition, distCovered / targetDistance);
			movingAtom.GetComponent<AtomManagerScript>().UpdateBonds();
			
			yield return new WaitForEndOfFrame();
		}
		Destroy(dummyStart);
		Destroy(dummyEnd);

		yield return null;
	}

	public IEnumerator moveAtomGroup(GameObject rootObj, GameObject stretchingBond, Vector3 moveVector, float startTime, float speed)
	{
		float targetDistance = moveVector.magnitude;

		GameObject dummyStart = new GameObject("dummy_startpos");
		dummyStart.transform.SetParent(transform.parent);
		dummyStart.transform.position = rootObj.transform.position;

		GameObject dummyEnd = new GameObject("dummy_endpos");
		dummyEnd.transform.SetParent(transform.parent);
		dummyEnd.transform.position = rootObj.transform.position + moveVector;

		while (rootObj.transform.localPosition != dummyEnd.transform.localPosition)
		{
			float distCovered = (Time.time - startTime) * speed;
			rootObj.transform.localPosition = Vector3.Lerp(dummyStart.transform.localPosition, dummyEnd.transform.localPosition, distCovered / targetDistance);
			stretchingBond.GetComponent<BondManagerScript>().UpdateBond();

			yield return new WaitForEndOfFrame();
		}

		for (int i = rootObj.transform.childCount - 1; i >= 0; i--)
		{
			rootObj.transform.GetChild(i).SetParent(atomsBonds.transform);
		}
		Destroy(rootObj);
		Destroy(dummyStart);
		Destroy(dummyEnd);
	}

	public IEnumerator rotateAtomGroup(GameObject rootObj, GameObject bond, GameObject staticAtom, Vector3 moveVector, float startTime, float speed)
	{
		GameObject dummyStart = new GameObject("dummy_startpos");
		dummyStart.transform.SetParent(rootObj.transform.parent);
		dummyStart.transform.position = rootObj.transform.position;

		GameObject dummyEnd = new GameObject("dummy_endpos");
		dummyEnd.transform.SetParent(rootObj.transform.parent);
		dummyEnd.transform.position = staticAtom.transform.position + moveVector;

		float targetDistance = Vector3.Distance(dummyStart.transform.position, dummyEnd.transform.position);

		//Debug.Log("corout. primer: " + targetDistance + " " + moveVector + " " + transform.position + moveVector);

		while (rootObj.transform.localPosition != dummyEnd.transform.localPosition)
		{
			float distCovered = (Time.time - startTime) * speed;
			rootObj.transform.localPosition = Vector3.Slerp(dummyStart.transform.localPosition - staticAtom.transform.localPosition, dummyEnd.transform.localPosition - staticAtom.transform.localPosition, distCovered / targetDistance);
			rootObj.transform.localPosition += staticAtom.transform.localPosition;
			bond.GetComponent<BondManagerScript>().UpdateBond();
			rootObj.transform.up = -bond.GetComponent<BondManagerScript>().GetBondVector(staticAtom);
			yield return new WaitForEndOfFrame();
		}
		for (int i = rootObj.transform.childCount - 1; i >= 0; i--)
		{
			//Debug.Log(i);
			rootObj.transform.GetChild(i).SetParent(atomsBonds.transform);
		}
		Destroy(rootObj);
		Destroy(dummyStart);
		Destroy(dummyEnd);
		yield return null;
	}
}