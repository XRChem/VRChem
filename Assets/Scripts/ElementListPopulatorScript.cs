using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
//using FileHelpers;
using UnityEngine.UI;

using LumenWorks.Framework.IO.Csv;

public class ElementListPopulatorScript : MonoBehaviour {

	public ElementDataProviderScript elementDataProviderScript;
	public GameObject ElementTogglePrefab;

	private List<int> results;
	private bool elementsLoaded = false;

	void OnEnable()
	{
		ElementDataProviderScript.dgd += ElementsAreLoaded;
	}
	void ElementsAreLoaded()
	{
		elementsLoaded = true;
		ElementDataProviderScript.dgd -= ElementsAreLoaded;
	}

	void Start () {
		results = new List<int>();

		using (CsvReader csv = new CsvReader(new StreamReader(Application.streamingAssetsPath + "/favourite_elements.csv"), true))
		{
			while (csv.ReadNextRecord())
			{
				int result = new int();
				result = int.Parse(csv[0]);
				results.Add(result);
			}
		}

		//var engine = new FileHelperEngine<FavElements> ();
		StartCoroutine (PopulateList ());
	}

	IEnumerator PopulateList()
	{
		while (!elementsLoaded) //waits for the element data provider script to get all data before setting element symbols
			yield return null;
		
		//bool first = true;
		foreach(var fav in results)
		{
			GameObject createdToggle = Instantiate (ElementTogglePrefab, transform) as GameObject;
			createdToggle.GetComponent<ElementToggleScript>().elementDataProviderScript = elementDataProviderScript;
			createdToggle.GetComponent<ElementToggleScript> ().SetElement (fav);
			createdToggle.GetComponent<Toggle> ().group = transform.parent.GetComponent<ToggleGroup> ();
			if (fav == 6) //default start with carbon
			{
				createdToggle.GetComponent<Toggle> ().isOn = true;
				//first = false;
			}
			//Debug.Log ("Favourite Element: " + fav.ElementName + ", " + fav.AtomicNumber);
		}
	}

	/*public void AddToFavourites()		NOT USED ATM
	{
		//check if already in favourites
		bool alreadyInFavs = false;
		foreach(var fav in results)
		{
			if (fav == elementDataProviderScript.SelectedElementNum)
			{
				alreadyInFavs = true;
				break;
			}
		}

		if(!alreadyInFavs)
		{
			//add to UI immediately and
			GameObject createdToggle = Instantiate (ElementTogglePrefab, transform) as GameObject;
			createdToggle.GetComponent<ElementToggleScript> ().SetElement (elementDataProviderScript.SelectedElementNum);
			createdToggle.GetComponent<Toggle> ().group = transform.parent.GetComponent<ToggleGroup> ();
			//append to favourtie_elements file
			FavElements newFav = new FavElements ();
			newFav.AtomicNumber = elementDataProviderScript.SelectedElementNum;
			//var engine = new FileHelperEngine<FavElements> ();
			//engine.AppendToFile ("Assets/favourite_elements.csv", newFav);
		}
	}*/
}