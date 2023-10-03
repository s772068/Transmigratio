using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using WorldMapStrategyKit.MapGenerator;
using WorldMapStrategyKit.MapGenerator.Geom;

namespace WorldMapStrategyKit {

    public partial class WMSK_Editor : MonoBehaviour {

        #region Map generation

        Dictionary<Segment, MapRegion> cellNeighbourHit;
        Dictionary<Segment, MapRegion> territoryNeighbourHit;
        List<Segment> territoryFrontiers;
        StringBuilder sb;
        readonly HashSet<string> usedNames = new HashSet<string>();

        Point[] centers;
        VoronoiFortune voronoi;

        public void ApplySeed() {
            UnityEngine.Random.InitState(seed);
            if (octavesBySeed || noiseOctaves == null || noiseOctaves.Length == 0) {
                int octaves = UnityEngine.Random.Range(5, 8);
                noiseOctaves = new NoiseOctave[octaves];
                for (int k = 0; k < octaves; k++) {
                    NoiseOctave octave = new NoiseOctave();
                    octave.amplitude = 1f / (k + 1f);
                    octave.frequency = Mathf.Pow(2, k) * (1f + UnityEngine.Random.value);
                    noiseOctaves[k] = octave;
                }
            }
        }


        void VoronoiTesselation() {
            bool usesUserDefinedSites = false;
            if (voronoiSites != null && voronoiSites.Count > 0) {
                numCells = voronoiSites.Count;
                usesUserDefinedSites = true;
            }
            if (centers == null || centers.Length != numCells) {
                centers = new Point[numCells];
            }
            for (int k = 0; k < centers.Length; k++) {
                if (usesUserDefinedSites) {
                    Vector2 p = voronoiSites[k];
                    centers[k] = new Point(p.x, p.y);
                } else {
                    centers[k] = new Point(UnityEngine.Random.Range(-0.49f, 0.49f), UnityEngine.Random.Range(-0.49f, 0.49f));
                }
            }

            if (voronoi == null) {
                voronoi = new VoronoiFortune();
            }
            for (int k = 0; k < goodGridRelaxation; k++) {
                voronoi.AssignData(centers);
                voronoi.DoVoronoi();
                if (k < goodGridRelaxation - 1) {
                    for (int j = 0; j < numCells; j++) {
                        Point centroid = voronoi.cells[j].centroid;
                        centers[j] = (centers[j] + centroid) / 2;
                    }
                }
            }

            // Make cell regions: we assume cells have only 1 region but that can change in the future
            for (int k = 0; k < voronoi.cells.Length; k++) {
                VoronoiCell voronoiCell = voronoi.cells[k];
                Vector2 center = voronoiCell.center.vector3;
                MapCell cell = new MapCell(center);
                MapRegion cr = new MapRegion(cell);
                if (edgeNoise > 0) {
                    cr.polygon = voronoiCell.GetPolygon(voronoiCell.center, edgeMaxLength, edgeNoise);
                } else {
                    cr.polygon = voronoiCell.GetPolygon();
                }
                if (cr.polygon != null) {
                    // Add segments
                    int segmentsCount = voronoiCell.segments.Count;
                    for (int i = 0; i < segmentsCount; i++) {
                        Segment s = voronoiCell.segments[i];
                        if (!s.deleted) {
                            if (edgeNoise > 0) {
                                cr.segments.AddRange(s.subdivisions);
                            } else {
                                cr.segments.Add(s);
                            }
                        }
                    }
                    cell.region = cr;
                    mapCells.Add(cell);
                }
            }
        }


        void CreateMapCells() {

            int requiredCells = Mathf.Max(numCountries, 2);
            if (generateProvinces) {
                requiredCells *= Mathf.Max(1, numProvincesPerCountryMin);
            }

            numCells = Mathf.Clamp(numCells, requiredCells, MAX_CELLS);
            if (mapCells == null) {
                mapCells = new List<MapCell>(numCells);
            } else {
                mapCells.Clear();
            }

            VoronoiTesselation();

            CellsFindNeighbours();

            CellsUpdateBounds();
        }


        void CellsUpdateBounds() {
            // Update cells polygon
            for (int k = 0; k < mapCells.Count; k++) {
                RegionUpdateBounds(mapCells[k].region);
            }
        }


        void RegionUpdateBounds(MapRegion region) {
            if (region.polygon.contours.Count == 0)
                return;
            Vector2[] points = region.polygon.contours[0].GetVector2Points();
            region.points = points;
            // Update bounding rect
            float minx, miny, maxx, maxy;
            minx = miny = float.MaxValue;
            maxx = maxy = float.MinValue;
            int pointCount = points.Length;
            for (int p = 0; p < pointCount; p++) {
                Vector2 point = points[p];
                if (point.x < minx)
                    minx = point.x;
                if (point.x > maxx)
                    maxx = point.x;
                if (point.y < miny)
                    miny = point.y;
                if (point.y > maxy)
                    maxy = point.y;
            }
            float rectWidth = maxx - minx;
            float rectHeight = maxy - miny;
            region.rect2D = new Rect(minx, miny, rectWidth, rectHeight);
            region.rect2DArea = rectWidth * rectHeight;
        }


        void CellsFindNeighbours() {

            if (cellNeighbourHit == null) {
                cellNeighbourHit = new Dictionary<Segment, MapRegion>(50000);
            } else {
                cellNeighbourHit.Clear();
            }
            int cellsCount = mapCells.Count;
            for (int k = 0; k < cellsCount; k++) {
                MapCell cell = mapCells[k];
                MapRegion region = cell.region;
                int numSegments = region.segments.Count;
                for (int i = 0; i < numSegments; i++) {
                    Segment seg = region.segments[i];
                    if (cellNeighbourHit.ContainsKey(seg)) {
                        MapRegion neighbour = cellNeighbourHit[seg];
                        if (neighbour != region) {
                            if (!region.neighbours.Contains(neighbour)) {
                                region.neighbours.Add(neighbour);
                                neighbour.neighbours.Add(region);
                            }
                        }
                    } else {
                        cellNeighbourHit[seg] = region;
                    }
                }
            }
        }


        void FindCountryFrontiers() {

            if (mapCountries == null || mapCountries.Count == 0)
                return;

            if (territoryFrontiers == null) {
                territoryFrontiers = new List<Segment>(cellNeighbourHit.Count);
            } else {
                territoryFrontiers.Clear();
            }
            if (territoryNeighbourHit == null) {
                territoryNeighbourHit = new Dictionary<Segment, MapRegion>(50000);
            } else {
                territoryNeighbourHit.Clear();
            }
            int terrCount = mapCountries.Count;
            Connector[] connectors = new Connector[terrCount];
            for (int k = 0; k < terrCount; k++) {
                connectors[k] = new Connector();
                MapCountry territory = mapCountries[k];
                territory.cells.Clear();
                if (territory.region == null) {
                    MapRegion territoryRegion = new MapRegion(territory);
                    territory.region = territoryRegion;
                }
                mapCountries[k].region.neighbours.Clear();
            }

            int cellCount = mapCells.Count;
            for (int k = 0; k < cellCount; k++) {
                MapCell cell = mapCells[k];
                if (cell.countryIndex >= terrCount)
                    continue;
                bool validCell = cell.visible && cell.countryIndex >= 0;
                if (validCell)
                    mapCountries[cell.countryIndex].cells.Add(cell);
                MapRegion region = cell.region;
                int numSegments = region.segments.Count;
                for (int i = 0; i < numSegments; i++) {
                    Segment seg = region.segments[i];
                    if (seg.border) {
                        if (validCell) {
                            territoryFrontiers.Add(seg);
                            int territory1 = cell.countryIndex;
                            connectors[territory1].Add(seg);
                            seg.territoryIndex = territory1;
                        }
                        continue;
                    }
                    if (territoryNeighbourHit.TryGetValue(seg, out MapRegion neighbour)) {
                        MapCell neighbourCell = (MapCell)neighbour.entity;
                        int territory1 = cell.countryIndex;
                        int territory2 = neighbourCell.countryIndex;
                        if (territory2 != territory1) {
                            territoryFrontiers.Add(seg);
                            if (validCell) {
                                connectors[territory1].Add(seg);
                                seg.territoryIndex = (territory2 >= 0) ? -1 : territory1;   // if segment belongs to a visible cell and valid territory2, mark this segment as disputed. Otherwise make it part of territory1
                                if (seg.territoryIndex < 0) {
                                    // add territory neigbhours
                                    MapRegion territory1Region = mapCountries[territory1].region;
                                    MapRegion territory2Region = mapCountries[territory2].region;
                                    if (!territory1Region.neighbours.Contains(territory2Region)) {
                                        territory1Region.neighbours.Add(territory2Region);
                                    }
                                    if (!territory2Region.neighbours.Contains(territory1Region)) {
                                        territory2Region.neighbours.Add(territory1Region);
                                    }
                                }
                            }
                            if (territory2 >= 0) {
                                connectors[territory2].Add(seg);
                            }
                        }
                    } else {
                        territoryNeighbourHit[seg] = region;
                        seg.territoryIndex = cell.countryIndex;
                    }
                }
            }

            for (int k = 0; k < terrCount; k++) {
                MapCountry country = mapCountries[k];
                country.region = new MapRegion(country);
                if (country.cells.Count > 0) {
                    country.region.polygon = connectors[k].ToPolygonFromLargestLineStrip();
                } else {
                    country.region.polygon = null;
                }
            }

        }



        void CreateMapCountries() {

            numCountries = Mathf.Clamp(numCountries, 1, Mathf.Min(numCells, MAX_TERRITORIES));

            if (mapCountries == null) {
                mapCountries = new List<MapCountry>(numCountries);
            } else {
                mapCountries.Clear();
            }

            for (int k = 0; k < mapCells.Count; k++) {
                mapCells[k].countryIndex = -1;
            }

            int cellsCount = mapCells.Count;
            for (int c = 0; c < numCountries; c++) {
                MapCountry territory = new MapCountry();
                int countryIndex = mapCountries.Count;
                int p = UnityEngine.Random.Range(0, cellsCount);
                int z = 0;
                MapCell cell = mapCells[p];
                while ((cell.countryIndex != -1 || !cell.visible || cell.ignoreTerritories) && z++ <= cellsCount) {
                    p++;
                    if (p >= cellsCount) {
                        p = 0;
                    }
                    cell = mapCells[p];
                }
                if (z > cellsCount)
                    break; // no more territories can be found - this should not happen

                cell.countryIndex = countryIndex;
                territory.cells.Add(cell);
                mapCountries.Add(territory);
            }

            // Continue conquering cells
            int countriesCount = mapCountries.Count;
            int[] countryCellIndex = new int[countriesCount];

            // Iterate one cell per country (this is not efficient but ensures balanced distribution)
            bool remainingCells = true;
            while (remainingCells) {
                while (remainingCells) {
                    remainingCells = false;
                    for (int k = 0; k < countriesCount; k++) {
                        MapCountry country = mapCountries[k];
                        int countryCellsCount = country.cells.Count;
                        for (int p = countryCellIndex[k]; p < countryCellsCount; p++) {
                            MapRegion cellRegion = country.cells[p].region;
                            int neighboursCount = cellRegion.neighbours.Count;
                            for (int n = 0; n < neighboursCount; n++) {
                                MapRegion otherRegion = cellRegion.neighbours[n];
                                MapCell otherCell = (MapCell)otherRegion.entity;
                                if (otherCell.countryIndex == -1 && otherCell.visible && !otherCell.ignoreTerritories) {
                                    otherCell.countryIndex = k;
                                    country.cells.Add(otherCell);
                                    remainingCells = true;
                                    p = countryCellsCount;
                                    break;
                                }
                            }
                            if (p < countryCellsCount) // no free neighbours left for this cell
                                countryCellIndex[k]++;
                        }
                    }
                }
                // Check if there's any other cell without territory
                for (int k = 0; k < cellsCount; k++) {
                    MapCell cell = mapCells[k];
                    if (cell.countryIndex == -1 && cell.visible && !cell.ignoreTerritories) {
                        int countryIndex = UnityEngine.Random.Range(0, countriesCount);
                        cell.countryIndex = countryIndex;
                        mapCountries[countryIndex].cells.Add(cell);
                        remainingCells = true;
                        break;
                    }
                }
            }

            FindCountryFrontiers();
            UpdateCountryBoundaries();
        }

        void UpdateCountryBoundaries() {
            if (mapCountries == null)
                return;

            // Update territory region
            int terrCount = mapCountries.Count;
            for (int k = 0; k < terrCount; k++) {
                MapCountry country = mapCountries[k];
                MapRegion countryRegion = country.region;

                if (countryRegion.polygon == null) {
                    continue;
                }
                countryRegion.points = countryRegion.polygon.contours[0].GetVector2Points();
                List<Point> points = countryRegion.polygon.contours[0].points;
                int pointCount = points.Count;
                for (int j = 0; j < pointCount; j++) {
                    Point p0 = points[j];
                    Point p1;
                    if (j == pointCount - 1) {
                        p1 = points[0];
                    } else {
                        p1 = points[j + 1];
                    }
                    countryRegion.segments.Add(new Segment(p0, p1));
                }

                // Update bounding rect
                float minx, miny, maxx, maxy;
                minx = miny = float.MaxValue;
                maxx = maxy = float.MinValue;
                int terrPointCount = countryRegion.points.Length;
                for (int p = 0; p < terrPointCount; p++) {
                    Vector2 point = countryRegion.points[p];
                    if (point.x < minx)
                        minx = point.x;
                    if (point.x > maxx)
                        maxx = point.x;
                    if (point.y < miny)
                        miny = point.y;
                    if (point.y > maxy)
                        maxy = point.y;
                }
                float rectWidth = maxx - minx;
                float rectHeight = maxy - miny;
                countryRegion.rect2D = new Rect(minx, miny, rectWidth, rectHeight);
                countryRegion.rect2DArea = rectWidth * rectHeight;
            }
        }


        void CreateMapProvinces() {

            // per each country, assign cells to new provinces
            if (mapProvinces == null) {
                mapProvinces = new List<MapProvince>();
            } else {
                mapProvinces.Clear();
            }

            // Countries do not support holes, so make sure cells contained in countries are visible
            int cellsCount = mapCells.Count;
            int countriesCount = mapCountries.Count;
            for (int k = 0; k < cellsCount; k++) {
                MapCell cell = mapCells[k];
                if (!cell.visible) {
                    for (int c = 0; c < countriesCount; c++) {
                        MapCountry country = mapCountries[c];
                        if (country.region.Contains(cell.center)) {
                            country.cells.Add(cell);
                            cell.countryIndex = c;
                            cell.visible = true;
                            break;
                        }
                    }
                }
            }

            for (int c = 0; c < countriesCount; c++) {
                CreateCountryProvinces(c);
            }

            FindProvinceFrontiers();

            UpdateProvinceBoundaries();
        }

        void CreateCountryProvinces(int countryIndex) {

            MapCountry country = mapCountries[countryIndex];

            List<MapCell> cells = country.cells;
            int cellsCount = cells.Count;
            for (int k = 0; k < cellsCount; k++) {
                cells[k].provinceIndex = -1;
            }

            int numProvinces = UnityEngine.Random.Range(numProvincesPerCountryMin, numProvincesPerCountryMax + 1);
            numProvinces = Mathf.Clamp(numProvinces, 1, cellsCount);

            for (int c = 0; c < numProvinces; c++) {
                MapProvince prov = new MapProvince();
                int p = UnityEngine.Random.Range(0, cellsCount);
                int z = 0;
                MapCell cell = cells[p];
                while ((cell.provinceIndex != -1 || !cell.visible || cell.ignoreTerritories) && z++ <= cellsCount) {
                    p++;
                    if (p >= cellsCount) {
                        p = 0;
                    }
                    cell = cells[p];
                }
                if (z > cellsCount)
                    break; // no more territories can be found - this should not happen

                cell.provinceIndex = c;
                prov.countryIndex = countryIndex;
                prov.capitalCenter = cell.center;
                prov.cells.Add(cell);
                mapProvinces.Add(prov);
                country.provinces.Add(prov);
            }

            // Continue conquering cells
            List<MapProvince> provinces = country.provinces;
            int provincesCount = provinces.Count;
            int[] provCellIndex = new int[provincesCount];

            // Iterate one cell per province (this is not efficient but ensures balanced distribution)
            bool remainingCells = true;
            while (remainingCells) {
                while (remainingCells) {
                    remainingCells = false;
                    for (int k = 0; k < provincesCount; k++) {
                        MapProvince prov = provinces[k];
                        int provCellsCount = prov.cells.Count;
                        for (int p = provCellIndex[k]; p < provCellsCount; p++) {
                            MapRegion cellRegion = prov.cells[p].region;
                            int neighboursCount = cellRegion.neighbours.Count;
                            for (int n = 0; n < neighboursCount; n++) {
                                MapRegion otherRegion = cellRegion.neighbours[n];
                                MapCell otherCell = (MapCell)otherRegion.entity;
                                if (otherCell.provinceIndex == -1 && otherCell.visible && !otherCell.ignoreTerritories) {
                                    otherCell.provinceIndex = k;
                                    prov.cells.Add(otherCell);
                                    remainingCells = true;
                                    p = provCellsCount;
                                    break;
                                }
                            }
                            if (p < provCellsCount) // no free neighbours left for this cell
                                provCellIndex[k]++;
                        }
                    }
                }
                // Check if there's any other cell without territory
                for (int k = 0; k < cellsCount; k++) {
                    MapCell cell = mapCells[k];
                    if (cell.provinceIndex == -1 && cell.visible && !cell.ignoreTerritories) {
                        int provinceIndex = UnityEngine.Random.Range(0, provincesCount);
                        cell.provinceIndex = provinceIndex;
                        mapProvinces[provinceIndex].cells.Add(cell);
                        remainingCells = true;
                        break;
                    }
                }
            }

        }

        void FindProvinceFrontiers() {

            if (mapProvinces == null || mapProvinces.Count == 0)
                return;

            if (territoryFrontiers == null) {
                territoryFrontiers = new List<Segment>(cellNeighbourHit.Count);
            } else {
                territoryFrontiers.Clear();
            }
            if (territoryNeighbourHit == null) {
                territoryNeighbourHit = new Dictionary<Segment, MapRegion>(50000);
            } else {
                territoryNeighbourHit.Clear();
            }
            int provincesCount = mapProvinces.Count;
            Connector[] connectors = new Connector[provincesCount];
            for (int k = 0; k < provincesCount; k++) {
                connectors[k] = new Connector();
                MapProvince prov = mapProvinces[k];
                prov.cells.Clear();
                if (prov.region == null) {
                    MapRegion provRegion = new MapRegion(prov);
                    prov.region = provRegion;
                }
                prov.region.neighbours.Clear();
            }

            int cellCount = mapCells.Count;
            for (int k = 0; k < cellCount; k++) {
                MapCell cell = mapCells[k];
                if (cell.provinceIndex >= provincesCount)
                    continue;
                bool validCell = cell.visible && cell.provinceIndex >= 0;
                if (validCell) {
                    mapProvinces[cell.provinceIndex].cells.Add(cell);
                }
                MapRegion region = cell.region;
                int numSegments = region.segments.Count;
                for (int i = 0; i < numSegments; i++) {
                    Segment seg = region.segments[i];
                    if (seg.border) {
                        if (validCell) {
                            territoryFrontiers.Add(seg);
                            int territory1 = cell.countryIndex;
                            connectors[territory1].Add(seg);
                            seg.territoryIndex = territory1;
                        }
                        continue;
                    }
                    if (territoryNeighbourHit.TryGetValue(seg, out MapRegion neighbour)) {
                        MapCell neighbourCell = (MapCell)neighbour.entity;
                        int territory1 = cell.countryIndex;
                        int territory2 = neighbourCell.countryIndex;
                        if (territory2 != territory1) {
                            territoryFrontiers.Add(seg);
                            if (validCell) {
                                connectors[territory1].Add(seg);
                                seg.territoryIndex = (territory2 >= 0) ? -1 : territory1;   // if segment belongs to a visible cell and valid territory2, mark this segment as disputed. Otherwise make it part of territory1
                                if (seg.territoryIndex < 0) {
                                    // add territory neigbhours
                                    MapRegion territory1Region = mapCountries[territory1].region;
                                    MapRegion territory2Region = mapCountries[territory2].region;
                                    if (!territory1Region.neighbours.Contains(territory2Region)) {
                                        territory1Region.neighbours.Add(territory2Region);
                                    }
                                    if (!territory2Region.neighbours.Contains(territory1Region)) {
                                        territory2Region.neighbours.Add(territory1Region);
                                    }
                                }
                            }
                            if (territory2 >= 0) {
                                connectors[territory2].Add(seg);
                            }
                        }
                    } else {
                        territoryNeighbourHit[seg] = region;
                        seg.territoryIndex = cell.countryIndex;
                    }
                }
            }

            for (int k = 0; k < provincesCount; k++) {
                MapProvince prov = mapProvinces[k];
                prov.region = new MapRegion(prov);
                if (prov.cells.Count > 0) {
                    prov.region.polygon = connectors[k].ToPolygonFromLargestLineStrip();
                } else {
                    prov.region.polygon = null;
                }
            }

        }


        void UpdateProvinceBoundaries() {
            if (mapProvinces == null)
                return;

            // Update territory region
            int provincesCount = mapProvinces.Count;
            for (int k = 0; k < provincesCount; k++) {
                MapProvince prov = mapProvinces[k];
                MapRegion provRegion = prov.region;

                if (provRegion.polygon == null) {
                    continue;
                }
                provRegion.points = provRegion.polygon.contours[0].GetVector2Points();
                List<Point> points = provRegion.polygon.contours[0].points;
                int pointCount = points.Count;
                for (int j = 0; j < pointCount; j++) {
                    Point p0 = points[j];
                    Point p1;
                    if (j == pointCount - 1) {
                        p1 = points[0];
                    } else {
                        p1 = points[j + 1];
                    }
                    provRegion.segments.Add(new Segment(p0, p1));
                }

                // Update bounding rect
                float minx, miny, maxx, maxy;
                minx = miny = float.MaxValue;
                maxx = maxy = float.MinValue;
                int terrPointCount = provRegion.points.Length;
                for (int p = 0; p < terrPointCount; p++) {
                    Vector2 point = provRegion.points[p];
                    if (point.x < minx)
                        minx = point.x;
                    if (point.x > maxx)
                        maxx = point.x;
                    if (point.y < miny)
                        miny = point.y;
                    if (point.y > maxy)
                        maxy = point.y;
                }
                float rectWidth = maxx - minx;
                float rectHeight = maxy - miny;
                provRegion.rect2D = new Rect(minx, miny, rectWidth, rectHeight);
                provRegion.rect2DArea = rectWidth * rectHeight;
            }
        }


        #endregion

        #region Drawing stuff

        /// <summary>
        /// Generate and replace provinces, city and country data
        /// </summary>
        /// <param name="changeStyle">If true, some appearance like frontiers color will be updated</param>
        /// <param name="saveData">If true, textures and new geodata will be saved to disk. This can only be used in Editor.</param>
        public void GenerateMap(bool saveData, bool changeStyle) {

            try {

                UnityEngine.Random.InitState(seed);

                map.ClearAll();

                GenerateHeightMap();
                GenerateHeightGradient();

                CreateMapCells();
                AssignHeightMapToCells();

                CreateMapCountries();
                ReplaceCountryGeodata();

                if (generateProvinces) {
                    CreateMapProvinces();
                    ReplaceProvinceGeodata();
                }
                if (generateCities) {
                    CreateMapCities();
                    ReplaceCityGeodata();
                }

                if (colorProvinces && generateProvinces) {
                    AssignColorsToProvinces();
                } else {
                    AssignColorsToCountries();
                }

                GenerateWorldTextures();

                if (generatePathfindingMaps) {
                    GeneratePathFindingTextures();
                }

                map.waterLevel = seaLevel;

                // Apply some complementary style
                if (changeStyle) {
                    if (heightGradientPreset != HeightMapGradientPreset.Custom) {
                        map.frontiersColor = Color.black;
                        map.frontiersColorOuter = Color.black;
                        map.provincesColor = new Color(0.5f, 0.5f, 0.5f, 0.35f);
                        if (heightGradientPreset == HeightMapGradientPreset.BlackAndWhite || heightGradientPreset == HeightMapGradientPreset.Grayscale) {
                            map.countryLabelsColor = Color.black;
                            map.countryLabelsShadowColor = new Color(0.9f, 0.9f, 0.9f, 0.75f);
                        } else {
                            map.countryLabelsColor = Color.white;
                            map.countryLabelsShadowColor = new Color(0.1f, 0.1f, 0.1f, 0.75f);
                        }
                    }
                    if (_map.earthStyle.isScenicPlus()) {
                        _map.earthStyle = EARTH_STYLE.Natural;
                    }
                    map.showFrontiers = true;
                    map.showCountryNames = true;
                }

                // Save map data
                if (saveData) {
#if UNITY_EDITOR
					SaveGeneratedMapData();
#endif
                } else {
                    // Set textures
                    map.SetEarthWaterMask(waterMaskTexture);
                    map.SetEarthTexture(backgroundTexture);
                    map.SetHeightmapTexture(heightMapTexture);
                    if (generatePathfindingMaps) {
                        map.SetPathFindingLandMap(pathFindingLandMapTexture);
                        map.SetPathFindingWaterMap(pathFindingWaterMapTexture);
                    }
                    map.earthBumpMapTexture = null;
                    map.earthBumpEnabled = false;
                }

                map.Redraw(true);

            } catch (Exception ex) {
                Debug.LogError("Error generating map: " + ex.ToString());
                if (!Application.isPlaying) {
#if UNITY_EDITOR
					EditorUtility.DisplayDialog("Error Generating Map", "An error occured while generating map. Try choosing another 'Seed' value, reducing 'Border Curvature' amount or number of provinces.", "Ok");
#endif
                }
            }

        }


        void ReplaceCountryGeodata() {

            UnityEngine.Random.InitState(seedNames);

            // Replace countries
            usedNames.Clear();
            int mapCountriesCount = mapCountries.Count;
            List<Country> newCountries = new List<Country>(mapCountriesCount);
            for (int k = 0; k < mapCountriesCount; k++) {
                MapCountry c = mapCountries[k];
                Vector2[] points = c.region.points;
                if (points == null || points.Length < 3)
                    continue;
                string name = GetUniqueRandomName(0, 4, usedNames);
                Country newCountry = new Country(name, "World", k);
                newCountry.attrib["mapColor"] = c.color;
                Region region = new Region(newCountry, newCountry.regions.Count);
                newCountry.regions.Add(region);
                region.UpdatePointsAndRect(points);
                map.RefreshCountryGeometry(newCountry);
                c.capitalCenter = newCountry.centroid;
                newCountries.Add(newCountry);
            }
            map.countries = newCountries.ToArray();
            countryChanges = true;

        }

        void ReplaceProvinceGeodata() {

            usedNames.Clear();
            int mapProvincesCount = mapCells.Count;
            List<Province> newProvinces = new List<Province>(mapProvincesCount);
            Province[] provinces = _map.provinces;
            if (provinces == null) {
                provinces = new Province[0];
            }
            for (int k = 0; k < mapProvincesCount; k++) {
                MapCell p = mapCells[k];
                if (!p.visible) {
                    continue;
                }
                Vector2[] points = p.region.points;
                if (points == null || points.Length < 3)
                    continue;
                int countryIndex = map.GetCountryIndex(p.center);
                if (countryIndex < 0)
                    continue;

                string name = GetUniqueRandomName(0, 4, usedNames);
                Province newProvince = new Province(name, countryIndex, k);
                //newProvince.attrib["mapColor"] = p.color;  // TODO: <- province.color
                newProvince.regions = new List<Region>();
                Region region = new Region(newProvince, newProvince.regions.Count);
                newProvince.regions.Add(region);
                region.UpdatePointsAndRect(points);
                map.RefreshProvinceGeometry(newProvince);
                newProvinces.Add(newProvince);

                Country country = map.countries[countryIndex];
                List<Province> countryProvinces = country.provinces != null ? new List<Province>(country.provinces) : new List<Province>();
                countryProvinces.Add(newProvince);
                country.provinces = countryProvinces.ToArray();
            }
            map.provinces = newProvinces.ToArray();
            provinceChanges = true;
        }

        void ReplaceCityGeodata() {
            map.cities = mapCities.ToArray();
            cityChanges = true;
        }

        #endregion


        #region Cities stuff

        void CreateMapCities() {

            int countryCount = map.countries.Length;

            mapCities.Clear();

            for (int k = 0; k < countryCount; k++) {
                int cityCount = UnityEngine.Random.Range(numCitiesPerCountryMin, numCitiesPerCountryMax + 1);
                if (cityCount == 0) continue;

                Country country = map.countries[k];

                int countryCityCount = 0;
                Vector2 countryCenter = map.GetCountryCentroid(k);
                if (!country.Contains(countryCenter)) {
                    countryCenter = country.mainRegion.GetRandomPointInside();
                    if (!country.Contains(countryCenter)) continue;
                }

                // add capital
                AddCity(countryCenter, CITY_CLASS.COUNTRY_CAPITAL, k);
                if (++countryCityCount >= cityCount) continue;

                // add province capitals
                if (generateProvinces) {
                    foreach (Province prov in country.provinces) {
                        Vector2 provCenter = map.GetProvinceCentroid(prov);
                        if (!prov.Contains(provCenter)) {
                            provCenter = prov.mainRegion.GetRandomPointInside();
                            if (!prov.Contains(provCenter)) continue;
                        }

                        // add province capital
                        AddCity(provCenter, CITY_CLASS.REGION_CAPITAL, k);
                        if (++countryCityCount >= cityCount) break;
                    }
                }

                // add more cities
                while (countryCityCount < cityCount) {
                    Vector2 cityPos = country.GetRandomPointInside();
                    if (!country.Contains(cityPos)) {
                        break;
                    }
                    AddCity(cityPos, CITY_CLASS.CITY, k);
                    countryCityCount++;
                }
            }
        }

        void AddCity(Vector2 pos, CITY_CLASS cityClass, int countryIndex) {
            City newCity;
            string provinceName = "";
            if (generateProvinces) {
                Province prov = map.GetProvince(pos);
                provinceName = prov != null ? prov.name : "";
            }
            string name = GetUniqueRandomName(0, 4, usedNames);
            newCity = new City(name, provinceName, countryIndex, 0, pos, cityClass, mapCities.Count + 1);
            mapCities.Add(newCity);
        }

        #endregion

        #region

#if UNITY_EDITOR
		public string GetGenerationMapOutputPath() {
			string rootFolder;
			string path = "";
			string[] paths = AssetDatabase.GetAllAssetPaths();
			for (int k = 0; k < paths.Length; k++) {
				if (paths[k].EndsWith("WorldMapStrategyKit")) {
					rootFolder = paths[k];
					path = rootFolder + "/Resources/WMSK/Geodata/" + outputFolder;
					break;
				}
			}
			return path;
		}
#endif

        void SaveGeneratedMapData() {

#if UNITY_EDITOR

			if (string.IsNullOrEmpty(outputFolder)) {
				outputFolder = "CustomMap";
			}

			string path = GetGenerationMapOutputPath();
			if (string.IsNullOrEmpty(path)) {
				Debug.LogError("Could not find WMSK folder.");
				return;
			}

			Directory.CreateDirectory(path);
			if (!Directory.Exists(path)) {
				EditorUtility.DisplayDialog("Invalid output folder", "The path " + path + " is no valid.", "Ok");
				return;
			}

			// Set Geodata folder to custom path
            string fullPathName, data;
			_map.geodataResourcesPath = "WMSK/Geodata/" + outputFolder;

			// Save country data
            if (countryChanges) {
            fullPathName = path + "/" + GetCountryGeoDataFileName();
			data = _map.GetCountryGeoData();
			File.WriteAllText(fullPathName, data, Encoding.UTF8);
			AssetDatabase.ImportAsset(fullPathName);
			countryChanges = false;
            }

			// Save province data
            if (provinceChanges) {
            fullPathName = path + "/" + GetProvinceGeoDataFileName();
			data = _map.GetProvinceGeoData();
			File.WriteAllText(fullPathName, data, Encoding.UTF8);
			AssetDatabase.ImportAsset(fullPathName);
			provinceChanges = false;
            }

			// Save cities
            if (cityChanges) {
			fullPathName = path + "/" + GetCityGeoDataFileName();
			data = _map.GetCityGeoData();
			File.WriteAllText(fullPathName, data, Encoding.UTF8);
			AssetDatabase.ImportAsset(fullPathName);
			cityChanges = false;
            }

			// Save heightmap
			byte[] bytes = heightMapTexture.EncodeToPNG();
			string outputFile = path + "/heightmap.png";
			File.WriteAllBytes(outputFile, bytes);
			AssetDatabase.ImportAsset(outputFile);
			TextureImporter timp = (TextureImporter)AssetImporter.GetAtPath(outputFile);
			timp.isReadable = true;
			timp.SaveAndReimport();

			// Save background texture
			bytes = backgroundTexture.EncodeToPNG();
			outputFile = path + "/worldColors.png";
			File.WriteAllBytes(outputFile, bytes);
			AssetDatabase.ImportAsset(outputFile);
			timp = (TextureImporter)AssetImporter.GetAtPath(outputFile);
			timp.isReadable = true;
			timp.SaveAndReimport();

			// Save normal map texture
			if (generateNormalMap) {
				bytes = heightMapTexture.EncodeToPNG();
				outputFile = path + "/normalMap.png";
				File.WriteAllBytes(outputFile, bytes);
				AssetDatabase.ImportAsset(outputFile);
				timp = (TextureImporter)AssetImporter.GetAtPath(outputFile);
				timp.textureType = TextureImporterType.NormalMap;
				timp.heightmapScale = normalMapBumpiness;
				timp.convertToNormalmap = true;
				timp.isReadable = true;
				timp.SaveAndReimport();
            }

			// Save water mask texture
			bytes = waterMaskTexture.EncodeToPNG();
			outputFile = path + "/waterMask.png";
			File.WriteAllBytes(outputFile, bytes);
			AssetDatabase.ImportAsset(outputFile);
			timp = (TextureImporter)AssetImporter.GetAtPath(outputFile);
			timp.isReadable = true;
			timp.SaveAndReimport();

            // Save path-finding land map texture
            if (generatePathfindingMaps) {
            bytes = pathFindingLandMapTexture.EncodeToPNG();
            outputFile = path + "/landmap.png";
            File.WriteAllBytes(outputFile, bytes);
            AssetDatabase.ImportAsset(outputFile);
            timp = (TextureImporter)AssetImporter.GetAtPath(outputFile);
            timp.isReadable = true;
            timp.SaveAndReimport();

            // Save path-finding land map texture
            bytes = pathFindingWaterMapTexture.EncodeToPNG();
            outputFile = path + "/watermap.png";
            File.WriteAllBytes(outputFile, bytes);
            AssetDatabase.ImportAsset(outputFile);
            timp = (TextureImporter)AssetImporter.GetAtPath(outputFile);
            timp.isReadable = true;
            timp.SaveAndReimport();

                _map.SetPathFindingLandMap(Resources.Load<Texture2D>(_map.geodataResourcesPath + "/landmap"));
                _map.SetPathFindingWaterMap(Resources.Load<Texture2D>(_map.geodataResourcesPath + "/watermap"));

            }
            
            // Set textures
            _map.SetEarthWaterMask(Resources.Load<Texture2D>(_map.geodataResourcesPath + "/waterMask"));
			_map.SetEarthTexture(Resources.Load<Texture2D>(_map.geodataResourcesPath + "/worldColors"));
			_map.SetHeightmapTexture(Resources.Load<Texture2D>(_map.geodataResourcesPath + "/heightmap"));
			_map.isDirty = true;
#endif

        }

        #endregion





    }

}