using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextFaceCameraScript : MonoBehaviour {

	private Text measureText; Vector3 pos;
	Transform detachedParent;
	void Start()
	{
		detachedParent = transform.parent;
		transform.SetParent(null);
		transform.localScale = Vector3.one * 0.07f;
		measureText = GetComponentInChildren<Text> ();
	}
	// Update is called once per frame
	void Update (){
		if (detachedParent.GetComponent<BondManagerScript> () != null)
		{
			measureText.text = (detachedParent.localScale.y * 20).ToString("F2") + " Å";
		}
		else
		{
			measureText.text = detachedParent.GetComponentInChildren<MeasureLineRenderScript> ().arcAngle.ToString("F1") + "\u00B0";
		}

		transform.position = detachedParent.position;
		transform.forward = transform.position - Camera.main.transform.position;
	}
}