using Events.Data;
using System;
using System.Collections.Generic;

namespace Events
{
    public static class AutoChoice
    {
        private static Dictionary<Controllers.Base, List<Desidion>> _events = new();

        public static Dictionary<Controllers.Base, List<Desidion>> Events => _events;

        static AutoChoice()
        {
            AutoChoicePanel.AutoChoiceUpdate += OnAutoChoiceUpdate;
            AutoChoicePanel.AutoChoiceModeUpdate += OnAutoModeChange;
        }

        public static void NewEvent(Controllers.Base newEvent, List<Desidion> desidions)
        {
            _events.Add(newEvent, new (desidions));
        }

        public static void RemoveEvent(Controllers.Base removeEvent)
        {
            _events.Remove(removeEvent);
        }

        private static void OnAutoChoiceUpdate(Controllers.Base eventUpdate, List<Desidion> newPriority, bool autoChoice)
        {
            _events[eventUpdate] = newPriority;
            eventUpdate.AutoChoice = autoChoice;
        }

        private static void OnAutoModeChange(Controllers.Base gameEvent, bool enable) => gameEvent.AutoChoice = enable;
    }
}
