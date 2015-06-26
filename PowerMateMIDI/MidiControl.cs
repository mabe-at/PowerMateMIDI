using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midi;

namespace PowerMateMIDI
{
    public class MidiControl
    {
        public Midi.Control control;
        public String Name { get; set; }

        public MidiControl(Midi.Control c)
        {
            control = c;
            Name = Enum.GetName(typeof(Midi.Control), c);
        }

        public override String ToString()
        {
            return Name;
        }
    }
}
