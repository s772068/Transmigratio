using UnityEngine;
using UnityEditor;

namespace WorldMapStrategyKit {

    public partial class WMSKEditorInspector {

        // Add a menu item called "Rebuild Countries Frontiers from Provinces".
        [MenuItem("CONTEXT/WMSK_Editor/Create Countries Frontiers From Provinces", false, 131)]
        static void RebuildCountriesFrontiersFromProvinces(MenuCommand command) {

            WMSK_Editor editor = (WMSK_Editor)command.context;
            if (editor.editingMode != EDITING_MODE.PROVINCES) {
                EditorUtility.DisplayDialog("Rebuild Countries Frontiers from Provinces", "This command is only available when in Countries + Provinces editing mode. Change the mode and try again.", "Ok");
                return;
            }
            if (editor.map.frontiersDetail != FRONTIERS_DETAIL.High) {
                EditorUtility.DisplayDialog("Rebuild Countries Frontiers from Provinces", "This command requires country frontiers detail set to High. Change the detail level and try again.", "Ok");
                return;
            }
            if (!EditorUtility.DisplayDialog("Rebuild Countries Frontiers from Provinces", "The countries geodata file will be replaced. Each new country will be the union of all provinces of that country.\nThis operation ensures that country frontiers and province borders are aligned. Continue (a backup will be created)?", "Proceed", "Cancel")) {
                return;
            }

            editor.ClearSelection();
            EditorCoroutines.Start(editor.GetCountryGeoDataFromProvinces(rebuildProgress, rebuildFinished));
        }

        static bool rebuildProgress(float progress, string title, string text) {
            if (progress < 1.0f) {
                return EditorUtility.DisplayCancelableProgressBar("Operation in progress", title + (text.Length > 0 ? " (" + text + ")" : ""), progress);
            } else {
                EditorUtility.ClearProgressBar();
            }
            return false;
        }

        static void rebuildFinished(bool cancelled) {
            EditorUtility.ClearProgressBar();
            if (cancelled) {
                EditorUtility.DisplayDialog("Operation Cancelled", "Some frontiers may have changed, others not. Use 'Revert' button to reload frontiers.", "Ok");
            } else {
                EditorUtility.DisplayDialog("Operation Complete", "Country frontiers now match provinces borders. Use 'Save' button to make changes permanent.", "Ok");
            }

        }


    }
}
