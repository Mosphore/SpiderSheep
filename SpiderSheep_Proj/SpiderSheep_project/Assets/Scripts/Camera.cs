using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(0, 0, Input.GetAxisRaw("Jump") * 50 * Time.deltaTime);
        transform.Rotate(0, Input.GetAxis("Horizontal") * 40 * Time.deltaTime, 0);
        transform.Rotate(Input.GetAxis("Vertical") * 40 * Time.deltaTime, 0, 0);
    }
}
