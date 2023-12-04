using UnityEngine;
using System;

namespace WorldMapStrategyKit {

    public struct CustomBorder {

        public float width;
        public float textureTiling;
        public Texture texture;
        public Color tintColor;
        public float animationSpeed;
        public float animationStartTime;
        public float animationAcumOffset;

        public void Init() {
            width = 0.1f;
            textureTiling = 1f;
            tintColor = Color.white;
        }

    }


}