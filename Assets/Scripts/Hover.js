//#pragma strict
/*
 * Written by: Mister Jason (internet Alias)
 * Modified by: Jake Nye on 03/05/2016
 * 
 * Comments: This script handles the perk guards smooth transitioning between two
 *           positions at a specified speed.
*/

var floatup;

function Start() {
    floatup = false;
}

function Update() {
    if ( floatup )
        floatingup();
    else if( !floatup )
        floatingdown();
}

function floatingup() {
    transform.position.y += 1 * Time.deltaTime;
    yield WaitForSeconds(1);
    floatup = false;
}

function floatingdown() {
    transform.position.y -= 1 * Time.deltaTime;
    yield WaitForSeconds(1);
    floatup = true;
}