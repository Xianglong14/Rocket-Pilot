using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour {

	[SerializeField] Vector3 movementVector;
	[SerializeField] float period = 6f; // 2 second

    // todo remove from inspector later
    [Range(0, 1)] [SerializeField] float movementFactor; // 0 for not moved, 1 for fully moved

	Vector3 startingPos;

	// Use this for initialization
	void Start () {
        startingPos = transform.position;

    }
	
	// Update is called once per frame
	void Update () {
		if(period == Mathf.Epsilon) { return; } // protect the period is 0
		// set movement factor
		float cycles = Time.time / period;

		const float tau = Mathf.PI * 2;
		float rawSinware = Mathf.Sin(cycles * tau); // The return value between -1 and +1

		movementFactor = rawSinware / 2f + 0.5f;
		Vector3 offset = movementVector * movementFactor;
		transform.position = startingPos + offset;
		
	}
}
