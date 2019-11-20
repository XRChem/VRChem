using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementToggleScript : MonoBehaviour {

	public ElementDataProviderScript elementDataProviderScript;

	private GameObject InteractionModeObject;
	public int atomicNumber;
	public Text symbolText;

	void Start()
	{
		int tempAtNu;
		if (int.TryParse (gameObject.name.ToString (), out tempAtNu))
			atomicNumber = tempAtNu;
		
		if (atomicNumber != 0)
			SetElement (atomicNumber);
	}

	public void SetElement(int _atomicNumber) //use while constructing from favourites
	{
		atomicNumber = _atomicNumber;
		symbolText.text = (elementDataProviderScript.GetElementData (atomicNumber)).Symbol;
	}

	public void ToggleElement(Toggle _selfToggle)
	{
		if (_selfToggle.isOn && elementDataProviderScript.SelectedElementNum != atomicNumber)
			elementDataProviderScript.SelectElement (atomicNumber);
	}
}
