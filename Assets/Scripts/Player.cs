﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : LivingEntity {

	Controller2D controller;
	BoxCollider2D collider;
	PlayerInfo playerInfo;

	Vector2 input;
	float velocityXSmoothing;
	float velocityYSmoothing;

	// Use this for initialization
	void Start () {
		controller = GetComponent<Controller2D>();
		collider = GetComponent<BoxCollider2D>();
		playerInfo = controller.playerInfo;
	}
	
	// Update is called once per frame
	void Update () {

		if (playerInfo.onDash && (Mathf.Abs(input.x) + Mathf.Abs(input.y) != 0)){
			controller.Dash(ref playerInfo);	
		} else if(playerInfo.onZhonya){
			controller.Zhonya(ref playerInfo);
		} else if(playerInfo.onFreeze){
			controller.Freeze(ref playerInfo);
		}
		
		if (playerInfo.inputEnabled){
			input = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			input = input.normalized;

			if (Input.GetKeyDown(KeyCode.Space)){
				controller.Dash(ref playerInfo);
			} else if (Input.GetKeyDown(KeyCode.C)){
				controller.Zhonya(ref playerInfo);
			} else if (Input.GetKeyDown(KeyCode.X)){
				controller.Freeze(ref playerInfo);
			}
		}
	}

	void FixedUpdate(){
		float targetVelocityX = input.x * playerInfo.moveSpeed;
		float targetVelocityY = input.y * playerInfo.moveSpeed;
		playerInfo.velocity.x = Mathf.SmoothDamp(playerInfo.velocity.x, targetVelocityX, ref velocityXSmoothing, playerInfo.accelerationTime);
		playerInfo.velocity.y = Mathf.SmoothDamp(playerInfo.velocity.y, targetVelocityY, ref velocityYSmoothing, playerInfo.accelerationTime);
		controller.Move(playerInfo.velocity * Time.deltaTime, input);
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.collider.tag == "Enemy" && playerInfo.attack){
			other.collider.GetComponent<LivingEntity>().TakeDamage(5);
		} else if (other.collider.tag == "Wall" && playerInfo.onDash){
			transform.GetComponent<LivingEntity>().TakeDamage(99999);
		}
	}

	void OnCollisionStay2D(Collision2D other) {
		if (other.collider.tag == "Enemy" && playerInfo.attack){
			other.collider.GetComponent<LivingEntity>().TakeDamage(5);
		} //else if (other.collider.tag == "Wall" && playerInfo.onDash){
			//transform.GetComponent<LivingEntity>().TakeDamage(99999);
		//}
	}
}
