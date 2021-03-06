﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	Rigidbody2D rb;
	[Range(0, 1)]
	public float ballSpeedIncrementOnBounce = 1f;
	public float ballMinSpeed = 1.5f;
	public float ballMaxSpeed = 25f;
	public AudioClip hitWall;

	private AudioSource source;
	private float lowPitchRange = .50F;
	private float highPitchRange = .85F;
	private float velToVol = .025F;
	float directionX;
	float directionY;

	// Use this for initialization
	void Start() {
		rb = GetComponent<Rigidbody2D>();
		Vector2 initialVelocity = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
		initialVelocity = initialVelocity.normalized;
		directionX = Mathf.Sign(initialVelocity.x);
		directionY = Mathf.Sign(initialVelocity.y);
		initialVelocity.x = initialVelocity.x > initialVelocity.y?ballMinSpeed * directionX: (initialVelocity.x / initialVelocity.y) * directionX;
		initialVelocity.y = initialVelocity.y > initialVelocity.x?ballMinSpeed * directionY: (initialVelocity.y / initialVelocity.x) * directionX;
		rb.velocity = initialVelocity;
		source = GetComponent<AudioSource>();
	}

	Vector2 CalculateBallVelocity(float speedIncrementPercentage) {
		Vector2 newVelocity = rb.velocity * (1 + speedIncrementPercentage);
		directionX = Mathf.Sign(newVelocity.x);
		directionY = Mathf.Sign(newVelocity.y);
		newVelocity = new Vector2(Mathf.Clamp(Mathf.Abs(newVelocity.x), ballMinSpeed, ballMaxSpeed) * directionX,
			Mathf.Clamp(Mathf.Abs(newVelocity.y), ballMinSpeed, ballMaxSpeed) * directionY);
		return newVelocity;
	}

	void OnCollisionEnter2D(Collision2D other) {
		source.pitch = Random.Range(lowPitchRange, highPitchRange);
		float hitVol = other.relativeVelocity.magnitude * velToVol;
		if (other.collider.tag == "Wall") {
			rb.velocity = CalculateBallVelocity(ballSpeedIncrementOnBounce);
			// float forward = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
			// transform.rotation = Quaternion.Euler(0f, 0f, forward);
			source.PlayOneShot(hitWall, hitVol);
			float shakeDuration = 0.2f * (Mathf.Max(Mathf.Abs(rb.velocity.x), Mathf.Abs(rb.velocity.y)) / ballMaxSpeed);
			float shakeAmount = 0.1f * (Mathf.Max(Mathf.Abs(rb.velocity.x), Mathf.Abs(rb.velocity.y)) / ballMaxSpeed);;
			CameraShaker.Shake(shakeDuration, shakeAmount);
		} else if (other.collider.tag == "Player") {
			other.collider.GetComponent<LivingEntity>().TakeDamage(999);
			CameraShaker.Shake(0.3f, 0.3f);
			}
	}
}
