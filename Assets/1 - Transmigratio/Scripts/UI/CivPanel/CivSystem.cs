using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.CivPanel
{
    public class CivSystem : MonoBehaviour
    {
        [SerializeField] private List<PolitElement> _politElements;
        [SerializeField] private CircularStat _politStat;

        private void OnEnable()
        {
            _politStat.Init(new List<float> { 55f, 20f, 10f, 6f, 5f, 4f });
        }
    }
}
