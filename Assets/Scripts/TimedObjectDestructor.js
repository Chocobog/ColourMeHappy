/*
* Written by: AJ Abotomey
* Last Modified: 10/05/2016
*/

var timeOut = 1.0; //time to destroy object
var detachChildren = false; //if attached to a parent

//when the script is called
function Awake ()
{
	Invoke ("DestroyNow", timeOut);
}

//Destroy the object
function DestroyNow ()
{
	if (detachChildren) {
		transform.DetachChildren ();
	}
	DestroyObject (gameObject);
}