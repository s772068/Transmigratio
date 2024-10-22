using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UpdateContentSize : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectSize;
        [SerializeField] private List<RectTransform> _elements;

        private void Start()
        {
            foreach (var child in _elements)
            {
                child.sizeDelta = new Vector2(_rectSize.rect.width, _rectSize.rect.height);
            }
        }
    }
}
