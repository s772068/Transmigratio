using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CircularStat : MonoBehaviour
    {
        [SerializeField] private List<Image> _circulars;
        
        /// <summary>
        /// Update stat
        /// </summary>
        /// <param name="values">Values from bigger to smaller</param>
        public void Init(List<float> values)
        {
            #if UNITY_EDITOR
            if (values.Count > _circulars.Count)
                Debug.LogWarning("Income values > circulars count");
            #endif
            
            float fill = 0f;
            for (int i = 0; i < _circulars.Count; i++)
            {
                if (i >= values.Count)
                    break;
                
                fill += values[i] / 100f;
                _circulars[i].fillAmount = fill;
            }
        }
    }
}