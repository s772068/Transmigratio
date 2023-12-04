using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using WorldMapStrategyKit.MapGenerator;

namespace WorldMapStrategyKit {


    [Serializable]
    public struct NoiseOctave {
        public bool disabled;
        public float frequency;
        public float amplitude;
        public bool ridgeNoise;
    }


    public enum MapGenerationQuality {
        Draft = 0,
        Final = 1
    }

    public enum HeightMapGradientPreset {
        ColorByHeight = 0,
        ColorByHeightLight = 1,
        Grayscale = 10,
        BlackAndWhite = 11,
        RandomHSVColors = 12,
        Custom = 20
    }



    public partial class WMSK_Editor : MonoBehaviour {

        // Custom inspector stuff
        public const int MAX_TERRITORIES = 256;
        public const int MAX_CELLS = 10000;
        public const int MAX_CELLS_SQRT = 100;
        public const int MAX_CELLS_FOR_RELAXATION = 5000;

        // suffix for the path after Resources/WMSK/Geodata location
        public string outputFolder = "CustomMap";

        public bool changeStyle = true;

        [Range(0, 10000)]
        public int seed = 1;

        [Range(0, 10000)]
        public int seedNames = 1;

        [SerializeField, Range(1, 32)]
        public int gridRelaxation = 1;

        int goodGridRelaxation {
            get {
                if (numCells >= MAX_CELLS_FOR_RELAXATION) {
                    return 1;
                } else {
                    return gridRelaxation;
                }
            }
        }

        [SerializeField, Range(0.001f, 0.1f)]
        public float edgeMaxLength = 0.05f;


        [SerializeField, Range(0f, 1f)]
        public float edgeNoise = 0.25f;

        public MapGenerationQuality mapGenerationQuality;

        public bool generateCities;

        [NonSerialized]
        public readonly List<City> mapCities = new List<City>();

        public int numCitiesPerCountryMin = 3;
        public int numCitiesPerCountryMax = 10;

        [NonSerialized]
        public List<MapCountry> mapCountries;

        /// <summary>
        /// Gets or sets the number of territories.
        /// </summary>
        [Range(1, MAX_TERRITORIES)]
        public int numCountries = 32;

        public int backgroundTextureWidth = 2048;
        public int backgroundTextureHeight = 1024;
        [NonSerialized]
        public Texture2D backgroundTexture;

        [NonSerialized]
        public Texture2D heightMapTexture;

        [NonSerialized]
        public Texture2D waterMaskTexture;

        public Texture2D userHeightMapTexture;
        public int heightMapWidth = 2048;
        public int heightMapHeight = 1024;
        public HeightMapGradientPreset heightGradientPreset = HeightMapGradientPreset.Grayscale;
        [Range(0, 1)] public float heightGradientMinHue;
        [Range(0, 1)] public float heightGradientMaxHue = 1;
        [Range(0, 1)] public float heightGradientMinSaturation;
        [Range(0, 1)] public float heightGradientMaxSaturation = 1f;
        [Range(0, 1)] public float heightGradientMinValue;
        [Range(0, 1)] public float heightGradientMaxValue = 1f;

        public bool gradientPerPixel;
        public Gradient heightGradient;

        // if colors must be applied to provinces
        public bool colorProvinces = true;

        // enhance borders in background texture
        public bool drawBorders = true;
        [Range(0, 1)]
        public float bordersIntensity = 0.65f;

        public int bordersWidth = 4;

        // blur/smooth borders
        public bool smoothBorders = true;

        // draw coast shoreline
        public bool drawSeaShoreline = true;

        public int shorelineWidth = 4;

        [NonSerialized]
        public Texture2D pathFindingLandMapTexture, pathFindingWaterMapTexture;

        public bool octavesBySeed = true;
        public NoiseOctave[] noiseOctaves;

        [Range(0.01f, 7f)]
        public float noisePower = 3f;

        [Range(0, 16)]
        public float islandFactor = 0.5f;

        public Rect landRect = new Rect(-0.5f, -0.5f, 1f, 1f);

        [Range(0, 1f)]
        public float seaLevel = 0.2f;

        public Color seaColor = new Color(0, 0.4f, 1f);

        [Range(-1, 1f)]
        public float elevationShift = 0f;

        /// <summary>
        /// Complete array of states and cells and the territory name they belong to.
        /// </summary>
        [NonSerialized]
        public List<MapCell> mapCells;

        [Range(2, MAX_CELLS)]
        public int numCells = 256;

        public bool generateProvinces;

        [NonSerialized]
        public List<MapProvince> mapProvinces;

        public int numProvincesPerCountryMin = 2;
        public int numProvincesPerCountryMax = 8;

        [NonSerialized]
        public List<Vector2> voronoiSites;

        public bool generateNormalMap;
        public float normalMapBumpiness = 0.1f;

        public bool generateScenicWaterMask;
        public bool generatePathfindingMaps;


    }
}

