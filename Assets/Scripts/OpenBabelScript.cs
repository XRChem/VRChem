using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using OpenBabel;

public class OpenBabelScript : MonoBehaviour {

	//private OBMol molecule = null;

	private OBForceField pFF;
	private OBMol molecule = new OBMol();

	public int RandomInt()
	{
		OBRandom generator = new OBRandom();
		generator.TimeSeed();

		return generator.NextInt();
	}

	private void VRAtomToOBAtom(GameObject VRatom, OBMol molecule)
	{
		OBAtom atomTemp = molecule.NewAtom();
		gameObject.GetComponent<AtomRefsScript>().AddAtom(VRatom, atomTemp);

		atomTemp.SetAtomicNum(VRatom.GetComponent<AtomManagerScript>().atomProperties.AtomicNum);

		int hyb = 3;
		foreach (GameObject bond in VRatom.GetComponent<AtomManagerScript>().bonds)
		{
			hyb -= bond.GetComponent<BondManagerScript>().bondMultiplicity - 1;
		}
		atomTemp.SetHyb(hyb);

		atomTemp.SetVector(VRatom.transform.localPosition.x * 10, VRatom.transform.localPosition.y * 10, VRatom.transform.localPosition.z * 10);      //Atom local position in Å (1 nm = 10 Å)

		Debug.Log("Atom " + VRatom.GetInstanceID() + " " + atomTemp.GetVector().GetX()/10);
	}

	private void OBAtomToVRAtom(OBAtom atom)
	{
		GameObject atomTemp = gameObject.GetComponent<AtomRefsScript>().GetAtom_OBtoVR(atom);

		OBVector3 coordinates = atom.GetVector();

		atomTemp.transform.localPosition = new Vector3(Convert.ToSingle(coordinates.GetX() / 10), Convert.ToSingle(coordinates.GetY() / 10), Convert.ToSingle(coordinates.GetZ() / 10));
		atomTemp.GetComponent<AtomManagerScript>().UpdateBonds();
	}

	private IEnumerator OBAtomToVRAtom_animated(OBAtom atom)
	{
		GameObject atomTemp = gameObject.GetComponent<AtomRefsScript>().GetAtom_OBtoVR(atom);

		OBVector3 coordinates = atom.GetVector();

		Vector3 newPos = new Vector3(Convert.ToSingle(coordinates.GetX() / 10), Convert.ToSingle(coordinates.GetY() / 10), Convert.ToSingle(coordinates.GetZ() / 10));
		Debug.Log(newPos);
		yield return StartCoroutine(atomTemp.GetComponent<AtomManagerScript>().moveAtom(atomTemp, newPos - atomTemp.transform.localPosition, Time.time, 0.2F));

		yield return null;
	}

	private void VRBondToOBBond(GameObject bond, OBMol molecule)
	{
		if (gameObject.GetComponent<AtomRefsScript>().GetAtom_VRtoOB(bond.GetComponent<BondManagerScript>().atomStart) == null)
			VRAtomToOBAtom(bond.GetComponent<BondManagerScript>().atomStart, molecule);
		if (gameObject.GetComponent<AtomRefsScript>().GetAtom_VRtoOB(bond.GetComponent<BondManagerScript>().atomEnd) == null)
			VRAtomToOBAtom(bond.GetComponent<BondManagerScript>().atomEnd, molecule);

		OBAtom startAtom = gameObject.GetComponent<AtomRefsScript>().GetAtom_VRtoOB(bond.GetComponent<BondManagerScript>().atomStart);
		OBAtom endAtom = gameObject.GetComponent<AtomRefsScript>().GetAtom_VRtoOB(bond.GetComponent<BondManagerScript>().atomEnd);

		molecule.AddBond((int)startAtom.GetIdx(), (int)endAtom.GetIdx(), bond.GetComponent<BondManagerScript>().bondMultiplicity);

		/*OBBond bondTemp = molecule.NewBond();
		gameObject.GetComponent<ObjectIDsScript>().Add(bond.GetInstanceID(), bondTemp.GetId());

		bondTemp.SetBondOrder(bond.GetComponent<BondManagerScript>().bondMultiplicity);

		if (gameObject.GetComponent<ObjectIDsScript>().ConvertID_VRtoOB(bond.GetComponent<BondManagerScript>()._atomStart.GetInstanceID()) == null)
			VRAtomToOBAtom(bond.GetComponent<BondManagerScript>()._atomStart, molecule);
		if (gameObject.GetComponent<ObjectIDsScript>().ConvertID_VRtoOB(bond.GetComponent<BondManagerScript>()._atomEnd.GetInstanceID()) == null)
			VRAtomToOBAtom(bond.GetComponent<BondManagerScript>()._atomEnd, molecule);

		bondTemp.SetBegin(molecule.GetAtomById((uint)gameObject.GetComponent<ObjectIDsScript>().ConvertID_VRtoOB(bond.GetComponent<BondManagerScript>()._atomStart.GetInstanceID())));
		bondTemp.SetEnd(molecule.GetAtomById((uint)gameObject.GetComponent<ObjectIDsScript>().ConvertID_VRtoOB(bond.GetComponent<BondManagerScript>()._atomEnd.GetInstanceID())));*/
	}

	public IEnumerator UpdateOptimizedPositions(OBForceField pFF, OBMol molecule)
	{
		pFF.GetCoordinates(molecule);

		foreach (OBAtom atom in molecule.Atoms())
		{
			OBAtomToVRAtom(atom);
		}

		yield return null;
	}

	public IEnumerator UpdateOptimizedPositions_animated(OBForceField pFF, OBMol molecule)
	{
		List<IEnumerator> coroutineList = new List<IEnumerator>();

		pFF.GetCoordinates(molecule);

		foreach (OBAtom atom in molecule.Atoms())
		{
			IEnumerator coroutine = OBAtomToVRAtom_animated(atom);
			coroutineList.Add(coroutine);
			StartCoroutine(coroutine);
		}

		while (true)    //Wait until all coroutines above are done
		{
			bool done = true;
			foreach (IEnumerator coroutine in coroutineList)
			{
				if (coroutine.Current != null)
				{
					done = false;
					Debug.Log(coroutine.Current);
					yield return new WaitForSeconds(0.1F);
				}
			}
			if (done)
			{
				Debug.Log("All coroutines done");
				break;
			}
		}

		yield return 1;
	}

	public IEnumerator Optimize(int rounds)
	{
		Debug.Log("opt");
		gameObject.GetComponent<AtomRefsScript>().ClearAtomRefsList();

		molecule.Clear();
		//molecule = new OBMol();

		foreach (Transform item in gameObject.transform)
		{
			if (item.gameObject.GetComponent<AtomManagerScript>() != null && gameObject.GetComponent<AtomRefsScript>().GetAtom_VRtoOB(item.gameObject) == null)
			{
				VRAtomToOBAtom(item.gameObject, molecule);
			}
			else if (item.gameObject.GetComponent<BondManagerScript>() != null)
			{
				VRBondToOBBond(item.gameObject, molecule);
			}
		}

		if (pFF != null)		//TODO REMOVE?
			pFF.Dispose();

		pFF = OBForceField.FindForceField("MMFF94");

		if (pFF == null)
		{
			Debug.Log("FF not found");
			yield break;
		}

		if (!pFF.Setup(molecule))
		{
			Debug.Log("FF setup failed");
			yield break;
		}

		pFF.ConjugateGradientsInitialize();

		yield return StartCoroutine(UpdateOptimizedPositions(pFF, molecule));



		for (int i = 0; i < rounds; i++)
		{
			pFF.ConjugateGradientsTakeNSteps(1);

			yield return StartCoroutine(UpdateOptimizedPositions(pFF, molecule));
		}

		//gameObject.GetComponent<AtomRefsScript>().ClearAtomRefsList();

		yield return 1;

	}

	public IEnumerator OptimizeNoInitialization()		//TODO REMOVE!!
	{
		pFF.ConjugateGradientsTakeNSteps(1);

		yield return StartCoroutine(UpdateOptimizedPositions(pFF, molecule));
	}
}