using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace WorldMapStrategyKit {

    public partial class CustomWWW
       : CustomYieldInstruction
       , IDisposable {

        public CustomWWW(string url) {
            _uwr = UnityWebRequest.Get(url);
            _uwr.SendWebRequest();
        }


        bool IsError() {
#if UNITY_2020_3_OR_NEWER
            return _uwr.result == UnityWebRequest.Result.ConnectionError;
#else
            return _uwr.isNetworkError;
#endif
        }

        public byte[] bytes {
            get {
                if (!WaitUntilDoneIfPossible())
                    return new byte[] { };
				if (IsError())
                    return new byte[] { };
                var dh = _uwr.downloadHandler;
                if (dh == null)
                    return new byte[] { };
                return dh.data;
            }
        }

        public int bytesDownloaded {
            get { return (int)_uwr.downloadedBytes; }
        }

        public string error {
            get {
                if (!_uwr.isDone)
                    return null;
				if (IsError())
                    return _uwr.error;
                if (_uwr.responseCode >= 400) {
                    return string.Format("Error {0} {1}", _uwr.responseCode, _uwr.error);
                }
                return null;
            }
        }

        public bool isDone { get { return _uwr.isDone; } }

        public string text {
            get {
                if (!WaitUntilDoneIfPossible())
                    return "";
				if (IsError())
                    return "";
                var dh = _uwr.downloadHandler;
                if (dh == null)
                    return "";
                return dh.text;
            }
        }

        private Texture2D CreateTextureFromDownloadedData(bool markNonReadable) {
            if (!WaitUntilDoneIfPossible())
                return new Texture2D(2, 2);
			if (IsError())
                return null;
            var dh = _uwr.downloadHandler;
            if (dh == null)
                return null;
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(dh.data, markNonReadable);
            return texture;
        }

        public Texture2D texture { get { return CreateTextureFromDownloadedData(false); } }

        public Texture2D textureNonReadable { get { return CreateTextureFromDownloadedData(true); } }

        public void LoadImageIntoTexture(Texture2D texture) {
            if (!WaitUntilDoneIfPossible())
                return;
			if (IsError()) {
                Debug.LogError("Cannot load image: download failed");
                return;
            }
            var dh = _uwr.downloadHandler;
            if (dh == null) {
                Debug.LogError("Cannot load image: internal error");
                return;
            }
            texture.LoadImage(dh.data, false);
        }

        public ThreadPriority threadPriority { get; set; }

        public float uploadProgress {
            get {
                var progress = _uwr.uploadProgress;
                // UWR returns negative if not sent yet, CustomWWW always returns between 0 and 1
                if (progress < 0)
                    progress = 0.0f;
                return progress;
            }
        }

        public string url { get { return _uwr.url; } }

        public override bool keepWaiting { get { return !_uwr.isDone; } }

        public void Dispose() {
            _uwr.Dispose();
        }

        private bool WaitUntilDoneIfPossible() {
            if (_uwr.isDone)
                return true;
            if (url.StartsWith("file://", StringComparison.OrdinalIgnoreCase)) {
                // Reading file should be already done on non-threaded platforms
                // on threaded simply spin until done
                while (!_uwr.isDone) { }

                return true;
            } else {
                Debug.LogError("You are trying to load data from a www stream which has not completed the download yet.\nYou need to yield the download or wait until isDone returns true.");
                return false;
            }
        }

        private UnityWebRequest _uwr;
        //private AssetBundle _assetBundle;
        //private Dictionary<string, string> _responseHeaders;
    }



}


