using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateModeScript : MonoBehaviour {

	//set atom and bond prefabs in prefab of interaction mode object
	public GameObject atomPrefab, bondPrefab, doubleBondPrefab, tripleBondPrefab;
    public ContextSelectionScript contextSelectionScript;

	private GameObject initialAtom, createdAtom, createdBond, bondtoReplace;
	private bool midCreation = false, creatingNew = false, replacingOrBonding = false;

	public void StartCreate(GameObject startCreateCaller, GameObject startCreateTarget)
	{
        transform.GetChild(0).GetComponent<ContextSelectionScript>().Deselection();

		Debug.Log((startCreateCaller == null ? "null" : startCreateCaller.GetInstanceID().ToString()) + " " + (startCreateTarget == null ? "null" : startCreateTarget.GetInstanceID().ToString()));
		if (midCreation == true)		//Return if creation already in progress (e.g. when trying to StartCreate with 2 controllers at the same time)
			return;

		gameObject.GetComponent<LiveOptimizeScript>().midCreation = true;	//TODO REMOVE

		midCreation = true;

		if (startCreateTarget != null)		//starting on existing object
		{
			replacingOrBonding = true;
			if (startCreateTarget.GetComponent<AtomManagerScript>() != null)
			{
				initialAtom = startCreateTarget;
				createdAtom = CreateAtom(initialAtom.transform.position);
				createdBond = CreateBond(initialAtom, createdAtom);
				createdAtom.GetComponent<Rigidbody>().isKinematic = false;
				createdAtom.GetComponent<FixedJoint>().connectedBody = startCreateCaller.GetComponent<Rigidbody>();
				createdAtom.layer = LayerMask.NameToLayer("Ignore Raycast");
			}
			else if (startCreateTarget.GetComponent<BondManagerScript>() != null)
				bondtoReplace = startCreateTarget;
			else
				return;		//Initial object was not an atom/bond.
		}
		else			//starting on blank, so create atom here and fix joint, release on pinch release
		{
			creatingNew = true;
			createdAtom = CreateAtom (startCreateCaller.transform.position);
			createdAtom.GetComponent<Rigidbody> ().isKinematic = false;
			createdAtom.GetComponent<FixedJoint> ().connectedBody = startCreateCaller.GetComponent<Rigidbody>();
			createdAtom.layer = LayerMask.NameToLayer("Ignore Raycast");
		}
	}

	public void EndCreate(GameObject endCreateCaller, GameObject endCreateTarget)
	{
		Debug.Log((endCreateCaller == null ? "null" : endCreateCaller.GetInstanceID().ToString()) + " " + (endCreateTarget == null ? "null" : endCreateTarget.GetInstanceID().ToString()));
		
		if (creatingNew && createdAtom.GetComponent<FixedJoint>().connectedBody == endCreateCaller.GetComponent<Rigidbody>())   //release free floating created atom
		{
			createdAtom.GetComponent<FixedJoint> ().connectedBody = null;
			createdAtom.GetComponent<Rigidbody> ().isKinematic = true;
			createdAtom.layer = LayerMask.NameToLayer("atoms");                                     //Set atom layer back to "atoms" so raycasts can hit it again
			if (UIManagerScript.autoSaturation)
				StartCoroutine (CheckAtomSaturationAndSave(createdAtom));
		}

		else if (replacingOrBonding) 
		{
			if (endCreateTarget)          
			{
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
				//create bond between them --TODO check if there is no bond already
				//print("bonding atoms");
<<<<<<< HEAD
=======
				bool bondExists = false;
>>>>>>> 578079b... Signed-off-by: Krupakar Dhinakaran <krupakar.dhinakaran@aalto.fi> added undo redo but there are still some bugs, there are not buttons just use z and x
				foreach (GameObject atom in _rightPinchTarget.GetComponent<AtomManagerScript>().bondedAtoms)
=======
				if(!endCreateTarget.GetComponent<AtomManagerScript>().bondedAtoms.Contains(initialAtom))
>>>>>>> df9d52d... Create mode control methods rewritten.
				{
					createdBond.GetComponent<BondManagerScript>().DeleteBond();
					createdAtom.GetComponent<AtomManagerScript>().DeleteAtom();

					CreateBond(initialAtom, endCreateTarget);
					GetComponent<UndoRedoScript>().AddEntry();
				}
<<<<<<< HEAD
<<<<<<< HEAD
				createdBond = Instantiate(bondPrefab, _rightPinchTarget.transform.position, Quaternion.identity, SceneAtomBondObject.transform) as GameObject;
=======
				createdBond = Instantiate (bondPrefab, _rightPinchTarget.transform.position, Quaternion.identity, transform) as GameObject;
>>>>>>> 0c636a4... split up creation, movement, deletion, and selection; remade mouse versions to use mouse x movement for rotation instead of a 3d mouse gameobject
				createdBond.GetComponent<BondManagerScript>().UpdateBond(initialAtom, _rightPinchTarget);
=======
				if(!bondExists)
=======
				else
>>>>>>> df9d52d... Create mode control methods rewritten.
=======
				if (initialAtom)
>>>>>>> 68934d2... Added preliminary support for live optimization
				{
					if (initialAtom != endCreateTarget)      //release on another atom, create bond between them
					{
						if (!endCreateTarget.GetComponent<AtomManagerScript>().bondedAtoms.Contains(initialAtom))
						{
							createdBond.GetComponent<BondManagerScript>().DeleteBond();
							createdAtom.GetComponent<AtomManagerScript>().DeleteAtom();

							CreateBond(initialAtom, endCreateTarget);
							GetComponent<UndoRedoScript>().AddEntry();

							gameObject.GetComponent<LiveOptimizeScript>().midCreation = false; //TODO REMOVE
						}
						else   //release on an already bonded atom
						{
							createdAtom.GetComponent<FixedJoint>().connectedBody = null;
							createdAtom.GetComponent<Rigidbody>().isKinematic = true;
							createdBond.GetComponent<BondManagerScript>().DeleteBond();
							createdAtom.GetComponent<AtomManagerScript>().DeleteAtom();

							gameObject.GetComponent<LiveOptimizeScript>().midCreation = false; //TODO REMOVE

						}
					}

					else if (initialAtom == endCreateTarget && endCreateTarget != contextSelectionScript.actionAtom && endCreateTarget != contextSelectionScript.relatedAtom1 && endCreateTarget != contextSelectionScript.relatedAtom2)  // Replace atom (release on same atom)
					{
						createdBond.GetComponent<BondManagerScript>().DeleteBond();
						createdAtom.GetComponent<AtomManagerScript>().DeleteAtom();

						Debug.Log("Replace");
						StartCoroutine(ReplaceAtom(endCreateTarget));
					}
				}
<<<<<<< HEAD
>>>>>>> 578079b... Signed-off-by: Krupakar Dhinakaran <krupakar.dhinakaran@aalto.fi> added undo redo but there are still some bugs, there are not buttons just use z and x
			}
			else if (endCreateTarget && bondtoReplace == endCreateTarget && endCreateTarget != ContextSelectionScript.baseBond && endCreateTarget != ContextSelectionScript.actionBond)		//Release on initial bond; change bond multiplicity
			{
				createdBond.GetComponent<BondManagerScript>().DeleteBond();
				createdAtom.GetComponent<AtomManagerScript>().DeleteAtom();

				ChangeBondMultiplicity(bondtoReplace);
			}
			else if (endCreateTarget && initialAtom == endCreateTarget && endCreateTarget != ContextSelectionScript.actionAtom && endCreateTarget != ContextSelectionScript.relatedAtom1 && endCreateTarget != ContextSelectionScript.relatedAtom2)	// Replace atom (release on same atom)
			{
<<<<<<< HEAD
<<<<<<< HEAD
				//replace atom TODO replace with different element atom
<<<<<<< HEAD
				createdAtom = Instantiate(atomPrefab, _rightPinchTarget.transform.position, Quaternion.identity, SceneAtomBondObject.transform) as GameObject;
=======
				StartCoroutine(ReplaceAtom(_rightPinchTarget));
<<<<<<< HEAD
				/*createdAtom = Instantiate(atomPrefab, _rightPinchTarget.transform.position, Quaternion.identity, transform) as GameObject;
>>>>>>> 630bd32... First introduction of double/triple bonds
				var createdAtomManager = createdAtom.GetComponent<AtomManagerScript>();
				createdAtomManager.InheritBonds(_rightPinchTarget);
				GameObject.Destroy(_rightPinchTarget);
				createdAtomManager.UpdateBonds();
				createdAtomManager.SetProperties();
				createdAtomManager.Saturate();
				createdAtomManager.RecalculateDistances();
<<<<<<< HEAD
=======
				if (UIManagerScript.autoSaturation)
					createdAtomManager.Saturate();*/
<<<<<<< HEAD
>>>>>>> 630bd32... First introduction of double/triple bonds
				}
<<<<<<< HEAD
=======
				createdAtom = Instantiate(atomPrefab, _rightPinchTarget.transform.position, Quaternion.identity, transform) as GameObject;
				createdAtom.GetComponent<AtomManagerScript> ().InheritBonds (_rightPinchTarget);
				GameObject.Destroy (_rightPinchTarget);
				createdAtom.GetComponent<AtomManagerScript> ().UpdateBonds ();
				createdAtom.GetComponent<AtomManagerScript> ().SetProperties ();
				if(UIManagerScript.autoSaturation)
					createdAtom.GetComponent<AtomManagerScript> ().Saturate ();
=======
>>>>>>> 578079b... Signed-off-by: Krupakar Dhinakaran <krupakar.dhinakaran@aalto.fi> added undo redo but there are still some bugs, there are not buttons just use z and x
			}
>>>>>>> 0c636a4... split up creation, movement, deletion, and selection; remade mouse versions to use mouse x movement for rotation instead of a 3d mouse gameobject
			else if (!_rightPinchTarget && pinchPoint != null) //release on empty space
=======
=======
=======
				createdBond.GetComponent<BondManagerScript>().DeleteBond();
				createdAtom.GetComponent<AtomManagerScript>().DeleteAtom();

				Debug.Log("Replace");
				StartCoroutine(ReplaceAtom(endCreateTarget));
>>>>>>> df9d52d... Create mode control methods rewritten.
=======

				else if (bondtoReplace == endCreateTarget && endCreateTarget != contextSelectionScript.baseBond && endCreateTarget != contextSelectionScript.actionBond)      //Release on initial bond; change bond multiplicity
				{
					ChangeBondMultiplicity(bondtoReplace);

				}
>>>>>>> 68934d2... Added preliminary support for live optimization
			}
>>>>>>> eb66a16... added color to periodic table and delete area size

<<<<<<< HEAD
<<<<<<< HEAD
			else if (initialAtom && !_rightPinchTarget && pinchPoint != null) //release on empty space
>>>>>>> 9fb3230... added periodic table with save to quicklist option, added cycling through single double triple bonds with pinch and release on bond
=======
			else if (initialAtom && !endCreateTarget && endCreateCaller != null)	//release on empty space
>>>>>>> df9d52d... Create mode control methods rewritten.
=======
			else if (initialAtom && !endCreateTarget && endCreateCaller != null)    //release on empty space
>>>>>>> 68934d2... Added preliminary support for live optimization
			{
				createdAtom.GetComponent<FixedJoint>().connectedBody = null;
				createdAtom.GetComponent<Rigidbody>().isKinematic = true;
				createdAtom.layer = LayerMask.NameToLayer("atoms");                                     //Set atom layer back to "atoms" so raycasts can hit it again
				if (UIManagerScript.autoSaturation)
					StartCoroutine(CheckAtomSaturationAndSave(createdAtom));
			}
		}

		initialAtom = null; //resets actions so that bonds find new initial points
		midCreation = false;
		replacingOrBonding = false;
		creatingNew = false;

		gameObject.GetComponent<LiveOptimizeScript>().moleculeChanged = true;  //TODO REMOVE

	}

	public void HideCreatedAtom(GameObject raycastHitObject)		//Used to hide the created atom when e.g. hovering over an existing atom
	{
		if (createdAtom && createdAtom.GetComponent<MeshRenderer>().enabled && midCreation)
		{
			createdAtom.GetComponent<MeshRenderer>().enabled = false;
			if (raycastHitObject.GetComponent<AtomManagerScript>() != null)
			{
				createdAtom.GetComponent<Rigidbody>().isKinematic = true;
				createdAtom.transform.position = raycastHitObject.transform.position;       //Set the temporary bond to end at the pointed atom
			}
		}
	}

	public void ShowCreatedAtom()
	{
		if (createdAtom && !createdAtom.GetComponent<MeshRenderer>().enabled && midCreation)
		{
			createdAtom.GetComponent<MeshRenderer>().enabled = true;
			if (createdBond.GetComponent<BondManagerScript>().atomEnd != createdAtom)
				createdBond.GetComponent<BondManagerScript>().atomEnd = createdAtom;

			createdAtom.GetComponent<Rigidbody>().isKinematic = false;
		}
	}

	private void Update()
	{
<<<<<<< HEAD
		if(Input.GetKey(KeyCode.C) && Input.GetMouseButtonDown(0) && activeMode) //equivalent to pinch start
		{
			//start create
			mousePinching = true;

			Ray mouseRay; RaycastHit mouseHit;
			mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (Physics.Raycast (mouseRay, out mouseHit, 10, mouseMask))
			{
				mouseReplacing = true;
				if (mouseHit.collider.gameObject.GetComponent<BondManagerScript> () != null)
					bondtoReplace = mouseHit.collider.gameObject;
				else if(mouseHit.collider.gameObject.GetComponent<AtomManagerScript>() != null)
					initialAtom = mouseHit.collider.gameObject;
			}
			else
			{
				mouseCreatingNew = true;
				createdAtom = CreateAtom(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(transform.position).z)));
				//createdAtom.GetComponent<AtomManagerScript> ().SetProperties ();
			}
		}


		if(mousePinching)
		{
			if(createdAtom && mouseCreatingNew)
				createdAtom.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint (transform.position).z));
		}

		if(mousePinching && Input.GetMouseButtonUp(0))
		{
			//end create
			mousePinching = false;
			if(mouseCreatingNew)
			{
				if (UIManagerScript.autoSaturation)
					StartCoroutine (CheckAtomSaturationAndSave(createdAtom));
				mouseCreatingNew = false;
				createdAtom = null;
			}
			else if(mouseReplacing)
			{
				Ray mouseRay; RaycastHit mouseHit;
				mouseRay = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (mouseRay, out mouseHit, 10, mouseMask))
				{
					if (mouseHit.collider.gameObject.GetComponent<AtomManagerScript>() != null && mouseHit.collider.gameObject != initialAtom) //bonding
					{
<<<<<<< HEAD
						createdBond = Instantiate (bondPrefab, mouseHit.collider.gameObject.transform.position, Quaternion.identity, transform) as GameObject;
						createdBond.GetComponent<BondManagerScript>().UpdateBond(initialAtom, mouseHit.collider.gameObject);
=======
						bool bondExists = false;
						foreach (GameObject atom in mouseHit.collider.gameObject.GetComponent<AtomManagerScript>().bondedAtoms)
						{
							if (atom == initialAtom)
							{
								bondExists = true;
								Debug.Log("BOND EXISTS!");
							}
						}
						if (!bondExists)
						{
							createdBond = CreateBond(initialAtom, mouseHit.collider.gameObject);
							GetComponent<UndoRedoScript>().AddEntry();
						}
>>>>>>> dd78710... Added save/load feature
					}
					else if(mouseHit.collider.gameObject == initialAtom && initialAtom != ContextSelectionScript.actionAtom && initialAtom != ContextSelectionScript.relatedAtom1 && initialAtom != ContextSelectionScript.relatedAtom2) //replacing
					{
<<<<<<< HEAD
						createdAtom = Instantiate(atomPrefab, mouseHit.collider.gameObject.transform.position, Quaternion.identity, transform) as GameObject;
						createdAtom.GetComponent<AtomManagerScript> ().InheritBonds (mouseHit.collider.gameObject);
						GameObject.Destroy (mouseHit.collider.gameObject);
						createdAtom.GetComponent<AtomManagerScript> ().UpdateBonds ();
						createdAtom.GetComponent<AtomManagerScript> ().SetProperties ();
						if(UIManagerScript.autoSaturation)
							createdAtom.GetComponent<AtomManagerScript> ().Saturate ();
=======
						StartCoroutine(ReplaceAtom(mouseHit.collider.gameObject));
<<<<<<< HEAD
						/*createdAtom = Instantiate(atomPrefab, mouseHit.collider.gameObject.transform.position, Quaternion.identity, transform) as GameObject;
						var createdAtomManager = createdAtom.GetComponent<AtomManagerScript>();

						createdAtomManager.InheritBonds (mouseHit.collider.gameObject);
						GameObject.Destroy (mouseHit.collider.gameObject);
						createdAtomManager.UpdateBonds ();
						createdAtomManager.SetProperties ();
						createdAtomManager.RecalculateDistances();
						if (UIManagerScript.autoSaturation)
							createdAtomManager.Saturate ();*/
>>>>>>> 630bd32... First introduction of double/triple bonds
=======
>>>>>>> 578079b... Signed-off-by: Krupakar Dhinakaran <krupakar.dhinakaran@aalto.fi> added undo redo but there are still some bugs, there are not buttons just use z and x
					}
					else if(mouseHit.collider.gameObject == bondtoReplace && bondtoReplace != ContextSelectionScript.actionBond && bondtoReplace != ContextSelectionScript.baseBond)
					{
						//changing bond type
						ChangeBondMultiplicity(bondtoReplace);

						/*print("changing bond mult");
						GameObject tempNewBond;
						if (bondtoReplace.GetComponent<BondManagerScript> ().bondMultiplicity == 1)
							tempNewBond = Instantiate (doubleBondPrefab, transform);
						else if (bondtoReplace.GetComponent<BondManagerScript> ().bondMultiplicity == 2)
							tempNewBond = Instantiate (tripleBondPrefab, transform);
						else
							tempNewBond = Instantiate (bondPrefab, transform);

						tempNewBond.GetComponent<BondManagerScript>().InheritAtoms(bondtoReplace);
						Destroy(bondtoReplace);

						Debug.Log("desaturate");
<<<<<<< HEAD
<<<<<<< HEAD
						StartCoroutine( CheckBothSaturationsAndSave (tempNewBond, bondtoReplace));
=======
						StartCoroutine (CheckBondSaturationAndSave(tempNewBond));
=======
						StartCoroutine (CheckBondSaturationAndSave(tempNewBond));*/
>>>>>>> c02ad8e... Fixed crash when changing bond multiplicity in VR
						//StartCoroutine (CheckSaturationAndSave(tempNewBond.GetComponent<BondManagerScript>()._atomEnd)); //creates 2 undo history entries - im just going to leave it that way for now
>>>>>>> 3fa6523... All atom animations should work independently
					}
				}
				else
				{
					//create new atom
					//print("creating new BONDED atom");
					createdAtom = CreateAtom(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(initialAtom.transform.position).z)));
					//create bond between them
					createdBond = CreateBond (initialAtom, createdAtom);

					createdAtom.transform.up = -createdAtom.GetComponent<AtomManagerScript>().bonds[0].transform.up;

					if (UIManagerScript.autoSaturation)
<<<<<<< HEAD
<<<<<<< HEAD
						StartCoroutine (CheckSaturationAndSave(createdAtom));
=======
					{
						StartCoroutine(createdAtom.GetComponent<AtomManagerScript>().CheckSaturation());
					}
>>>>>>> 02f5469... .mol file coordinate system fixed
=======
						StartCoroutine (CheckAtomSaturationAndSave(createdAtom));
>>>>>>> 3fa6523... All atom animations should work independently
				}
				mouseReplacing = false;
				initialAtom = null;
				createdAtom = null;
				createdBond = null;
			}
		}
		if (Input.GetKey(KeyCode.M) && activeMode)		//TODO remove lol
			StartCoroutine(RemoveEverything());

		if (Input.GetKeyDown(KeyCode.O) && activeMode)
=======
		if (midCreation && createdBond != null)
>>>>>>> df9d52d... Create mode control methods rewritten.
		{
			createdBond.GetComponent<BondManagerScript>().UpdateBond();
		}
	}

	public GameObject CreateAtom (Vector3 position)
	{
		GameObject newAtom = Instantiate(atomPrefab, position, Quaternion.identity, transform) as GameObject;
		newAtom.GetComponent<AtomManagerScript>().SetProperties();
		
		return newAtom;
	}

	/*public GameObject CreateAtom(Vector3 position, string element)
	{
		GameObject newAtom = Instantiate(atomPrefab, position, Quaternion.identity, transform) as GameObject;
		newAtom.GetComponent<AtomManagerScript>().SetProperties(element);

		return newAtom;
	}*/

	public GameObject CreateBond (GameObject startAtom, GameObject endAtom)
	{
		GameObject newBond = Instantiate(bondPrefab, startAtom.transform.position, Quaternion.identity, transform) as GameObject;
		newBond.GetComponent<BondManagerScript>().UpdateBond(startAtom, endAtom);
		newBond.GetComponent<BondManagerScript>().UpdateBondLength();

		return newBond;
	}

	public GameObject CreateBond(GameObject startAtom, GameObject endAtom, int multiplicity)
	{
		GameObject newBond;

<<<<<<< HEAD
		if (multiplicity == 2)
			newBond = Instantiate(doubleBondPrefab, startAtom.transform.position, Quaternion.identity, transform) as GameObject;
		else if (multiplicity == 3)
			newBond = Instantiate(tripleBondPrefab, startAtom.transform.position, Quaternion.identity, transform) as GameObject;
		else
			newBond = Instantiate(bondPrefab, startAtom.transform.position, Quaternion.identity, transform) as GameObject;

=======
		switch (multiplicity)
		{
			case 1:
				newBond = Instantiate(bondPrefab, startAtom.transform.position, Quaternion.identity, transform) as GameObject;
				break;
			case 2:
				newBond = Instantiate(doubleBondPrefab, startAtom.transform.position, Quaternion.identity, transform) as GameObject;
				break;
			case 3:
				newBond = Instantiate(tripleBondPrefab, startAtom.transform.position, Quaternion.identity, transform) as GameObject;
				break;
			default:
				return null;

		}
		newBond.GetComponent<BondManagerScript>().bondMultiplicity = multiplicity;
>>>>>>> 02f5469... .mol file coordinate system fixed
		newBond.GetComponent<BondManagerScript>().UpdateBond(startAtom, endAtom);

		return newBond;
	}

	private void ChangeBondMultiplicity (GameObject bondToReplace)
	{
		print("changing bond mult");

		GameObject tempNewBond;
		if (bondToReplace.GetComponent<BondManagerScript>().bondMultiplicity == 1)
			tempNewBond = Instantiate(doubleBondPrefab, transform);
		else if (bondToReplace.GetComponent<BondManagerScript>().bondMultiplicity == 2)
			tempNewBond = Instantiate(tripleBondPrefab, transform);
		else
			tempNewBond = Instantiate(bondPrefab, transform);

		tempNewBond.GetComponent<BondManagerScript>().InheritAtoms(bondtoReplace);
		Destroy(bondtoReplace);
		Debug.Log("desaturate");

		StartCoroutine(CheckBondSaturationAndSave(tempNewBond));
	}

	IEnumerator CheckAtomSaturationAndSave(GameObject _createdAtom)
	{
		//wait for saturation and then add history entry
		yield return StartCoroutine(_createdAtom.GetComponent<AtomManagerScript> ().CheckSaturation ());
		//print ("Saturation done, now save");
		GetComponent<UndoRedoScript>().AddEntry();
		gameObject.GetComponent<LiveOptimizeScript>().midCreation = false; //TODO REMOVE

	}

	IEnumerator CheckBondSaturationAndSave(GameObject _createdBond)
	{
		//wait for saturation and then add history entry
		//yield return StartCoroutine(_createdBond.GetComponent<BondManagerScript>().SetBondLength(_createdBond.GetComponent<BondManagerScript>().atomStart));

		yield return StartCoroutine(_createdBond.GetComponent<BondManagerScript>().atomStart.GetComponent<AtomManagerScript>().CheckSaturation());
		Debug.Log("atomStart done");
		yield return StartCoroutine(_createdBond.GetComponent<BondManagerScript>().atomEnd.GetComponent<AtomManagerScript>().CheckSaturation());
		Debug.Log("atomEnd done");
		print("Saturation done, now save");
		GetComponent<UndoRedoScript>().AddEntry();
		gameObject.GetComponent<LiveOptimizeScript>().midCreation = false; //TODO REMOVE
	}

<<<<<<< HEAD
	IEnumerator CheckBothSaturationsAndSave(GameObject newBond, GameObject oldBond)
	{
		print (oldBond + " " + newBond);
		yield return StartCoroutine (newBond.GetComponent <BondManagerScript> ().InheritAtoms (oldBond));
		Destroy (oldBond);
		bondtoReplace = null;

		IEnumerator one = newBond.GetComponent<BondManagerScript>()._atomStart.GetComponent<AtomManagerScript> ().CheckSaturation (), two = newBond.GetComponent<BondManagerScript>()._atomEnd.GetComponent<AtomManagerScript> ().CheckSaturation ();
		yield return StartCoroutine (one);
		yield return StartCoroutine (two);
		//while (one.Current != null || two.Current != null)
			//yield return new WaitForSeconds (0.1f);
		print ("Saturation done, now sve");
		GetComponent<UndoRedoScript>().AddEntry();
	}
=======
>>>>>>> 3fa6523... All atom animations should work independently

	private IEnumerator ReplaceAtom (GameObject pointedObject)
	{
		Debug.Log("Replace");

		createdAtom = CreateAtom(pointedObject.transform.position);

		var createdAtomManager = createdAtom.GetComponent<AtomManagerScript>();
		if (pointedObject.GetComponent<AtomManagerScript>().bonds.Count > 0)
			createdAtomManager.InheritBonds(pointedObject);
		Destroy(pointedObject);

		createdAtomManager.UpdateBonds();

		if (createdAtomManager.bonds.Count == 1)	//If the new atom only has 1 bond, move new atom to correct distance. The existing part should stay in place.
		{
			if (createdAtomManager.bonds[0].GetComponent<BondManagerScript>().atomStart == createdAtom)
			{
				yield return StartCoroutine(createdAtomManager.bonds[0].GetComponent<BondManagerScript>().SetBondLength(createdAtomManager.bonds[0].GetComponent<BondManagerScript>().atomEnd));
			}
			else
			{
				yield return StartCoroutine(createdAtomManager.bonds[0].GetComponent<BondManagerScript>().SetBondLength(createdAtomManager.bonds[0].GetComponent<BondManagerScript>().atomStart));
			}
		}

		Debug.Log("recalculated");
		if (UIManagerScript.autoSaturation)
			yield return StartCoroutine (CheckAtomSaturationAndSave(createdAtom));
		else
			gameObject.GetComponent<LiveOptimizeScript>().midCreation = false; //TODO REMOVE
		//yield return null;
	}

	public void RemoveEverythingWrapper() //called from UI buttons
	{
		StartCoroutine (RemoveEverything ());
	}

	private IEnumerator RemoveEverything ()
	{
		gameObject.GetComponent<LiveOptimizeScript>().midCreation = true; //TODO REMOVE

		transform.GetComponentInChildren<ContextSelectionScript> ().Deselection ();	//added to solve selection crash when building again

		GameObject[] atoms = GameObject.FindGameObjectsWithTag("atoms");
		GameObject[] bonds = GameObject.FindGameObjectsWithTag("bonds");

		foreach (GameObject atom in atoms)
		{
			Destroy(atom.GetComponent<FixedJoint>());
			atom.GetComponent<SphereCollider>().isTrigger = false;
			atom.GetComponent<Rigidbody>().isKinematic = false;
			atom.GetComponent<Rigidbody>().useGravity = true;
			atom.GetComponent<Rigidbody>().mass = 0.05F;
			atom.GetComponent<Rigidbody>().AddExplosionForce(0.5F, transform.position, 0.5F, 0.01F);
		}
		foreach (GameObject bond in bonds)
		{
			bond.GetComponent<CapsuleCollider>().isTrigger = false;
			bond.GetComponent<Rigidbody>().isKinematic = false;
			bond.GetComponent<Rigidbody>().useGravity = true;
			bond.GetComponent<Rigidbody>().mass = 0.05F;
			bond.GetComponent<Rigidbody>().AddExplosionForce(0.5F, transform.position, 0.5F, 0.01F);
		}

		yield return new WaitForSeconds(2F);

		foreach (GameObject atom in atoms)
			Destroy(atom);
		foreach (GameObject bond in bonds)
			Destroy(bond);

		GetComponent<UndoRedoScript>().AddEntry();

		gameObject.GetComponent<LiveOptimizeScript>().midCreation = false; //TODO REMOVE
		gameObject.GetComponent<LiveOptimizeScript>().moleculeChanged = true; //TODO REMOVE

	}
}