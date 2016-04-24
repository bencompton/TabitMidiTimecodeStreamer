using Sanford.Multimedia.Midi;
using System;
using System.Windows.Forms;

using TabitMtcStreamer.MtcStreaming;

namespace TabitMtcStreamer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private Orchestrator orchestrator;        

        private void Form1_Load(object sender, EventArgs e)
        {
            var devicePicker = new Sanford.Multimedia.Midi.UI.DeviceDialog();
            var dialogResult = devicePicker.ShowDialog();                        

            try
            {
                orchestrator = new Orchestrator(120, devicePicker.InputDeviceID, devicePicker.OutputDeviceID, 16);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                orchestrator.Dispose();
            }
            catch(Exception ex)
            {
                //Eat it
            }
        }
    }
}
