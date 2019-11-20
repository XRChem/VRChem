using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class MeasureLineRenderScript : MonoBehaviour {
	private int circleSegments = 50; //segments to make whole circle, it will not all be drawn, only an arc will be drawn with as many segments as required
	private float circleRadius = 0.05f, startingAngle, endingAngle;
	private LineRenderer line;
	public float arcAngle;

	void Awake()
	{
		line = GetComponent<LineRenderer> ();
	}

	public void ShowAngleArc(Vector3 startArc, Vector3 endArc)
	{
		//rotate to plane of 3 atoms - 2 bonds
		Vector3 crossPerp = Vector3.Cross ((startArc - transform.position), (endArc - transform.position)).normalized;
		transform.rotation = Quaternion.LookRotation(startArc - transform.position, crossPerp);
		arcAngle = Vector3.Angle (startArc - transform.position, endArc - transform.position);
		//draw arc
		float x, z;
		float drawAngle = 0f;
		int drawnSegments = (int)((arcAngle / 360) * circleSegments);
		//line.SetVertexCount(drawnSegments + 1);
		line.positionCount = drawnSegments + 1;
		for (int i = 0; i <= drawnSegments; i++)
		{
			x = Mathf.Sin (Mathf.Deg2Rad * drawAngle) * circleRadius;
			z = Mathf.Cos (Mathf.Deg2Rad * drawAngle) * circleRadius;

			line.SetPosition (i, new Vector3(x * 10, 0, z * 10));
			drawAngle += (arcAngle / drawnSegments);
		}
	}

	public void ShowAngleArc(float angle, Vector3 axis)
	{
		arcAngle = angle;
		//draw arc
		transform.up = axis;
		float x, z;
		float drawAngle = 0f;
		int drawnSegments = (int)((Mathf.Abs(angle) / 360) * circleSegments);
		//line.SetVertexCount(drawnSegments + 1);
		line.positionCount = drawnSegments + 1;
		for (int i = 0; i <= drawnSegments; i++)
		{
			x = Mathf.Sin (Mathf.Deg2Rad * drawAngle) * circleRadius;
			z = Mathf.Cos (Mathf.Deg2Rad * drawAngle) * circleRadius;

			line.SetPosition (i, new Vector3(x*10, 0, z*10) ); 
			drawAngle += (angle / drawnSegments);
		}
	}

	public void ShowDistanceSegment(GameObject atom1, GameObject atom2)
	{
		//draw segment
		//line.SetVertexCount (2);

		line.positionCount = 2;
		line.SetPosition (0, atom1.transform.position);
		line.SetPosition (1, atom2.transform.position);
	}

	public void ShowDistanceSegment()
	{
		//draw segment
		//line.SetVertexCount (2);
		line.positionCount = 2;
		line.SetPosition (0, new Vector3(0, 1, 0) );
		line.SetPosition (1, new Vector3(0, -1, 0) );
	}
}
