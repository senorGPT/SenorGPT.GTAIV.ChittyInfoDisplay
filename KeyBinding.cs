using System;

namespace SenorGPT.GTAIV.ChittyInfoDisplay
{
    public class KeyBinding
    {
        public string Name { get; set; }
        public int Key { get; set; }
        public bool IsPressed { get; set; }
        public Action Pressed { get; set; }
        public Action Held { get; set; }
        public Action Released { get; set; }
        public int? ModifierKey { get; set; }
    }
}

