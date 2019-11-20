using UnityEngine;
using System.Collections;

public class IndexTargetColliderScript : MonoBehaviour {

	public LeapEventDelegatorScript leapEventDelegatorScript;

	void OnTriggerEnter(Collider other)
	{
		leapEventDelegatorScript.RightSetIndexTarget (other.gameObject);
	}
}