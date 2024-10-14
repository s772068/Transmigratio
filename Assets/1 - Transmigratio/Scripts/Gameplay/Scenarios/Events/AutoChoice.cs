using Gameplay.Scenarios.Events.Data;
using System.Collections.Generic;

namespace Gameplay.Scenarios.Events {
    public static class AutoChoice {
        private static Dictionary<Base, List<IDesidion>> _events = new();

        public static Dictionary<Base, List<IDesidion>> Events => _events;

        static AutoChoice() {
            AutoChoicePanel.AutoChoiceUpdate += OnAutoChoiceUpdate;
            AutoChoicePanel.AutoChoiceModeUpdate += OnAutoModeChange;
        }

        public static void NewEvent(Base newEvent, List<IDesidion> desidions) {
            if (!_events.ContainsKey(newEvent))
                _events.Add(newEvent, new (desidions));
        }

        public static void RemoveEvent(Base removeEvent) {
            _events.Remove(removeEvent);
        }

        private static void OnAutoChoiceUpdate(Base eventUpdate, List<IDesidion> newPriority, bool autoChoice) {
            _events[eventUpdate] = newPriority;
            eventUpdate.AutoChoice = autoChoice;
        }

        private static void OnAutoModeChange(Base gameEvent, bool enable) => gameEvent.AutoChoice = enable;
    }
}
