using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadScript : MonoBehaviour {

	public GameObject atomsBonds;

	/*public void SaveMolFile()
	{
		GameObject[] atoms, bonds;

		atoms = GameObject.FindGameObjectsWithTag("atoms");
		bonds = GameObject.FindGameObjectsWithTag("bonds");

		Debug.Log("saving");
		using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Public\Documents\test.mol"))
		{
			string line = string.Empty;
			Dictionary<int, int> atomRefs = new Dictionary<int, int>();			//TODO proper padding

			//HEADER (lines 1-4)
			file.WriteLine("Molecule name");        //line 1: molecule name, or blank
			file.WriteLine("{0,2}VR-Chem{1}3D", "OP", DateTime.Now.ToString("MMddyyHHmm"));     //line 2: user initials (2 char), program name (max 8?), date in MMddYYHHmm, dimensions (2D or 3D), or blank line
			file.WriteLine("*Comment line*");       //line 3: comment line, or blank
			file.WriteLine("0 0 0   0 0      999 V3000");  //line 4: some junk, 999, and molfile version number V3000 / V2000

			//CTAB
			file.WriteLine("M V30   BEGIN CTAB");		//Begin molecule data, all lines below here must start with M V30
			file.WriteLine("M V30   COUNTS {0} {1} {2} {3} {4}", atoms.Length, bonds.Length, 0, 0, 0); //COUNTS line: num of atoms, num of bonds, num of subgroups, num of 3d constraints, chirality (0/1)

			file.WriteLine("M V30   BEGIN ATOM");

			Vector3 minPos = Vector3.zero;

			for (int i = 0; i < atoms.Length; i++)
			{
				if (i == 0)
					minPos = atoms[0].transform.position;
				else
				{
					if (Vector3.Magnitude(atoms[i].transform.position) < Vector3.Magnitude(minPos))
						minPos = atoms[i].transform.position;
				}
			}


			for (int i = 0; i < atoms.Length; i++)
			{
				file.WriteLine("M V30   {0} {1}   {2} {3} {4}   {5}", i, atoms[i].GetComponent<AtomManagerScript>().atomProperties.Symbol, (atoms[i].transform.position.x - minPos.x) * 10, (atoms[i].transform.position.y - minPos.y) * 10, -(atoms[i].transform.position.z - minPos.z) * 10, 0); //index, symbol, x, y, -z, (saved cooridinates in Å = 10 nm, coordinates in program in nm; z-axis inverted from left-handed to right-handed system), atom-atom mapping (0/1)
				atomRefs.Add(atoms[i].GetInstanceID(), i);
			}
			file.WriteLine("M V30   END ATOM");

			file.WriteLine("M V30   BEGIN BOND");

			BondManagerScript bondScript;
			for (int i = 0; i < bonds.Length; i++)
			{
				bondScript = bonds[i].GetComponent<BondManagerScript>();
				//atomRefs.TryGetValue(bondScript._atomEnd.GetInstanceID(), out endAtomRef);

				file.WriteLine("M V30   {0} {1} {2} {3}", i, bondScript.bondMultiplicity, atomRefs[bondScript.atomStart.GetInstanceID()], atomRefs[bondScript.atomEnd.GetInstanceID()]);		//bond index, bond type, index of atom1, index of atom 2
			}

			file.WriteLine("M V30   END BOND");

			file.WriteLine("M V30   END CTAB");
			file.WriteLine("M END");
		}
	}

	public void OpenMolFile(string filepath)		//TODO CHECK FOR INVALID FILES
	{
		using (System.IO.StreamReader file = new System.IO.StreamReader(filepath))
		{
			int molFileVersion = 2;		//MDL Mol file version. 2 = V2000, 3 = V3000. Defaults to V2000 which is more common.

			string line = string.Empty;
			string[] splitLine;
			GameObject atom;

			Dictionary<int, GameObject> atomRefs = new Dictionary<int, GameObject>();

			line = file.ReadLine();     //Read 1st line: Molecule name
			line = file.ReadLine();     //2nd line: user initials (2 char), program name (max 8?), date in MMddYYHHmm, dimensions (2D or 3D)
			line = file.ReadLine();     //3rd: Comments
			line = file.ReadLine();     //4th: Connections table. E.g. V2000: [ 39 42  0  0  0  0  0  0  0  0  0    V2000 ] (39 atoms, 42 bonds, 0 atom lists, an obsolete field, chirality (0/1), 0 stext entries, 4 obsoletes, 0 additional properties lines, MOL version (should be V2000 or V3000, can be anything)
			
			if (line.Contains("V2000"))
				molFileVersion = 2;
			if (line.Contains("V3000"))
				molFileVersion = 3;

			if (molFileVersion == 2)
			{
				int lineNum = 0;

				while (!file.EndOfStream)
				{
					line = file.ReadLine();
					lineNum++;

					splitLine = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

					if (splitLine.Length > 9)		//Atoms block
					{
						Vector3 position = new Vector3(float.Parse(splitLine[0]) / 10, float.Parse(splitLine[1]) / 10, -(float.Parse(splitLine[2]) / 10));      //Divide by 10 to convert from Å to nm; Change z coordinate from rhd to lhd
						atom = atomsBonds.GetComponent<CreateModeScript>().CreateAtom(position + atomsBonds.transform.position, splitLine[3]);
						atomRefs.Add(lineNum, atom);
					}
					else if (splitLine.Length < 9)	//Bonds block
					{
						atomsBonds.GetComponent<CreateModeScript>().CreateBond(atomRefs[(int.Parse(splitLine[0]))], atomRefs[int.Parse(splitLine[1])], int.Parse(splitLine[2]));
					}
				}
			}

			if (molFileVersion == 3)
			{
				while (!line.Contains("BEGIN ATOM"))
					line = file.ReadLine();

				line = file.ReadLine();     //Skip BEGIN ATOM line

				while (!line.Contains("END ATOM"))
				{
					splitLine = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

					Vector3 position = new Vector3(float.Parse(splitLine[4]) / 10, float.Parse(splitLine[5]) / 10, -(float.Parse(splitLine[6]) / 10));		//Divide by 10 to convert from Å to nm
					atom = atomsBonds.GetComponent<CreateModeScript>().CreateAtom(position + atomsBonds.transform.position, splitLine[3]);
					atomRefs.Add(int.Parse(splitLine[2]), atom);

					line = file.ReadLine();
				}

				while (!line.Contains("BEGIN BOND"))
				{
					line = file.ReadLine();
				}

				line = file.ReadLine();

				while (!line.Contains("END BOND"))
				{
					splitLine = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
					atomsBonds.GetComponent<CreateModeScript>().CreateBond(atomRefs[(int.Parse(splitLine[4]))], atomRefs[int.Parse(splitLine[5])], int.Parse(splitLine[3]));

					line = file.ReadLine();

				}
			}

			file.Close();
		}
	}

	public void QuitApplication()
	{
		Application.Quit ();
	}*/
}
