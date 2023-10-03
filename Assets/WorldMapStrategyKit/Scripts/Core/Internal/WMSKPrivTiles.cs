//#define DEBUG_TILES
using UnityEngine;
using System;
using System.Linq;
using System.Text;

#if UNITY_WSA && !UNITY_EDITOR
using System.Threading.Tasks;





#else
using System.Threading;
#endif
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

	public partial class WMSK : MonoBehaviour {

		class ZoomLevelInfo {
			public int xMax, yMax;
			public GameObject tilesContainer;
			public int zoomLevelHash, yHash;
		}

		/// <summary>
		/// This is the minimum zoom level for tiles download. TILE_MIN_ZOOM_LEVEL must stay at 5. A lower value produces tiles that are too big to adapt to the sphere shape resulting in intersection artifacts with higher zoom levels.
		/// </summary>
		public const int TILE_MIN_ZOOM_LEVEL = 4;
		const string PREFIX_MIN_ZOOM_LEVEL = "z4_";

		/// <summary>
		/// TILE_MAX_ZOOM_LEVEL can be increased if needed
		/// </summary>
		public const int TILE_MAX_ZOOM_LEVEL = 19;


		/// <summary>
		/// The maximum zoom level to allow cursor on screen. On higher zoom levels, floating point issues cause cursor to be unaccurate
		/// </summary>
		const int TILE_MAX_CURSOR_ZOOM_LEVEL = 11;


		const int TILE_MIN_SIZE = 256;
		const float TILE_MAX_QUEUE_TIME = 10;
		const string TILES_ROOT = "Tiles";
		int[] tileIndices = { 2, 3, 0, 2, 0, 1 };
		Vector2[] tileUV = {
			Misc.Vector2up,
			Misc.Vector2one,
			Misc.Vector2right,
			Misc.Vector2zero
		};
		Vector4[] placeHolderUV = {
			new Vector4 (0, 0.5f, 0.5f, 1f),
			new Vector4 (0.5f, 0.5f, 1f, 1f),
			new Vector4 (0, 0f, 0.5f, 0.5f),
			new Vector4 (0.5f, 0, 1f, 0.5f)
		};
		Color[][] meshColors = {
			new Color[] {
				new Color (1, 0, 0, 0),
				new Color (1, 0, 0, 0),
				new Color (1, 0, 0, 0),
				new Color (1, 0, 0, 0)
			},
			new Color[] {
				new Color (0, 1, 0, 0),
				new Color (0, 1, 0, 0),
				new Color (0, 1, 0, 0),
				new Color (0, 1, 0, 0)
			},
			new Color[] {
				new Color (0, 0, 1, 0),
				new Color (0, 0, 1, 0),
				new Color (0, 0, 1, 0),
				new Color (0, 0, 1, 0)
			},
			new Color[] {
				new Color (0, 0, 0, 1),
				new Color (0, 0, 0, 1),
				new Color (0, 0, 0, 1),
				new Color (0, 0, 0, 1)
			}
		};
		Vector2[] offsets = {
			new Vector2 (0, 0),
			new Vector2 (0.99999f, 0),
			new Vector2 (0.99999f, 0.99999f),
			new Vector2 (0, 0.99999f)
		};
		Color color1000 = new Color (1, 0, 0, 0);
		int _concurrentLoads;
		int _currentZoomLevel;
		int _webDownloads, _cacheLoads, _resourceLoads;
		long _webDownloadTotalSize, _cacheLoadTotalSize;
		int _tileSize = 0;
		string _tileServerCopyrightNotice;
		string _tileLastError;
		DateTime _tileLastErrorDate;
		Dictionary<int, TileInfo> cachedTiles;
		ZoomLevelInfo[] zoomLevelsInfo = new ZoomLevelInfo[20];
		List<TileInfo> loadQueue, newQueue;
		List<TileInfo> inactiveTiles;
		bool shouldCheckTiles, resortTiles;
		Material tileMatRef, tileMatTransRef;
		Transform tilesRoot;
		int subserverSeq;
		long _tileCurrentCacheUsage;
		FileInfo[] cachedFiles;
		float currentTileSize;
		Plane[] cameraPlanes, wrapCameraPlanes;
		float lastDisposalTime;
		Texture2D currentEarthTexture;
		int spreadLoadAmongFrames;
		Vector3 currentCameraPosition;
		string cachePath;

		void InitTileSystem () {
			_tileServerCopyrightNotice = GetTileServerCopyrightNotice (_tileServer);

			cachePath = Application.persistentDataPath + "/TilesCache";
			if (!Directory.Exists (cachePath)) {
				Directory.CreateDirectory (cachePath);
			}

			if (!Application.isPlaying)
				return;

			if (_tileTransparentLayer) {
				tileMatRef = Resources.Load<Material> ("WMSK/Materials/TileOverlayAlpha") as Material;
				tileMatTransRef = Resources.Load<Material> ("WMSK/Materials/TileOverlayTransAlpha") as Material;
				Color alpha = new Color (1f, 1f, 1f, _tileMaxAlpha);
				tileMatRef.color = alpha;
				tileMatTransRef.color = alpha;
			} else {
				tileMatRef = Resources.Load<Material> ("WMSK/Materials/TileOverlay") as Material;
				tileMatTransRef = Resources.Load<Material> ("WMSK/Materials/TileOverlayTrans") as Material;
			}
			cameraPlanes = new Plane[6];
			wrapCameraPlanes = new Plane[6];

			if (cameraMain != null && cameraMain.clearFlags == CameraClearFlags.Skybox) {
				cameraMain.clearFlags = CameraClearFlags.Color;
				Debug.LogWarning("Tile system is not compatible with skybox. Setting camera clear flag to solid color...");
			}
			if (earthMat != null) {
				currentEarthTexture = (Texture2D)earthMat.mainTexture;
			} else {
				currentEarthTexture = Texture2D.whiteTexture;
			}

			_tileSize = 0;

			InitZoomLevels ();
			if (loadQueue != null)
				loadQueue.Clear ();
			else
				loadQueue = new List<TileInfo> ();
			if (inactiveTiles != null)
				inactiveTiles.Clear ();
			else
				inactiveTiles = new List<TileInfo> ();
			if (cachedTiles != null)
				cachedTiles.Clear ();
			else
				cachedTiles = new Dictionary<int, TileInfo> ();

			if (Application.isPlaying)
				PurgeCacheOldFiles ();

			if (tilesRoot == null) {
				tilesRoot = transform.Find (TILES_ROOT);
			}
			if (tilesRoot != null)
				DestroyImmediate (tilesRoot.gameObject);
			if (tilesRoot == null) {
				GameObject tilesRootObj = new GameObject (TILES_ROOT);
				tilesRoot = tilesRootObj.transform;
				tilesRoot.SetParent (transform, false);
				tilesRoot.gameObject.layer = gameObject.layer;
			}
			shouldCheckTiles = true;

		}

		void DestroyTiles () {
			if (tilesRoot != null) {
				DestroyImmediate (tilesRoot.gameObject);
			}
		}

		/// <summary>
		/// Reloads tiles
		/// </summary>
		public void ResetTiles () {
			DestroyTiles ();
			InitTileSystem ();
		}

		void PurgeCacheOldFiles () {
			PurgeCacheOldFiles (_tileMaxLocalCacheSize);
		}

		public void TileRecalculateCacheUsage () {
			_tileCurrentCacheUsage = 0;
			if (!Directory.Exists (cachePath))
				return;
			DirectoryInfo dir = new DirectoryInfo (cachePath);
			cachedFiles = dir.GetFiles ().OrderBy (p => p.LastAccessTime).ToArray ();
			for (int k = 0; k < cachedFiles.Length; k++) {
				_tileCurrentCacheUsage += cachedFiles [k].Length;
			}
		}

		/// <summary>
		/// Purges the cache old files.
		/// </summary>
		/// <param name="maxSize">Max size is in Mb.</param>
		void PurgeCacheOldFiles (long maxSize) {
			_tileCurrentCacheUsage = 0;
			if (!Directory.Exists (cachePath))
				return;
			DirectoryInfo dir = new DirectoryInfo (cachePath);
			// Delete old jpg files
			FileInfo[] jpgs = dir.GetFiles ("*.jpg.*");
			for (int k = 0; k < jpgs.Length; k++) {
				jpgs [k].Delete ();
			}

			cachedFiles = dir.GetFiles ().OrderBy (p => p.LastAccessTime).ToArray ();
			maxSize *= 1024 * 1024;
			for (int k = 0; k < cachedFiles.Length; k++) {
				if (_tilePreloadTiles && cachedFiles [k].Name.StartsWith (PREFIX_MIN_ZOOM_LEVEL)) {
					continue;
				}
				_tileCurrentCacheUsage += cachedFiles [k].Length;
			}
			if (_tileCurrentCacheUsage <= maxSize)
				return;

			// Purge files until total size gets under max cache size
			for (int k = 0; k < cachedFiles.Length; k++) {
				_tileCurrentCacheUsage -= cachedFiles [k].Length;
				cachedFiles [k].Delete ();
				if (_tileCurrentCacheUsage <= maxSize)
					return;
			}
		}

		void InitZoomLevels () {
			for (int k = 0; k < zoomLevelsInfo.Length; k++) {
				ZoomLevelInfo zi = new ZoomLevelInfo ();
				zi.xMax = (int)Mathf.Pow (2, k);
				zi.yMax = zi.xMax;
				zi.zoomLevelHash = (int)Mathf.Pow (4, k + 1);
				zi.yHash = (int)Mathf.Pow (2, k + 1);
				zoomLevelsInfo [k] = zi;
			}
		}

		void LateUpdateTiles () {
			if (!Application.isPlaying || cachedTiles == null)
				return;

			if (Time.time - lastDisposalTime > 3) {
				lastDisposalTime = Time.time;
				MonitorInactiveTiles ();
			}

			if (shouldCheckTiles || flyToActive) {
				shouldCheckTiles = false;
				currentCameraPosition = currentCamera.transform.position;

				_currentZoomLevel = GetTileZoomLevel ();
				int startingZoomLevel = TILE_MIN_ZOOM_LEVEL - 1;
				ZoomLevelInfo zi = zoomLevelsInfo [startingZoomLevel];
				int currentLoadQueueSize = loadQueue.Count;

				int qCount = loadQueue.Count;
				for (int k = 0; k < qCount; k++) {
					loadQueue [k].visible = false;
				}

                GeometryUtility.CalculateFrustumPlanes(currentCamera.projectionMatrix * currentCamera.worldToCameraMatrix, cameraPlanes);
                if (_wrapHorizontally && _wrapCamera.enabled) {
                    GeometryUtility.CalculateFrustumPlanes(_wrapCamera.projectionMatrix * _wrapCamera.worldToCameraMatrix, wrapCameraPlanes);
                }

                for (int k = 0; k < zi.xMax; k++) {
					for (int j = 0; j < zi.yMax; j++) {
						CheckTiles (null, _currentZoomLevel, k, j, startingZoomLevel, 0);
					}
				}

				if (currentLoadQueueSize != loadQueue.Count)
					resortTiles = true;
				if (resortTiles) {
					resortTiles = false;
					loadQueue.Sort ((TileInfo x, TileInfo y) => {
						if (x.distToCamera < y.distToCamera)
							return -1;
						else if (x.distToCamera > y.distToCamera)
							return 1;
						else
							return 0;
					});
				}
				// Ensure local cache max size is not exceeded
				long maxLocalCacheSize = _tileMaxLocalCacheSize * 1024 * 1024;
				if (cachedFiles != null && _tileCurrentCacheUsage > maxLocalCacheSize) {
					for (int f = 0; f < cachedFiles.Length; f++) {
						if (cachedFiles [f] != null && cachedFiles [f].Exists) {
							if (_tilePreloadTiles && cachedFiles [f].Name.StartsWith (PREFIX_MIN_ZOOM_LEVEL)) {
								continue;
							}

							_tileCurrentCacheUsage -= cachedFiles [f].Length;
							cachedFiles [f].Delete ();
						}
						if (_tileCurrentCacheUsage <= maxLocalCacheSize)
							break;
					}
				}

			}

			CheckTilesContent (_currentZoomLevel);

			spreadLoadAmongFrames = _tileMaxTileLoadsPerFrame;
		}

		void MonitorInactiveTiles () {
			int inactiveCount = inactiveTiles.Count;
			bool changes = false;
			bool releasedMemory = false;
			for (int k = 0; k < inactiveCount; k++) {
				TileInfo ti = inactiveTiles [k];
				if (ti == null || ti.gameObject == null || ti.visible || ti.texture == currentEarthTexture || ti.loadStatus != TILE_LOAD_STATUS.Loaded) {
					inactiveTiles [k] = null;
					ti.isAddedToInactive = false;
					changes = true;
					continue;
				}
				if (Time.time - ti.inactiveTime > _tileKeepAlive) {
					inactiveTiles [k] = null;
					ti.isAddedToInactive = false;
					ti.loadStatus = TILE_LOAD_STATUS.Inactive;
					// tile is now invisible, setup material for when it appears again:
					ti.ClearPlaceholderImage ();
					if (ti.source != TILE_SOURCE.Resources) {
						Destroy (ti.texture);
					}
					ti.texture = currentEarthTexture;
					// Reset parentcoords on children
					if (ti.children != null) {
						int cCount = ti.children.Count;
						for (int c = 0; c < cCount; c++) {
							TileInfo tiChild = ti.children [c];
							if (!tiChild.animationFinished) {
								tiChild.ClearPlaceholderImage ();
							}
						}

					}
					changes = true;
					releasedMemory = true;
				}
			}
			if (changes) {
				List<TileInfo> newInactiveList = new List<TileInfo> ();
				for (int k = 0; k < inactiveCount; k++) {
					if (inactiveTiles [k] != null)
						newInactiveList.Add (inactiveTiles [k]);
				}
				inactiveTiles.Clear ();
				inactiveTiles = newInactiveList;
				if (releasedMemory) {
					Resources.UnloadUnusedAssets ();
					GC.Collect ();
				}
			}
		}

		void CheckTilesContent (int currentZoomLevel) {
			int qCount = loadQueue.Count;
			bool cleanQueue = false;
			for (int k = 0; k < qCount; k++) {
				TileInfo ti = loadQueue [k];
				if (ti == null) {
					cleanQueue = true;
					continue;
				}
				if (ti.loadStatus == TILE_LOAD_STATUS.InQueue) {
					if (ti.zoomLevel <= currentZoomLevel && ti.visible) {
						if (_tilePreloadTiles && ti.zoomLevel == TILE_MIN_ZOOM_LEVEL && ReloadTextureFromCacheOrMarkForDownload (ti)) {
							loadQueue [k] = null;
							cleanQueue = true;
							continue;
						}
						if (_concurrentLoads <= _tileMaxConcurrentDownloads) {
							ti.loadStatus = TILE_LOAD_STATUS.Loading;
							_concurrentLoads++;
							StartCoroutine (LoadTileContentBackground (ti));
						}
					} else if (Time.time - ti.queueTime > TILE_MAX_QUEUE_TIME) {
						ti.loadStatus = TILE_LOAD_STATUS.Inactive;
						loadQueue [k] = null;
						cleanQueue = true;
					}
				}
			}

			if (cleanQueue) {
				if (newQueue == null) {
					newQueue = new List<TileInfo> (qCount);
				} else {
					newQueue.Clear ();
				}
				for (int k = 0; k < qCount; k++) {
					TileInfo ti = loadQueue [k];
					if (ti != null)
						newQueue.Add (ti);
				}
				loadQueue.Clear ();
				loadQueue.AddRange (newQueue);
			}
		}

		#if DEBUG_TILES
		GameObject root;

		void AddMark (Vector3 worldPos)
		{
			GameObject mark = GameObject.CreatePrimitive (PrimitiveType.Cube);
			mark.transform.SetParent (root.transform);
			mark.transform.position = worldPos;
			mark.transform.localScale = Vector3.one * 100f;
			mark.GetComponent<Renderer> ().material.color = Color.yellow;
		}
#endif

		void CheckTiles (TileInfo parent, int currentZoomLevel, int xTile, int yTile, int zoomLevel, int subquadIndex) {
			// Is this tile visible?
			TileInfo ti;
			int tileCode = GetTileHashCode (xTile, yTile, zoomLevel);
			if (!cachedTiles.TryGetValue (tileCode, out ti)) {
				ti = new TileInfo (xTile, yTile, zoomLevel, subquadIndex, currentEarthTexture);
				ti.parent = parent;
				if (parent != null) {
					if (parent.children == null)
						parent.children = new List<TileInfo> ();
					parent.children.Add (ti);
				}
				for (int k = 0; k < 4; k++) {
					Vector2 latlon = Conversion.GetLatLonFromTile (xTile + offsets [k].x, yTile + offsets [k].y, zoomLevel);
					ti.latlons [k] = latlon;
					ti.cornerLocalPos [k] = Conversion.GetLocalPositionFromLatLon (latlon);
				}
				cachedTiles [tileCode] = ti;
			}

            // Check if tile is within restricted area
            if (_tileRestrictToArea) {
                if (ti.latlons[0].x < _tileMinMaxLatLon.x || ti.latlons[2].x > _tileMinMaxLatLon.z || ti.latlons[0].y > _tileMinMaxLatLon.w || ti.latlons[2].y < _tileMinMaxLatLon.y) {
                    return;
                }
            }

			// Check if any tile corner is visible
			// Phase I
			Vector3 minWorldPos = Misc.Vector3max;
			Vector3 maxWorldPos = Misc.Vector3min;
			Vector3 tmp = Misc.Vector3zero;
			for (int c = 0; c < 4; c++) {
				Vector3 wpos = transform.TransformPoint (ti.cornerLocalPos [c]);
				ti.cornerWorldPos [c] = wpos;
				if (wpos.x < minWorldPos.x)
					minWorldPos.x = wpos.x;
				if (wpos.y < minWorldPos.y)
					minWorldPos.y = wpos.y;
				if (wpos.z < minWorldPos.z)
					minWorldPos.z = wpos.z;
				if (wpos.x > maxWorldPos.x)
					maxWorldPos.x = wpos.x;
				if (wpos.y > maxWorldPos.y)
					maxWorldPos.y = wpos.y;
				if (wpos.z > maxWorldPos.z)
					maxWorldPos.z = wpos.z;
			}

			FastVector.Average (ref minWorldPos, ref maxWorldPos, ref tmp);
			Bounds bounds = new Bounds (tmp, maxWorldPos - minWorldPos);
			Vector3 tileMidPoint = bounds.center;


#if DEBUG_TILES
			if (root == null) {
				root = new GameObject ();
				root.transform.SetParent (transform);
				root.transform.localPosition = Vector3.zero;
				root.transform.localRotation = Misc.QuaternionZero; //Quaternion.Euler (0, 0, 0);
			}
#endif

			bool insideViewport = false;
			float minX = currentCamera.pixelWidth * 2f, minY = currentCamera.pixelHeight * 2f;
			float maxX = -minX, maxY = -minY;
			for (int c = 0; c < 4; c++) {
				Vector3 scrPos = currentCamera.WorldToScreenPoint (ti.cornerWorldPos [c]);
				insideViewport = insideViewport || (scrPos.z > 0 && scrPos.x >= 0 && scrPos.x < currentCamera.pixelWidth && scrPos.y >= 0 && scrPos.y < currentCamera.pixelHeight);
				if (scrPos.x < minX)
					minX = scrPos.x;
				if (scrPos.x > maxX)
					maxX = scrPos.x;
				if (scrPos.y < minY)
					minY = scrPos.y;
				if (scrPos.y > maxY)
					maxY = scrPos.y;
			}
			if (!insideViewport) {
				insideViewport = GeometryUtility.TestPlanesAABB (cameraPlanes, bounds);
				if (!insideViewport && _wrapHorizontally && _wrapCamera.enabled) {
					insideViewport = GeometryUtility.TestPlanesAABB (wrapCameraPlanes, bounds);
				}
			}

			ti.insideViewport = insideViewport;
			ti.visible = false;
			if (insideViewport) {
				if (!ti.created) {
					CreateTile (ti);
				}

				if (!ti.gameObject.activeSelf) {
					ti.gameObject.SetActive (true);
				}

				// Manage hierarchy of tiles
				float aparentSize = maxY - minY;
				bool tileIsBig = aparentSize > currentTileSize;

				#if DEBUG_TILES
				if (ti.gameObject != null) {
					ti.gameObject.GetComponent<TileInfoEx> ().bigTile = tileIsBig;
					ti.gameObject.GetComponent<TileInfoEx> ().zoomLevel = ti.zoomLevel;
				}
				#endif

				if ((tileIsBig || zoomLevel < TILE_MIN_ZOOM_LEVEL) && zoomLevel < _tileMaxZoomLevel) {
					// Load nested tiles
					CheckTiles (ti, currentZoomLevel, xTile * 2, yTile * 2, zoomLevel + 1, 0);
					CheckTiles (ti, currentZoomLevel, xTile * 2 + 1, yTile * 2, zoomLevel + 1, 1);
					CheckTiles (ti, currentZoomLevel, xTile * 2, yTile * 2 + 1, zoomLevel + 1, 2);
					CheckTiles (ti, currentZoomLevel, xTile * 2 + 1, yTile * 2 + 1, zoomLevel + 1, 3);
					ti.renderer.enabled = false;
				} else {
					ti.visible = true;
					
					// Show tile renderer
					if (!ti.renderer.enabled) {
						ti.renderer.enabled = true;
					}

					// If parent tile is loaded then use that as placeholder texture
					if (ti.zoomLevel > TILE_MIN_ZOOM_LEVEL && ti.parent.loadStatus == TILE_LOAD_STATUS.Loaded && !ti.placeholderImageSet) {
						ti.placeholderImageSet = true;
						ti.parentTextureCoords = placeHolderUV [ti.subquadIndex];
						ti.SetPlaceholderImage (ti.parent.texture);
					}

					if (ti.loadStatus == TILE_LOAD_STATUS.Loaded) {
						if (!ti.hasAnimated) {
							ti.hasAnimated = true;
							ti.Animate (1f, AnimationEnded);
						}
					} else if (ti.loadStatus == TILE_LOAD_STATUS.Inactive) {
						ti.distToCamera = FastVector.SqrDistance (ref ti.cornerWorldPos [0], ref currentCameraPosition) * ti.zoomLevel;
						ti.loadStatus = TILE_LOAD_STATUS.InQueue;
						ti.queueTime = Time.time;
						loadQueue.Add (ti);
					}
					if (ti.children != null) {
						for (int k = 0; k < 4; k++) {
							TileInfo tiChild = ti.children [k];
							HideTile (tiChild);
						}
					}
				}
			} else {
				HideTile (ti);
			}
		}

		void HideTile (TileInfo ti) {
			if (ti.gameObject != null && ti.gameObject.activeSelf) {
				ti.gameObject.SetActive (false);
				ti.visible = false;
				if (ti.loadStatus == TILE_LOAD_STATUS.Loaded && ti.zoomLevel >= TILE_MIN_ZOOM_LEVEL) {
					if (_tilePreloadTiles && ti.zoomLevel == TILE_MIN_ZOOM_LEVEL)
						return;

					if (!ti.isAddedToInactive) {
						ti.isAddedToInactive = true;
						inactiveTiles.Add (ti);
					}
					ti.inactiveTime = Time.time;
				}
			}
		}

		void AnimationEnded (TileInfo ti) {
			shouldCheckTiles = true;
			// Switch tile material to solid
			TileInfo parent = ti.parent != null ? ti.parent : ti;
			ti.renderer.sharedMaterial = parent.normalMat;
		}

		int GetTileHashCode (int x, int y, int zoomLevel) {
			ZoomLevelInfo zi = zoomLevelsInfo [zoomLevel];
			if (zi == null)
				return 0;
			int hashCode = zi.zoomLevelHash + zi.yHash * y + x;
			return hashCode;
		}

		TileInfo GetTileInfo (int x, int y, int zoomLevel) {
			int tileCode = GetTileHashCode (x, y, zoomLevel);
			TileInfo ti = null;
			cachedTiles.TryGetValue (tileCode, out ti);
			return ti;
		}

		int GetTileZoomLevel () {
			// Get screen dimensions of central tile
			int zoomLevel0 = 1;
			int zoomLevel1 = TILE_MAX_ZOOM_LEVEL;
			int zoomLevel = TILE_MIN_ZOOM_LEVEL;
			Vector3 localPosition;
			if (!GetCurrentMapLocation(out localPosition)) return _currentZoomLevel;
			Vector2 latLon = Conversion.GetLatLonFromLocalPosition (localPosition);
			int xTile, yTile;
			currentTileSize = _tileSize > TILE_MIN_SIZE ? _tileSize : TILE_MIN_SIZE;
			currentTileSize *= (3.0f - _tileResolutionFactor);
			float dist = 0;
			for (int i = 0; i < 5; i++) {
				zoomLevel = (zoomLevel0 + zoomLevel1) / 2;
				Conversion.GetTileFromLatLon (zoomLevel, latLon.x, latLon.y, out xTile, out yTile);
				Vector2 latLonTL = Conversion.GetLatLonFromTile (xTile, yTile, zoomLevel);
				Vector2 latLonBR = Conversion.GetLatLonFromTile (xTile + 0.99999f, yTile + 0.99999f, zoomLevel);
				Vector3 localPointTL = Conversion.GetLocalPositionFromLatLon (latLonTL);
				Vector3 localPointBR = Conversion.GetLocalPositionFromLatLon (latLonBR);
				Vector3 wposTL = currentCamera.WorldToScreenPoint (transform.TransformPoint (localPointTL));
				Vector3 wposBR = currentCamera.WorldToScreenPoint (transform.TransformPoint (localPointBR));
				dist = Mathf.Max (Mathf.Abs (wposBR.x - wposTL.x), Mathf.Abs (wposTL.y - wposBR.y));
				if (dist > currentTileSize) {
					zoomLevel0 = zoomLevel;
				} else {
					zoomLevel1 = zoomLevel;
				}
			}
			if (dist > currentTileSize)
				zoomLevel++;

			zoomLevel = Mathf.Clamp (zoomLevel, TILE_MIN_ZOOM_LEVEL, TILE_MAX_ZOOM_LEVEL);
			return zoomLevel;
		}

		void CreateTile (TileInfo ti) {
			Vector2 latLonTL = ti.latlons [0];
			Vector2 latLonBR;
			ZoomLevelInfo zi = zoomLevelsInfo [ti.zoomLevel];
			int tileCode = GetTileHashCode (ti.x + 1, ti.y + 1, ti.zoomLevel);
			TileInfo cachedTile;
			if (cachedTiles.TryGetValue (tileCode, out cachedTile)) {
				latLonBR = cachedTile.latlons [0];
			} else {
				latLonBR = Conversion.GetLatLonFromTile (ti.x + 1, ti.y + 1, ti.zoomLevel);
			}
			// Create container
			GameObject parentObj;
			if (ti.parent == null) {
				parentObj = zi.tilesContainer;
				if (parentObj == null) {
					parentObj = new GameObject ("Tiles" + ti.zoomLevel);
					parentObj.transform.SetParent (tilesRoot, false);
					parentObj.layer = tilesRoot.gameObject.layer;
					zi.tilesContainer = parentObj;
				}
			} else {
				parentObj = ti.parent.gameObject;
			}

			// Prepare mesh vertices
			Vector3[] tileCorners = new Vector3[4];
			tileCorners [0] = Conversion.GetLocalPositionFromLatLon (latLonTL);
			tileCorners [1] = Conversion.GetLocalPositionFromLatLon (new Vector2 (latLonTL.x, latLonBR.y));
			tileCorners [2] = Conversion.GetLocalPositionFromLatLon (latLonBR);
			tileCorners [3] = Conversion.GetLocalPositionFromLatLon (new Vector2 (latLonBR.x, latLonTL.y));
			// Add small offset to avoid seams on higher zoom levels
			if (ti.zoomLevel > 150) {
				const float offset = 0.00000001f;
				tileCorners [1].x += offset;
				tileCorners [2].x += offset;
				tileCorners [2].y -= offset;
				tileCorners [3].y -= offset;
			}

			// Setup tile materials
			TileInfo parent = ti.parent != null ? ti.parent : ti;
			if (parent.normalMat == null) {
				parent.normalMat = Instantiate (tileMatRef);
				if (disposalManager != null) disposalManager.MarkForDisposal(parent.normalMat);
			}
			if (parent.transMat == null) {
				parent.transMat = Instantiate (tileMatTransRef);
				if (disposalManager != null) disposalManager.MarkForDisposal(parent.transMat);
			}
			
			Material tileMat = ti.zoomLevel < TILE_MIN_ZOOM_LEVEL ? parent.normalMat : parent.transMat;

			// UVs wrt Earth texture
			Vector2 tl = new Vector2 ((latLonTL.y + 180) / 360f, (latLonTL.x + 90) / 180f);
			Vector2 br = new Vector2 ((latLonBR.y + 180) / 360f, (latLonBR.x + 90) / 180f);
			if (tl.x > 0.5f && br.x < 0.5f) {
				br.x = 1f;
			}
			ti.worldTextureCoords = new Vector4 (tl.x, br.y, br.x, tl.y);
			ti.ClearPlaceholderImage ();

			if (ti.zoomLevel < TILE_MIN_ZOOM_LEVEL) {
				ti.loadStatus = TILE_LOAD_STATUS.Loaded;
			}

			ti.texture = currentEarthTexture;
			ti.renderer = CreateObject (parentObj.transform, "Tile", tileCorners, tileIndices, tileUV, tileMat, ti.subquadIndex);
			ti.gameObject = ti.renderer.gameObject;
			ti.renderer.enabled = false;
			ti.created = true;

#if DEBUG_TILES
			ti.gameObject.AddComponent<TileInfoEx> ();
#endif

		}

		internal IEnumerator LoadTileContentBackground (TileInfo ti) {
			yield return new WaitForEndOfFrame ();

			string url = GetTileURL (_tileServer, ti);
			if (string.IsNullOrEmpty (url)) {
				Debug.LogError ("Tile server url not set. Aborting");
				yield break;
			}

			long downloadedBytes = 0;
			string error = null;
			string filePath = "";
			byte[] textureBytes = null;
			ti.source = TILE_SOURCE.Unknown;

			// Check if tile is given by external event
			if (OnTileRequest != null) {
				if (OnTileRequest (ti.zoomLevel, ti.x, ti.y, out ti.texture, out error) && ti.texture != null) {
					ti.source = TILE_SOURCE.Resources;
				}
			}

			// Check if tile is in Resources
			if (ti.source == TILE_SOURCE.Unknown && _tileEnableOfflineTiles) {
				string path = GetTileResourcePath (ti.x, ti.y, ti.zoomLevel, false);
				ResourceRequest request = Resources.LoadAsync<Texture2D> (path);
				yield return request;
				if (request.asset != null) {
					ti.texture = (Texture2D)request.asset;
					ti.source = TILE_SOURCE.Resources;
				} else if (tileOfflineTilesOnly) {
					ti.texture = tileResourceFallbackTexture;
					ti.source = TILE_SOURCE.Resources;
				}
			}

            CustomWWW www = null;
			if (ti.source == TILE_SOURCE.Unknown) {
				www = getCachedWWW (url, ti);
				yield return www;
				error = www.error;
			}

			for (int tries = 0; tries < 100; tries++) {
				if (spreadLoadAmongFrames > 0)
					break;
				yield return new WaitForEndOfFrame ();
			}
			spreadLoadAmongFrames--;
			_concurrentLoads--;
			
			if (!ti.visible) {	// non visible textures are ignored
				ti.loadStatus = TILE_LOAD_STATUS.InQueue;
				yield break;
			}

			if (!string.IsNullOrEmpty (error)) {
				_tileLastError = "Error getting tile: " + error + " url=" + url;
				_tileLastErrorDate = DateTime.Now;
				if (_tileDebugErrors) {
					Debug.Log (_tileLastErrorDate + " " + _tileLastError);
				}
				ti.loadStatus = TILE_LOAD_STATUS.InQueue;
				yield break;
			}

			// Load texture
			if (ti.source != TILE_SOURCE.Resources) {
				downloadedBytes = www.bytesDownloaded;
				textureBytes = www.bytes;
				ti.texture = www.textureNonReadable;
				www.Dispose ();
				www = null;

				// Check texture consistency
				if (ti.loadedFromCache || _tileEnableLocalCache) {
					filePath = GetLocalFilePathForURL (url, ti);
				}

				if (ti.loadedFromCache && ti.texture.width <= 16) { // Invalid texture in local cache, retry
					if (File.Exists (filePath)) {
						File.Delete (filePath);
					}
					ti.loadStatus = TILE_LOAD_STATUS.Inactive;
					ti.queueTime = Time.time;
					yield break;
				}
			}

			ti.texture.wrapMode = TextureWrapMode.Clamp;
			_tileSize = ti.texture.width;

			// Save texture
			if (_tileEnableLocalCache && ti.source != TILE_SOURCE.Resources && !File.Exists (filePath)) {
				_tileCurrentCacheUsage += textureBytes.Length;
				BackgroundSaver saver = new BackgroundSaver (textureBytes, filePath);
				saver.Start ();
			}

			// Update stats
			switch (ti.source) {
			case TILE_SOURCE.Cache:
				_cacheLoads++;
				_cacheLoadTotalSize += downloadedBytes;
				break;
			case TILE_SOURCE.Resources:
				_resourceLoads++;
				break;
			default:
				_webDownloads++;
				_webDownloadTotalSize += downloadedBytes;
				break;
			}

			if (loadQueue.Contains (ti)) {
				loadQueue.Remove (ti);
			}

			FinishLoadingTile (ti);
		}



		Renderer CreateObject (Transform parent, string name, Vector3[] vertices, int[] indices, Vector2[] uv, Material mat, int subquadIndex) {
			GameObject obj = new GameObject (name);
			obj.transform.SetParent (parent, false);
			obj.layer = parent.gameObject.layer;
			obj.transform.localPosition = Misc.Vector3zero;
			obj.transform.localScale = Misc.Vector3one;
			obj.transform.localRotation = Misc.QuaternionZero;
			Mesh mesh = new Mesh ();
			mesh.vertices = vertices;
			mesh.triangles = indices;
			mesh.uv = uv;
			Color[] meshColor;
			if (vertices.Length != 4) {
				meshColor = new Color[vertices.Length];
				for (int k = 0; k < vertices.Length; k++)
					meshColor [k] = color1000;
			} else {
				meshColor = meshColors [subquadIndex];
			}
			mesh.colors = meshColor;
			MeshFilter mf = obj.AddComponent<MeshFilter> ();
			mf.sharedMesh = mesh;
			MeshRenderer mr = obj.AddComponent<MeshRenderer> ();
			mr.sharedMaterial = mat;
			mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			mr.receiveShadows = false;
			return mr;
		}

		class BackgroundSaver {
			byte[] tex;
			string filePath;

			public BackgroundSaver (byte[] tex, string filePath) {
				this.tex = tex;
				this.filePath = filePath;
			}

			public void Start () {
				#if UNITY_WSA && !UNITY_EDITOR
                Task.Run(() => SaveTextureToCache());
				#elif UNITY_WEBGL
				SaveTextureToCache();
				#else
				Thread thread = new Thread (SaveTextureToCache);
				thread.Start ();
				#endif
			}

			void SaveTextureToCache () {
				File.WriteAllBytes (filePath, tex);
			}

		}

		StringBuilder filePathStr = new StringBuilder (250);

		string GetLocalFilePathForURL (string url, TileInfo ti) {
			filePathStr.Length = 0;
			filePathStr.Append (cachePath);
			filePathStr.Append ("/z");
			filePathStr.Append (ti.zoomLevel);
			filePathStr.Append ("_x");
			filePathStr.Append (ti.x);
			filePathStr.Append ("_y");
			filePathStr.Append (ti.y);
			filePathStr.Append ("_");
			filePathStr.Append (url.GetHashCode ());
			filePathStr.Append (".png");
			return filePathStr.ToString ();
		}


		public string GetTileResourcePath (int x, int y, int zoomLevel, bool fullPath = true) {
			filePathStr.Length = 0;
			if (fullPath) {
				filePathStr.Append (_tileResourcePathBase);
				filePathStr.Append ("/");
			}
			filePathStr.Append ("Tiles");
			filePathStr.Append ("/");
			filePathStr.Append ((int)_tileServer);
			filePathStr.Append ("/z");
			filePathStr.Append (zoomLevel);
			filePathStr.Append ("_x");
			filePathStr.Append (x);
			filePathStr.Append ("_y");
			filePathStr.Append (y);
			if (fullPath) {
				filePathStr.Append (".png");
			}
			return filePathStr.ToString ();
		}


		CustomWWW getCachedWWW (string url, TileInfo ti) {
			string filePath = GetLocalFilePathForURL (url, ti);
            CustomWWW www;
			bool useCached = false;
			useCached = _tileEnableLocalCache && System.IO.File.Exists (filePath);
			if (useCached) {
				if (!_tilePreloadTiles || !filePath.Contains (PREFIX_MIN_ZOOM_LEVEL)) {
					//check how old
					System.DateTime written = File.GetLastWriteTimeUtc (filePath);
					System.DateTime now = System.DateTime.UtcNow;
					double totalHours = now.Subtract (written).TotalHours;
					if (totalHours > 300) {
						File.Delete (filePath);
						useCached = false;
					}
				}
			}
			ti.source = useCached ? TILE_SOURCE.Cache : TILE_SOURCE.Online;
			if (useCached) {
#if UNITY_STANDALONE_WIN || UNITY_WSA
				string pathforwww = "file:///" + filePath;
#else
				string pathforwww = "file://" + filePath;
#endif
                www = new CustomWWW (pathforwww);
			} else {
                www = new CustomWWW (url);
			}
			return www;
		}

		bool ReloadTextureFromCacheOrMarkForDownload (TileInfo ti) {
			if (!_tileEnableLocalCache)
				return false;

			string url = GetTileURL (_tileServer, ti);
			if (string.IsNullOrEmpty (url)) {
				return false;
			}

			string filePath = GetLocalFilePathForURL (url, ti);
			if (System.IO.File.Exists (filePath)) {
				//check how old
				System.DateTime written = File.GetLastWriteTimeUtc (filePath);
				System.DateTime now = System.DateTime.UtcNow;
				double totalHours = now.Subtract (written).TotalHours;
				if (totalHours > 300) {
					File.Delete (filePath);
					return false;
				}
			} else {
				return false;
			}
			byte[] bb = System.IO.File.ReadAllBytes (filePath);
			ti.texture = new Texture2D (0, 0);
			ti.texture.LoadImage (bb);
			if (ti.texture.width <= 16) { // Invalid texture in local cache, retry
				if (File.Exists (filePath)) {
					File.Delete (filePath);
				}
				return false;
			}
			ti.texture.wrapMode = TextureWrapMode.Clamp;

			_cacheLoads++;
			_cacheLoadTotalSize += bb.Length;

			FinishLoadingTile (ti);
			return true;
		}

		void FinishLoadingTile (TileInfo ti) {
			// Good to go, update tile info
			ti.SetTexture (ti.texture);
			ti.loadStatus = TILE_LOAD_STATUS.Loaded;
			shouldCheckTiles = true;
		}

	}

}