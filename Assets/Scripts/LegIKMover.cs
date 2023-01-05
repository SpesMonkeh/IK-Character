using UnityEngine;

public sealed class LegIKMover : MonoBehaviour
{
	[SerializeField] int isGrounded;
	[SerializeField] int positionIndex;
	[SerializeField] float liftDistance;
	[SerializeField] float moveThreshold = .5f;
	[SerializeField] float limbMoveSpeed = 1f;
	[SerializeField] Vector3 halfwayPoint;
	[SerializeField] Vector3 targetPoint;
	[SerializeField] LayerMask groundLayer;
	[SerializeField] Transform limbTarget;
	[SerializeField] LegIKMover oppositeLimb;

	RaycastHit2D groundCheckHit;

	const float NORMAL_SCALE_MULTIPLIER = .1f;
	const float GROUND_CHECK_RAY_DISTANCE = 5f;

	public bool IsGrounded => isGrounded is 1;
	public Vector3 Position => transform.position;
	
	public void CheckGround()
	{
		groundCheckHit = Physics2D.Raycast(transform.position, Vector2.down, GROUND_CHECK_RAY_DISTANCE, groundLayer);
		if (groundCheckHit.collider == default) return;
		
		Vector2 point = groundCheckHit.point + groundCheckHit.normal * NORMAL_SCALE_MULTIPLIER;
		Debug.DrawLine(groundCheckHit.point, point, Color.red);
		transform.position.Set(point.x, point.y, 0f);
	}
	
	void Update()
	{
		CheckGround();
		DetermineMovement();
		CheckIfGrounded();
	}

	void DetermineMovement()
	{
		Vector3 tfPos = transform.position;
		Vector3 limbTargetPos = limbTarget.position;
		
		switch (positionIndex)
		{
			case 0 when oppositeLimb.IsGrounded && Vector3.Distance(limbTargetPos, tfPos) > moveThreshold:
				targetPoint = tfPos;
				halfwayPoint = (targetPoint + limbTargetPos) * .5f;
				halfwayPoint.y += liftDistance;
				positionIndex = 1;
				break;
			case 1:
				UpdatePositionValues(ref limbTargetPos, halfwayPoint);
				break;
			case 2:
				UpdatePositionValues(ref limbTargetPos, targetPoint);
				break;
		}

		limbTarget.position = limbTargetPos;
	}

	void UpdatePositionValues(ref Vector3 limbTargetPos, Vector3 nextMovePoint)
	{
		const int max_pos_index = 2;
		const float next_pos_dist = .1f;
		int nextPosIndex = positionIndex >= max_pos_index ? 0 : positionIndex + 1;
		limbTargetPos = Vector3.Lerp(limbTargetPos, nextMovePoint, limbMoveSpeed * Time.deltaTime);
		if (Vector3.Distance(limbTargetPos, nextMovePoint) <= next_pos_dist)
			positionIndex = nextPosIndex;
	}
	
	void CheckIfGrounded() => isGrounded = positionIndex is 0 && groundCheckHit.transform != default ? 1 : 0;
}