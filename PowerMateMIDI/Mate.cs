using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using GriffinPowerMate;
using HidLibrary;

namespace PowerMateMIDI
{
    public class Mate : INotifyPropertyChanged
    {
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        public MidiDevice SelectedDevice { get; set; }
        public MidiControl SelectedControl { get; set; }

        private MidiChannel _sChannel;

        private Timer aTimer;

        public MidiChannel SelectedChannel
        {
            get { return _sChannel; }
            set
            {
                _sChannel = value;
                OnPropertyChanged("SelectedChannel");
            }
        }
        public MateManager mateManager = null;

        public String Name { get; set; }

        private static int counter = 0;
        private int id = 0;

        private Boolean stopped = true;
        private Boolean blinking = false;
        private Boolean pressed = false;

        public Boolean Stopped
        {
            get { return stopped; }
            set
            {
                stopped = value;
                OnPropertyChanged("Stopped");
            }
        }

        public Mate(HidDevice d, MidiDevice md, MidiControl mc, MidiChannel mchan)
        {
            mateManager = new MateManager();
            mateManager.OpenDevice(d);
            mateManager.ButtonDown += new EventHandler<PowerMateEventArgs>(ButtonDown);
            mateManager.ButtonUp += new EventHandler<PowerMateEventArgs>(ButtonUp);

            Dark();
            counter++;
            id = counter;
            Name = "Powermate " + id;
            SelectedDevice = md;
            SelectedControl = mc;
            SelectedChannel = mchan;
        }

        public override string ToString()
        {
            //return "Powermate " + id;
            return Name;
        }

        private void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public Boolean StartStop()
        {
            Blink();
            if (isStarted())
            {
                /*if (SelectedDevice != null && SelectedDevice.device != null && SelectedDevice.device.IsOpen)
                {
                    SelectedDevice.device.Close();
                }*/
                Stopped = true;
            }
            else
            {
                if (SelectedDevice != null && SelectedDevice.device != null) {
                    if (!SelectedDevice.device.IsOpen)
                    {
                        SelectedDevice.device.Open();
                    }
                    if (SelectedDevice.device.IsOpen)
                    {
                        Stopped = false;
                    }
                }
            }
            UnBlink();
            return true;
        }

        public Boolean isStarted()
        {
            return !Stopped;
        }

        public void Blink()
        {
            if (!blinking)
            {
                blinking = true;
                aTimer = new Timer(1500);
                aTimer.Elapsed += new ElapsedEventHandler(StopBlink);
                mateManager.SetLedPulseSpeed(8);
                mateManager.SetLedPulseEnabled(true);
                aTimer.Start();
            }
        }

        public void StopBlink(object source, ElapsedEventArgs e)
        {
            aTimer.Stop();
            blinking = false;
            UnBlink();
        }

        public void UnBlink() {
            if (!blinking)
            {
                mateManager.SetLedPulseEnabled(false);
                if (isStarted())
                {
                    Light();
                }
                else
                {
                    Dark();
                }
            }
        }

        public void Light()
        {
            if (!isStarted() || !pressed)
            {
                mateManager.SetLedPulseEnabled(false);
                mateManager.SetLedBrightness(128);
            }
        }

        public void Dark()
        {
            mateManager.SetLedPulseEnabled(false);
            mateManager.SetLedBrightness(0);
        }

        public void ButtonUp(object sender, PowerMateEventArgs e)
        {
            if (isStarted())
            {
                pressed = false;
                Light();
                SendMessage(0);
            }
            //Console.WriteLine("PowerMate button up event");
            //Console.WriteLine("PowerMate state button: {0}", (e.State.ButtonState == PowerMateButtonState.Up ? "Up" : "Down"));
            //Console.WriteLine("PowerMate state knob displacement: {0}", e.State.KnobDisplacement);
        }

        public void ButtonDown(object sender, PowerMateEventArgs e)
        {
            if (isStarted())
            {
                pressed = true;
                Dark();
                SendMessage(127);
            }
            //Console.WriteLine("PowerMate button down event");
            //Console.WriteLine("PowerMate state button: {0}", (e.State.ButtonState == PowerMateButtonState.Up ? "Up" : "Down"));
            //Console.WriteLine("PowerMate state knob displacement: {0}", e.State.KnobDisplacement);
        }

        public void SendMessage(int v)
        {
            if (SelectedDevice != null && SelectedDevice.device.IsOpen)
            {
                SelectedDevice.device.SendControlChange(SelectedChannel.channel, SelectedControl.control, v);
                //SelectedDevice.device.SendControlChange(Midi.Channel.Channel1, Midi.Control.Button1, v);
            }
        }
    }
}
