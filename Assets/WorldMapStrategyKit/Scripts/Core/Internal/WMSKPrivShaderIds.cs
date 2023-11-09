using UnityEngine;

namespace WorldMapStrategyKit {

    public partial class WMSK : MonoBehaviour {

        static partial class ShaderParams {
            public static int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
            public static int OutlineColor = Shader.PropertyToID("_OutlineColor");
            public static int TerrainWMSKData = Shader.PropertyToID("_WMSK_Data");
            public static int TerrainWMSKClip = Shader.PropertyToID("_WMSK_Clip");
            public static int SunDirection = Shader.PropertyToID("_SunLightDirection");
            public static int Thickness = Shader.PropertyToID("_Thickness");
            public static int MaxPixelWidth = Shader.PropertyToID("_MaxPixelWidth");
            public static int OuterColor = Shader.PropertyToID("_OuterColor");
            public static int DashAmount = Shader.PropertyToID("_DashAmount");
            public static int WaterLevel = Shader.PropertyToID("_WaterLevel");
            public static int WaterColor = Shader.PropertyToID("_WaterColor");
            public static int AlphaOnWater = Shader.PropertyToID("_AlphaOnWater");
            public static int MainTex = Shader.PropertyToID("_MainTex");
            public static int TerrestrialMap = Shader.PropertyToID("_TerrestrialMap");
            public static int TextureScale = Shader.PropertyToID("_TextureScale");
            public static int Brightness = Shader.PropertyToID("_Brightness");
            public static int CloudShadowStrength = Shader.PropertyToID("_CloudShadowStrength");
            public static int WrapEnabled = Shader.PropertyToID("_WrapEnabled");

            public static int FaceColor = Shader.PropertyToID("_FaceColor");
            public static int UnderlayColor = Shader.PropertyToID("_UnderlayColor");

            public static int StencilComp = Shader.PropertyToID("_StencilComp");

            public static int AnimationAcumOffset = Shader.PropertyToID("_AnimationAcumOffset");
            public static int AnimationStartTime = Shader.PropertyToID("_AnimationStartTime");
            public static int AnimationSpeed = Shader.PropertyToID("_AnimationSpeed");

            public const string SKW_OUTLINE = "OUTLINE_ON";
            public const string SKW_VIEWPORT_UNLIT = "WMSK_VIEWPORT_UNLIT";
        }
    }

}
