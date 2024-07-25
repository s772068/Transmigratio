using Events.Data;
using System;
using System.Collections.Generic;

namespace Events
{
    public static class AutoChoice
    {
        private static Dictionary<Controllers.Base, List<Desidion>> _events = new();

        public static Dictionary<Controllers.Base, List<Desidion>> Events => _events;
        public static Action AutoChoiceUpdate;


        public static void NewEvent(Controllers.Base newEvent, List<Desidion> desidions)
        {
            _events.Add(newEvent, new (desidions));
            AutoChoiceUpdate?.Invoke();
        }

        public static void RemoveEvent(Controllers.Base removeEvent)
        {
            _events.Remove(removeEvent);
            AutoChoiceUpdate?.Invoke();
        }
    }
}
