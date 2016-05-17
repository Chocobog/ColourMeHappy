﻿using UnityEngine;
using System.Collections;
/// <summary>
/// Written By: OrangeLightning 
/// http://answers.unity3d.com/questions/203653/smart-crosshair.html
/// Modified By: Joshua Hurn
/// Last Modified: 17/05/2016 
/// 
/// A straightforward C# conversion of the JavaScript smartCrosshair (tentatively named)
/// Works similarly to original
/// </summary>
public class smartCrosshair : MonoBehaviour
{
    #region Fields
    public bool drawCrosshair = true;
    public Color crosshairColor = Color.white;
    public float width = 1;
    public float height = 3;

    RaycastHit hit;

    [System.Serializable]
    public class spreading
    {
        public float sSpread = 20;
        public float maxSpread = 60;
        public float minSpread = 20;
        public float spreadPerSecond = 30;
        public float decreasePerSecond = 25;
    }

    public spreading spread = new spreading();

    Texture2D tex;
    float newHeight;
    GUIStyle lineStyle;

    #endregion

    #region Functions
    void Awake()
    {
        tex = new Texture2D(1, 1);
        lineStyle = new GUIStyle();
        lineStyle.normal.background = tex;
    }

    void OnGUI()
    {
        Vector2 centerPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        float screenRatio = Screen.height / 100;

        newHeight = height * screenRatio;

        if (drawCrosshair)
        {
            GUI.Box(new Rect(centerPoint.x - (width / 2), centerPoint.y - (newHeight + spread.sSpread), width, newHeight), GUIContent.none, lineStyle);
            GUI.Box(new Rect(centerPoint.x - (width / 2), (centerPoint.y + spread.sSpread), width, newHeight), GUIContent.none, lineStyle);
            GUI.Box(new Rect((centerPoint.x + spread.sSpread), (centerPoint.y - (width / 2)), newHeight, width), GUIContent.none, lineStyle);
            GUI.Box(new Rect(centerPoint.x - (newHeight + spread.sSpread), (centerPoint.y - (width / 2)), newHeight, width), GUIContent.none, lineStyle);
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            spread.sSpread += spread.spreadPerSecond * Time.deltaTime;
            Fire();
        }

        spread.sSpread -= spread.decreasePerSecond * Time.deltaTime;
        spread.sSpread = Mathf.Clamp(spread.sSpread, spread.minSpread, spread.maxSpread);
    }

    void Fire() { }

    void SetColor(Texture2D myTexture, Color myColor)
    {
        for (int y = 0; y < myTexture.height; y++)
        {
            for (int x = 0; x < myTexture.width; x++)
                myTexture.SetPixel(x, y, myColor);
            myTexture.Apply();
        }
    }

    void Update()
    {  
        //send out raycast
        if(Physics.Raycast(transform.position, transform.forward,out hit, 300))
        {
            //if enemy hit with raycast change cross hairs to red
            if (hit.transform.tag == "Enemy")
                SetColor(tex, Color.red);
            else
                SetColor(tex, Color.white);
        }
    }
    #endregion
}