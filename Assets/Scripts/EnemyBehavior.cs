﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyBehavior : MonoBehaviour {

	Controller2D controller;

	Rigidbody2D ball;
	public CircleCollider2D ballCollider;
	CircleCollider2D enemyCollider;

	public LayerMask ballCollisionMask;
	RaycastOrigins raycastOrigins;

	public BoxCollider2D map;
	public Spawner spawner;

	bool dangerZone = false;
	Path path;

	//	Pathfinding
	Seeker seeker;
	// The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = .3f;
	// The waypoint we are currently moving towards
	private int currentWaypoint = 0;

	void Start(){
		controller = GetComponent<Controller2D>();
		ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<Rigidbody2D>();
		raycastOrigins = new RaycastOrigins();
		seeker = GetComponent<Seeker>();
	}

	public void NextMovement(ref Vector2 input){

		// Wait for dash to end
		if (controller.playerInfo.onDash){
			controller.Dash();
		} else if (controller.playerInfo.onZhonya){
			controller.Zhonya();
		}

		if(controller.playerInfo.inputEnabled){
			Vector2 direction = Vector2.zero;
			if (dangerZone){

				RaycastHit2D hit = Physics2D.Raycast(ball.transform.position, ball.velocity, Mathf.Infinity, ballCollisionMask);

				if(hit){
					if (!controller.playerInfo.onDash) {
						for (int i = 0; i < controller.powerups.Length; i++){
							if (controller.powerups[i] == Controller2D.Powerups.Dash){
								controller.UsePowerup(i);
								break;
							}
						}
					}
					if (!controller.playerInfo.onZhonya) {
						for (int i = 0; i < controller.powerups.Length; i++){
							if (controller.powerups[i] == Controller2D.Powerups.Zhonya){
								controller.UsePowerup(i);
								break;
							}
						}
					}
				} else {
					dangerZone = false;
				}
			}
			if (!dangerZone && spawner.instantiatedPowerup != null ){
				if (path != null && currentWaypoint < path.vectorPath.Count){
					if (currentWaypoint < path.vectorPath.Count){
						// Direction to the next waypoint
						direction = (path.vectorPath[currentWaypoint]-transform.position).normalized;
						if (Vector2.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
								currentWaypoint++;
						}
					}
				} else {
					seeker.StartPath(transform.position, spawner.instantiatedPowerup.transform.position, OnPathComplete);
				}
				//	Calculate path to powerup
				//Vector2 powerupPosition = spawner.instantiatedPowerup.transform.position;

				//direction = new Vector2(powerupPosition.x - transform.position.x, powerupPosition.y - transform.position.y);
			} else if(!dangerZone){
				direction = ball.velocity.Rotate(90f);

				if (Mathf.Sign(ball.velocity.x) == 1 && 
					transform.position.x <= ball.transform.position.x && 
					Mathf.Sign(direction.x) == 1){
					direction.x *= -1;
				}

				if (Mathf.Sign(ball.velocity.x) == -1 && 
					transform.position.x >= ball.transform.position.x && 
					Mathf.Sign(direction.x) == -1){
					direction.x *= -1;
				}

				if (Mathf.Sign(ball.velocity.y) == 1 && 
					transform.position.y <= ball.transform.position.y && 
					Mathf.Sign(direction.y) == 1){
					direction.y *= -1;
				}

				if (Mathf.Sign(ball.velocity.y) == -1 && 
					transform.position.y >= ball.transform.position.y && 
					Mathf.Sign(direction.y) == -1){
					direction.y *= -1;
				}
			}
			input = direction.normalized;
			controller.playerInfo.input = input;
		}
	}

	public void OnPathComplete (Path p) {
        if (!p.error) {
            path = p;
            // Reset the waypoint counter so that we start to move towards the first point in the path
            currentWaypoint = 0;
        }
    }

	public void UpdateRaycastOrigins(){
		Bounds bounds = ballCollider.bounds;
		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	public struct RaycastOrigins{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Ball"){
			dangerZone = true;
		}
	}

	void OnTriggerExit2D(Collider2D other){
		if (other.tag == "Ball"){
			dangerZone = false;
		}
	}

}
