//#pragma strict
/*
 * Written by: Mister Jason (internet Alias)
 * Modified by: Jake Nye on 03/05/2016
 * 
 * Comments: This script handles the perk guards smooth transitioning between two
 *           positions at a specified speed.
*/

var floatup; //boolean to say if floating up or not

//initialise here
function Start() {
    floatup = false;
}

//Called each frame
function Update() {
    if ( floatup )
        floatingup();
    else if( !floatup )
        floatingdown();
}

//Move the guard up in the y axis
function floatingup() {
    transform.position.y += 1 * Time.deltaTime;
    yield WaitForSeconds(1);
    floatup = false;
}

//Move the guard down in the y axis
function floatingdown() {
    transform.position.y -= 1 * Time.deltaTime;
    yield WaitForSeconds(1);
    floatup = true;
}