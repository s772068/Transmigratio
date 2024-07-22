using System.Collections.Generic;
using WorldMapStrategyKit;
using Newtonsoft.Json;
using UnityEngine;

namespace Database.Data {
    /// <summary>
    /// Класс для работы с картой. Через него же взаимодействие с wmsk
    /// </summary>
    [System.Serializable]
    public class Map {
        public List<Region> AllRegions;

        [HideInInspector] public WMSK WMSK;

        public void Init() {
            WMSK = WMSK.instance;

            AllRegions = new List<Region>();
            AllRegions.Clear();

            string json;        //сюда будет записываться json

            TextAsset textAsset = Resources.Load<TextAsset>("RegionStartParams");
            json = textAsset.text;
            /*
            #if UNITY_EDITOR || UNITY_IOS
                    path = Application.streamingAssetsPath + "/Config/RegionStartParams.json";
                    Transmigratio.AddingDebugText.Invoke("Unity editor or iOS\n");
                    json = File.ReadAllText(path);
            #elif UNITY_ANDROID
                    path = "jar:file://" + Application.dataPath + "!/assets/Config/RegionStartParams.json";
                    WWW reader = new WWW(path);
                    while (!reader.isDone) { }
                    json = reader.text;
                    Transmigratio.AddingDebugText.Invoke("Android\n");
            #endif
            */

            AllRegions = JsonConvert.DeserializeObject<List<Region>>(json);

            //allRegions = JsonUtility.FromJson<List<TM_Region>>(json);

            foreach (Region region in AllRegions) {
                WMSK.ToggleCountrySurface(region.Id, true, Color.clear);
                region.Init();
            }
        }

        public Region GetRegionBywmskId(int WMSKId) {
            foreach (Region region in AllRegions) {
                if (region.Id == WMSKId) return region;
            }
            return null;
        }
    }
}