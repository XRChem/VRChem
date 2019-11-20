using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventListener_Leap : MonoBehaviour {

	public LeapEventDelegatorScript leapEventDelegatorScript;
	public CreateModeScript createModeScript;
	public HandleScript handleScript;

	//activate listeners
	/*void OnEnable()
	{
		leapEventDelegatorScript.rpStart += createModeScript.StartCreate;
		leapEventDelegatorScript.rpEnd += createModeScript.EndCreate;
		leapEventDelegatorScript.lgStart += handleScript.StartRotate;
		leapEventDelegatorScript.lgEnd += handleScript.EndRotate;

	}
	void OnDisable()
	{
		leapEventDelegatorScript.rpStart -= createModeScript.StartCreate;
		leapEventDelegatorScript.rpEnd -= createModeScript.EndCreate;
		leapEventDelegatorScript.lgStart -= handleScript.StartRotate;
		leapEventDelegatorScript.lgEnd -= handleScript.EndRotate;
	}*/

	//when mode is changed
	/*void ModeListener(int _mode)
	{
		Debug.Log("TEST");

		if (_mode == thisMode)
		{
			interactionModeScript.rpStart += createModeScript.StartCreate;
			interactionModeScript.rpEnd += createModeScript.EndCreate;
			//activeMode = true;
		}
		else
		{
			interactionModeScript.rpStart -= createModeScript.StartCreate;
			interactionModeScript.rpEnd -= createModeScript.EndCreate;
			//activeMode = false;
		}
	}*/
}