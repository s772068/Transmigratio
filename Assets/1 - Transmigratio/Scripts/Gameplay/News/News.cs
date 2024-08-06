using System;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Gameplay
{
    public class News
    {
        private List<NewsSO> _newsList;
        private NewsPanel _prefab;
        private Transform _panelsParent;
        private int Year => Transmigratio.Instance.TMDB.Year;

        public static Action<string> NewsTrigger;
        public static Action<NewsSO> NewsClosed;

        public News(Transform panelsParent, NewsPanel prefab, List<NewsSO> news)
        {
            _newsList = new(news);
            _prefab = prefab;
            _panelsParent = panelsParent;
            NewsTrigger += OnNewsTrigger;
            NewsClosed += DeleateNews;
            Timeline.TickShow += OnTick;
        }
        ~News()
        {
            NewsTrigger -= OnNewsTrigger;
            NewsClosed -= DeleateNews;
            Timeline.TickShow -= OnTick;
        }

        private void OnNewsTrigger(string newsTitle)
        {
            NewsSO news = _newsList.Find(match => match.Title == newsTitle);

            if (news == null)
                return;

            PanelFabric.CreateNews(_panelsParent, _prefab, news);
        }

        private void OnTick()
        {
            foreach (NewsSO news in _newsList)
            {
                if (news.Type != NewsSO.NewsType.Date || news.DateInvoke != Year)
                    continue;

                PanelFabric.CreateNews(_panelsParent, _prefab, news);
            }
        }

        private void DeleateNews(NewsSO news)
        {
            _newsList.Remove(news);
        }
    }
}
