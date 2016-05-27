using UnityEngine;
using System.Collections;

public class ChkPoint : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(transform.position.x * Time.deltaTime, 0f, 0f);
	}
}
