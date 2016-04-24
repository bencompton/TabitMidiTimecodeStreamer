using System;

using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Timers;

namespace TabitMtcStreamer.MtcStreaming
{
    public class TabitTimecodeListener
    {
        public event EventHandler<TransmissionStartedEventArgs> TimecodeTransmissionStarted;
        public event EventHandler TimecodeTransmissionStopped;
        
        private InputDevice MidiIn;
        private int MidiChannel;
        private NoteTimecodeReceiver NoteReceiver;
        private ITimer Timer;
        private bool TabitCurrentlyPlaying;

        public TabitTimecodeListener(InputDevice midiIn, int midiChannel)
        {
            MidiIn = midiIn;
            MidiChannel = midiChannel;
            NoteReceiver = new NoteTimecodeReceiver();

            Timer = TimerFactory.Create();
            Timer.Mode = TimerMode.Periodic;
            Timer.Period = 10;
            Timer.Tick += Timer_Tick;            

            MidiIn.StartRecording();
            MidiIn.ChannelMessageReceived += MidiIn_ChannelMessageReceived;            
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (TabitCurrentlyPlaying && (DateTime.Now - NoteReceiver.LastNoteReceived).TotalSeconds > 1)
            {
                Timer.Stop();
                TimecodeTransmissionStopped(this, new EventArgs());                
                TabitCurrentlyPlaying = false;
                NoteReceiver = null;                
            }                        
        }

        private void MidiIn_ChannelMessageReceived(object sender, ChannelMessageEventArgs e)
        {
            if (e.Message.MidiChannel == MidiChannel)
            {
                if (e.Message.MessageType == MessageType.Channel && e.Message.Command == ChannelCommand.NoteOn)
                {
                    lock (MidiIn)
                    { 
                        if (NoteReceiver == null)
                        {
                            NoteReceiver = new NoteTimecodeReceiver();
                        }

                        NoteReceiver.AddNote(e.Message.Data1);

                        if (NoteReceiver.StartBeatNumber != null && !TabitCurrentlyPlaying)
                        {
                            TimecodeTransmissionStarted(this, new TransmissionStartedEventArgs(NoteReceiver.StartBeatNumber.Value));
                            TabitCurrentlyPlaying = true;
                            Timer.Start();
                        }
                    }
                }
            }
        }

        public class TransmissionStartedEventArgs : EventArgs
        {
            public int CurrentBeatNumber { get; private set; }
            
            public TransmissionStartedEventArgs(int currentBeatNumber)
            {
                CurrentBeatNumber = currentBeatNumber;
            }
        }
    }
}
