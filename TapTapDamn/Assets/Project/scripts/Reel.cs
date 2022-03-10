using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reel : MonoBehaviour {
    public float speed;
    public Transform target;
    public bool start = false;

    private Vector3 startPosition;
    private Quaternion startRotation;
	// Use this for initialization
	void Start () {
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
	
	// Update is called once per frame
	void Update () { //лучше использовать fixedUpdate для этого
        if (start)
            transform.RotateAround(target.localPosition, new Vector3(0.0f, 0.0f, 1.0f), 100 * Time.deltaTime * speed);
        else
            transform.SetPositionAndRotation(startPosition, startRotation);
    }
}
