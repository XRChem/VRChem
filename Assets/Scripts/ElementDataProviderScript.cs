using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using LumenWorks.Framework.IO.Csv;

public class ElementDataProviderScript : MonoBehaviour {

	public int SelectedElementNum { get; set; }
	private List<ChemElement> elementData;

	public delegate void DoneGettingData();
	public static event DoneGettingData dgd;

	void Start () {
		SelectedElementNum = 6;					//Carbon selected at startup

		elementData = new List<ChemElement>();

		using (CsvReader csv = new CsvReader(new StreamReader(Application.streamingAssetsPath + "/element_properties.csv"), true))		//Read atom data from CSV to a list<ChemElement>
		{
			while (csv.ReadNextRecord())
			{
				int parseResult = 0;

				ChemElement result = new ChemElement();
				result.AtomicNum = int.Parse(csv[0]);
				result.Name = csv[1];
				result.Symbol = csv[2];
				result.covRadiusSingle = int.Parse(csv[3]);
				result.covRadiusDouble = (int.TryParse(csv[4], out parseResult) == true) ? parseResult : (int?)null; 
				result.covRadiusTriple = (int.TryParse(csv[5], out parseResult) == true) ? parseResult : (int?)null;
				result.atomSize = float.Parse(csv[6]);
				result.VSEPR_X = (int.TryParse(csv[7], out parseResult) == true) ? parseResult : (int?)null;
				result.VSEPR_E = (int.TryParse(csv[8], out parseResult) == true) ? parseResult : (int?)null;
				result.ColorR = (int.TryParse(csv[9], out parseResult) == true) ? parseResult : (int?)null;
				result.ColorG = (int.TryParse(csv[10], out parseResult) == true) ? parseResult : (int?)null;
				result.ColorB = (int.TryParse(csv[11], out parseResult) == true) ? parseResult : (int?)null;
				result.ColorA = (int.TryParse(csv[12], out parseResult) == true) ? parseResult : (int?)null;

				elementData.Add(result);
			}
		}

		foreach (ChemElement elem in elementData)
		{
			elem.Name = elem.Name.Trim();
			elem.Symbol = elem.Symbol.Trim();
		}

		if (dgd != null)
			dgd (); //sending event that tells elements that data is ready.
	}

	public void SelectElement (int elemNum) {
		SelectedElementNum = elemNum;
	}

	public ChemElement GetElementData ()  {
		if (SelectedElementNum< 1 || SelectedElementNum > elementData.Count) {
			return null;
		}

		return elementData [SelectedElementNum - 1];
	}

	public ChemElement GetElementData(int atomicNum)
	{
		if (atomicNum < 1 || atomicNum > elementData.Count)
		{
			return null;
		}

		return elementData[atomicNum - 1];
	}

	/*public ChemElement GetElementData(string element)
	{
		foreach (ChemElement line in elementData)
		{
			if (line.Name == element || line.Symbol == element)
				return line;
		}

		return null;
	}*/
}

public class ChemElement {
	public int AtomicNum;

	public string Name;

	public string Symbol;

	public int covRadiusSingle;		//Covalent radii in single/double/triple bonds
	public int? covRadiusDouble;	//int? = nullable int, value might not be present in data table
	public int? covRadiusTriple;

	public float atomSize;

	public int? VSEPR_X;
	public int? VSEPR_E;

	public int? ColorR;
	public int? ColorG;
	public int? ColorB;
	public int? ColorA;
}