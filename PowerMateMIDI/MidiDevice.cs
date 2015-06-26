using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midi;

namespace PowerMateMIDI
{
    public class MidiDevice
    {
        public OutputDevice device;
        public String Name { get; set; }

        public MidiDevice(OutputDevice d)
        {
            device = d;
            Name = d.Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
