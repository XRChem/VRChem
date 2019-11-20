using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MeasureModeScript : MonoBehaviour {

	//mode for measuring distance, angle and torsion between atoms and bonds
	public MeasureLineRenderScript distance1Object, distance2Object, angleObject; //re do with prefab and instantiate --!
	public GameObject d1Text, d2Text, AngleText;

	public bool measuringAtoms = false, measuringBonds = false; // based on canvas options, so default is free select first object

	private GameObject measureTarget, measureObject1, measureObject2, measureObject3;
	private float distance1, distance2, angle, torsion;
	private bool isMeasuring = false, isMeasuringAtoms;

	//need to restrict selection of adjascent bonds --!

	delegate void SetMeasureTarget();
	SetMeasureTarget measureTargetSet;

	void Start()
	{
		measureTargetSet = SetFirstObject;
	}

	//buttons call these
	public void SetMeasuringAtoms()
	{
		measuringAtoms = true;
		measuringBonds = false;
	}
	public void SetMeasuringBonds()
	{
		measuringAtoms = false;
		measuringBonds = true;
	}
	public void SetFreePickFirstObject()
	{
		measuringAtoms = false;
		measuringBonds = false;
	}
	public void ClearTargets()
	{
		measureObject1.GetComponent<cakeslice.Outline> ().enabled = false;
		measureObject2.GetComponent<cakeslice.Outline> ().enabled = false;
		measureObject3.GetComponent<cakeslice.Outline> ().enabled = false;
		measureObject1 = null; measureObject2 = null; measureObject3 = null;
		measureTargetSet = SetFirstObject;
	}
		
	//Leap Actions call these

	//while pointing

	public void IsMeasuring(bool _pointing)
	{
		isMeasuring = _pointing;
	}

	//object in target range
	public void SetMeasureObject(GameObject _target)
	{
		measureTarget = _target;
		
		if(isMeasuring && measureTargetSet != null) // if pointing while target is in range, select it
			measureTargetSet ();
	}

	//target not in range
	public void ClearMeasureObject()
	{
		measureTarget = null;
	}


	void SetFirstObject()
	{
		if (!measuringAtoms && !measuringBonds)
		{
			measureObject1 = measureTarget;
			measureObject1.GetComponent<cakeslice.Outline> ().enabled = true;
			if (measureTarget.GetComponent<AtomManagerScript> () != null)
			{
				isMeasuringAtoms = true;
				measureTargetSet = SetSecondObject;
			}
			else if (measureTarget.GetComponent<BondManagerScript> () != null) 
			{
				isMeasuringAtoms = false;
				distance1 = measureObject1.transform.localScale.y * 2;
				distance1Object.ShowDistanceSegment (); //change to instantiate
				d1Text.transform.position = measureObject1.transform.position;
				d1Text.GetComponent<Text> ().text = distance1 + "Å";
				measureTargetSet = SetSecondObject;
			}
		}
		else if (measuringAtoms && measureTarget.GetComponent<AtomManagerScript> () != null)
		{
			measureObject1 = measureTarget;
			measureObject1.GetComponent<cakeslice.Outline> ().enabled = true;
			measureTargetSet = SetSecondObject;
		}
		else if (measuringBonds && measureTarget.GetComponent<BondManagerScript> () != null)
		{
			measureObject1 = measureTarget;
			measureObject1.GetComponent<cakeslice.Outline> ().enabled = true;
			distance1 = measureObject1.transform.localScale.y * 2;

			distance1Object.ShowDistanceSegment (); //change to instantiate
			d1Text.transform.position = measureObject1.transform.position;
			d1Text.GetComponent<Text>().text = distance1 + "Å";
			measureTargetSet = SetSecondObject;
		}
	}

	void SetSecondObject()
	{
		//if second object is also atom, show distance 1
		if (((!measuringAtoms && !measuringBonds && isMeasuringAtoms) || measuringAtoms) && measureTarget.GetComponent<AtomManagerScript>() != null && measureTarget != measureObject1)
		{
			measureObject2 = measureTarget;
			measureObject2.GetComponent<cakeslice.Outline> ().enabled = true;
			distance1 = (measureObject2.transform.position - measureObject1.transform.position).magnitude;
			distance1Object.ShowDistanceSegment (measureObject1, measureObject2);
			d1Text.transform.position = measureObject1.transform.position + (measureObject2.transform.position - measureObject1.transform.position) / 2;
			d1Text.GetComponent<Text> ().text = distance1 + "Å";
			measureTargetSet = SetThirdObject;
		}
		//if second object is also bond show angle
		else if (((!measuringAtoms && !measuringBonds && !isMeasuringAtoms) || measuringBonds ) && measureTarget.GetComponent<BondManagerScript> () != null && measureTarget != measureObject1)// && measureTarget.GetComponent<cakeslice.Outline>().enabled)
		{
			measureObject2 = measureTarget;
			measureObject2.GetComponent<cakeslice.Outline> ().enabled = true;
			//figure out central atom and then display correct angle
			GameObject atom1, atom2, atom3, atom4;
			measureObject1.GetComponent<BondManagerScript> ().GiveBondAtoms (out atom1, out atom2);
			measureObject2.GetComponent<BondManagerScript> ().GiveBondAtoms (out atom3, out atom4);
			if (atom1 == atom3) {
				angle = Vector3.Angle (atom2.transform.position - atom1.transform.position, atom4.transform.position - atom3.transform.position);
				//angleObject.ShowAngleArc (atom1, atom2, atom4, angle);
				AngleText.transform.position = atom1.transform.position;
				AngleText.GetComponent<Text> ().text = angle + " deg";
			} else if (atom2 == atom3) {
				angle = Vector3.Angle (atom1.transform.position - atom2.transform.position, atom4.transform.position - atom3.transform.position);
				//angleObject.ShowAngleArc (atom2, atom1, atom4, angle);
				AngleText.transform.position = atom2.transform.position;
				AngleText.GetComponent<Text> ().text = angle + " deg";
			} else if (atom1 == atom4) {
				angle = Vector3.Angle (atom2.transform.position - atom1.transform.position, atom3.transform.position - atom4.transform.position);
				//angleObject.ShowAngleArc (atom1, atom2, atom3, angle);
				AngleText.transform.position = atom1.transform.position;
				AngleText.GetComponent<Text> ().text = angle + " deg";
			} else if (atom2 == atom4) {
				angle = Vector3.Angle (atom1.transform.position - atom2.transform.position, atom3.transform.position - atom4.transform.position);
				//angleObject.ShowAngleArc (atom2, atom1, atom3, angle);
				AngleText.transform.position = atom2.transform.position;
				AngleText.GetComponent<Text> ().text = angle + " deg";
			} else {
				print ("bonds not directly related");
			}
			// also show the second distance
			distance2 = measureObject2.transform.localScale.y * 2;
			distance2Object.ShowDistanceSegment ();
			d2Text.transform.position = measureObject2.transform.position;
			d2Text.GetComponent<Text>().text = distance1 + "Å";
			measureTargetSet = null;
		}
	}

	void SetThirdObject()
	{
		//3rd object also atom, show line and and angle
		if (measureTarget.GetComponent<AtomManagerScript> () != null && (measuringAtoms || isMeasuringAtoms))
		{
			measureObject3 = measureTarget;
			measureObject3.GetComponent<cakeslice.Outline> ().enabled = true;

			distance2 = (measureObject3.transform.position - measureObject2.transform.position).magnitude;
			angle = Vector3.Angle (measureObject1.transform.position - measureObject2.transform.position, measureObject3.transform.position - measureObject2.transform.position);

			distance2Object.ShowDistanceSegment (measureObject2, measureObject3);
			//angleObject.ShowAngleArc (measureObject2, measureObject1, measureObject3, angle);

			d2Text.transform.position = measureObject2.transform.position + (measureObject3.transform.position - measureObject2.transform.position)/2;
			d2Text.GetComponent<Text>().text = distance1 + "Å";
			AngleText.transform.position = measureObject2.transform.position;
			AngleText.GetComponent<Text>().text = angle + " deg";
			measureTargetSet = null;
		}
	}

}
