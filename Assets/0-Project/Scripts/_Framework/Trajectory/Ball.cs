using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
	[HideInInspector] public Rigidbody rb;
	[HideInInspector] public Collider col;

	[HideInInspector] public Vector3 pos { get { return transform.position; } }

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
		col = GetComponent<Collider>();
	}

	public void Push(Vector3 force)
	{
		rb.AddForce(force, ForceMode.Impulse);
	}

	public void ActivateRb()
	{
		rb.isKinematic = false;
	}

	public void DesactivateRb()
	{
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		rb.isKinematic = true;
	}
}
