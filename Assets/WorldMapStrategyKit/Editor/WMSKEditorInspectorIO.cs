using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {

    public partial class WMSKEditorInspector {

        private static void CopyGeodataFiles(string sourceDirName, string destDirName, bool copySubDirs) {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists) {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles("*.txt");
            foreach (FileInfo file in files) {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs) {
                foreach (DirectoryInfo subdir in dirs) {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    CopyGeodataFiles(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        static void CheckBackup(out string geoDataFolder) {
            string[] paths = AssetDatabase.GetAllAssetPaths();
            bool backupFolderExists = false;
            string rootFolder = "";
            geoDataFolder = "";
            for (int k = 0; k < paths.Length; k++) {
                if (paths[k].EndsWith(WMSK.instance.geodataResourcesPath)) {
                    geoDataFolder = paths[k];
                } else if (paths[k].EndsWith("WorldMapStrategyKit")) {
                    rootFolder = paths[k];
                } else if (paths[k].EndsWith("WorldMapStrategyKit/Backup")) {
                    backupFolderExists = true;
                }
            }

            if (backupFolderExists) {
                // Back-up the back-up
                string backupFolderName = "Backup Old " + DateTime.Now.ToString("yyyy'-'MM'-'dd'-'HH'-'mm'-'ss");
                CopyGeodataFiles(rootFolder + "/Backup", rootFolder + "/" + backupFolderName, false);
            } else {
                AssetDatabase.CreateFolder(rootFolder, "Backup");
            }

            // Perform the backup
            string fullFileName;
            string backupFolder = rootFolder + "/Backup";
            fullFileName = geoDataFolder + "/countries110.txt";
            if (File.Exists(fullFileName)) {
                AssetDatabase.CopyAsset(fullFileName, backupFolder + "/countries110.txt");
            }
            fullFileName = geoDataFolder + "/countries10.txt";
            if (File.Exists(fullFileName)) {
                AssetDatabase.CopyAsset(fullFileName, backupFolder + "/countries10.txt");
            }
            fullFileName = geoDataFolder + "/" + WMSK.instance.countryAttributeFile;
            if (File.Exists(fullFileName)) {
                AssetDatabase.CopyAsset(fullFileName, backupFolder + "/" + WMSK.instance.countryAttributeFile + ".json");
            }
            fullFileName = geoDataFolder + "/provinces10.txt";
            if (File.Exists(fullFileName)) {
                AssetDatabase.CopyAsset(fullFileName, backupFolder + "/provinces10.txt");
            }
            fullFileName = geoDataFolder + "/" + WMSK.instance.provinceAttributeFile;
            if (File.Exists(fullFileName)) {
                AssetDatabase.CopyAsset(fullFileName, backupFolder + "/" + WMSK.instance.provinceAttributeFile + ".json");
            }
            fullFileName = geoDataFolder + "/cities10.txt";
            if (File.Exists(fullFileName)) {
                AssetDatabase.CopyAsset(fullFileName, backupFolder + "/cities10.txt");
            }
            fullFileName = geoDataFolder + "/" + WMSK.instance.cityAttributeFile;
            if (File.Exists(fullFileName)) {
                AssetDatabase.CopyAsset(fullFileName, backupFolder + "/" + WMSK.instance.cityAttributeFile + ".json");
            }
            fullFileName = geoDataFolder + "/mountPoints.json";
            if (File.Exists(fullFileName)) {
                AssetDatabase.CopyAsset(fullFileName, backupFolder + "/mountPoints.json");
            }
        }


        string GetAssetsFolder() {
            string fullPathName = Application.dataPath;
            int pos = fullPathName.LastIndexOf("/Assets");
            if (pos > 0)
                fullPathName = fullPathName.Substring(0, pos + 1);
            return fullPathName;
        }

        bool SaveMapChanges() {

            if (Application.isPlaying) {    // preserve changes done at runtime not detected by Editor
                _editor.countryChanges = true;
                _editor.countryAttribChanges = true;
                _editor.provinceChanges = true;
                _editor.provinceAttribChanges = true;
                _editor.cityChanges = true;
                _editor.cityAttribChanges = true;
                _editor.mountPointChanges = true;
            }

            if (!_editor.countryChanges && !_editor.provinceChanges && !_editor.cityChanges && !_editor.mountPointChanges &&
                !_editor.countryAttribChanges && !_editor.provinceAttribChanges && !_editor.cityAttribChanges)
                return false;

            // First we make a backup if it doesn't exist
            string geoDataFolder;
            CheckBackup(out geoDataFolder);

            string dataFileName, fullPathName;
            // Save changes to countries
            if (_editor.countryChanges) {
                dataFileName = _editor.GetCountryGeoDataFileName();
                fullPathName = GetAssetsFolder() + geoDataFolder + "/" + dataFileName;
                string data = _map.GetCountryGeoData();
                File.WriteAllText(fullPathName, data, System.Text.Encoding.UTF8);
                _editor.countryChanges = false;
            }
            // Save changes to country attributes
            if (_editor.countryAttribChanges) {
                fullPathName = GetAssetsFolder() + geoDataFolder + "/" + _map.countryAttributeFile + ".json";
                string data = _map.GetCountriesAttributes(true);
                File.WriteAllText(fullPathName, data, System.Text.Encoding.UTF8);
                _editor.countryAttribChanges = false;
            }
            // Save changes to provinces
            if (_editor.provinceChanges) {
                dataFileName = _editor.GetProvinceGeoDataFileName();
                fullPathName = GetAssetsFolder();
                string fullAssetPathName = fullPathName + geoDataFolder + "/" + dataFileName;
                string data = _map.GetProvinceGeoData();
                File.WriteAllText(fullAssetPathName, data, System.Text.Encoding.UTF8);
                _editor.provinceChanges = false;
            }
            // Save changes to province attributes
            if (_editor.provinceAttribChanges) {
                fullPathName = GetAssetsFolder() + geoDataFolder + "/" + _map.provinceAttributeFile + ".json";
                string data = _map.GetProvincesAttributes(true);
                File.WriteAllText(fullPathName, data, System.Text.Encoding.UTF8);
                _editor.provinceAttribChanges = false;
            }
            // Save changes to cities
            if (_editor.cityChanges) {
                _editor.FixOrphanCities();
                dataFileName = _editor.GetCityGeoDataFileName();
                fullPathName = GetAssetsFolder() + geoDataFolder + "/" + dataFileName;
                File.WriteAllText(fullPathName, _map.GetCityGeoData(), System.Text.Encoding.UTF8);
                _editor.cityChanges = false;
            }
            // Save changes to cities attributes
            if (_editor.cityAttribChanges) {
                fullPathName = GetAssetsFolder() + geoDataFolder + "/" + _map.cityAttributeFile + ".json";
                string data = _map.GetCitiesAttributes(true);
                File.WriteAllText(fullPathName, data, System.Text.Encoding.UTF8);
                _editor.cityAttribChanges = false;
            }
            // Save changes to mount points
            if (_editor.mountPointChanges) {
                dataFileName = _editor.GetMountPointGeoDataFileName();
                fullPathName = GetAssetsFolder() + geoDataFolder + "/" + dataFileName;
                File.WriteAllText(fullPathName, _map.GetMountPointsGeoData(), System.Text.Encoding.UTF8);
                _editor.mountPointChanges = false;
            }
            AssetDatabase.Refresh();
            return true;
        }

        static void ExportProvincesMap(string outputFile) {

            WMSK map = WMSK.instance;
            if (map == null)
                return;

            // Get all triangles and its colors
            const int width = 8192;
            const int height = 4096;
            Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            Color[] colors = new Color[width * height];

            int provincesCount = map.provinces.Length;
            HashSet<Color> provinceColors = new HashSet<Color>();

            for (int prov = 0; prov < provincesCount; prov++) {
                Color color;
                do {
                    float g = UnityEngine.Random.Range(0.1f, 1f); // avoids full black (used by background)
                    color = new Color(UnityEngine.Random.value, g, UnityEngine.Random.value);
                } while (provinceColors.Contains(color));
                provinceColors.Add(color);
                Province province = map.provinces[prov];
                map.EnsureProvinceDataIsLoaded(province);
                int regionsCount = province.regions.Count;
                for (int pr = 0; pr < regionsCount; pr++) {
                    GameObject surf = map.ToggleProvinceRegionSurface(prov, pr, true, color);
                    // Get triangles and paint over the texture
                    MeshFilter mf = surf.GetComponent<MeshFilter>();
                    if (mf == null || mf.sharedMesh.GetTopology(0) != MeshTopology.Triangles)
                        continue;
                    Vector3[] vertex = mf.sharedMesh.vertices;
                    int[] index = mf.sharedMesh.GetTriangles(0);

                    for (int i = 0; i < index.Length; i += 3) {
                        Vector2 p1 = Conversion.ConvertToTextureCoordinates(vertex[index[i]], width, height);
                        Vector2 p2 = Conversion.ConvertToTextureCoordinates(vertex[index[i + 1]], width, height);
                        Vector2 p3 = Conversion.ConvertToTextureCoordinates(vertex[index[i + 2]], width, height);
                        // Sort points
                        if (p2.x > p3.x) {
                            Vector3 p = p2;
                            p2 = p3;
                            p3 = p;
                        }
                        if (p1.x > p2.x) {
                            Vector3 p = p1;
                            p1 = p2;
                            p2 = p;
                            if (p2.x > p3.x) {
                                p = p2;
                                p2 = p3;
                                p3 = p;
                            }
                        }

                        Drawing.DrawTriangle(colors, width, height, p1, p2, p3, color, true);
                    }
                }
            }
            texture.SetPixels(colors);
            texture.Apply();

            if (File.Exists(outputFile)) {
                File.Delete(outputFile);
            }
            File.WriteAllBytes(outputFile, texture.EncodeToPNG());
            AssetDatabase.Refresh();

            map.HideProvinceSurfaces();
            TextureImporter imp = (TextureImporter)AssetImporter.GetAtPath(outputFile);
            if (imp != null) {
                imp.maxTextureSize = 8192;
                imp.SaveAndReimport();
            }
        }
    }
}
