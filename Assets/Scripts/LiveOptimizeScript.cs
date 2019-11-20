using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveOptimizeScript : MonoBehaviour {

	public OpenBabelScript openBabelScript;
	public bool OptimizeToggle, midCreation, moleculeChanged;

	// Use this for initialization
	void Start () {
		OptimizeToggle = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (OptimizeToggle && !midCreation && transform.childCount > 1)
		{
			if (moleculeChanged)
			{
				StartCoroutine(openBabelScript.Optimize(1));
				moleculeChanged = false;
			}
			else
				StartCoroutine(openBabelScript.OptimizeNoInitialization());
		}
	}

	public void ToggleOptimizeToggle()
	{
		if (OptimizeToggle == false)
			OptimizeToggle = true;
		else if (OptimizeToggle == true)
			OptimizeToggle = false;
	}
}