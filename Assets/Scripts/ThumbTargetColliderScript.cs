using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThumbTargetColliderScript : MonoBehaviour {

	public LeapEventDelegatorScript leapEventDelegatorScript;

	void OnTriggerEnter(Collider other)
	{
		leapEventDelegatorScript.RightSetThumbTarget (other.gameObject);
	}

	public void DeleteColliderResize(Slider _delSlider)
	{
		GetComponent<BoxCollider> ().transform.localScale = new Vector3(0.1f * _delSlider.value, 0.1f * _delSlider.value, 0.1f * _delSlider.value);
	}

	public void Show()
	{
		gameObject.SetActive (true);
	}
	public void Hide()
	{
		gameObject.SetActive (false);
	}
}
