using System;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

	public partial class Country: AdminEntity {

		/// <summary>
		/// Continent name.
		/// </summary>
		public string continent;

		/// <summary>
		/// List of provinces that belongs to this country.
		/// </summary>
		public Province[] provinces;

		/// <summary>
		/// The index of the capital city
		/// </summary>
		public int capitalCityIndex = -1;


		int[] _neighbours;

		/// <summary>
		/// Custom array of countries that could be reached from this country. Useful for Country path-finding.
		/// It defaults to natural neighbours of the country but you can modify its contents and add your own potential destinations per country.
		/// </summary>
		public override int[] neighbours {
			get {
				if (_neighbours == null) {
					int cc = 0;
					List<Country> nn = new List<Country>();
					if (regions != null) {
						regions.ForEach(r => {
								if (r != null && r.neighbours != null) {
									r.neighbours.ForEach(n => {
											if (n != null) {
												Country otherCountry = (Country)n.entity;
												if (!nn.Contains(otherCountry))
													nn.Add(otherCountry);
											}
										}
									);

								}
							});
						cc = nn.Count;
					}
					_neighbours = new int[cc];
					for (int k = 0; k < cc; k++) {
						_neighbours[k] = WMSK.instance.GetCountryIndex(nn[k]);
					}
				}
				return _neighbours;
			}
			set {
				_neighbours = value;
			}
		}

        /// <summary>
        /// True for a country acting as a provinces pool created by CreateCountryProvincesPool function.
        /// The effect of this field is that all transfer operations will ignore its borders which results in a faster operation
        /// </summary>
        public bool isPool;

		// Standardized codes
		public string fips10_4 = "";
		public string iso_a2 = "";
		public string iso_a3 = "";
		public string iso_n3 = "";

        /// <summary>
        /// If provinces will be shown for this country
        /// </summary>
        public bool showProvinces = true;

        /// <summary>
        /// If province highlighting is enabled for this country
        /// </summary>
        public bool allowProvincesHighlight = true;

		/// <summary>
        /// Current number of visible cities of this country
        /// </summary>
		[NonSerialized]
		public int visibleCities;

		/// <summary>
		/// Creates a new country
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="continent">Continent.</param>
		/// <param name="uniqueId">For user created countries, use a number between 1-999 to uniquely identify this country.</param>
		public Country(string name, string continent, int uniqueId) {
			this.name = name;
			this.continent = continent;
			this.regions = new List<Region>();
			this.uniqueId = uniqueId;
			this.attrib = new JSONObject();
			this.mainRegionIndex = -1;
			this.provinces = new Province[0];
		}

		public Country Clone() {
			Country c = new Country(name, continent, uniqueId);
			c.center = center;
            c.regions = new List<Region>(regions.Count);
            for (int k=0;k<regions.Count;k++) {
                c.regions.Add(regions[k].Clone());
            }
			c.customLabel = customLabel;
			c.labelColor = labelColor;
			c.labelColorOverride = labelColorOverride;
			c.labelFontOverride = labelFontOverride;
			c.labelVisible = labelVisible;
			c.labelOffset = labelOffset;
			c.labelRotation = labelRotation;
			c.provinces = provinces;
			c.hidden = this.hidden;
			c.attrib = new JSONObject();
			c.attrib.Absorb(attrib);
            c.regionsRect2D = regionsRect2D;
			return c;
		}

	}

    [Serializable]
	public struct CountryJSON {
		public string name;
		public string continent;
		public List<RegionJSON> regions;
		public bool hidden;
		public int uniqueId;
		public string fips10_4;
        public string iso_a2;
        public string iso_a3;
        public string iso_n3;
		public JSONObject attrib;
    }

    [Serializable]
	public class CountriesJSONData {
		public List<CountryJSON> countries = new List<CountryJSON>();
    }

}