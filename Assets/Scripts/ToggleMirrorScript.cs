using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleMirrorScript : MonoBehaviour {

	private Toggle thisToggle;

	void Start()
	{
		thisToggle = GetComponent<Toggle> ();
	}

	public void RemoteToggle()
	{
		thisToggle.isOn = !thisToggle.isOn;
	}
}
