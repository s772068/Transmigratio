// World Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK

using UnityEngine;
using System;
using System.Linq;
using System.Threading;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

    public delegate bool OnTileRequestEvent(int zoomLevel, int x, int y, out Texture2D texture, out string error);


    public partial class WMSK : MonoBehaviour {

        public OnTileRequestEvent OnTileRequest;


        [SerializeField]
        int _tileMaxConcurrentDownloads = 10;

        /// <summary>
        /// Gets or sets the maximum number of concurrent web downloads at a given time.
        /// </summary>
        public int tileMaxConcurrentDownloads {
            get { return _tileMaxConcurrentDownloads; }
            set {
                if (_tileMaxConcurrentDownloads != value) {
                    _tileMaxConcurrentDownloads = Mathf.Max(value, 1);
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        int _tileMaxTileLoadsPerFrame = 2;

        /// <summary>
        /// Gets or sets the maximum number of tile loads per frame.
        /// </summary>
        public int tileMaxTileLoadsPerFrame {
            get { return _tileMaxTileLoadsPerFrame; }
            set {
                if (_tileMaxTileLoadsPerFrame != value) {
                    _tileMaxTileLoadsPerFrame = Mathf.Max(value, 1);
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _showTiles;

        /// <summary>
        /// Enables or disables integration with Online Tile Systems
        /// </summary>
        public bool showTiles {
            get { return _showTiles; }
            set {
                if (_showTiles != value) {
                    _showTiles = value;
                    RestyleEarth();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _tileTransparentLayer = false;

        /// <summary>
        /// Blends tiles with background imagery. Disabled for performance purposes (when disabled, tiles will use an opaque shader which renders faster)
        /// </summary>
        public bool tileTransparentLayer {
            get { return _tileTransparentLayer; }
            set {
                if (_tileTransparentLayer != value) {
                    _tileTransparentLayer = value;
                    RestyleEarth();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        [Range(0, 1f)]
        float _tileMaxAlpha = 1f;

        /// <summary>
        /// Gets or sets the tile max alpha (transparency). Reduce to force transparent layers when tiles are opaque.
        /// </summary>
        public float tileMaxAlpha {
            get { return _tileMaxAlpha; }
            set {
                if (_tileMaxAlpha != value) {
                    _tileMaxAlpha = Mathf.Clamp01(value);
                    ResetTiles();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _tileEnableLocalCache = true;

        /// <summary>
        /// Enables or disables local cache for tile storage.
        /// </summary>
        public bool tileEnableLocalCache {
            get { return _tileEnableLocalCache; }
            set {
                if (_tileEnableLocalCache != value) {
                    _tileEnableLocalCache = value;
                    isDirty = true;
                    PurgeCacheOldFiles();
                }
            }
        }

        [SerializeField]
        long _tileMaxLocalCacheSize = 200;

        /// <summary>
        /// Gets or sets the size of the local cache in Mb.
        /// </summary>
        public long tileMaxLocalCacheSize {
            get { return _tileMaxLocalCacheSize; }
            set {
                if (_tileMaxLocalCacheSize != value) {
                    _tileMaxLocalCacheSize = value;
                    isDirty = true;
                }
            }
        }


        /// <summary>
        /// Gets number of tiles pending load
        /// </summary>
        public int tileQueueLength {
            get { return loadQueue == null ? 0 : loadQueue.Count; }
        }

        /// <summary>
        /// Gets current active tile downloads
        /// </summary>
        public int tileConcurrentLoads {
            get { return _concurrentLoads; }
        }

        /// <summary>
        /// Gets current tile zoom level
        /// </summary>
        public int tileCurrentZoomLevel {
            get { return _currentZoomLevel; }
        }

        /// <summary>
        /// Gets number of total tiles downloaded from web
        /// </summary>
        public int tileWebDownloads {
            get { return _webDownloads; }
        }

        /// <summary>
        /// Gets number of total tiles downloaded from the application Resources folder
        /// </summary>
        public int tileResourceDownloads {
            get { return _resourceLoads; }
        }

        /// <summary>
        /// Gets total size in byte sof tiles downloaded from web
        /// </summary>
        public long tileWebDownloadsTotalSize {
            get { return _webDownloadTotalSize; }
        }

        /// <summary>
        /// Gets number of total tiles downloaded from local cache
        /// </summary>
        public int tileCacheLoads {
            get { return _cacheLoads; }
        }

        /// <summary>
        /// Gets total size in byte sof tiles downloaded from local cache
        /// </summary>
        public long tileCacheLoadsTotalSize {
            get { return _cacheLoadTotalSize; }
        }

        [SerializeField]
        TILE_SERVER _tileServer = TILE_SERVER.OpenStreeMap;

        /// <summary>
        /// Gets or sets the tile server.
        /// </summary>
        public TILE_SERVER tileServer {
            get { return _tileServer; }
            set {
                if (_tileServer != value) {
                    _tileServer = value;
                    ResetTiles();
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        string _tileServerCustomUrl = "http://$N$.tile.openstreetmap.org/$Z$/$X$/$Y$.png";

        /// <summary>
        /// Gets or sets the tile server Url. Only used whtn Tile Server is set to Custom.
        /// </summary>
        public string tileServerCustomUrl {
            get { return _tileServerCustomUrl; }
            set {
                if (_tileServerCustomUrl != value) {
                    _tileServerCustomUrl = value;
                    ResetTiles();
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        [Range(1f, 2f)]
        float _tileResolutionFactor = 1.5f;

        /// <summary>
        /// Gets or sets the tile resolution factor.
        /// </summary>
        public float tileResolutionFactor {
            get { return _tileResolutionFactor; }
            set {
                if (_tileResolutionFactor != value) {
                    _tileResolutionFactor = Mathf.Clamp(value, 1f, 2f);
                    isDirty = true;
                }
            }
        }

        /// <summary>
        /// Returns the credits or copyright message required to be displayed with the active tile server. Returns null if credit not required.
        /// </summary>
        /// <value>The tile server credits.</value>
        public string tileServerCopyrightNotice {
            get {
                if (string.IsNullOrEmpty(_tileServerCopyrightNotice)) {
                    _tileServerCopyrightNotice = GetTileServerCopyrightNotice(_tileServer);
                }
                return _tileServerCopyrightNotice;

            }
        }


        /// <summary>
        /// Returns last logged error
        /// </summary>
        public string tileLastError {
            get { return _tileLastError; }
        }

        /// <summary>
        /// Returns last logged error date & time
        /// </summary>
        public DateTime tileLastErrorDate {
            get { return _tileLastErrorDate; }
        }


        [SerializeField]
        string _tileServerAPIKey;

        /// <summary>
        /// Returns current tile server API key
        /// </summary>
        public string tileServerAPIKey {
            get { return _tileServerAPIKey; }
            set {
                if (_tileServerAPIKey != value) {
                    _tileServerAPIKey = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        string _tileServerClientId;

        /// <summary>
        /// Returns current tile server client id used by some providers
        /// </summary>
        public string tileServerClientId {
            get { return _tileServerClientId; }
            set {
                if (_tileServerClientId != value) {
                    _tileServerClientId = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        string _tileServerLayerTypes;

        /// <summary>
        /// Returns current tile server layer types used by some providers
        /// </summary>
        public string tileServerLayerTypes {
            get { return _tileServerLayerTypes; }
            set {
                if (_tileServerLayerTypes != value) {
                    _tileServerLayerTypes = value;
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        string _tileServerTimeOffset = "current";

        /// <summary>
        /// Returns current tile server time offset used by some providers
        /// </summary>
        public string tileServerTimeOffset {
            get { return _tileServerTimeOffset; }
            set {
                if (_tileServerTimeOffset != value) {
                    _tileServerTimeOffset = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _tileDebugErrors = true;

        /// <summary>
        /// Enables/disables error dump to console or log file.
        /// </summary>
        public bool tileDebugErrors {
            get { return _tileDebugErrors; }
            set {
                if (_tileDebugErrors != value) {
                    _tileDebugErrors = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        bool _tilePreloadTiles = false;

        /// <summary>
        /// Tries to load all first zoom level of tiles at start so map shows complete from the beginning
        /// </summary>
        public bool tilePreloadTiles {
            get { return _tilePreloadTiles; }
            set {
                if (_tilePreloadTiles != value) {
                    _tilePreloadTiles = value;
                    isDirty = true;
                }
            }
        }


        /// <summary>
        /// Returns the current disk usage of the tile cache in bytes.
        /// </summary>
        public long tileCurrentCacheUsage {
            get { return _tileCurrentCacheUsage; }
        }

        [SerializeField]
        float _tileKeepAlive = 15f;

        /// <summary>
        /// Time that an inactive tile remains in memory before being destroyed
        /// </summary>
        public float tileKeepAlive {
            get { return _tileKeepAlive; }
            set {
                if (_tileKeepAlive != value) {
                    _tileKeepAlive = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _tileEnableOfflineTiles;

        /// <summary>
        /// Enables or disables loading tiles from application resources.
        /// </summary>
        public bool tileEnableOfflineTiles {
            get { return _tileEnableOfflineTiles; }
            set {
                if (_tileEnableOfflineTiles != value) {
                    _tileEnableOfflineTiles = value;
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        bool _tileOfflineTilesOnly = true;

        /// <summary>
        /// If enabled, only tiles from Resources path will be loaded
        /// </summary>
        public bool tileOfflineTilesOnly {
            get { return _tileOfflineTilesOnly; }
            set {
                if (_tileOfflineTilesOnly != value) {
                    _tileOfflineTilesOnly = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        string _tileResourcePathBase = "Assets/Resources";

        /// <summary>
        /// Returns path for the application resource path where tiles can be stored using the tile downloader
        /// </summary>
        public string tileResourcePathBase {
            get { return _tileResourcePathBase; }
            set {
                if (_tileResourcePathBase != value) {
                    _tileResourcePathBase = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        int _tileMaxZoomLevel = TILE_MAX_ZOOM_LEVEL;

        /// <summary>
        /// Gets or sets the maximum zoom level for the tiles.
        /// </summary>
        public int tileMaxZoomLevel {
            get { return _tileMaxZoomLevel; }
            set {
                if (_tileMaxZoomLevel != value) {
                    _tileMaxZoomLevel = Mathf.Clamp(value, 0, TILE_MAX_ZOOM_LEVEL);
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        int _tileLinesMaxZoomLevel = 10;

        /// <summary>
        /// Gets or sets the maximum zoom level on which frontiers and other lines can be drawn over the map
        /// </summary>
        public int tileLinesMaxZoomLevel {
            get { return _tileLinesMaxZoomLevel; }
            set {
                if (_tileLinesMaxZoomLevel != value) {
                    _tileLinesMaxZoomLevel = Mathf.Clamp(value, 0, TILE_MAX_ZOOM_LEVEL);
                    isDirty = true;
                }
            }
        }



        [SerializeField]
        Texture2D _tileResourceFallbackTexture;

        /// <summary>
        /// Texture for a tile which is not found in Resources path and tileOfflineTilesOnly is enabled
        /// </summary>
        public Texture2D tileResourceFallbackTexture {
            get {
                if (_tileResourceFallbackTexture == null) {
                    return Texture2D.blackTexture;
                } else {
                    return _tileResourceFallbackTexture;
                }
            }
            set {
                if (_tileResourceFallbackTexture != value) {
                    _tileResourceFallbackTexture = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool _tileRestrictToArea;

        /// <summary>
        /// Enables area restriction
        /// </summary>
        public bool tileRestrictToArea {
            get { return _tileRestrictToArea; }
            set { if (value != _tileRestrictToArea) { _tileRestrictToArea = value; isDirty = true; } }
        }

        [SerializeField]
        Vector4 _tileMinMaxLatLon;

        /// <summary>
        /// Minimum / maximum latitude/longitude allowed for fetching tiles. Only applies if tileRestrictToArea is enabled.
        /// </summary>
        public Vector4 tileMinMaxLatLon {
            get { return _tileMinMaxLatLon; }
            set { if (value != _tileMinMaxLatLon) { _tileMinMaxLatLon = value; isDirty = true; }  }
        }

        public void PurgeTileCache() {
            PurgeCacheOldFiles(0);
        }


        /// <summary>
        /// Navigates to a given tile
        /// </summary>
        public void FlyToTile(int x, int y, int zoomLevel) {
            FlyToTile(x, y, zoomLevel, _navigationTime);

        }

        /// <summary>
        /// Navigates to a given tile
        /// </summary>
        public void FlyToTile(int x, int y, int zoomLevel, float duration) {
            Vector2 mapPos = Conversion.GetLocalPositionFromTile(x + 0.5f, y + 0.5f, zoomLevel);
            float tileWidth = transform.localScale.x / Mathf.Pow(2, zoomLevel);
            float zl = GetFrustumZoomLevel(tileWidth, tileWidth);
            FlyToLocation(mapPos, duration, zl);
        }
    }

}