using System;

namespace Events.Data {
    public struct Desidion {
        public string Title;
        public Func<int> OnGetPoints;
        public Action OnClick;
    }
}
