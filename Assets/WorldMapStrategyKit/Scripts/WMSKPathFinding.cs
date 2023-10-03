// World Strategy Kit for Unity - Main Script
// (C) Kronnect Technologies SL
// Don't modify this script - changes could be lost if you upgrade to a more recent version of WMSK
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WorldMapStrategyKit.PathFinding;
using System.Threading;
using System.Threading.Tasks;

namespace WorldMapStrategyKit {
    public delegate float OnPathFindingCrossPosition(Vector2 position);
    public delegate float OnPathFindingCrossAdminEntity(int entityIndex);
    public delegate float OnPathFindingCrossCell(int cellIndex);
    public delegate void FindRouteASyncFinishMethod(List<Vector2> points, float totalCost);
    public delegate void FindRouteCellsASyncFinishMethod(List<int> cellIndices, float totalCost);

    public partial class WMSK : MonoBehaviour {

        /// <summary>
        /// Fired when path finding algorithmn evaluates a cell (used with FindRoute and map positions). Return the increased cost for that map position.
        /// </summary>
        public event OnPathFindingCrossPosition OnPathFindingCrossPosition;

        /// <summary>
        /// Fired when path finding algorithmn evaluates a country (used with FindRoute and countries). Return the increased cost for that map position.
        /// </summary>
        public event OnPathFindingCrossAdminEntity OnPathFindingCrossCountry;

        /// <summary>
        /// Fired when path finding algorithmn evaluates a province (used with FindRoute and provinces). Return the increased cost for that map position.
        /// </summary>
        public event OnPathFindingCrossAdminEntity OnPathFindingCrossProvince;

        /// <summary>
        /// Fired when path finding algorithmn evaluates a cell (used with FindRoute and cells). Return the increased cost for that map position.
        /// </summary>
        public event OnPathFindingCrossCell OnPathFindingCrossCell;


        #region Public properties

        [SerializeField]
        HeuristicFormula
            _pathFindingHeuristicFormula = HeuristicFormula.MaxDXDY;

        /// <summary>
        /// The path finding heuristic formula to estimate distance from current position to destination
        /// </summary>
        public HeuristicFormula pathFindingHeuristicFormula {
            get { return _pathFindingHeuristicFormula; }
            set {
                if (value != _pathFindingHeuristicFormula) {
                    _pathFindingHeuristicFormula = value;
                    isDirty = true;
                }
            }
        }

        [SerializeField]
        float
            _pathFindingMaxCost = 200000;

        /// <summary>
        /// The maximum search cost of the path finding execution.
        /// </summary>
        public float pathFindingMaxCost {
            get { return _pathFindingMaxCost; }
            set {
                if (value != _pathFindingMaxCost) {
                    _pathFindingMaxCost = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        int
            _pathFindingMaxSteps = 2000;

        /// <summary>
        /// The maximum number of steps for any path.
        /// </summary>
        public int pathFindingMaxSteps {
            get { return _pathFindingMaxSteps; }
            set {
                if (value != _pathFindingMaxSteps) {
                    _pathFindingMaxSteps = value;
                    isDirty = true;
                }
            }
        }


        [SerializeField]
        bool
            _enablePathFindingSystem = true;

        /// <summary>
        /// Enables or disables path-finding system. Disabling the system will save memory and improve initialization time.
        /// </summary>
        public bool enablePathFindingSystem {
            get { return _enablePathFindingSystem; }
            set {
                if (_enablePathFindingSystem != value) {
                    isDirty = true;
                    _enablePathFindingSystem = value;
                }
            }
        }

        [SerializeField]
        bool
            _pathFindingVisualizeMatrixCost;

        /// <summary>
        /// Switches the map texture with a texture with the matrix costs colors
        /// </summary>
        public bool pathFindingVisualizeMatrixCost {
            get { return _pathFindingVisualizeMatrixCost; }
            set {
                if (_pathFindingVisualizeMatrixCost != value) {
                    isDirty = true;
                    _pathFindingVisualizeMatrixCost = value;
                    if (_pathFindingVisualizeMatrixCost) {
                        UpdatePathfindingMatrixCostTexture();
                        DestroySurfaces();
                    } else {
                        if (earthMat != null)
                            earthMat = null;
                    }
                    RestyleEarth();
                }
            }
        }

        [SerializeField]
        bool
            _pathFindingEnableCustomRouteMatrix;

        /// <summary>
        /// Enables user-defined location crossing costs for path finding engine.
        /// </summary>
        public bool pathFindingEnableCustomRouteMatrix {
            get { return _pathFindingEnableCustomRouteMatrix; }
            set { _pathFindingEnableCustomRouteMatrix = value; }
        }

        /// <summary>
        /// Returns a copy of the current custom route matrix or set it.
        /// </summary>
        public float[] pathFindingCustomRouteMatrix {
            get {
                float[] copy = new float[customRouteMatrix.Length];
                Array.Copy(_customRouteMatrix, copy, _customRouteMatrix.Length);
                return copy;
            }
            set {
                _customRouteMatrix = value;
                if (finder != null) {
                    finder.SetCustomRouteMatrix(_customRouteMatrix);
                }
                if (_pathFindingVisualizeMatrixCost)
                    UpdatePathfindingMatrixCostTexture();
            }
        }

        [SerializeField]
        Texture2D
    _pathFindingLandMap;

        /// <summary>
        /// Land map for path finding purposes.
        /// </summary>
        public Texture2D pathFindingLandMap {
            get {
                if (_pathFindingLandMap == null) {
                    _pathFindingLandMap = Resources.Load<Texture2D>("WMSK/Textures/PathFindingLandMap8K");
                }
                return _pathFindingLandMap;
            }
            set {
                if (_pathFindingLandMap != value) {
                    _pathFindingLandMap = value;
                    pathfindingLandMask = null;
                    CheckRouteLandAndWaterMask();
                    isDirty = true;
                }
            }
        }

        /// <summary>
        /// Sets the texture of the land map for path finding purposes. Calling this method will ensure the textures are reloaded.
        /// </summary>
        public void SetPathFindingLandMap(Texture2D texture) {
            _pathFindingLandMap = null;
            pathFindingLandMap = texture;
        }


        [SerializeField]
        Texture2D
    _pathFindingWaterMap;

        /// <summary>
        /// Watermap for path finding purposes.
        /// </summary>
        public Texture2D pathFindingWaterMap {
            get {
                if (_pathFindingWaterMap == null) {
                    _pathFindingWaterMap = Resources.Load<Texture2D>("WMSK/Textures/PathFindingWaterMap8K");
                }
                return _pathFindingWaterMap;
            }
            set {
                if (_pathFindingWaterMap != value) {
                    _pathFindingWaterMap = value;
                    pathfindingWaterMask = null;
                    CheckRouteLandAndWaterMask();
                    isDirty = true;
                }
            }
        }

        /// <summary>
        /// Sets the texture of the water map for path finding purposes. Calling this method will ensure the textures are reloaded.
        /// </summary>
        public void SetPathFindingWaterMap(Texture2D texture) {
            _pathFindingWaterMap = null;
            pathFindingWaterMap = texture;
        }


        #endregion

        #region Path finding APIs


        /// <summary>
        /// Returns an optimal route from startPosition to endPosition with options.
        /// </summary>
        /// <returns>The route.</returns>
        /// <param name="terrainCapability">Type of terrain that the unit can pass through</param>
        /// <param name="minAltitude">Minimum altitude (0..1)</param>
        /// <param name="maxAltitude">Maximum altutude (0..1)</param>
        /// <param name="maxSearchCost">Maximum search cost for the path finding algorithm. A value of -1 will use the global default defined by pathFindingMaxCost</param>
        public List<Vector2> FindRoute(string startCityName, string startCountryName, string endCityName, string endCountryName, TERRAIN_CAPABILITY terrainCapability = TERRAIN_CAPABILITY.Any, float minAltitude = 0, float maxAltitude = 1f, float maxSearchCost = -1, int maxSearchSteps = -1) {
            City city1 = GetCity(startCityName, startCountryName);
            City city2 = GetCity(endCityName, endCountryName);
            if (city1 == null || city2 == null)
                return null;
            return FindRoute(city1, city2, terrainCapability, minAltitude, maxAltitude, maxSearchCost, maxSearchSteps);
        }


        /// <summary>
        /// Returns an optimal route from startPosition to endPosition with options.
        /// </summary>
        /// <returns>The route.</returns>
        /// <param name="terrainCapability">Type of terrain that the unit can pass through</param>
        /// <param name="minAltitude">Minimum altitude (0..1)</param>
        /// <param name="maxAltitude">Maximum altutude (0..1)</param>
        /// <param name="maxSearchCost">Maximum search cost for the path finding algorithm. A value of -1 will use the global default defined by pathFindingMaxCost</param>
        public List<Vector2> FindRoute(City startCity, City endCity, TERRAIN_CAPABILITY terrainCapability = TERRAIN_CAPABILITY.Any, float minAltitude = 0, float maxAltitude = 1f, float maxSearchCost = -1, int maxSearchSteps = -1) {
            if (startCity == null || endCity == null)
                return null;
            return FindRoute(startCity.unity2DLocation, endCity.unity2DLocation, terrainCapability, minAltitude, maxAltitude, maxSearchCost, maxSearchSteps);
        }

        /// <summary>
        /// Returns an optimal route from startPosition to endPosition with options.
        /// </summary>
        /// <returns>The route.</returns>
        /// <param name="startPosition">Start position in map coordinates (-0.5...0.5)</param>
        /// <param name="endPosition">End position in map coordinates (-0.5...0.5)</param>
        /// <param name="terrainCapability">Type of terrain that the unit can pass through</param>
        /// <param name="minAltitude">Minimum altitude (0..1)</param>
        /// <param name="maxAltitude">Maximum altutude (0..1)</param>
        /// <param name="maxSearchCost">Maximum search cost for the path finding algorithm. A value of -1 will use the global default defined by pathFindingMaxCost</param>
        public List<Vector2> FindRoute(Vector2 startPosition, Vector2 endPosition, TERRAIN_CAPABILITY terrainCapability = TERRAIN_CAPABILITY.Any, float minAltitude = 0, float maxAltitude = 1f, float maxSearchCost = -1, int maxSearchSteps = -1) {
            float dummy;
            return FindRoute(startPosition, endPosition, out dummy, terrainCapability, minAltitude, maxAltitude, maxSearchCost, maxSearchSteps);
        }


        /// <summary>
        /// Returns an optimal route from startPosition to endPosition with options. This method is designed to be called as coroutine. Upon termination, the "finished" method will be called passing the list of points of the path and the total cost.
        /// </summary>
        /// <returns>The route.</returns>
        /// <param name="finished">Delegate to be called when the path calculation finishes. This delegate receives the list of points and the total cost.</param>
        /// <param name="startPosition">Start position in map coordinates (-0.5...0.5)</param>
        /// <param name="endPosition">End position in map coordinates (-0.5...0.5)</param>
        /// <param name="totalCost">The total cost of traversing the path</param>
        /// <param name="terrainCapability">Type of terrain that the unit can pass through</param>
        /// <param name="minAltitude">Minimum altitude (0..1)</param>
        /// <param name="maxAltitude">Maximum altutude (0..1)</param>
        /// <param name="maxSearchCost">Maximum search cost for the path finding algorithm. A value of -1 will use the global default defined by pathFindingMaxCost</param>
        public IEnumerator FindRouteAsCoroutine(FindRouteASyncFinishMethod finished, Vector2 startPosition, Vector2 endPosition, TERRAIN_CAPABILITY terrainCapability = TERRAIN_CAPABILITY.Any, float minAltitude = 0, float maxAltitude = 1f, float maxSearchCost = -1, int maxSearchSteps = -1) {
            List<Vector2> points = null;
            float totalCost = 0;

            Task find = Task.Run(() => points = FindRoute(startPosition, endPosition, out totalCost, terrainCapability, minAltitude, maxAltitude, maxSearchCost, maxSearchSteps));
            yield return new WaitUntil(() => find.IsCompleted);
            finished(points, totalCost);
        }


        /// <summary>
        /// True if a FindRoute operation is being executed.
        /// </summary>
        public bool findRouteIsBusy;


        /// <summary>
        /// Returns an optimal route from startPosition to endPosition with options.
        /// </summary>
        /// <returns>The route.</returns>
        /// <param name="startPosition">Start position in map coordinates (-0.5...0.5)</param>
        /// <param name="endPosition">End position in map coordinates (-0.5...0.5)</param>
        /// <param name="totalCost">The total cost of traversing the path</param>
        /// <param name="terrainCapability">Type of terrain that the unit can pass through</param>
        /// <param name="minAltitude">Minimum altitude (0..1)</param>
        /// <param name="maxAltitude">Maximum altutude (0..1)</param>
        /// <param name="maxSearchCost">Maximum search cost for the path finding algorithm. A value of -1 will use the global default defined by pathFindingMaxCost</param>
        public List<Vector2> FindRoute(Vector2 startPosition, Vector2 endPosition, out float totalCost, TERRAIN_CAPABILITY terrainCapability = TERRAIN_CAPABILITY.Any, float minAltitude = 0, float maxAltitude = 1f, float maxSearchCost = -1, int maxSearchSteps = -1) {
            totalCost = 0;
            if (findRouteIsBusy || !_enablePathFindingSystem) {
                return null;
            }
            List<Vector2> routePoints = null;

            try {
                findRouteIsBusy = true;
                ComputeRouteMatrix(terrainCapability, minAltitude, maxAltitude);
                Point startingPoint = new Point((int)((startPosition.x + 0.5f) * EARTH_ROUTE_SPACE_WIDTH),
                                                   (int)((startPosition.y + 0.5f) * EARTH_ROUTE_SPACE_HEIGHT));
                Point endingPoint = new Point((int)((endPosition.x + 0.5f + 0.5f / EARTH_ROUTE_SPACE_WIDTH) * EARTH_ROUTE_SPACE_WIDTH),
                                                 (int)((endPosition.y + 0.5f + 0.5f / EARTH_ROUTE_SPACE_HEIGHT) * EARTH_ROUTE_SPACE_HEIGHT));
                endingPoint.X = Mathf.Clamp(endingPoint.X, 0, EARTH_ROUTE_SPACE_WIDTH - 1);
                endingPoint.Y = Mathf.Clamp(endingPoint.Y, 0, EARTH_ROUTE_SPACE_HEIGHT - 1);

                // Helper to find a minimum path in case the destination position is on a different terrain type
                if (terrainCapability == TERRAIN_CAPABILITY.OnlyWater) {
                    int arrayIndex = endingPoint.Y * EARTH_ROUTE_SPACE_WIDTH + endingPoint.X;
                    if ((earthRouteMatrix[arrayIndex] & 4) == 0) {
                        int regionIndex = -1;
                        int countryIndex = GetCountryIndex(endPosition, out regionIndex);
                        if (countryIndex >= 0 && regionIndex >= 0) {
                            List<Vector2> coastPositions = GetCountryCoastalPoints(countryIndex, regionIndex, 0.001f);
                            float minDist = float.MaxValue;
                            Vector2 bestPosition = Misc.Vector2zero;
                            int coastPositionsCount = coastPositions.Count;
                            // Get nearest position to the ship which is on water
                            for (int k = 0; k < coastPositionsCount; k++) {
                                Vector2 waterPosition;
                                if (ContainsWater(coastPositions[k], 0.001f, out waterPosition)) {
                                    float dist = FastVector.SqrDistance(ref endPosition, ref waterPosition); // (endPosition - waterPosition).sqrMagnitude;
                                    if (dist < minDist) {
                                        minDist = dist;
                                        bestPosition = waterPosition;
                                    }
                                }
                            }
                            if (minDist < float.MaxValue) {
                                endPosition = bestPosition;
                            }
                            endingPoint = new Point((int)((endPosition.x + 0.5f + 0.5f / EARTH_ROUTE_SPACE_WIDTH) * EARTH_ROUTE_SPACE_WIDTH),
                                (int)((endPosition.y + 0.5f + 0.5f / EARTH_ROUTE_SPACE_HEIGHT) * EARTH_ROUTE_SPACE_HEIGHT));
                            arrayIndex = endingPoint.Y * EARTH_ROUTE_SPACE_WIDTH + endingPoint.X;
                            if ((earthRouteMatrix[arrayIndex] & 4) == 0) {
                                Vector2 direction = Misc.Vector2zero;
                                for (int k = 1; k <= 10; k++) {
                                    if (k == 10 || startPosition == endPosition) {
                                        findRouteIsBusy = false;
                                        return null;
                                    }
                                    FastVector.NormalizedDirection(ref endPosition, ref startPosition, ref direction);
                                    Vector2 p = endPosition + direction * (float)k / EARTH_ROUTE_SPACE_WIDTH;
                                    endingPoint = new Point((int)((p.x + 0.5f + 0.5f / EARTH_ROUTE_SPACE_WIDTH) * EARTH_ROUTE_SPACE_WIDTH),
                                        (int)((p.y + 0.5f + 0.5f / EARTH_ROUTE_SPACE_HEIGHT) * EARTH_ROUTE_SPACE_HEIGHT));
                                    arrayIndex = endingPoint.Y * EARTH_ROUTE_SPACE_WIDTH + endingPoint.X;
                                    if ((earthRouteMatrix[arrayIndex] & 4) > 0)
                                        break;
                                }
                            }
                        }
                    }
                }

                // Minimum distance for routing?
                if (Mathf.Abs(endingPoint.X - startingPoint.X) > 0 || Mathf.Abs(endingPoint.Y - startingPoint.Y) > 0) {
                    finder.Formula = _pathFindingHeuristicFormula;
                    finder.MaxSearchCost = maxSearchCost < 0 ? _pathFindingMaxCost : maxSearchCost;
                    finder.MaxSteps = maxSearchSteps < 0 ? _pathFindingMaxSteps : maxSearchSteps;
                    if (_pathFindingEnableCustomRouteMatrix) {
                        finder.OnCellCross = FindRoutePositionValidator;
                    } else {
                        finder.OnCellCross = null;
                    }
                    List<PathFinderNode> route = finder.FindPath(startingPoint, endingPoint, out totalCost);
                    if (route != null) {
                        routePoints = new List<Vector2>(route.Count);
                        routePoints.Add(startPosition);
                        for (int r = route.Count - 1; r >= 0; r--) {
                            float x = (float)route[r].X / EARTH_ROUTE_SPACE_WIDTH - 0.5f;
                            float y = (float)route[r].Y / EARTH_ROUTE_SPACE_HEIGHT - 0.5f;
                            Vector2 stepPos = new Vector2(x, y);

                            // due to grid effect the first step may be farther than the current position, so we skip it in that case.
                            if (r == route.Count - 1 && (endPosition - startPosition).sqrMagnitude < (endPosition - stepPos).sqrMagnitude)
                                continue;

                            routePoints.Add(stepPos);
                        }
                    } else {
                        findRouteIsBusy = false;
                        return null;    // no route available
                    }
                }

                // Add final step if it's appropiate
                bool hasWater = ContainsWater(endPosition);
                if (terrainCapability == TERRAIN_CAPABILITY.Any || terrainCapability == TERRAIN_CAPABILITY.Air ||
                             (terrainCapability == TERRAIN_CAPABILITY.OnlyWater && hasWater) ||
                             (terrainCapability == TERRAIN_CAPABILITY.OnlyGround && !hasWater)) {
                    if (routePoints == null) {
                        routePoints = new List<Vector2>();
                        routePoints.Add(startPosition);
                        routePoints.Add(endPosition);
                    } else {
                        routePoints[routePoints.Count - 1] = endPosition;
                    }
                }

                // Check that ground units ends in a position where GetCountryIndex returns a valid index
                if (terrainCapability == TERRAIN_CAPABILITY.OnlyGround) {
                    int rr = routePoints.Count - 1;
                    Vector2 dd = routePoints[rr - 1] - routePoints[rr];
                    dd *= 0.1f;
                    while (GetCountryIndex(routePoints[rr]) < 0) {
                        routePoints[rr] += dd;
                    }
                }

            } catch { }
            findRouteIsBusy = false;
            return routePoints;
        }

        /// <summary>
        /// Resets the custom route matrix. Use this custom route matrix to set location customized costs.
        /// </summary>
        public void PathFindingCustomRouteMatrixReset() {
            if (_customRouteMatrix == null || _customRouteMatrix.Length == 0) {
                _customRouteMatrix = new float[EARTH_ROUTE_SPACE_WIDTH * EARTH_ROUTE_SPACE_HEIGHT];
            }
            for (int k = 0; k < _customRouteMatrix.Length; k++) {
                _customRouteMatrix[k] = -1;
            }
            if (_pathFindingVisualizeMatrixCost) {
                UpdatePathfindingMatrixCostTexture();
            }
        }

        /// <summary>
        /// Sets the movement cost for a given map position.
        /// </summary>
        public void PathFindingCustomRouteMatrixSet(Vector2 position, int cost) {
            PathFindingCustomRouteMatrixSet(new List<Vector2>() { position }, cost);
        }

        /// <summary>
        /// Sets the movement cost for a list of map positions.
        /// </summary>
        public void PathFindingCustomRouteMatrixSet(List<Vector2> positions, int cost) {
            if (_customRouteMatrix == null)
                PathFindingCustomRouteMatrixReset();
            int positionsCount = positions.Count;
            for (int p = 0; p < positionsCount; p++) {
                Point point = Map2DToMatrixCostPosition(positions[p]);
                if (point.X < 0 || point.X >= EARTH_ROUTE_SPACE_WIDTH || point.Y < 0 || point.Y >= EARTH_ROUTE_SPACE_HEIGHT)
                    return;
                int location = PointToRouteMatrixIndex(point);
                _customRouteMatrix[location] = cost;
            }
            if (_pathFindingVisualizeMatrixCost)
                UpdatePathfindingMatrixCostTexture();
        }


        /// <summary>
        /// Prewarms country region positions for later use of path-finding
        /// </summary>
        public void PathFindingPrewarmCountryPositions() {
            int countryCount = countries.Length;

            Parallel.For(0, countryCount, (c) => {
                Country country = _countries[c];
                int regionCount = country.regions.Count;
                for (int cr = 0; cr < regionCount; cr++) {
                    Region region = country.regions[cr];
                    if (region.pathFindingPositions == null) {
                        Rect rect = region.rect2D;
                        Point start = Map2DToMatrixCostPosition(new Vector2(rect.xMin, rect.yMin));
                        Point end = Map2DToMatrixCostPosition(new Vector2(rect.xMax, rect.yMax));
                        region.pathFindingPositions = new List<int>();
                        Vector2 position;
                        for (int j = start.Y; j <= end.Y; j++) {
                            float y = (float)j / EARTH_ROUTE_SPACE_HEIGHT - 0.5f;
                            position.y = y;
                            int yy = j * EARTH_ROUTE_SPACE_WIDTH;
                            for (int k = start.X; k <= end.X; k++) {
                                float x = (float)k / EARTH_ROUTE_SPACE_WIDTH - 0.5f;
                                position.x = x;
                                if (region.Contains(position)) {
                                    int pos = yy + k;
                                    region.pathFindingPositions.Add(pos);
                                }
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Sets the movement cost for a region of the map.
        /// </summary>
        public void PathFindingCustomRouteMatrixSet(Region region, int cost) {
            if (_customRouteMatrix == null)
                PathFindingCustomRouteMatrixReset();
            if (region.pathFindingPositions == null) {
                Rect rect = region.rect2D;
                Point start = Map2DToMatrixCostPosition(new Vector2(rect.xMin, rect.yMin));
                Point end = Map2DToMatrixCostPosition(new Vector2(rect.xMax, rect.yMax));
                region.pathFindingPositions = new List<int>();
                Vector2 position;
                for (int j = start.Y; j <= end.Y; j++) {
                    float y = (float)j / EARTH_ROUTE_SPACE_HEIGHT - 0.5f;
                    position.y = y;
                    int yy = j * EARTH_ROUTE_SPACE_WIDTH;
                    for (int k = start.X; k <= end.X; k++) {
                        int pos = yy + k;
                        if (_customRouteMatrix[pos] != cost) {
                            float x = (float)k / EARTH_ROUTE_SPACE_WIDTH - 0.5f;
                            position.x = x;
                            if (region.Contains(position)) {
                                _customRouteMatrix[pos] = cost;
                                region.pathFindingPositions.Add(pos);
                            }
                        }
                    }
                }
            } else {
                int maxk = region.pathFindingPositions.Count;
                for (int k = 0; k < maxk; k++) {
                    int position = region.pathFindingPositions[k];
                    _customRouteMatrix[position] = cost;
                }
            }
            if (_pathFindingVisualizeMatrixCost)
                UpdatePathfindingMatrixCostTexture();
        }

        /// <summary>
        /// Sets the movement cost for a country.
        /// </summary>
        public void PathFindingCustomRouteMatrixSet(Country country, int cost) {
            int rcount = country.regions.Count;
            Parallel.For(0, rcount, (r) => {
                PathFindingCustomRouteMatrixSet(country.regions[r], cost);
            });
            country.crossCost = cost;
        }

        /// <summary>
        /// Sets the movement cost for a province.
        /// </summary>
        public void PathFindingCustomRouteMatrixSet(Province province, int cost) {
            int rcount = province.regions.Count;
            Parallel.For(0, rcount, (r) => {
                PathFindingCustomRouteMatrixSet(province.regions[r], cost);
            });
            province.crossCost = cost;
        }

        /// <summary>
        /// Returns the indices of the provinces the path is crossing.
        /// </summary>
        public List<int> PathFindingGetProvincesInPath(List<Vector2> path) {
            if (path == null)
                return null;
            List<int> provincesIndices = new List<int>();
            for (int k = 0; k < path.Count; k++) {
                int provinceIndex = GetProvinceIndex(path[k]);
                if (provinceIndex >= 0 && !provincesIndices.Contains(provinceIndex))
                    provincesIndices.Add(provinceIndex);
            }
            return provincesIndices;
        }

        /// <summary>
        /// Returns the indices of the provinces the path is crossing.
        /// </summary>
        public List<int> PathFindingGetCountriesInPath(List<Vector2> path) {
            if (path == null)
                return null;
            List<int> countriesIndices = new List<int>();
            for (int k = 0; k < path.Count; k++) {
                int countryIndex = GetCountryIndex(path[k]);
                if (countryIndex >= 0 && !countriesIndices.Contains(countryIndex))
                    countriesIndices.Add(countryIndex);
            }
            return countriesIndices;
        }

        /// <summary>
        /// Returns the indices of the cities the path is crossing.
        /// </summary>
        public List<int> PathFindingGetCitiesInPath(List<Vector2> path) {
            if (path == null)
                return null;
            List<int> citiesIndices = new List<int>();
            for (int k = 0; k < path.Count; k++) {
                int countryIndex = GetCountryIndex(path[k]);
                int cityIndex = GetCityNearPoint(path[k], countryIndex);
                if (cityIndex >= 0 && !citiesIndices.Contains(cityIndex))
                    citiesIndices.Add(cityIndex);
            }
            return citiesIndices;
        }

        /// <summary>
        /// Returns the indices of the mount points the path is crossing.
        /// </summary>
        public List<int> PathFindingGetMountPointsInPath(List<Vector2> path) {
            if (path == null)
                return null;
            List<int> mountPointsIndices = new List<int>();
            for (int k = 0; k < path.Count; k++) {
                int mountPointIndex = GetMountPointNearPoint(path[k]);
                if (mountPointIndex >= 0 && !mountPointsIndices.Contains(mountPointIndex))
                    mountPointsIndices.Add(mountPointIndex);
            }
            return mountPointsIndices;
        }

        #endregion

        #region Specific PathFinding APIs related to countries and provinces

        /// <summary>
        /// Returns an optimal route from starting Country to destination Country. The capability to move from one country to another is determined by the neighbours array of the Country object.
        /// </summary>
        /// <returns>The result is a list of country indices.</returns>
        /// <param name="startingCountryIndex">The index for the starting country</param>
        /// <param name="destinationCountryIndex">The index for the destination country</param>
        /// <param name="maxSearchCost">Maximum search cost for the path finding algorithm. A value of -1 will use the global default defined by pathFindingMaxCost</param>
        public List<int> FindRoute(Country startingCountry, Country destinationCountry, float maxSearchCost = -1) {
            int startingCountryIndex = GetCountryIndex(startingCountry);
            int destinationCountryIndex = GetCountryIndex(destinationCountry);
            if (destinationCountryIndex == startingCountryIndex || destinationCountryIndex < 0 || startingCountryIndex < 0)
                return null;
            List<int> routePoints = null;

            if (finderCountries == null) {
                finderCountries = new PathFinderAdminEntity(countries);
            }
            finderCountries.SearchLimit = maxSearchCost < 0 ? _pathFindingMaxCost : maxSearchCost;
            if (OnPathFindingCrossCountry != null) {
                finderCountries.OnAdminEntityCross = FindRouteCountryValidator;
            } else {
                finderCountries.OnAdminEntityCross = null;
            }
            List<PathFinderNodeAdmin> route = finderCountries.FindPath(startingCountryIndex, destinationCountryIndex);
            if (route != null) {
                routePoints = new List<int>(route.Count);
                routePoints.Add(startingCountryIndex);
                for (int r = route.Count - 1; r >= 0; r--) {
                    routePoints.Add(route[r].Index);
                }
            } else {
                return null;    // no route available
            }

            // Add final step if it's appropiate
            return routePoints;
        }

        /// <summary>
        /// Returns an optimal route from starting Province to destination Province. The capability to move from one province to another is determined by the neighbours array of the Province object.
        /// </summary>
        /// <returns>The result is a list of province indices.</returns>
        /// <param name="startingProvince">The index for the starting province</param>
        /// <param name="destinationProvince">The index for the destination province</param>
        /// <param name="maxSearchCost">Maximum search cost for the path finding algorithm. A value of -1 will use the global default defined by pathFindingMaxCost</param>
        public List<int> FindRoute(Province startingProvince, Province destinationProvince, float maxSearchCost = -1) {
            int startingProvinceIndex = GetProvinceIndex(startingProvince);
            int destinationProvinceIndex = GetProvinceIndex(destinationProvince);
            if (destinationProvinceIndex == startingProvinceIndex || destinationProvinceIndex < 0 || startingProvinceIndex < 0)
                return null;
            List<int> routePoints = null;

            if (finderProvinces == null) {
                finderProvinces = new PathFinderAdminEntity(provinces);
            }
            finderProvinces.SearchLimit = maxSearchCost < 0 ? _pathFindingMaxCost : maxSearchCost;
            if (OnPathFindingCrossProvince != null) {
                finderProvinces.OnAdminEntityCross = FindRouteProvinceValidator;
            } else {
                finderProvinces.OnAdminEntityCross = null;
            }
            List<PathFinderNodeAdmin> route = finderProvinces.FindPath(startingProvinceIndex, destinationProvinceIndex);
            if (route != null) {
                routePoints = new List<int>(route.Count);
                routePoints.Add(startingProvinceIndex);
                for (int r = route.Count - 1; r >= 0; r--) {
                    routePoints.Add(route[r].Index);
                }
            } else {
                return null;    // no route available
            }

            // Add final step if it's appropiate
            return routePoints;
        }

        #endregion


        #region Specific PathFinding APIs related to cells


        /// <summary>
        /// Returns a copy of the current cells cost array or set it.
        /// </summary>
        public CellCosts[] pathFindingCustomCellCosts {
            get {
                int cc = _cellsCosts.Length;
                CellCosts[] copy = new CellCosts[cc];
                for (int c = 0; c < cc; c++) {
                    copy[c] = _cellsCosts[c];
                    if (copy[c].crossCost != null) {
                        float[] y = new float[6];
                        Array.Copy(copy[c].crossCost, y, 6);
                        copy[c].crossCost = y;
                    }
                }
                return copy;
            }
            set {
                if (cells != null && _cellsCosts != null && _cellsCosts.Length == cells.Length) {
                    _cellsCosts = value;
                    finderCells.SetCustomCellsCosts(_cellsCosts);
                }
            }
        }



        /// <summary>
        /// Returns an optimal route from starting Cell to destination Cell. The capability to move from one province to another is determined by the neighbours array of the Cell object.
        /// </summary>
        /// <returns>The result is a list of province indices.</returns>
        /// <param name="startingCell">The index for the starting province</param>
        /// <param name="destinationCell">The index for the destination province</param>
        /// <param name="terrainCapability">The allowed terrains for the route. Any/Ground/Water</param>
        /// <param name="maxSearchCost">Maximum search cost for the path finding algorithm. A value of -1 will use the global default defined by pathFindingMaxCost</param>
        /// <param name="maxSearchSteps">Maximum number of steps for the path finding algorithm. A value of -1 will use the global default defined by pathFindingMaxSteps</param>
        public List<int> FindRoute(Cell startingCell, Cell destinationCell, TERRAIN_CAPABILITY terrainCapability = TERRAIN_CAPABILITY.Any, float maxSearchCost = -1, int maxSearchSteps = -1) {
            float dummy;
            return FindRoute(startingCell, destinationCell, out dummy, terrainCapability, maxSearchCost: maxSearchCost, maxSearchSteps: maxSearchSteps);
        }


        /// <summary>
        /// Returns an optimal route from starting Cell to destination Cell. This method is designed to be called as coroutine. Upon termination, the "finished" method will be called passing the list of points of the path and the total cost.
        /// </summary>
        /// <returns>The route.</returns>
        /// <param name="finished">Delegate to be called when the path calculation finishes. This delegate receives the list of indices of cells contained in the path and the total cost.</param>
        /// <param name="startPosition">Start position in map coordinates (-0.5...0.5)</param>
        /// <param name="endPosition">End position in map coordinates (-0.5...0.5)</param>
        /// <param name="totalCost">The total cost of traversing the path</param>
        /// <param name="terrainCapability">Type of terrain that the unit can pass through</param>
        /// <param name="minAltitude">Minimum altitude (0..1)</param>
        /// <param name="maxAltitude">Maximum altutude (0..1)</param>
        /// <param name="maxSearchCost">Maximum search cost for the path finding algorithm. A value of -1 will use the global default defined by pathFindingMaxCost</param>
        public IEnumerator FindRouteAsCoroutine(FindRouteCellsASyncFinishMethod finished, Cell startingCell, Cell destinationCell, TERRAIN_CAPABILITY terrainCapability, float maxSearchCost = -1, float minAltitude = 0f, float maxAltitude = 1f, int maxSearchSteps = -1) {
            List<int> cellIndices = null;
            float totalCost = 0;

            Task find = Task.Run(() => cellIndices = FindRoute(startingCell, destinationCell, out totalCost, terrainCapability, maxSearchCost, minAltitude, maxAltitude, maxSearchSteps));
            yield return new WaitUntil(() => find.IsCompleted);
            finished(cellIndices, totalCost);
        }


        /// <summary>
        /// Returns an optimal route from starting Cell to destination Cell. The capability to move from one province to another is determined by the neighbours array of the Cell object.
        /// </summary>
        /// <returns>The result is a list of province indices.</returns>
        /// <param name="startingCell">The index for the starting province</param>
        /// <param name="destinationCell">The index for the destination province</param>
        /// <param name="terrainCapability">The allowed terrains for the route. Any/Ground/Water</param>
        /// <param name="maxSearchCost">Maximum search cost for the path finding algorithm. A value of -1 will use the global default defined by pathFindingMaxCost</param>
        /// <param name="minAltitude">Minimum terrain altitude allowed</param>
        /// <param name="minAltitude">Maximum terrain altitude allowed</param>
        public List<int> FindRoute(Cell startingCell, Cell destinationCell, out float totalCost, TERRAIN_CAPABILITY terrainCapability, float maxSearchCost = -1, float minAltitude = 0f, float maxAltitude = 1f, int maxSearchSteps = -1) {
            totalCost = 0;
            if (findRouteIsBusy) {
                return null;
            }
            List<int> routePoints = null;
            try {
                findRouteIsBusy = true;
                ComputeCellsCostsInfo();
                int startingCellIndex = GetCellIndex(startingCell);
                int destinationCellIndex = GetCellIndex(destinationCell);
                if (destinationCellIndex == startingCellIndex || destinationCellIndex < 0 || startingCellIndex < 0) {
                    findRouteIsBusy = false;
                    return null;
                }
                if (finderCells == null) {
                    findRouteIsBusy = false;
                    return null;
                }
                finderCells.Formula = _pathFindingHeuristicFormula;
                finderCells.MaxSearchCost = maxSearchCost < 0 ? _pathFindingMaxCost : maxSearchCost;
                finderCells.MaxSteps = maxSearchSteps < 0 ? _pathFindingMaxSteps : maxSearchSteps;
                finderCells.TerrainCapability = terrainCapability;
                finderCells.MinAltitude = minAltitude;
                finderCells.MaxAltitude = maxAltitude;

                if (OnPathFindingCrossCell != null) {
                    finderCells.OnCellCross = FindRouteCellValidator;
                } else {
                    finderCells.OnCellCross = null;
                }
                Point startPoint = new Point(startingCell.column, startingCell.row);
                Point endPoint = new Point(destinationCell.column, destinationCell.row);
                List<PathFinderNode> route = finderCells.FindPath(startPoint, endPoint, out totalCost);
                if (route != null) {
                    int routeCount = route.Count;
                    routePoints = new List<int>(routeCount);
                    for (int r = routeCount - 1; r >= 0; r--) {
                        routePoints.Add(route[r].Y * _gridColumns + route[r].X);
                    }
                } else {
                    routePoints = null;
                }
            } catch { }
            findRouteIsBusy = false;
            return routePoints;
        }

        /// <summary>
        /// Sets the cost for crossing a given cell side. Note that the cost is only assigned in one direction (from this cell to the outside).
        /// </summary>
        public void PathFindingCellSetSideCost(int cellIndex, CELL_SIDE side, int cost) {
            if (_cellsCosts == null || cellIndex < 0 || cellIndex >= _cellsCosts.Length)
                return;
            _cellsCosts[cellIndex].SetSideCrossCost(side, cost);
        }

        /// <summary>
        /// Sets the cost for crossing any cell side. Note that the cost is only assigned in one direction (from this cell to the outside).
        /// </summary>
        public void PathFindingCellSetAllSidesCost(int cellIndex, int cost) {
            if (_cellsCosts == null || cellIndex < 0 || cellIndex >= _cellsCosts.Length)
                return;
            _cellsCosts[cellIndex].SetAllSidesCost(cost);
        }


        /// <summary>
        /// Sets the extra cost for crossing a given cell side. The cell side is automatically determined and both directions are used.
        /// </summary>
        public void PathFindingCellSetSideCost(int cell1, int cell2, int cost) {
            if (cells == null || cell1 < 0 || cell1 >= cells.Length)
                return;
            if (cell2 < 0 || cell2 >= cells.Length)
                return;
            int row0 = cells[cell1].row;
            int col0 = cells[cell1].column;
            int row1 = cells[cell2].row;
            int col1 = cells[cell2].column;
            CELL_SIDE side1, side2;
            // top or bottom
            if (col0 == col1) {
                if (row0 < row1) {
                    side1 = CELL_SIDE.Top;
                    side2 = CELL_SIDE.Bottom;
                } else {
                    side1 = CELL_SIDE.Bottom;
                    side2 = CELL_SIDE.Top;
                }
            } else if (col0 < col1) {
                if (row0 < row1) {
                    side1 = CELL_SIDE.TopRight;
                    side2 = CELL_SIDE.BottomLeft;
                } else {
                    side1 = CELL_SIDE.BottomRight;
                    side2 = CELL_SIDE.TopLeft;
                }
            } else {
                if (row0 < row1) {
                    side1 = CELL_SIDE.TopLeft;
                    side2 = CELL_SIDE.BottomRight;
                } else {
                    side1 = CELL_SIDE.BottomLeft;
                    side2 = CELL_SIDE.TopRight;
                }
            }
            _cellsCosts[cell1].SetSideCrossCost(side1, cost);
            _cellsCosts[cell2].SetSideCrossCost(side2, cost);
        }


        /// <summary>
        /// Gets the extra cost for crossing a given cell side
        /// </summary>
        public float PathFindingCellGetSideCost(int cellIndex, CELL_SIDE side) {
            if (_cellsCosts == null || cellIndex < 0 || cellIndex >= _cellsCosts.Length)
                return -1;
            return _cellsCosts[cellIndex].crossCost[(int)side];
        }


        /// <summary>
        /// Sets land mask bits in a circle defined by a position and radius
        /// </summary>
        /// <param name="pos">Position in local map coordinates (-0.5..0.5)</param>
        /// <param name="size">Size of the filling box in the same local map space coordinates</param>
        public void PathFindingSetLand(Vector2 pos, float size) {
            if (!CheckRouteLandAndWaterMask()) return;
            SetPathFindingMaskValue(pos, size, pathfindingLandMask);
        }


        /// <summary>
        /// Sets water mask bits in a circle defined by a position and radius
        /// </summary>
        /// <param name="pos">Position in local map coordinates (-0.5..0.5)</param>
        /// <param name="size">Size of the filling box in the same local map space coordinates</param>
        public void PathFindingSetWater(Vector2 pos, float size) {
            if (!CheckRouteLandAndWaterMask()) return;
            SetPathFindingMaskValue(pos, size, pathfindingWaterMask);
        }

        #endregion

    }

}