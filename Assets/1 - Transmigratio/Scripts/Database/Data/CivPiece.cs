using System.Collections.Generic;
using Events.Controllers.Local;
using Scenes.Game;
using System;

using GlobalEvents = Events.Controllers.Global;

namespace Database.Data {
    /// <summary>
    /// ��������� CivPiece - ��� ���� "�������" ����������� � ���������� �������. 
    /// ���� ����������� ���� � ��� ��������, ��� ������, ��� ��� ������� �� ��� �������� CivPiece
    /// </summary>
    [Serializable]
    public class CivPiece {
        public Population Population;
        public float PopulationGrow;
        public float GivenFood;
        public float RequestFood;
        public float ReserveFood;
        public float TakenFood;

        public int EventsCount => events.Count;
        public int RegionID;
        public string CivName;

        public Action Destroy;

        private List<Events.Controllers.Base> events = new();
        private float _prevPopulationGrow;

        public Region Region => Database.map.AllRegions[RegionID];
        public Civilization Civilization => Database.humanity.Civilizations[CivName];
        private Base Database => Transmigratio.Instance.Database;

        /// <summary>
        /// ������������� ��� ��������� � ������� ����� �������� ��� ��� ������ ����
        /// </summary>
        public void Init(int region, string civilization, int startPopulation, float reserve) {
            RegionID = region;
            Population = new Population();
            Population.value = startPopulation;
            CivName = civilization;
            ReserveFood = reserve;  //����������� ���������� ��� � �������
        }

        public void AddEvent(Events.Controllers.Base e) => events.Add(e);
        public void RemoveEvent(Events.Controllers.Base e) => events.Remove(e);

        /// <summary>
        /// ��������� ��������� ������� �� ���
        /// </summary>
        public void DeltaPop() {
            float faunaKr = (float) (Math.Pow(Region.Fauna.GetMaxQuantity().value, 0.58d) / 10f);
            TakenFood = Population.value / 100f * faunaKr * Civilization.ProdModeK;
            RequestFood = Population.Value / 150f;
            if (ReserveFood > RequestFood) GivenFood = RequestFood;
            else GivenFood = ReserveFood;
            ReserveFood += TakenFood - GivenFood;
            _prevPopulationGrow = PopulationGrow;
            PopulationGrow = Population.Value * Civilization.GovernmentCorruption * GivenFood / RequestFood - Population.Value / 3f;

            Population.value += (int) PopulationGrow;
            if (Population.value <= 50) { Destroy?.Invoke(); return; }

            if (_prevPopulationGrow >= 0 && PopulationGrow < 0) {
                Hunger.onActivate?.Invoke(this);
                GlobalEvents.Migration.OnMigration(this);
            } else if (_prevPopulationGrow < 0 && PopulationGrow >= 0) {
                Hunger.onDeactivate?.Invoke(this);
            }
        }
    }
}
