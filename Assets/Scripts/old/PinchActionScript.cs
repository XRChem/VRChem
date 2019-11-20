/*using UnityEngine;
using System.Collections;

public class PinchActionScript : MonoBehaviour {

	private Rigidbody pinchPointRigidBody;
	private GameObject _target, initialAtom, createdAtom, createdBond;

	public GameObject atomPrefab, bondPrefab;

	private bool pinching = false, validTarget = false, creating = false, replacingOrBonding = false, bondingNewAtom = false, bondingOldAtoms = false;
	//delegate void DelegatePinch();
	//DelegatePinch startPinch, endPinch;

	//set it up so that pinch functions are assigned to different functions depending whether we are in creation, selection, movement, or measurement mode
	//and depending on which hand is being used

	void Awake()
	{
		pinchPointRigidBody = GetComponent<Rigidbody> ();		
	}

	//when hand approaches object it is set as a target, there is a valid target
	public void SetTarget(GameObject target)
	{
		if (_target == null)
		{
			_target = target;
			validTarget = true;
			//print ("valid pinch target");
		}
	}

	//when hand moves away, it set target to null, no valid target
	public void ClearTarget()
	{
		_target = null;
		validTarget = false;
		//print ("no more pinch target");
	}
		
	//called by grab detection (pinch + 3 extended fingers)
	public void PinchAction()
	{
		pinching = true;

		if (validTarget)
		{
			//starting on valid target atom -- if release here, replace the target; if move, create new atom with bond to the target
			replacingOrBonding = true;
			initialAtom = _target;
			//draw line from target atom to pinch position. GL_LINES?? IEnumerator??
		}
		else 
		{
			//starting on blank, so create atom here and fix joint, release on pinch release
			//print("creating new atom");
			creating = true;
			createdAtom = Instantiate (atomPrefab, transform.position, Quaternion.identity) as GameObject;
			createdAtom.transform.position = transform.position;
			createdAtom.GetComponent<FixedJoint> ().connectedBody = pinchPointRigidBody;
		}
	}

	//Release Action
	public void ReleaseAction()
	{
		//release free floating created atom
		if (creating && createdAtom.GetComponent<FixedJoint>().connectedBody == pinchPointRigidBody) 
		{
			createdAtom.GetComponent<FixedJoint> ().connectedBody = null;
			createdAtom.GetComponent<AtomManagerScript> ().CheckSaturation ();
			creating = false;
		}

		else if (replacingOrBonding && initialAtom != null) 
		{
			if (validTarget && initialAtom != _target) //release on another atom
			{
				//create bond between them --TODO check if there is no bond already
				//print("bonding atoms");
				createdBond = Instantiate (bondPrefab, transform.position, Quaternion.identity) as GameObject;
				createdBond.GetComponent<BondManagerScript>().UpdateBond(initialAtom, _target);
			}
			else if(validTarget && initialAtom == _target)// releases on same atom
			{
				//replace atom TODO replace with different element atom
				createdAtom = Instantiate(atomPrefab, _target.transform.position, Quaternion.identity) as GameObject;
				createdAtom.GetComponent<AtomManagerScript> ().InheritBonds (_target);
				GameObject.Destroy (_target);
				createdAtom.GetComponent<AtomManagerScript> ().UpdateBonds ();
				createdAtom.GetComponent<AtomManagerScript> ().CheckSaturation ();
			}
			else if (!validTarget) //release on empty space
			{
				//create new atom
				//print("creating new BONDED atom");
				createdAtom = Instantiate(atomPrefab, transform.position, Quaternion.identity) as GameObject;

				//create bond between them
				createdBond = Instantiate (bondPrefab, transform.position, Quaternion.identity) as GameObject;
				createdBond.GetComponent<BondManagerScript>().UpdateBond(initialAtom, createdAtom);
				createdAtom.GetComponent<AtomManagerScript> ().CheckSaturation ();
			}

			replacingOrBonding = false;
			initialAtom = null; //resets actions so that bonds find new initial points
		}
			
	}

}
*/