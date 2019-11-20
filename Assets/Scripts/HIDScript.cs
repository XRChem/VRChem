using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VRTK;

public class HIDScript : MonoBehaviour {

	public GameObject leapRigCamera, leapParentObject, viveCamera, viveParentObject, vrCanvas;

	public enum HID
	{
		OpenVR = 0,
		LeapMotion,
		Keyboard
	};

	public HID selectedHID;

	public void SelectHIDWrapper(int newSelectedHID)
	{
		StartCoroutine(SelectHID(newSelectedHID));
	}

	public IEnumerator SelectHID(int newSelectedHID)
	{
		if (newSelectedHID != (int)selectedHID)
		{
			if (newSelectedHID == (int)HID.LeapMotion)
			{
				if (viveParentObject.activeInHierarchy)
				{
					vrCanvas.GetComponent<VRTK_UICanvas>().enabled = false;
					vrCanvas.GetComponent<Canvas>().worldCamera = leapRigCamera.GetComponent<Camera>();
					//leapParentObject.transform.position = viveParentObject.transform.position;
					viveParentObject.SetActive(false);
					yield return new WaitForEndOfFrame();
					leapParentObject.SetActive(true);

					GameObject ViveEventSystem = viveParentObject.transform.Find("EventSystem").gameObject;
					if (ViveEventSystem.GetComponent<EventSystem>() != null && ViveEventSystem.GetComponent<EventSystem>() != ViveEventSystem.GetComponent<VRTK_EventSystem>())
					{
						Destroy(ViveEventSystem.GetComponent<EventSystem>());
					}
					//UIManager.GetComponent<UIManagerScript>().SetMode(BuildToggle);
				}
				selectedHID = HID.LeapMotion;
			}
			else if (newSelectedHID == (int)HID.OpenVR)
			{
				if (leapParentObject.activeInHierarchy)
				{
					//viveParentObject.transform.position = leapParentObject.transform.position;
					leapParentObject.SetActive(false);
					yield return new WaitForEndOfFrame();
					viveParentObject.SetActive(true);
					gameObject.GetComponent<Canvas>().worldCamera = viveCamera.GetComponent<Camera>();
					gameObject.GetComponent<VRTK_UICanvas>().enabled = true;

					GameObject.Find("Pointer 0").SetActive(false);
				}
				selectedHID = HID.OpenVR;
			}
		}
	}
}
