using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenBabel;

public class AtomRefsScript : MonoBehaviour
{
	private List<AtomRefsEntry> AtomRefsList;

	public struct AtomRefsEntry
	{
		public GameObject VRAtom;
		public OBAtom OBAtom;

		public AtomRefsEntry(GameObject VRAtom_new, OBAtom OBAtom_new)
		{
			VRAtom = VRAtom_new;
			OBAtom = OBAtom_new;
		}
	}

	void Start()
	{
		AtomRefsList = new List<AtomRefsEntry>();
	}

	public void AddAtom (GameObject VRAtom, OBAtom OBAtom)
	{
		AtomRefsEntry NewEntry = new AtomRefsEntry(VRAtom, OBAtom);
		AtomRefsList.Add(NewEntry);
	}

	public void ClearAtomRefsList()
	{
		AtomRefsList.Clear();
	}

	public OBAtom GetAtom_VRtoOB(GameObject atom)
	{
		foreach (AtomRefsEntry entry in AtomRefsList)
		{
			if (entry.VRAtom == atom)
				return entry.OBAtom;
		}
		return null;
	}

	public GameObject GetAtom_OBtoVR(OBAtom atom)
	{
		foreach (AtomRefsEntry entry in AtomRefsList)
		{
			if (entry.OBAtom.GetId() == atom.GetId())
			{
				return entry.VRAtom;
			}
		}
		Debug.Log("not OK");
		return null;
	}
}