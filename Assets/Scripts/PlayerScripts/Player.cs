﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {

	[Header("Player Attributes")]
	public float speed;
	public float startHealth;

	[Header("Player References")]
	public Transform holdPosition;
	public Image healthBar;
	public GameObject building;

	private Rigidbody playerRB;
	private Vector3 movement;

	private Transform selectedObject;
	private bool isObjectSelected;
	private bool isCarrying;
	private float health;

	private int floorMask;
	private float camRayLength = 100f;

	private Animator anim;

	[Header("Timers")]
	public float startPickUpTimer = 1f;
	public float startBuildTimer = 1f;

	private float pickupTimer;
	private float buildTimer;

	void Awake() {
		floorMask = LayerMask.GetMask ("Floor");
		playerRB = GetComponent<Rigidbody> ();
		health = startHealth;
		anim = GetComponent<Animator> ();
	}

	void FixedUpdate() {
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");

		Move (horizontal, vertical);
		Turn ();
		Animate (horizontal, vertical);

		if (isObjectSelected && !isCarrying && Input.GetKeyDown(KeyCode.E)) {
			if (pickupTimer <= 0f) {
				PickUp ();
				pickupTimer = startPickUpTimer;
			}
		} else if (isCarrying && Input.GetKeyDown(KeyCode.E)) {
			if (pickupTimer <= 0f) {
				Drop ();
				pickupTimer = startPickUpTimer;
			}
		}

		if (Input.GetKeyDown(KeyCode.F) & buildTimer <= 0f) {
			Build ();
			buildTimer = startBuildTimer;
		}


		buildTimer -= Time.deltaTime;
		pickupTimer -= Time.deltaTime;
	}

	void Move(float horizontal, float vertical) {
		movement.Set (horizontal, 0f, vertical);
		movement = movement.normalized * speed * Time.deltaTime;


		playerRB.MovePosition (transform.position + movement);
	}

	void Turn() {
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit floorHit;

		if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask)) {
			Vector3 playerToMouse = floorHit.point - transform.position;
			playerToMouse.y = 0f;

			Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
			playerRB.MoveRotation(newRotation);
		}
	}

	void PickUp() {
		selectedObject.transform.SetPositionAndRotation (holdPosition.position, holdPosition.rotation);
		selectedObject.transform.SetParent (holdPosition);
		isCarrying = true;
		Debug.Log ("PickedUp");
	}

	void Drop() {
		Debug.Log ("Dropped");
		holdPosition.DetachChildren ();
		isCarrying = false;
	}

	void Animate(float h, float v) {
		bool running = h != 0f || v != 0f;
		anim.SetBool ("IsRunning", running);
	}

	void Build() {
		Instantiate (building, holdPosition.position, holdPosition.rotation);
		return;
	}

	public void ObjectSelected(Transform selectedObject) {
		this.selectedObject = selectedObject;
		isObjectSelected = true;
	}

	public void ObjectedDeselected() {
		selectedObject = null;
		isObjectSelected = false;
	}

	public void Damage(float damage) {
		health -= damage;
		healthBar.fillAmount = health / startHealth;
	}

}