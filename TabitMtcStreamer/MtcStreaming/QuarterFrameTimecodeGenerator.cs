using System;

using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Timers;

namespace TabitMtcStreamer.MtcStreaming
{
    public class QuarterFrameTimecodeGenerator
    {
        private int CurrentPiece = 0;
        private TimeCode timeCode;
        private OutputDevice outDevice;
        private ITimer Timer;
        private const int FrameRate = 25;
        
        public QuarterFrameTimecodeGenerator(OutputDevice outDevice, TimeCode timeCode)
        {
            this.outDevice = outDevice;
            this.timeCode = timeCode;
            Timer = TimerFactory.Create();
        }

        public void Start()
        {
            Timer_Tick(null, null);
            Timer.Mode = TimerMode.Periodic;
            Timer.Period = 1000 / FrameRate / 4;            
            Timer.Tick += Timer_Tick;
            Timer.Start();
        }

        public void Stop()
        {
            Timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            outDevice.Send(new SysCommonMessage(SysCommonType.MidiTimeCode, timeCode.GetPiece(CurrentPiece)));

            CurrentPiece++;

            if (CurrentPiece > 7)
            {
                CurrentPiece = 0;
                timeCode.AddFrames(2);
            }
        }
    }
}
