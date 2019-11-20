using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToCamera : MonoBehaviour
{
    private int type;   //Super quick fix, 0 = atom, 1 = bond
    private Camera mainCamera;

    private void OnEnable()
    {
        mainCamera = Camera.main;
        if (transform.parent.GetComponent<AtomManagerScript>() != null)
        {
            type = 0;
            transform.LookAt(mainCamera.transform);
        }
        else
        {
            type = 1;
            transform.LookAt(mainCamera.transform, transform.parent.up);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (type == 0)
        {
            transform.LookAt(mainCamera.transform);
        }
        else
        {
            transform.LookAt(mainCamera.transform, transform.parent.up);
        }
    }
}
