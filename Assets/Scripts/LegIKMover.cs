using System;
using UnityEngine;

public sealed class LegIKMover : MonoBehaviour
{
	[SerializeField] float moveDistance;
	[SerializeField] LayerMask groundLayer;
	[SerializeField] Transform limbSolverTarget;

	const float NORMAL_SCALE_MULTIPLIER = .1f;
	const float GROUND_CHECK_RAY_DISTANCE = 5f;
	
	public void CheckGround()
	{
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, GROUND_CHECK_RAY_DISTANCE, groundLayer);
		if (hit.collider == default) return;
		
		Vector2 point = hit.point + hit.normal * NORMAL_SCALE_MULTIPLIER;
		Debug.DrawLine(hit.point, point, Color.red);
		transform.position.Set(point.x, point.y, 0f);
	}
	
	void Update()
	{
		CheckGround();

		if (Vector2.Distance(limbSolverTarget.position, transform.position) <= moveDistance) return;
		limbSolverTarget.position = transform.position;
	}
}