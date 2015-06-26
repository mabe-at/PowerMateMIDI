using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using HidLibrary;
using Midi;

namespace PowerMateMIDI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<MidiDevice> midiDevices { get; set; }
        public List<HidDevice> hidDevices { get; set; }
        public ObservableCollection<MidiControl> midiControls { get; set; }
        public ObservableCollection<MidiChannel> midiChannels { get; set; }
        public ObservableCollection<Mate> Mates { get; set; }

        public Mate SelectedMate { get; set; }

        private const int VendorId = 0x077d;
        private const int ProductId = 0x0410;

        public MainWindow()
        {
            Mates = new ObservableCollection<Mate>();
            midiDevices = new ObservableCollection<MidiDevice>();
            midiControls = new ObservableCollection<MidiControl>();
            midiChannels = new ObservableCollection<MidiChannel>();
            LoadData();
            InitializeComponent();
            this.DataContext = this;
        }

        public void LoadData() {

            // get all MidiChannels
            foreach (Midi.Channel val in Enum.GetValues(typeof(Midi.Channel)))
            {
                midiChannels.Add(new MidiChannel(val));
            }

            // get all MidiControls
            foreach (Midi.Control val in Enum.GetValues(typeof(Midi.Control)))
            {
                midiControls.Add(new MidiControl(val));
            }

            // get MidiDevices
            ReadOnlyCollection<OutputDevice> outputDevices = OutputDevice.InstalledDevices;
            foreach (OutputDevice mDevice in outputDevices)
            {
                midiDevices.Add(new MidiDevice(mDevice));
            }
            
            // get PowerMates
            List<HidDevice> hidDevices = HidDevices.Enumerate(VendorId, ProductId).ToList();

            // build Mates List
            int dc = 0;
            foreach(HidDevice d in hidDevices) {
                Mates.Add(new Mate(d, midiDevices[0],midiControls[5],midiChannels[dc]));
                dc++;
            }
        }

        private void Row_Selected(object sender, RoutedEventArgs e)
        {
            if (SelectedMate != null)
            {
                SelectedMate.Blink();
            }
        }

        private void StartStop_Clicked(object sender, RoutedEventArgs e)
        {
            Button b = (Button) sender;
            if (SelectedMate != null)
            {
                if (SelectedMate.StartStop())
                {
                    if (SelectedMate.isStarted())
                    {
                        b.Content = "Stop";
                        EnDisableCombos(false);
                    }
                    else
                    {
                        b.Content = "Start";
                        EnDisableCombos(true);
                    }
                }
            }
        }

        private void EnDisableCombos(Boolean state)
        {

        }

        private void OnPropertyChanged(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}
