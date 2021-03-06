﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
public class Zhonya : MonoBehaviour {

	Controller2D controller;
	BoxCollider2D userCollider;
	Animator animator;

	public float zhonyaTime = 2f;
	public float zhonyaDrawbackTime = 2f;

	private AudioSource source;
	public AudioClip zhonyas;
	public AudioClip stun;

	Color zhonyaColor = new Color(255f, 255f, 255f, 0.2f);
	Color defaultColor = new Color(255f, 255f, 255f, 1f);

	void Start(){
		controller = GetComponent<Controller2D>();
		userCollider = GetComponent<BoxCollider2D>();
		animator = GetComponent<Animator>();
		source = GetComponent<AudioSource>();
	}

	public void Use(){
		StartCoroutine("StartZhonya");
	}

	IEnumerator StartZhonya() {

		ActivateZhonya();
		float zhonyaTimeRemaining = 0f;

		while (zhonyaTimeRemaining < zhonyaTime){
			zhonyaTimeRemaining += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		
		DeactivateZhonya();
		float zhonyaDrawbackTimeRemaining = 0f;

		while (zhonyaDrawbackTimeRemaining < zhonyaDrawbackTime){
			zhonyaDrawbackTimeRemaining += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		EndDrawback();
	}

	void ActivateZhonya(){
		controller.playerInfo.onZhonya = true;
		controller.StopMovement();
		controller.ChangeColor(zhonyaColor);
		userCollider.isTrigger = true;
		source.PlayOneShot(zhonyas, 1);
	}

	void DeactivateZhonya(){
		controller.playerInfo.onZhonya = false;
		userCollider.isTrigger = false;
		controller.ChangeColor(defaultColor);
		source.Stop();
		source.PlayOneShot(stun, 1);
		animator.SetBool("isStunned", true);
	}

	void EndDrawback(){
		controller.playerInfo.onZhonya = false;
		animator.SetBool("isStunned", false);
		controller.RecoverMovement();
	}

}
