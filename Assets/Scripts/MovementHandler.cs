using System.Linq;
using UnityEngine;

public sealed class MovementHandler : MonoBehaviour
{
	[SerializeField] float moveSpeed = 2f;
	[SerializeField] float currentSpeed;
	[SerializeField] float yOffset;
	[SerializeField] float offsetTimer;
	[SerializeField] float groundCheckDistance = 1.5f;
	[SerializeField] float breathingSpeed = 1f;
	[SerializeField] float breathingHeight = .5f;
	[SerializeField] LayerMask groundLayer;
	[SerializeField] Transform rayPoint;
	[SerializeField] Rigidbody2D rigidBody;
	[SerializeField] LegIKMover[] legTargets;

	int isGrounded;
	
	const string HORIZONTAL = "Horizontal";

	void Awake()
	{
		rigidBody = GetComponentInParent<Rigidbody2D>();
	}

	void Update()
	{
		currentSpeed = moveSpeed * Input.GetAxis(HORIZONTAL);

		Transform tf = transform;
		Vector3 tfPos = tf.position;
		tfPos.x += currentSpeed * Time.deltaTime;
		tf.position = tfPos;
		
		CalculateGround();
		Idle();
	}

	void FixedUpdate()
	{
		if (legTargets.Any(leg => leg.IsGrounded))
		{
			rigidBody.constraints |= RigidbodyConstraints2D.FreezePositionY;
		}
		else
		{
			rigidBody.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
			rigidBody.WakeUp();
		}

		isGrounded = rigidBody.constraints is RigidbodyConstraints2D.FreezePositionY ? 1 : 0;
	}

	void CalculateGround()
	{
		if (isGrounded is not 1) return;
		
		float offset = yOffset + offsetTimer;
		RaycastHit2D hit = Physics2D.Raycast(rayPoint.position, Vector2.down, groundCheckDistance, groundLayer);
		if (hit.collider == default) return;
		
		Vector3 point = legTargets.Aggregate(Vector3.zero, (current, legIK) => current + legIK.Position);
		point.y = point.y / legTargets.Length + offset;

		Transform tf = transform;
		Vector3 tfPos = tf.position;
		tfPos.y = point.y;
		tf.position = tfPos;
	}
	
	void Idle()
	{
		switch (Exhale())
		{
			case false when offsetTimer < breathingHeight:
				offsetTimer += Time.deltaTime * (breathingSpeed * .1f);
				break;
			case true when offsetTimer > -breathingHeight:
				offsetTimer -= Time.deltaTime * (breathingSpeed * .1f);
				break;
		}
	}

	bool Exhale()
	{
		if (offsetTimer > breathingHeight) return true;
		return !(offsetTimer < -breathingHeight);
	}
}