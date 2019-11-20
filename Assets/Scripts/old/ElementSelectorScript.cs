using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementSelectorScript : MonoBehaviour {

	//this will move to the CreateModeScript

	//private static string creationElement = "C"; //this is the element that is chosen for creation
	public int CreationElementNumber = 6;				//Changed to using element numbers instead of symbols

	/*public void SetElement(Text elementName) //when the toggle buttons are pressed, they change the static variable
	{
		creationElement = elementName.text;
	}*/

	public void SetElement (int elemNum) {
		CreationElementNumber = elemNum;
	}

	/*public void GetProperties(out Color _color, out float _size, out string _elementSymbol, out int _elementNumber)
	{
		ChemElement ElementData = gameObject.GetComponent<ElementDataProviderScript> ().GetElementData (CreationElementNumber);

		_size = 1;
		_elementSymbol = ElementData.Symbol;
		_elementNumber = CreationElementNumber;
		_color = Color.magenta;

		if (ElementData.ColorR != null && ElementData.ColorG != null && ElementData.ColorB != null && ElementData.ColorA != null) {
			Color temp = new Color ((float)ElementData.ColorR, (float)ElementData.ColorG, (float)ElementData.ColorB, (float)ElementData.ColorA);

			bool UpperLimit = (temp.r <= 1 && temp.g <= 1 && temp.b <= 1 && temp.a <= 1);
			bool LowerLimit = (temp.r >= 0 && temp.g >= 0 && temp.b >= 0 && temp.a >= 0);

			if (UpperLimit && LowerLimit) {
				_color = temp;
			} 
		}*/


		/*_color.r = (float) ElementData.ColorR;
		_color.g = (float) ElementData.ColorG;
		_color.b = (float) ElementData.ColorB;
		_color.a = (float) ElementData.ColorA;*/

		/*switch (creationElement)
		{
			case "C":
			_color = Color.black;	
			break;
			case "H":
			_color = Color.white;
			break;
			case "N":
			_color = Color.blue;
			break;
			case "O":
			_color = Color.red;
			break;
			default:
			_color = Color.cyan;
			break;
		}*/
	//}
}