using Gameplay;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace UI
{
    public class NewsPanel : Panel
    {
        [SerializeField] private Image _image;
        [SerializeField] private LocalizeStringEvent _title;
        [SerializeField] private LocalizeStringEvent _description;
        private NewsSO _news;
        private bool _isInit = false;

        private void OnDestroy()
        {
            News.NewsClosed?.Invoke(_news);
        }

        public void Init(NewsSO news)
        {
            if (_isInit)
                return;

            _isInit = true;
            _news = news;
            _image.sprite = news.Image;
            _title.SetEntry(news.Title);
            _description.SetEntry(news.Description);
        }
    }
}
