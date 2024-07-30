using System;
using UnityEngine;

namespace Gameplay
{
    public class Intervention
    {
        [SerializeField] private int _interventionPoints = 100;

        public int InterventionPoints => _interventionPoints;

        public static Func<int, bool> UseIntervention;
        public static event Action InterventionPointsUpdate;

        public Intervention(int startPoints = 100)
        {
            _interventionPoints = startPoints;
            UseIntervention += OnUseIntervention;
        }
        ~Intervention()
        {
            UseIntervention -= OnUseIntervention;
        }

        private bool OnUseIntervention(int points)
        {
            if (points > _interventionPoints)
                return false;

            _interventionPoints -= points;
            InterventionPointsUpdate?.Invoke();
            return true;
        }
    }
}
