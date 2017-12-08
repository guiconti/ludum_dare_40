﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerUI))]
public class Controller2D : MonoBehaviour {

	Rigidbody2D rb;
	BoxCollider2D collider;
	Animator animator;

	[HideInInspector]
	public PlayerInfo playerInfo;

	PlayerUI playerUI;

	public AudioClip death;
  private AudioSource source;

	SpriteRenderer spriteRenderer;

	public float moveSpeed = 20f;
	float accelerationTime = .1f;

	public Vector2 playerInput;

	void Awake() {
		playerInfo = new PlayerInfo(moveSpeed, accelerationTime, Vector3.zero);
		source = GetComponent<AudioSource>();
	}

	void Start() {
		rb = GetComponent<Rigidbody2D>();
		collider = GetComponent<BoxCollider2D>();
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		playerUI = GetComponent<PlayerUI>();

		LivingEntity livingEntity = GetComponent<LivingEntity>();
		livingEntity.OnDeath += TriggerDeathAnimation;
		livingEntity.OnDeath += PlayDeathSFX;
	}

	public void Move(Vector2 moveAmount, Vector2 input) {
		rb.MovePosition(rb.position + moveAmount);
		playerInput = input;

		//	Update animation
		animator.SetFloat("inputX", input.x);
		animator.SetFloat("inputY", input.y);
		if (input.x != 0 || input.y != 0) {
			animator.SetBool("isWalking", true);
			animator.SetFloat("lastInputX", input.x);
			animator.SetFloat("lastInputY", input.y);
		} else {
			animator.SetBool("isWalking", false);
		}
	}

	public void StopMovement() {
		playerInfo.inputEnabled = false;
		playerInfo.moveSpeed = 0;
		playerInfo.accelerationTime = 0;
		playerInfo.velocity.x = 0;
		playerInfo.velocity.y = 0;
	}

	public void ChangeColor(Color color) {
		spriteRenderer.color = color;
	}

	public void RecoverMovement() {
		playerInfo.inputEnabled = true;
		playerInfo.moveSpeed = moveSpeed;
		playerInfo.accelerationTime = accelerationTime;
	}

	void TriggerDeathAnimation() {
		animator.SetBool("isDead", true);
	}

	void PlayDeathSFX(){
		source.PlayOneShot(death, 1.5f);
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.collider.tag == "Enemy" && playerInfo.attack) {
			other.collider.GetComponent<LivingEntity>().TakeDamage(5);
		} else if (other.collider.tag == "Wall" && playerInfo.onDash) {
			transform.GetComponent<LivingEntity>().TakeDamage(99999);
		}
	}

	void OnCollisionStay2D(Collision2D other) {
		if (other.collider.tag == "Enemy" && playerInfo.attack) {
			other.collider.GetComponent<LivingEntity>().TakeDamage(5);
		} //else if (other.collider.tag == "Wall" && playerInfo.onDash){
		//transform.GetComponent<LivingEntity>().TakeDamage(99999);
		//}
	}

}
