***************************************************
*              WORLD MAP STRATEGY KIT             *
*       Copyright (C) Kronnect Technologies SL    *
*                  README FILE                    *
***************************************************


How to use this asset
---------------------

Firstly, you should run the Demo Scenes provided to get an idea of the overall functionality.
Later, you should read the documentation and experiment with the API/prefabs.


Demo Scenes
----------- 
There're several demo scenes, located in "Demos" folder. Just go there from Unity, open them in order to get an idea of the asset possibilities.


Documentation/API reference
---------------------------
The PDF is located in the Doc folder. It contains instructions on how to use the prefab and the API so you can control it from your code.


Youtube Videos
--------------
Check out the sample videos available on our Youtube Channel!
https://www.youtube.com/KronnectGames


Support
-------
Please read the documentation PDF and browse/play with the demo scene and sample source code included before contacting us for support :-)

* Support-Web: https://kronnect.com/support
* Support-Discord: https://discord.gg/EH2GMaM
* Email: contact@kronnect.com
* Twitter: @Kronnect


Version history
---------------

Current version
- Added "Velocity Threshold" parameter to "Allow Interaction When Flying" option

Version 13.4.2
- [Fix] Fixed Map Editor backup issue
- [Fix] Cloud layer render queue is correctly set under URP with Unity 2021.3
- [Fix] Fixes an issue with combine meshes and include all regions property

Version 13.4
- Added "Combine Surfaces" option under "Country" section in the inspector. When enabled, surfaces of same country colored with ToggleCountrySurface will be combined, reducing draw calls
- Country/provinces merge optimizations
- [Fix] Fixed textured outline borders between adjacent countries not rendering correctly

Version 13.3
- [Fix] Map Generator fixes

Versopm 13.2
- [Fix] Prevents OnClick event when performing very quick drags
- [Fix] Map Generator fixes

Version 13.1
- Random Map Generator improvements. New demo scene "109 Random Map Generation" with sample script
- API: GameObjectAnimator is now a partial class
- API: GameObjectAnimator: added autoScaleMin/autoScaleMax optional properties
- Custom borders optimizations with dynamic batching
- Improved cache of materials using color and texture which reduces drawcalls

Version 13.0
- Map Editor: multiple country and province regions can be selected by holding down control key
- Map Editor: pressing Shift + X will delete selected regions
- API: added GetRenderViewportDistanceToCamera(camera)
- [Fix] Map Editor: improved Reshape/Smooth operation
- [Fix] API: fixed CountryRemoveProvinces() no removing some provinces from source country

Version 12.9
- Internal improvements and minor fixes related to viewport rendering in Editor
- [Fix] World-wrapping mode fixes when using UI Panel

Version 12.8
- Performance optimizations to GameObjectAnimator component
- GameObjectAnimator: added "minZoomLevel" and "maxZoomLevel" properties. Let you specify the visibility of the unit based on zoom level

Version 12.7.1
- [Fix] Fixed regression in demo scene "007 Sprite Movement"

Version 12.7
- Added option to disable path-finding system which saves memory and speeds up initialization
- Memory optimizations for mobile
- API: added PathFindingSetLand, PathFindingSetWater methods which allow runtime modification of path-finding land/water masks

Version 12.6
- Added "Allow Interaction While Flying" option to inspector
- Added Labels Rendering Mode (blended with world texture or floating above viewport)
- Internal changes to province transfer functions
- [Fix] Fixed air units and trajectories direction with world wrapping option

Version 12.5
- API: added "GetCityPosition(cityName, countryName)" overload
- [Fix] Map Editor: fixed potential issue if all cities are removed
- [Fix] Fixed rendering issue with 16K style in linear color space
- [Fix] Fixed some logic regarding world wrapping
- [Fix] Fixed render queue of certain materials when using URP and viewport mode

Version 12.4.2
   - Added message when assigning a terrain to the viewport and terrain shaders can't be located
   - [Fix] Grid alpha on water option now uses the current water mask instead of the default texture in the material

Version 12.4.1
   - Map Editor: some optimizations to the move operation tool
   - Map Editor: random map generation now produces land & water map textures for pathfinding as well
   - [Fix] Map didn't redraw with latest changes after using the territory importer tool if SceneView was not visible/active

Version 12.4
   - Change: pathfinding heightmap removed and replaced with 2 texture maps: land and water
   - [Fix] Fixed an exception when highlighting provinces with countries as enclaves
   - [Fix] Fixed a potential cast exception with API CountryRemoveProvinces

Version 12.3
   - Added demo scene: "104b Loading and Saving JSON"
   - API: added jSON support for storage of geodata and attributes (GetCountriesDataJSON/SetCountriesDataJSON, GetProvincesDataJSON/SetProvincesDataJSON, GetCitiesDataJSON/SetCitiesDataJSON, GetMountPointsDataJSON/SetMountPointsDataJSON)
   - Option to show/hide water in scenic styles
   - [Fix] Fixed navigation block in Gibraltar and other straights

Version 12.2.1
   - [Fix] Fixed neighbour search issue when building with IL2CPP

Version 12.2
   - Added Highlight Country/Province Keep Texture options to inspector
   - Performance optimizations in ProvinceToCountry methods
   - Canvas elements visibility on units are now managed by Game Object Animator
   - API: added CountryIsNeighbour, CountryMakeNeighbours
   - [Fix] Fixed OnCountryExit event not triggered when country highlighted is disabled
   - [Fix] Fixed neighbours of Republic of Congo and South Sudan in high definition geodata file

Version 12.1
   - Improved demo scene 405 so UI panel is kept within screen borders
   - [Fix] Fixed country, province and city attribute filename being reset when entering playmode
   - [Fix] Fixed province persistence issue when building the app
   - [Fix] Fixed URP pipeline detector issue which could not load correct shaders

Version 12.0
   - Memory and performance optimizations in viewport mode
   - API: added fitWindowHeightLimitTop, fitWindowHeightLimitBottom, fitWindowWidthLimitRight, fitWindowWidthLimitLeft
   - [Fix] Fixed GenerateMap() potential name repetition issue

Version 11.9
   - Minimum Unity version required: 2020.3.16
   - Added new demo scene "007 Sprite Movement"
   - [Fix] Fixed sprite rotation issues
   - [Fix] Fixed interaction when input system is set to 'Both' in player settings

Version 11.8 31/May/2022
   - Added "Max Pixel Width" option to country thicker borders section
   - API: new methods SetWaterMask, SetEarthTexture, SetHeightmapTexture which ensures the textures are refreshed (useful if texture contents may be changed from any process)
   - API: added FindRouteAsCoroutine which runs in background thread
   - Map Editor: improvements related to province editing
   - Map Editor: added option to add province to country regions to inspector (provinces created in editor do not create country regions by default)
   - Map Editor: added option to merge adjacent regions of countries to inspector
   - Map Editor: frontiers are now fully redrawn when executing a reshape move operation
   - [Fix] Fixed heightmap texture not being refreshed when generating map from scripting

Version 11.7 29/Apr/2022
   - Added compatibility with "Enter Playmode Options" for faster iteration
   - Map Editor: transferring all provinces to background/pool country will also remove any country region
   - Managed gameobjects on terrain viewport are now clipped when out of viewport rectangle 
   - [Fix] Fixes related to Text Mesh Pro rendering

Version 11.6 21/Mar/2022
   - New demo scene: 407b Path on UI Element
   - Added Shadow offset option to country and province labels sections
   - Map Editor: added "Transfer All Provinces to Background/Pool Country" button

Version 11.5 5/Feb/2022
   - New option in the inspector to position country and province labels on the centroid instead of using the geometric center
   - API: added GetCellAltitude(cellIndex). Returns the altitude of a cell as it would be used by GetCellsByAltitude or path-finding methods.
   - API: added GetCountryCentroid, GetProvinceCentroid.
   - API: added region.centroid as an alternate to region.center which is always inside the polygon

Version 11.4 7/Jan/2022
   - API: added AddCircleOnSphere which draws a circle having into account the spherical shape of the world
   - API: added PathFindingPrewarmCountryPositions. This method is also used at start is PreWarm option is used. Use it to avoid path-finding hiccups the first time the custom matrix cost is set.

Version 11.3 22/Dec/2021
   - Minimum Unity version is now 2019.4.13f1
   - Support for the New Input System
   - New Earth style: Scenic 8K
   - New unit terrain capability: Air. This let the unit fly over oceans although it will still have into account any custom cost assigned to terrain
   - Allow toggling tiles at runtime (setting showTiles = true/false)

Version 11.2.3 28/Nov/2021
   - Improved Euclidean-based path finding routes

Version 11.2.2 18/Nov/2021
   - [Fix] Fixed issue with aerial units moving using MapLap option
   - [Fix] API: fixed OnProvinceExit not firing within same country

Version 11.2.1 27/Oct/2021
   - Added TextMesh Pro material property to inspector
   - [Fix] Fixed TextMesh Pro default "Lato" font to include required symbols by TMPro

Version 11.2 11/Oct/2021
   - Map Editor: added "Rebuild Country Frontiers from Provinces" option (click gear icon)
   - [Fix] Map Editor: fixed overseas province option bug
   - [Fix] Fixed MergeAdjacentAreas API issue
   - [Fix] Minor internal fixes to prevent console errors while editing code

Version 11.1.1 1/Oct/2021
   - Map Editor: improvements to certain operations like Magnet or Circle Move
   - [Fix] Fixed zoom interaction issue with 2D camera

Version 11.1 20/Sep/2021
   - Territory importer: overlapping regions are now automatically merged
   - Map Editor: the sanitize button will now detect overlapping regions and merge them
   - Added auto-fade options for province labels
   - When highlighting a province, enclaves are now removed from the generated surface
   - API: some GetMountPoints methods are now public
   - API Change: Fixed BlinkProvince effect when smoothBlink mode is false
   - API Change: Added BlinkCity smooth blinik effect which changes animation speed
   - API: GetProvinceBorderPoints: added optional pointIndices list as a return argument (useful to detect if points are sequential)
   - API: Added optional color argument to DrawProvinceLabels method
   - [Fix] Fixed FlyToCity method bug
   - [Fix] Fixed province colors issue when passing a custom color to DrawProvinces method

Version 10.9.2 10/Sep/2021
   - Map Editor: back-ups are now preserved. Each time a modification is saved, the previous backup is renamed and tagged with a timestamp in the filemame
   - Map Editor: new options in contextual gear menu to produce low-resolution country and province geodata files
   - [Fix] Fixed geodata files

Version 10.9.1 6/Sep/2021
   - [Fix] Fixed regional number issue in saved mount point json

Version 10.9 30/Aug/2021
   - API: added CountrySetProvinces, CountryAddProvinces, CountryRemoveProvinces

Version 10.8 3/Aug/2021
   - IMPORTANT: remove previous version before importing this update (make a backup of Geodata folder if you modified maps)
   - API: CountryTransferCountry: new parameter "mergeRegions", controls whether target regions get merged after union or not (faster)
   - API: CountryTransferProvinces: alternative to transfer several provinces in a more performant way
   - [Fix] API: fixed GetCellsInProvince() null exception error when grid is disabled
   - [Fix] Fixed an issue that highlighs a hidden country when setting showProvinces = true

Version 10.7 19/Jul/2021
   - Map Editor: territories importer now adds the option to specify a default country for overseas provinces
   - Map Editor: territories importer now stores the original texture color in "ImporterColor" attribute linked to each imported country or province (access it using country.attrib["ImporterColor"])
   - API: added CountryTransferProvince
   - [Fix] Fixed cities being reloaded when creating a new map
   - [Fix] Map Editor: fixes to territory importer

Version 10.6 7/Jun/2021
   - Map Editor: holding control + click will quickly remove a mount point
   - [Fix] Fixed issue with FindRoute with terrain capability is set to any when and Prewarm At Start option is enabled

Version 10.5.1 13/May/2021
   - API: added GetCountryCenter, GetProvinceCenter, GetCityPosition, GetMountPointPosition
   - [Fix] Fixed issue with changing province surface color

Version 10.5 30/Apr/2021
   - Added dash option for province borders
   - Support for multiple displays
   - API: added autoScale to AddMarker API for sprites. also added MarkerAutoScale component.
   - Prevents an error when assigning an UI Panel to the viewport component (suggestion: use the Map Panel native UI component instead)
   - [Fix] Country equalizer tool fixes
   - [Fix] Map Editor: new country doesn't get selected when creating it from an existing province

Version 10.4.1 19/Apr/2021
   - [Fix] Fixed GetProvinceBorderPoints method bug

Version 10.4 29/Mar/2021
   - API: added GetCellsWithinDistanceKM, returns cells within a distance in km to another cell
   - API: added new parameters to AddCircle (overdraw, renderOrder). Added example to demo scene 1.
   - [Fix] Map Editor: fixed an issue when removing shape points of a province

Version 10.3 10/Mar/2021
   - Added builtin drag and click capturing options to markers (MarkerClickHandler)
   - API: added GetAltitude(position), returns the heightmap elevation at a given map coordinate
   - [Fix] Fixed accuracy hit test issue with 2D markers

Version 10.2 5/Mar/2021
   - API: added ToggleCountryMainRegionOutline (main region only), ToggleCountryRegionOutline (specific region), ToggleCountryOutline (all regions)
   - [Fix] Fixed pathfinding issue with cells and word-wrapping mode
   - [Fix] Fixed potential issue when destroying gameobject animator components

Version 10.1 18/Feb/2021
   - Added "Highlight Country Recolor" option (defaults to true as normal behaviour). When disabled, the highlight can only draw an outline around countries and keep its current color/texture.
   - Added example for drawing interior country shadow (demo scene 1 / button "Interior Country Shadow")
   - API: added GetCellWithinRadius, GetCellWithinRectangle, GetCellsWithinCone
   - [Fix] Fixed issue when drag damping duration is set to 0

Version 10.0 16/Jan/2021
   - Added new map servers: MapBox (satellite, traffic, terrain, terrain-rgb, incidents, streets, countries), Google Maps (satellite + relief), ESRI (topo, streets, satellite, national geo style), Maps-For-Free, USGS satellite and OpenStreeMaps Hiking map.

Version 9.9
   - Decorators: added "Persistent" attribute. By default = true, means the decorator settings will be checked every 10 frames and applied again to the country if needed

Version 9.8.3
   - Change: switched path-finding cost from integer to float to improve granularity of estimations
   - General fixes and internal improvements
   - [Fix] Improvements of Text Mesh Pro integration

Version 9.8.1
   - Added "Max Cities Per Country". Refers to the max number of visible cities per country.
   - Added URP support to terrain mode
   - API: added CityAdd
   - [Fix] Fixed regression bug related to OnPointerRightDown event

Version 9.7
   - New UI Viewport mode: new Map Panel UI element will render map features directly inside a Canvas UI in 2D. See https://youtu.be/QuidxfCDkiI
   - Viewport mode 3D: UI Panel can now be dragged directly into the RenderViewport property to sync WMSK screen location with the UI Panel
   - Modifying current 2D map location in Game Object Animator inspector now refresh the unit position and altitude in Editor time
   - [Fix] API: fixed GetCellsInCountry() issue
   - [Fix] API: fixed lastDistanceFromCamera not being updated when allowUserZoom is disabled and a FlyTo operation is executed

Version 9.6.4
   - Country frontiers and province borders mesh generation optimizations
   - [Fix] Enclaves now are visible if "CoastLines" option is disabled
  
Version 9.6.3
   - Removed decorator max font size
   - Cities are no longer forced visible when opening the map editor
   - [Fix] Province/city geodata files fixes and additions

Version 9.6.2
   - [Fix] Fixed GameObjectAnimator unregistration when destroyed by user

Version 9.6.1
   - [Fix] Fixed aspect ratio of viewport when used with UI panel
   - [Fix] Fixed viewport shaders for URP
   - [Fix] Improved memory handling for temporary objects

Version 9.6
   - Performance optimization of GameObjectAnimators
   - Added zoom damping duration option
   - Improved zoom clamp when exceeding min/max distance
   - Standalone provinces (without a country region) can now be highlighted
   - Added "Respect Other UI" option to MarkerClickHandler script
   - [Fix] Fixed camera position change when issuing a Redraw() using UI panel as placeholder for map

Version 9.5
   - Improved city/mount point sprite autoscaling algorithm
   - Map Editor: added "Show Province Names" checkbox
   - Map Editor: inspector UI improvements
   - [Fix] Fixed viewport visibility issue when binding to UI panel

Version 9.4
   - Added "Grid Cut Out Borders" option
   - Added inertia effect when draggind and constant drag is enabled

Version 9.3.1
   - [Fix] Fixed issue when dragging the Viewport prefab to the scene

Version 9.3
   - API: added cacheMaterials property. Set this to false to force WMSK avoid caching materials (if you need to change directly renderer materials)
   - API: added GetCountryRegionIndex, GetProvinceRegionIndex to obtain region under certain map position
   - API: added renderer property to cell class. Contains a reference to the MeshRenderer component of the cell surface gameobject
   - API: added OnVGOCountryRegionEnter, OnVGOProvinceRegionEnter globa events
   - API: added OnCountryRegionEnter, OnprovinceRegionEnter events to GameObjectAnimator
   - API: added events OnFlyStart / OnFlyEnd
   - API: added ChangeDuration to GameObjectAnimator class

Version 9.2
   - MapPopulation demo scene: added button/example of provinces between two countries (France/Germany)
   - Tile system: added "Restrict To Area" option with lat/lon defined limits
   - Added FlyToTile method to navigate to a certain position defined by x, y and zoom level (tiled map format)
   - [Fix] Fixed precision issue with AddLine when using tiny coordinates
   - [Fix] Fixed "Show" geodata folder button bug in inspector
   - [Fix] Fixed "Export Provinces Color Map" issue

Version 9.1
   - OnMarkerMouseEnter & OnMarkerMouseExit events added to MarkerClickHandler component (uses in 2D Markers like sprites). Check demo scene 501 Viewport Intro and click Add Marker (2D Sprite).
   - Usability improvement when assigning terrain to WMSK
   - Map Editor: added "Map Tools" section with options to scale/shift/crop world map and texture
   - [Fix] Fixed custom/textured region borders not showing up in viewport mode

Version 9.0
   - Added "Hexify" option (countries and/or provinces) in Map Editor (available from the gear icon menu in the editor inspector)
   - Improved performance of grid generation
   - Bump map effect now uses diffuse wrap for better results
   - Support for LWRP 5.16+
   - Minimum Unity version required upped to 2017.4
   - API: added map.calc.prettyCurrentLatLon method
   - API: added SetGridDimensions
   - [Fix] Fixed issue when calling WMSK from Awake events from other scripts
   - [Fix] Fixed Map Editor error when a country has two provinces with same name

Version 8.5 13-JUN-2019
   - New demo scene 407 under "UI Examples". Video: https://youtu.be/HNh-kRgoo_M
   - New "UI Panel" property under Window Settings: automatically adapts viewport to UI panel position and size
   - Added frontiersCoastlines, provinceCoastlines options. Show/hides frontier segments on coasts.
   - FlyTo operations now are stopped if user clicks the map
   - [Fix] Fixed mesh collider issue in demo scene 508

Version 8.4.1 17-MAY-2019
   - [Fix] Fixed text encoding of some provinces in Turkey, Azerbaijan and Poland
   - [Fix] Fixed missing world texture in tiles downloader tool

Version 8.4 2-MAY-2019
   - API: Use GameObjectAnimator visible property to control unit visibility while allowing movement (vs disabling/enabling the whole gameobject)
   - [Fix] RectangleSelectionInitiate can now be called in the same frame the user clicks. Updated demo scene.
   - [Fix] Fixed mouse up event not firing correctly in MarkerClickHandler component

Version 8.3 17-APR-2019
   - Improvements to Map Generator. More options!
   - Added Screen Center option to inspector (Window section). Allows you to offset the screen position when centering the map or executing a FlyTo() operation. Useful when you have some data on one side of the screen and the centered location on the other side of the screen for example.

Version 8.2 6-APR-2019
   - API: added GetCountryCapital, GetCountryCapitalIndex
   - GameObjectAnimator: added updateWhenOffScreen to specify an unit must be scaled/rotated/positioned even if it's outside of viewport (by default false to improve performance with many units)
   - Optimized performance of unit management in viewport mode

Version 8.1 23-MAR-2019
   - Map Editor: improvements to Map Generator
   - Added option to show only current province under pointer name instead of displaying all province names of current country

Version 8.0 15-MAR-2019
   - Map Editor: new Map Generator section. Enables quick random world generation including countries, provinces, cities and random names plus textures (color, heightmap, water mask, bump map). Panel screenshot: https://i.imgur.com/utKvGpy.png
   - Improved performance of country and province labels rendering
   - Added Drag Damping Duration parameter to inspector
   - Implemented "Pinch and Throw" gesture when "Constant Drag Speed" is enabled
   - Optimizations to custom inspector
   - Added "Speed Multiplier" to "Allow User Keys" for dragging using keyboard
   - API: added "reverseMode" property to LineMarkerAnimator. Draws line from full length to 0.
   - API: new CountryCreate / ProvinceCreate methods
   - API: added allowHighlight property to Country and Province classes to allow per-entity highlight setting
   - API: added showProvinces / allowProvincesHighlight to Country class
   - [Fix] Dragging with keys no longer stop when clicking mouse buttons

Version 7.6 22-JAN-2019
   - New start/end line caps. New LineMarkerAnimator properties: startCap, startCapMaterial, startCapScale, startCapOffset, endCap, endCapMaterial, endCapScale, endCapOffset. Check demo scene 001 (click button Add Trajectories) or demo scene 504 PathAndLines.
   - Added "Redraw" button to inspector (Window section)
   - [Fix] Camera culling mask now refresh properly if WMSK root gameobject layer changes
   - [Fix] Disabled or workaround for code not compatible with Windows Universal Platform

Version 7.5 16-DEC-2018
   - Viewport heightmap now uses several mip-levels and show better detail on zoom
   - Increased range for viewport resolution parameters (now it supports up to 8K render texture)
   - Added Curvature Min Zoom (now you can specify different curvature levels for zoom max/min)
   - City icons are now showed on top of frontiers
   - API: added FlyToLatLon()

Version 7.4 28-NOV-2018
   - Added province parameter to GetCity/GetCityIndex methods
   - Optimized performance of CountryTransferProvinceRegion() method
   - API: VGORegisterGameObject now includes a safety check to avoid repeated uniqueId values between gameobjects
   - API: new VGOGet(rect, results, predicate) method
   - [Fix] Fixed province reference issue with mount points
   - [Fix] Fixed flipping issue with sprites
   - [Fix] Fixed Unity 2018.3 beta prefab & network API breaking changes

Version 7.3 9-NOV-2018
   - Ability to pause the game. API: map.paused = true|false
   - Added Lighting mode to render viewport in inspector
   - Reduced load/initialization time for water mask
   - Added NGUI event compatibility with Game Object Animator component
   - API: added timeSpeed (defaults to 1)
   - API: added FindRoute(city1, city2), FindRoute(city1Name, country1Name, city2Name, country2Name)
   - API: improved performance of RestoreCellMaterials method
   - API: added VGOGet(list) which fills the user-supplied list with all gameobjects added to the viewport
   - API: added GetCityRandom(province), GetCityIndexRandom(province)
   - API: added GetZoomExtents(rect)
   - Sprites added to the viewport with WMSK_MoveTo now receive a BoxCollider2D instead of BoxCollider if they have no collider attached
   - [Fix] Disabling grid at runtime leaves a colored hexagon on the map
   - [Fix] Fixed tile system issue in viewport mode when world wrapping is enabled
   - [Fix] Fixed issue with rectangle selection
   - [Fix] Changing GameObjectAnimator uniqueId disconnected the unit from the map
   - [Fix] Fixed colored regions clipping issue with map tiles
   - [Fix] Fixed Editor hang when dropping the Viewport prefab in SceneView

Version 7.2 24-OCT-2018
   - New curved map mode. Demo scene 515 under Viewport Examples.
   - Ticker Texts: added anchor property to customize text alignment (use it along horizontalOffset property)
   - [Fix] Fixed >65000 vertices mesh error in viewport mode when using infinite scrolling mode and at min zoom level
   - [Fix] Fixed FlyTo taking longest path when world wrapping is enabled

Version 7.1 8-OCT-2018
   - API: ImportProvincesColorMap() method to load provinces and generate new countries at runtime
   - Map Editor: new contextual option to export provinces color map
   - Map Editor: ability to select multiple provinces holding Control key
   - Map Editor: new option to merge multiple selected provinces with a single click
   - Map Editor: new option to create a new country from multiple selected provinces

Version 7.0 20-SEP-2018
   - Support for online/offline maps (tile system)
   - New tile downloader assistant
   - New tile-based demo scenes: 006 (2d) and 514 (viewport)
   - Improved performace of region triangulation
   - Improved performace of region data load
   - Improved performance of Territory Importer
   - Added "Max (Highlight) Screen Size" parameter to ignore highlighting when country/province occupies most of screen
   - [Fix] Fixed issue with OnDragStart event
   - [Fix] Fixed compatibility with Text Mesh Pro on Unity 2018.2
   - [Fix] Fixed map flickering in viewport mode when dragging near edge of map with constant drag speed and world wrapping enabled

Version 6.5 22-AUG-2018
   - Added new province Telangana to India
   - Map Editor: create new countries/provinces by drawing a split line over existing region. Select Create/Country or Province, click on a border (or Shift+S to snap), draw line and end with Shift+Z.
   - Map Editor: fixes
   - API: added CountryRemoveCell and ProvinceRemoveCell
   - API change: ProvinceDelete now only removes specified province. To remove all provinces from a country use CountryDeleteProvinces
   - Added Key Mapping options to customize displacement keys (previously WASD)
   - Decorators: explicit option in decorator inspector to specify if a different label must be used
   - [Fix] Workaround for Unity 2017.4.5+ regression bug related to shader field data type change

Version 6.4 8-JUL-2018
   - Added FIPS 10-4, ISO A2, ISO A3 and ISO N3 standard country codes
   - API: added GetCountryIndexByFIPS10_4, GetCountryIndexByISO_A2, GetCountryIndexByISO_A3 and GetCountryIndexByISO_N3
   - Repect Other UI option now works on mobile devices
   - Demo scene 1: added example of gradient fill with transparency (button on bottom/left)
   - [Fix] WMSK now emits a warning if no suitable camera is found
   - [Fix] Fixed LineMarkerAnimator skipping some line vertices
   - [Fix] Fixed bug with constant drag speed 
   - [Fix] Fixed bumpMap texture material reference
   - [Fix] Moved Custom HeightMap Texture field to Miscellanea to avoid confusion with bump mapping
   - [Fix] Fixed issue with decorators not being saved with the scene

Version 6.3 22-JUN-2018
   - Ability to add outline to a group of regions (demo Custom Borders inside General Examples updated)
   - API: new method RegionMerge
   - [Fix] Fixed zoom behaviour when enableFreeCamera is enabled or allowUserZoom is disabled
   - [Fix] Fixed encoding issue with city names

Version 6.2.3 4-JUN-2018
   - Memory optimizations for viewport mode
   - API: added visible property to GameObjectAnimator component
   - [Fix] Fixed line markers being disabled when parent unit exits map in viewport mode
   - [Fix] Fixed OnCountry/OnProvince events not firing when WASD keys are used
   - [Fix] Fixed issue with highlighting countries with province enclaves
   - [Fix] Fixed exception when assigning custom TextMesh Pro fonts
   - [Fix] Fixed mount points coordinate XML issue

Version 6.2 24-MAY-2018
   - New demo scene 05 Country Extrusion. Example: https://imgur.com/nCJHHQh
   - Improved zoom, panning and FlyTo methods when Free Camera Mode is enabled and camera is tilted
   - Added labels elevation for 2D Map mode
   - API: Added Block RayCast property to GameObjectAnimator
   - API: Added RegionGenerateExtrudedGameObject
   - API: Added RegionSetCustomElevation, RegionRemoveCustomElevation, RegionRemoveAllCustomElevations. New demo scene 13 in Viewport examples.
   - API: Added OnRegionEnter / OnRegionExit events
   - [Fix] Fixed Aland capital

Version 6.1 11-MAY-2018
   - Hex Grid: added Alpha On Water setting
   - Map Editor: added support for background/pool country
   - Map Editor: added shortcut Shift+B to snap to map border while drawing a region path
   - API: New GetCities overloads add additional filters
   - Usability: terrain mode now detected if terrain data is missing. Also allows setting a default world map heightmap.
   - [Fix] Fixed Screen Edge Scroll cancelling WASD movement
   - [Fix] Fixed WindowRect issue on Unity 2018.1
   - [Fix] Fixed UICanvasMiniMap prefab issue
   - [Fix] Workaround for rectTransform position bug in TMPro/Unity 2017.3+
   - [Fix] Fixed out of range exception when grid is computed and world wrapping is enabled
   - [Fix] Fixed drag issue with constant drag speed on
   - [Fix] Fixed constant drag speed issue on mobile devices


Version 6.0 24-APR-2018
   - New polygon clipping library (faster and more robust region transfer operations)
   - Road example added to demo scene 05 in Path Finding Examples folder (button "Build Road")
   - Hexagonal grid: improved generation performance + x4 maximum number of columns and rows
   - API: RegionErase can now erase multiple regions faster in one call (also invert mode)
   - Path Finding: calls to OnPathFindingCrossCell are now cached for the entire FindRoute call to improve performance of A* algorithm
   - Cosmetic changes to WMSK Inspector (also makes it more lightweight)
   - New memory manager improves memory usage when replaying scenes with lot of meshes / textures
   - Added sample code to show only the borders of a single country (demo scene 01 in 2D Map Examples folder)
   - Improved performance of FindRoute method
   - [Fix] Fixed brief visual glitch when unit crosses map borders following a route
   - [Fix] Fixed water mask texture so ships can cross Gibraltar's straight
   - [Fix] Fixed OnPathFindingCrossCell not being called when calling FindRoute between two cell objects

Version 5.6 2-APR-2018
   - New demo scene 08 / General Examples. Country removal at runtime.
   - API: added RegionErase method
   - API: added OnRegionClick event
   - API: added GetCountryRegionIndex, GetProvinceRegionIndex
   - API: added GetCountryZoomExtents / GetProvinceZoomExtents similar to GetCountryRegionExtents but applies to all regions
   - API: added centerRect to country/province objects to obtain the center of the region that encloses all entity regions
   - [Fix] Fixed viewport texture issue with SceneView and GameView windows active/visible on screen
   - [Fix] Several fixes & small improvements in demo scenes

Version 5.5 22-MAR-2018
   - Ability to add custom/animated textured borders to any country. New demo scene 07 in General Examples folder. Video: https://youtu.be/jBhr2u1fB5s
   - Smoother province borders when zoomed in
   - Dragging is now allowed over UI elements
   - API: new GameObjectAnimator events: OnMove / OnKilled
   - Updated demo scene Viewport / 03 MapPopulation: added following circle to moving ship
   - Reduced memory allocations when interacting with map
   - Performance optimizations
   - [Fix] Fixed null exception error in DemoPathFindingByCells when not using viewport

Version 5.4 5-MAR-2018
   - Exposed Default Max Steps parameter in WMSK inspector (Path Finding Settings)
   - Easy harbouring: clicking on a land position will make ship reach the nearest coastal point
   - API: GameObjectAnimator new field maxSearchSteps
   - Inspector: reworked Set SceneView Rect button. Now that button takes into account the SceneView and not the GameView.
   - [Fix] Fixed fliped image when world wrap mode is enabled on Unity 2017.2+
   - [Fix] Map Editor: fixed regression issue with delete continent/country command

Version 5.3 20-FEB-2018
   - New demo scene: General Examples/06 Provinces Pool. Example of CreateCountryProvincesPool function.
   - New option to control province label visibility under Show Province Names in inspector.
   - Map Editor: additive option -> imports new countries and provinces preserving existing entities
   - Map Editor: some accuracy improvements to territory importer and Magnet tool
   - API: new CreateCountryProvincesPool.
   - API: new DrawProvincesLabel / HideProvicesLabel functions.
   - API: new GetCities (country) / GetCities (province) functions.
   - [Fix] Fixed regression bug with autofade labels in non-text mesh pro mode

Version 5.2 30-JAN-2018
   - New optional integration with TextMesh Pro for drawing higher quality country labels (check manual for details on how to enable it)
   - Text Mesh Pro labels: added outline color and width options
   - New bump mapping options for natural and highres styles
   - Option to create smooth blink effects (BlinkCountry/BlinkProvince functions now accepts a smooth parameter)
   - New option "Use Time of Day": enables lighting changes with an hourly slider (0-24h)
   - API: Added GetMarkers: returns all added markers, can be filtered by country, province, region or cell
   - Map Editor: improvements to territory importer (provinces):
   	  - New option: detail level (fine/coarse)
   	  - New option: snap to country frontiers (useful when importing new provinces into existing country map)

Version 5.1.1 19-DEC-2017
  - [Fix] Fixed text encoding when saving changes from map editor

Version 5.1 13-DEC-2017
  - Map Editor: added new option to convert a province into a new country
  - API: added GetProvinceRegionSurfacesGameObject which returns the surface game object for any colored/textured province region

Version 5.0.1 1-DEC-2017
  - [Fix] Fixed flipped map texture projected onto terrain when MSAA is enabled
  - [Fix] Fixed stuck camera issue out of range in terrain mode

Version 5.0 27-NOV-2017
  - Map Editor: new terrain importer tool (located in the dropdown menu of the gear icon)
  - Country labels fade in/out effect: exposed internal customization options in Inspector
  - New Earth Style: Natural 16K
  - Unity native terrain support: first alpha -> WMSK projects into Unity terrain using special terrain shader
  - New terrain demo scenes under Demos/Terrain folder

Version 4.6 15-NOV-2017
  - New demo scene 31: PathFinding using hexagonal grid and altitude
  - New API: GetCellsByAltitude
  - Map Editor: improvements to Territory Importer
  - [Fix] Fixed class name conflict of script "GameLogic.cs" in Demo Scene 27

Version 4.5 7-NOV-2017
  - Added Resolution option under Viewport settings. Allows you to improve performance reducing render texture resolution.
  - Usability: adding a new Viewport to the scene will now automatically attach to the existing WMSK
  - [Fix] Fixed bug in CitiesDeleteFromContinent function
  - [Fix] ProvinceNeighbours now forces computing of neighbours if provinces are not visible
  - [Fix] Fixed a rare condition where province look up dictionary can't find existing provinces when reloading data
  - [Fix] Fixed pinch-to-zoom issues on mobile
  - [Fix] Fixed grid Exclusive option issue when country and province highlighted are not enabled

Version 4.4 6-OCT-2017
  - New demo scenes 29 & 30: use of UI Panels to show info about map cities
  - API: Simplified MoveTo() methods to 3 consistent overloaded functions plus new parameter to specify the DURATION_TYPE
  - API: Added GetCityCountryName, GetCityProvinceName functions
  - [Fix] Added Misc.InvariantCulture to all float parsing instructions to support foreign languages

Version 4.3 5-SEP-2017
  - New demo scene 28: missile war (N. Korea vs Japan)
  - Support for province labels. New inspector options.
  - New optional parameter for AddMarker2DSprite to allow them expose mouse events (see code behind "Add Marker" button in demo scene 1)
  - [Fix] Map Editor: province borders disappear when moving the mouse around the map
  - [Fix] Map Editor: selected target province changes after merging province

Version 4.2 16-AUG-2017
  - New demo scene 26: populating the UI with country and province data
  - New demo scene 27: adding custom attributes to countries with UI, saving & loading data
  - Added customCamera field for choosing a different camera instead of default "Main" camera
  - Change: OnProvinceClick is now triggered even if Province Highlight is set to false
  - Added Drag Threshold option to prevent accidental drag on high Dpi touch screens
  - [Fix] Fixed JSONObject class import issue with Unity 2017.1
  - [Fix] Map Editor: fixed region snapping helper shortcut issue
  
Version 4.1 31-JUL-2017
  - Decorators: option to apply texture to all regions
  - Updated scene 4 (MapPopulation) with an airplane unit demo
  - Some fixes and improvements related to NGUI integration
  - Hexagonal grid: added "Exclusive" option to enable/disable multiple highlights (country/province + cell highlight)
  - Render Viewport: added filtering mode (Bilinear / Trilinear)
  - API: Added OnDragStart / OnDragEnd events
  - API: New GetCountryRegionZoomExtents / GetProvinceRegionZoomExtents function which returns the zoom level required to show a country/province within screen borders
  - API: Added FogOfWarSetCell(s)
  - Fog of War: new elevation parameter
  - [Fix] Map Editor: fixed attributes reordering issue when updating values

Version 4.0 03-JUL-2017
  - New demo scene 24: create polygonal maps procedurally from scratch (no geodata loading)
  - New demo scene 25: create hexagonal maps procedurally from scratch
  - New Earth style: Texture
  - New APIs: CountryTransferCell, ProvinceTransferCell, earthMaterial, earthTexture, ClearAll, dontLoadGeodataAtStart, ToggleCountryOutline, frontiersDynamicWidth
  - Performance optimizations to viewport mode
  - Map Editor: improved region split operations
  - NGUI support: map interaction can be disabled when pointer hovers a GUI element
  - [Fix] Fixed HideCountrySurface/ToggleCountrySurface coloring issue

Version 3.7 27-JUN-2017
  - Country outlines: new option for detailed outline including texture and custom width.
  - Map Editor: new Regions section under selected Country / Province
  - Map Editor: new option under Provinces section to allow Simplify/Reduce provinces for the selected country
  - Map Editor: new option to convert a region into a new country (located in the new Regions section)
  - Map Editor: reduced memory usage of Province Equalizer tool
  - Map Editor: optimized performance and implementation of Country Equalizer tool
  - General optimizations

Version 3.6 12-JUN-2017
  - New "Thicker borders" option (under Country section)
  - Update demo scene 22 to show path finding costs on map
  - Map Editor: can be used at runtime inside Unity Editor
  - Map Editor: improved Territory Importer tool
  - API: Added GetCountryFrontierPoints / GetProvinceBorderPoints
  - [Fix] Just selecting and showing WMSK inspector marks the scene with unsaved changes even if there's no changes to WMSK

Version 3.5 2-JUN-2017
  - Revamped hexagonal grid based pathfinding methods and API. Manual updated.
  - New Cell effects: CellFadeOut, CellFlash, CellBlink.
  - Map Editor: new Equalize Countries option under gear icon
  - New demo scene 23: Load/Save demo
  - New Earth style with 16K texture
  - API: Added AddLine(cell1, cell2)
  - API: Added two new events for units (OnCountryEnter, OnProvinceEnter). Also available as global events.
  - [Fix] CountryRename now affects any decorator properly
  - [Fix] Added missing Belagrade to Serbia
  - [Fix] Fixed error when upgrading a province to a new country and province region data has been previously loaded
    
Version 3.0  11-MAY-2017
  - New demo scenes 20, 21 and 22 with pathfinding per country / province / cells examples
  - API: Added FindRoute(country1, country2), FindRoute(province1, province2) and FindRoute(cell1, cell2)
  - [Fix] Map Editor: fixed bugs in territory importer tool
  - [Fix] Fixed path finding missing some cells under heavy usage
  
Version 2.9  10-APR-2017
  - Improved performance of grid cells colorization
  - Updated demo scene 11: Pathfinding Advanced with options to show custom matrix over the map and example of blocking frontier pass
  - Added "Show Matrix Cost" option in inspector to debug pathfinding custom matrix cost
  - API: Added extra width parameter to GetCountryFrontierPoints
  - API: Added AddMarker2DText to add custom text on any map position
  - API: Added duration optional parameter to SetZoomLevel method
  - API: Changed WMSK_MoveTo syntax to make it easier to extend/use
  - [Fix] Removed console warnings on Unity 5.6
  
Version 2.8  14-MAR-2017
  - New UICanvasMiniMap prefab for easy hanlding of minimap inside a Canvas UI. New demo scene 7b.
  - Interaction is now halted when cursor is on minimap
  - [Fix] Fixed minimap wrong scale on certain scenarios
  - [Fix] Fixed Territory Importer tool errors when importing province map
  - [Fix] Fixed demo scenes namespace conflict with PUN+ 

Version 2.7 06-FEB-2017   
  - Added two new heuristics to PathFinding engine (DiagonalShortCut and Euclidean).
  - Improved Ininite Scroll demo scene: added trails
  - API: Added PathFindingCustomRouteMatrixSet(cell)
  - [Fix] Fixed path finding custom costs calculation bug
  - [Fix] Fixed coordinate error in ContainsWater API
  - [Fix] Fixed null error when using Cell's attrib property

Version 2.6.1 31-JAN-2017   
  - PathFinding advanced demo scene: added grid mode button
  - [Fix] User defined path finding custom cost matrix was being reseted internally between calls

Version 2.6 - 30-JAN-2017
  - Map Editor: new provinces equalizer tool to balance number of provinces among countries
  - API: Added countryRegionHighlightedShape and provinceRegionHighlightedShape
  - [Fix] Fixed incorrect import settings for elevation map textures
  - [Fix] Fixed compatibility issues with Unity 5.5
  - [Fix] Map Editor: fixed province issue when renaming country
  - [Fix] Map Editor: fixed context menu options not showing up in Unity 5.4.1

Version 2.5 - 29-NOV-2016
  - Improved support for enclaves at province level
  - Improved province transfer functionality
  - API: added ProvinceToCountry API to generate a new country from a province.
  - Added shadow support to lines drawn on 2D mode and normal Earth styles
  - Scene 14: added example of country transfer
  - Scene 17: added example or provinces transfer
  - AllowKeys option is now independent of AllowUserDrag
  - [Fix] Fixed an error saving provinces from the map editor

Version 2.4 - 10-NOV-2016
  - New Territories Importer tool (available under Map Editor gear icon)
  - Map Editor: added hotkeys to shape creation tool (Shift+C Close Polygon, Shift+X Remove last point, Shift+S Snap to nearest vertex, Shift+F: Fast Contour, Escape clear shape)
  
Version 2.3 - 13-OCT-2016
  - Ability to merge provinces at runtime. New demo scene 17. New API: ProvinceTransferProvinceRegion
  - Bullet support for viewport gameobjects. New demo scene 18. New API: WMSK_Fire
  - Rectangle selection support for viewport gameobjects. New demo scene 19. New API: RectangleSelectionInitiate.
  - New options in inspector to customize water level and foam effect in Scenic Plus style.
  - Support for country and province enclaves (new option in inspector / miscellanea)
  - API: added two new events: OnCountryHighlight/OnProvinceHighlight which allows cancelling highlighting and further control on highlighting
  - API: added GetVisibleCountries/GetVisibleProvinces methods
  - API: added GetVisibleCountriesInWindowRect/GetVisibleProvincesInWindowRect methods
  - API: added GetCountryFrontierPoints/GetProvinceBorderPoints to get common frontier/border points between 2 countries or provinces
  - API: added isNear to GameObjectAnimator component
  - API: added Stop to GameObjectAnimator component
  - Map Editor: added Hide Off-Screen Countries to editor component gear menu
  - Map Editor: added Delete Off-Screen Countries to editor component gear menu
  - Map Editor: added option to merge provinces of same country
  - Country/province highlighting small performance improvement 
  - Project compatibility with World Political Map Globe Edition
  - [Fix] Fixed error when saving country or provinces attributes from Map Editor.
  - [Fix] Viewport camera is now disabled as well when WMSK object is disabled so it does not waste ms
  - [Fix] Fixed error when transferring a province to another country
  - [Fix] Fixed movement issue of units ending some times over the coastal line hence GetCountryIndex(unit pos) returned -1
  - [Fix] Fixed WMSK_Fire API bullet starting position and arc trajectory. Changed duration parameter to speed. Added 'test' parameter to make easier to adjust the startAnchor parameter.
  - [Fix] Fixed viewport gameobjects orientation not changing when switching between view modes (viewport <-> normal 2D)
  - [Fix] Fixed cities null province reference in geodata files
  
 Version 2.2 - 01-SEP-2016
  - New APIs/events for selecting units. New demo scene 16. Manual updated.
  - [Fix] Fixed RespectOtherUI behaviour on mobile.

Version 2.1 - 22-AUG-2016
  - Map no longer changes position when dragging/zooming (now it's the camera what moves)
  - Ability to switch from viewport to normal mode and viceversa. New demo scene 15.
  - New buoyancy effects for naval units. Set new properties in Viewport section and at unit level.
  - Added AddMarkerSprite & AddMarker3DObject for easier position of sprites and also 3D game objects over the standalone map
  - API: new GetCurrentMapCoordinates returns coordinates of the currently visible center of the map
  - API: new isOnWater property of GameObjectAnimators returns whether the unit is currently on water
  - [Fix] Enclaves San Marino and Lesotho no longer get filled when selecting Italy or South Africa
  - [Fix] Fixed text not appearing in tickers with viewport and overlay mode enabled

Version 2.0 - 19-MAY-2016
  - Country expansion/grow at runtime

Version 1.4 - 13-APR-2016
  - Horizontal wrap/infinite scrolling mode
  - Map Editor: added cursor position and snap option below reshape menu

Version 1.3 - 14-MAR-2016
  - New demo scene 12 "Provinces" to showcase future province-related functionality
  - Significant performance improvement of highlighting system
  - Ability to constraint window rect. New inspector section.
  - Can customize path finding route matrix costs. New Demo Scene #11. Manual updated. New APIs: PathFindingCustomRouteMatrixReset & PathFindingCustomRouteMatrixSet
  - Can modify city icons from inspector.
  - API: Added events to GameObjectAnimator: OnMoveStart and OnMoveEnd
  - API: Added GetCountryCoastalPoints / GetProvinceCoastalPoints.
  - API: Added PathFindingGetCountriesInPath, PathFindingGetProvincesInPath, PathFindingGetCitiesInPath & PathFindingGetMountPointsInPath
  - Added geodataResourcesPath property to support different locations for datafiles
  - Added pivotY property to GameObjectAnimator to improve positioning of GameObjects
  - [Fix] Fixed colored countries being cleared when ToggleCountrySurface was called during startup
  - [Fix] Fixed bug with HideProvinceSurface when the province was highlighted

Version 1.2 - 2-MAR-2016
  - Custom Attributes based on JSON. New APIs, inspector section & editor interface. Manual updated.
  - Hexagonal grid. New APIs, inspector section. Manual updated. 2 new demo scenes added.
  - Optimized path finding performance.
  - Added new menu GameObject / Create Other / WMSK Viewport for fast creation of viewport and map in one step.

Version 1.1 - 12-FEB-2016
  - New day/night cycles. New demo scene.
  - Can add circles and rings to the map
  - MiniMap. New component & API (WMSKMiniMap). New demo scene. Manual updated.
  - New grouping option for viewport game objects (group property). Ability to toggle group visibility.
  - New preserveOriginalRotation for viewport game objects.
  - Ability to set/clear fog on entire countries or regions (API: FogOfWarSetCountry, FogOfWarSetCountryRegion, FogOfWarSetProvince, FogOfWarSetProvinceRegion)
  - Added Maldives and Male (capital) to geodata files
  - Added RespectOtherUI and showCursorAlways
  - [Fix] Scroll On Screen Edges fail with viewport. Fixed.
  - [Fix] Colorizing countries was not using alpha component of the color. Fixed.
  - [Fix] Fixed circles aspect ratio.

Version 1.0 - 1-FEB-2016
New features (with respect to World Political Map 2D Edition):
  - Mount Points. Allows you to define custom landmarks for special purposes. Mount Points attributes includes a name, position, a type and a collection of tags. Manual updated.
  - New Scenic Plus style (animated water, coast foam effect and blurred/softened geography for very close zooms). Still WIP (clouds).
  - New Scenic Plus Alternate 1 (variant from previous one)
  - 3D surface for viewport with adjustable height.
  - New cloud layer for viewport with ajustable speed, elevation, alpha and ground shadows. New APIs (see manual: Earth section).
  - Fog of War layer for viewport. Can set custom transparency on given coordinates. New APIs (see manual: Fog Of War section).
  - New API for units handling over viewport through extensions for GameObject (WMSK prefix). Demo scene included.
  - New Path Finding support based on A-Star algorithm. Added terrainCapability, minAltitude and maxAltitude properties to GameObjectAnimator script.
  - New APIs for detecting water (ContainsWater)
  - New APIs for adding animated grounded and aerial paths. Check demo scene PathAndLines.
  - New line drawing system supporting animated dashed lines.
  - New options for country decorator and for country object (now can click-select/hide/rotate/offset/font size any country label)
  - Editor: Mount Point Mass Creation tool
  - Can use alpha component when colorizing countries or provinces (transparent coloring)
  - 2 new free fonts added: Volkron and Actor.
  - Map hovering performance improvement.
  - Tickers: new overlay mode option for displaying ticker texts
  - New prewarm option to compute heavy tasks at initialization time and make it smoother during play
  - Now province highlighting can be enabled irrespective of country highlighting state
  

Credits
-------

- All code, data files and images, otherwise specified, is (C) Copyright 2016-18 Kronnect
- JSON parser: derived by original code by Matt Schoen, distributed under MIT license
- PathFinding algorithm: derived from original code by Franco, Gustavo, distributed under Public Domain
- Non high-res Earth textures derived from NASA source (Visible Earth)
- Flag images: Licensed under Public Domain via Wikipedia
- Demo models: Unity (tank) and public domain (tower)




