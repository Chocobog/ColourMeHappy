using UnityEngine;
using System.Collections;

public class minimapShadows : MonoBehaviour {

    public Shader unlitShader;

    void Start()
    {
        //unlitShader = Shader.Find("Unlit/Texture");
        GetComponent<Camera>().SetReplacementShader(unlitShader, "");
    }

    // Update is called once per frame
    void Update () {
	
	}
}
