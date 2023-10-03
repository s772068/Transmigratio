using UnityEngine;
using System;
using System.Collections;

public class CloudLayerAnimator : MonoBehaviour {

    static class ShaderParams {
        public static int TextureOffset = Shader.PropertyToID("_TextureOffset");
        public static int CloudMapOffset = Shader.PropertyToID("_CloudMapOffset");
    }

    [NonSerialized]
    public float speed;

    [NonSerialized]
    public Vector2 cloudMainTextureOffset;

    [NonSerialized]
    public Material earthMat;

    [NonSerialized]
    public Material cloudMat;

    Vector2 tdisp;

    public void Update() {
        tdisp.x += Time.deltaTime * speed * 0.001f;
        Vector2 offset = cloudMainTextureOffset + tdisp;
        if (cloudMat != null) {
            cloudMat.mainTextureOffset = offset;
            cloudMat.SetVector(ShaderParams.TextureOffset, offset); // for URP version
        }
        if (earthMat != null) earthMat.SetVector(ShaderParams.CloudMapOffset, tdisp);
    }
}
