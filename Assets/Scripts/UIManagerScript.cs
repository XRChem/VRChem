using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript: MonoBehaviour {

	//public delegate void MC(int _m); //Mode Change
	//public static event MC modeChange;

	//Canvas Elements to toggle visibility - set from editor
	public GameObject MenuToggle, BuildToggle, MeasureToggle, SelectToggle, PeriodicTablePanel,  PeriodicTableVRPanel;
	//to control which is active within script
	private GameObject activePanel, activeVrPanel;
	private int interactMode;
	private bool menuOn;

	//options that can be checked in other scripts while creating, or rotating, etc
	public static bool autoSaturation, rotationSnapping; // stretchAlongBond;

	void Start()
	{
		//SetMode (BuildToggle); //starting mode, and options
		menuOn = false;
		autoSaturation = true;
		rotationSnapping = true;
	}

	/*public void SetMenu(GameObject _selfObject)
	{
		menuOn = !menuOn;
		LinkedPanelScript tempPanel = _selfObject.GetComponent<LinkedPanelScript> ();
		tempPanel.linkedPanel.SetActive (!tempPanel.linkedPanel.activeSelf);
	}*/

	//toggling modes from canvas buttons
	/*public void SetMode(GameObject _selfObject)
	{
		LinkedPanelScript tempPanel = _selfObject.GetComponent<LinkedPanelScript> ();
		if(_selfObject.GetComponent<Toggle>().isOn && tempPanel.modeNum != interactMode)
		{
			interactMode = tempPanel.modeNum;// prevent toggle loop

			tempPanel.linkedToggle.isOn = !tempPanel.linkedToggle.isOn;

			if (activePanel != null)
				activePanel.SetActive (false);
			if (activeVrPanel != null)
				activeVrPanel.SetActive (false);
			PeriodicTablePanel.SetActive (false);
			PeriodicTableVRPanel.SetActive (false);
			if (tempPanel.linkedPanel != null) {
				tempPanel.linkedPanel.SetActive (true);
				activePanel = tempPanel.linkedPanel;
			}
			if (tempPanel.linkedVrPanel != null) {
				tempPanel.linkedVrPanel.SetActive (true);
				activeVrPanel = tempPanel.linkedVrPanel;
			}

			//broadcast mode change
			if (modeChange != null)
				modeChange(interactMode);
		}
	}*/

	//build menu options

	public void SetAutosaturation(GameObject _selfToggleObject)
	{
		if(_selfToggleObject.GetComponent<Toggle>().isOn != autoSaturation)
		{
			autoSaturation = !autoSaturation;
			_selfToggleObject.GetComponent<LinkedPanelScript> ().linkedToggle.isOn = !_selfToggleObject.GetComponent<LinkedPanelScript> ().linkedToggle.isOn;
		}
	}

	public void SetRotationSnapping(GameObject _selfToggleObject)
	{
		if(_selfToggleObject.GetComponent<Toggle>().isOn != rotationSnapping)
		{
			rotationSnapping = !rotationSnapping;
			_selfToggleObject.GetComponent<LinkedPanelScript> ().linkedToggle.isOn = !_selfToggleObject.GetComponent<LinkedPanelScript> ().linkedToggle.isOn;
		}
	}

	/*public void SetBondStretch(GameObject _selfToggleObject)
	{
		if(_selfToggleObject.GetComponent<Toggle>().isOn)
		{
			if (_selfToggleObject.GetComponent<LinkedPanelScript> ().modeNum == 1)
				stretchAlongBond = true;
			else
				stretchAlongBond = false;
			_selfToggleObject.GetComponent<LinkedPanelScript> ().linkedToggle.isOn = !_selfToggleObject.GetComponent<LinkedPanelScript> ().linkedToggle.isOn;
		}
	}*/

	//measure menu options

	//select menu options

	//open periodic table
	public void OpenPeriodicTable(GameObject _self) //a negative bool will close it
	{
		LinkedPanelScript linked = _self.GetComponent<LinkedPanelScript> ();
		linked.linkedPanel.SetActive (true);
		_self.transform.parent.gameObject.SetActive (false); //this only works because the 'periodic table' button and the 'X' close button in the periodic table are direct children of the panel that should be hidden
	}

	public void ShowHidePanel (GameObject panel)		//Toggle panel visibility
	{
		if (!panel.activeInHierarchy)
			panel.SetActive(true);
		else
			panel.SetActive(false);
	}
}
