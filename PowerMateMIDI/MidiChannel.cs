using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Midi;

namespace PowerMateMIDI
{
    public class MidiChannel
    {
        public Midi.Channel channel;
        public String Name { get; set; }

        public MidiChannel(Midi.Channel c)
        {
            channel = c;
            Name = Enum.GetName(typeof(Midi.Channel), c);
        }

        public override String ToString()
        {
            return Name;
        }
    }
}
