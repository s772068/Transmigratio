using System;
using System.Collections.Generic;

namespace WorldMapStrategyKit {
    public partial class Province : AdminEntity {

        int[] _neighbours;

        /// <summary>
        /// Custom array of provinces that could be reached from this province. Useful for Province path-finding.
        /// It defaults to natural neighbours of the province but you can modify its contents and add your own potential destinations per province.
        /// </summary>
        public override int[] neighbours {
            get {
                if (_neighbours == null) {
                    int cc = 0;
                    List<Province> nn = new List<Province>();
                    if (regions != null) {
                        regions.ForEach(r => {
                            if (r != null && r.neighbours != null) {
                                r.neighbours.ForEach(n => {
                                    if (n != null) {
                                        Province otherProvince = (Province)n.entity;
                                        if (!nn.Contains(otherProvince))
                                            nn.Add(otherProvince);
                                    }
                                }
                                );

                            }
                        });
                        cc = nn.Count;
                    }
                    _neighbours = new int[cc];
                    for (int k = 0; k < cc; k++) {
                        _neighbours[k] = WMSK.instance.GetProvinceIndex(nn[k]);
                    }
                }
                return _neighbours;
            }
            set {
                _neighbours = value;
            }
        }

        #region internal fields

        // Used internally. Don't change fields below.
        public string packedRegions;
        public int countryIndex;

        #endregion

        public Province(string name, int countryIndex, int uniqueId) {
            this.name = name;
            this.countryIndex = countryIndex;
            this.regions = null; // lazy load during runtime due to size of data
            this.center = Misc.Vector2zero;
            this.uniqueId = uniqueId;
            this.attrib = new JSONObject();
            this.mainRegionIndex = -1;
        }

        public Province Clone() {
            Province p = new Province(name, countryIndex, uniqueId);
            p.countryIndex = countryIndex;
            if (regions != null) {
                p.regions = new List<Region>(regions.Count);
                for (int k = 0; k < regions.Count; k++) {
                    p.regions.Add(regions[k].Clone());
                }
            }
            p.center = center;
            p.mainRegionIndex = mainRegionIndex;
            p.attrib = new JSONObject();
            p.attrib.Absorb(attrib);
            p.regionsRect2D = regionsRect2D;
            return p;
        }

    }


    [Serializable]
    public struct ProvinceJSON {
        public string name;
        public string countryName;
        public List<RegionJSON> regions;
        public int uniqueId;
        public JSONObject attrib;
    }

    [Serializable]
    public class ProvincesJSONData {
        public List<ProvinceJSON> provinces = new List<ProvinceJSON>();
    }
}

