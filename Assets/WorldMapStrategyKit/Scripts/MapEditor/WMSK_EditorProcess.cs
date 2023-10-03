using UnityEngine;

namespace WorldMapStrategyKit {


	public delegate bool EditorOperationProgress(float percentage, string title, string text);
	public delegate void EditorOperationFinish(bool cancelled);

	public struct EditorOpContext {
		public string title;
		public EditorOperationProgress progress;
		public EditorOperationFinish finish;
	}

	public partial class WMSK_Editor : MonoBehaviour {
		bool cancelled;
	}


}
