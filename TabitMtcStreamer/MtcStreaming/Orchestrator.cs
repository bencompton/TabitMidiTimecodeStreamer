using System;

using Sanford.Multimedia.Midi;

namespace TabitMtcStreamer.MtcStreaming
{
    public class Orchestrator : IDisposable
    {
        private OutputDevice MidiOut;
        private InputDevice MidiIn;
        private int Tempo;
        private int MidiChannel;
        private QuarterFrameTimecodeGenerator QFTG;
        private TabitTimecodeListener TabitListener;

        public Orchestrator(int tempo, int midiInDeviceId, int midiOutDeviceId, int midiChannel)
        {
            Tempo = tempo;
            MidiIn = new InputDevice(midiInDeviceId);
            MidiOut = new OutputDevice(midiOutDeviceId);                        
            MidiChannel = midiChannel - 1;
            TabitListener = new TabitTimecodeListener(MidiIn, MidiChannel);
            TabitListener.TimecodeTransmissionStarted += TabitListener_TimecodeTransmissionStarted;
            TabitListener.TimecodeTransmissionStopped += TabitListener_TimecodeTransmissionStopped;
        }

        private void TabitListener_TimecodeTransmissionStarted(object sender, TabitTimecodeListener.TransmissionStartedEventArgs e)
        {
            StartPlaying(e.CurrentBeatNumber);
        }

        private void TabitListener_TimecodeTransmissionStopped(object sender, EventArgs e)
        {
            StopPlaying();
        }

        private void StartPlaying(int currentBeatNumber)
        {
            var TabitCurrentTimecode = new TimeCode();
            TabitCurrentTimecode.FillFromBeatNumber(Tempo, currentBeatNumber);            

            MidiOut.Send(new SysExMessage(TabitCurrentTimecode.GetFullTimeCode()));            

            QFTG = new QuarterFrameTimecodeGenerator(MidiOut, TabitCurrentTimecode);
            QFTG.Start();
        }

        private void StopPlaying()
        {
            QFTG.Stop();
        }

        public void Dispose()
        {
            MidiIn.StopRecording();
            MidiIn.Dispose();
            MidiOut.Dispose();
        }
    }
}
