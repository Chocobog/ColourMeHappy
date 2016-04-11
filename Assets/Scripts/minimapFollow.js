/*
* @Written By: Joshua Hurn
* @Last Modified: 08/04/2016
*
* This script grabs the x and z co-ordinates of the player and allows the minimap to follow the player
* Done in Javascript over C# as C# is unable to get these exact positions without having issues
*/

var player : Transform; //references the player transform properties in the GUI
var storedShadowDistance : float;

function Update () {
    //Grabs the x and z co-ordinates of the player and does not show the y co-ordinates 
    gameObject.transform.position.x = player.transform.position.x; 
    gameObject.transform.position.z = player.transform.position.z;
}


function OnPreRender () {
    storedShadowDistance = QualitySettings.shadowDistance;
    QualitySettings.shadowDistance = 0;
}


function OnPostRender () {
    QualitySettings.shadowDistance = storedShadowDistance;
}