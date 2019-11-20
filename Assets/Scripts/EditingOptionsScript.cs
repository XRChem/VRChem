using UnityEngine;
using System.Collections;

public class EditingOptionsScript : MonoBehaviour {
	/* this doesn't yet allow dynamic change of options because there is no callback function for changes to public variables
	 * from editor. I do not want to use an update function just for this. It will work with the UI Canvas elements	*/

	//if bond angles need to be calculated, we must prevent extra bonds
	//saturtion only when created, after that, user can delete bonds
	public bool saturateNewAtoms = true, calculateBondAngles = true, continuousCalculations = false, preventExtraBonds = true,
	allowFewerBonds = true, snappingAngles = false;
}
