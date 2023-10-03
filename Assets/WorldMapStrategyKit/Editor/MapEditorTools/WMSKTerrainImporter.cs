using UnityEngine;
using UnityEditor;

namespace WorldMapStrategyKit {

	[ExecuteInEditMode]
	public class WMSKTerrainImporter : EditorWindow {

		const string DEFAULT_HEIGHTMAP_FILENAME = "/terrain_heightmap.png";
		const string DEFAULT_TERRAIN_TEXTURE_FILENAME = "/terrain_texture.png";


		enum TerrainResolution {
			Low_1024x1024,
			Medium_2048x2048,
			High_4096x4096,
			Maximum_8192x8192
		}

		Terrain terrain;
		TerrainResolution resolution;
		bool rotate180;


		public static void ShowWindow () {
			int w = 400;
			int h = 180;
			Rect rect = new Rect (Screen.currentResolution.width / 2 - w / 2, Screen.currentResolution.height / 2 - h / 2, w, h);
			WMSKTerrainImporter window = GetWindowWithRect<WMSKTerrainImporter> (rect, true, "Terrain Importer", true);
			window.ShowUtility ();
		}

		void OnGUI () {
			if (WMSK.instance == null) {
				DestroyImmediate (this);
				EditorGUIUtility.ExitGUI ();
				return;
			}

			EditorGUILayout.HelpBox ("This tool will import the heightmap and combined texture of an existing terrain.", MessageType.Info);
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Terrain", GUILayout.Width (120));
			terrain = (Terrain)EditorGUILayout.ObjectField (terrain, typeof(Terrain), true);
			EditorGUILayout.EndHorizontal ();				
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Resolution", GUILayout.Width (120));
			resolution = (TerrainResolution)EditorGUILayout.EnumPopup (resolution);
			EditorGUILayout.EndHorizontal ();				
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Rotate 180°", GUILayout.Width (120));
			rotate180 = EditorGUILayout.Toggle (rotate180);
			EditorGUILayout.EndHorizontal ();				

			EditorGUILayout.Separator ();
			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.HelpBox ("This operation can take some time. Please wait until it finishes.", MessageType.Warning);
			if (terrain == null)
				GUI.enabled = false;
			if (GUILayout.Button ("Start")) {
				ImportTerrain ();
			}
			GUI.enabled = true;
			if (GUILayout.Button ("Cancel")) {
				Close ();
			}
		}


		void OnDestroy () {
			WMSK.instance.editor.terrainImporterActive = false;
		}

		void ImportTerrain () {
												
			if (terrain == null) {
				EditorUtility.DisplayDialog ("Import Terrain Features", "Please select an existing terrain first.", "Ok");
				return;
			}

			// Extract heightmap
			TerrainData terrainData = terrain.terrainData;
			const int hmWidth = 2048;
			const int hmHeight = 1024;
			float[] heights = new float[hmWidth * hmHeight];
			float maxHeight = float.MinValue;
			int index = 0;
			if (rotate180) {
				for (int j = 0; j < hmHeight; j++) {
					float z = 1.0f - (float)j / hmHeight;
					for (int k = 0; k < hmWidth; k++) {
						float x = 1.0f - (float)k / hmWidth;
						float h = terrain.terrainData.GetInterpolatedHeight (x, z);
						heights [index++] = h;
						if (h > maxHeight) {
							maxHeight = h;
						}
					}
				}
			} else {
				for (int j = 0; j < hmHeight; j++) {
					float z = (float)j / hmHeight;
					for (int k = 0; k < hmWidth; k++) {
						float x = (float)k / hmWidth;
						float h = terrain.terrainData.GetInterpolatedHeight (x, z);
						heights [index++] = h;
						if (h > maxHeight) {
							maxHeight = h;
						}
					}
				}
			}

			Color[] colors = new Color[heights.Length];
			for (int k = 0; k < heights.Length; k++) {
				float h = heights [k] / maxHeight;
				colors [k] = new Color (h, h, h, 1.0f);
			}

			Texture2D hm = new Texture2D (hmWidth, hmHeight, TextureFormat.ARGB32, false);
			hm.SetPixels (colors);
			hm.Apply ();

			// Save heightmap
			string wmskResourcesPath = GetWMSKResourcesPath ();
			string texPath = wmskResourcesPath + "/Terrain";
			System.IO.Directory.CreateDirectory (texPath);

			byte[] bytes = hm.EncodeToPNG ();
			string terrainHeightMapFullFilename = texPath + DEFAULT_HEIGHTMAP_FILENAME;
			System.IO.File.WriteAllBytes (terrainHeightMapFullFilename, bytes);

			// Snapshot terrain
			const int snapshotLayer = 21;
			int oldLayer = terrain.gameObject.layer;
			Vector3 oldPos = terrain.gameObject.transform.position;
			Transform lightTransform = GetSceneLight ();
			Quaternion oldLightRot = lightTransform.rotation;

			// Setup terrain
			terrain.gameObject.layer = snapshotLayer;
			terrain.transform.position = new Vector3 (-1000 - terrainData.size.x / 2, -10000, -1000 - terrainData.size.z / 2);
			// Setup lighting
			lightTransform.rotation = Misc.QuaternionX90;

			// Create snapshot cam
			GameObject camGO = new GameObject ("SnapshotCam");
			Camera cam = camGO.AddComponent<Camera> ();
			cam.orthographic = true;
			float maxSize = Mathf.Max (terrainData.size.x, terrainData.size.z);
			cam.orthographicSize = maxSize / 2;
			float camh = 10 + maxHeight * terrainData.size.y;
			cam.transform.position = new Vector3 (-1000, -10000 + camh, -1000);
			cam.transform.rotation = Quaternion.Euler (90, 0, rotate180 ? 180 : 0);
			cam.farClipPlane = camh + 1f;

			int texSize;
			switch (resolution) {
			case TerrainResolution.Low_1024x1024:
				texSize = 1024;
				break;
			case TerrainResolution.High_4096x4096:
				texSize = 4096;
				break;
			case TerrainResolution.Maximum_8192x8192:
				texSize = 8192;
				break;
			default:
				texSize = 2048;
				break;
			}
			RenderTexture rt = new RenderTexture (texSize, texSize, 24, RenderTextureFormat.ARGB32);
			Texture2D terrainTex = new Texture2D (texSize, texSize, TextureFormat.ARGB32, false);
			cam.targetTexture = rt;
			cam.cullingMask = 1 << snapshotLayer;
			cam.Render ();

			// Obtain result texture
			RenderTexture.active = rt;
			terrainTex.ReadPixels (new Rect (0, 0, rt.width, rt.height), 0, 0);
			terrainTex.Apply ();

			// Clean up
			cam.targetTexture = null;
			RenderTexture.active = null;
			rt.Release ();
			DestroyImmediate (camGO);
			if (lightTransform.gameObject.name.Equals ("WMSK Temporary Light")) {
				DestroyImmediate (lightTransform.gameObject);
			}

			lightTransform.rotation = oldLightRot;
			terrain.gameObject.layer = oldLayer;
			terrain.transform.position = oldPos;

			// Save terrain texture
			bytes = terrainTex.EncodeToPNG ();
			string terrainTextureFullFilename = texPath + DEFAULT_TERRAIN_TEXTURE_FILENAME;
			System.IO.File.WriteAllBytes (terrainTextureFullFilename, bytes);

			AssetDatabase.Refresh ();

			TextureImporter texImp = (TextureImporter)TextureImporter.GetAtPath (terrainHeightMapFullFilename);
			texImp.isReadable = true;
			texImp.SaveAndReimport ();

			texImp = (TextureImporter)TextureImporter.GetAtPath (terrainTextureFullFilename);
			texImp.isReadable = true;
			texImp.SaveAndReimport ();

			// Assign imported terrain
			WMSK.instance.earthStyle = EARTH_STYLE.Texture;
			string resourceEarthTex = "WMSK/Terrain/" + System.IO.Path.GetFileNameWithoutExtension (DEFAULT_TERRAIN_TEXTURE_FILENAME);
			WMSK.instance.earthTexture = Resources.Load<Texture2D> (resourceEarthTex);
			string resourceHeightmap = "WMSK/Terrain/" + System.IO.Path.GetFileNameWithoutExtension (DEFAULT_HEIGHTMAP_FILENAME);
			WMSK.instance.heightMapTexture = Resources.Load<Texture2D> (resourceHeightmap);
			EditorUtility.SetDirty (WMSK.instance);
			EditorUtility.DisplayDialog ("Terrain Import Complete", "Heightmap and Earth texture created at Resources/" + texPath, "Ok");
			Close ();
		}

		string GetWMSKResourcesPath () {

			string[] paths = AssetDatabase.GetAllAssetPaths ();
			for (int k = 0; k < paths.Length; k++) {
				if (paths [k].EndsWith ("Resources/WMSK/Textures")) {
					return System.IO.Path.GetDirectoryName (paths [k]); // Get parent of directory
				}
			}
			return "";
		}

		Transform GetSceneLight () {
			if (WMSK.instance.sun != null)
				return WMSK.instance.transform;
			Light[] lights = FindObjectsOfType<Light> ();
			for (int k = 0; k < lights.Length; k++) {
				if (lights [k].type == LightType.Directional)
					return lights [k].transform;
			}
			GameObject lightGO = new GameObject ("WMSK Temporary Light", typeof(Light));
			return lightGO.transform;
		}


	}

}