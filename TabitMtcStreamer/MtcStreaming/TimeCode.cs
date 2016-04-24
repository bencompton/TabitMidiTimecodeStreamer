using System;
using System.Collections.Generic;

namespace TabitMtcStreamer.MtcStreaming
{
    public class TimeCode
    {
        private const int FrameRate = 25;

        public int Hour { get; private set; }
        public int Minute { get; private set; }
        public int Second { get; private set; }
        public int Frame { get; private set; }

        public void AddFrames(int frameCount)
        {
            Frame += frameCount;

            if (Frame > FrameRate)
            {
                Second++;
                Frame -= FrameRate;
            }

            if (Second > 60)
            {
                Minute++;
                Second -= 60;
            }

            if (Minute > 60)
            {
                Hour++;
                Minute -= 60;                
            }
        }

        public byte[] GetFullTimeCode()
        {
            return new List<byte>() {
                0xF0,
                0x7F,
                0x7F,
                0x01,
                0x01,
                (byte)Hour,
                (byte)Minute,
                (byte)Second,
                (byte)Frame,
                0xF7
            }
            .ToArray();            
        }

        public int FillFromBeatNumber(int tempo, int beatNumber)
        {
            double millisecondsPerBeat = 60000 / tempo;            
            double millisecondsPerFrame = 1000 / FrameRate;
            double totalMillisecondsElapsed = beatNumber * millisecondsPerBeat;

            var time = TimeSpan.FromMilliseconds(totalMillisecondsElapsed);
            Hour = time.Hours;
            Minute = time.Minutes;
            Second = time.Seconds;
            Frame = (int)Math.Floor(time.Milliseconds / millisecondsPerFrame);

            return (int)(time.Milliseconds % millisecondsPerFrame);
        }

        public byte GetPiece(int pieceNumber)
        {
            int piece = 0;

            piece += pieceNumber << 4;

            switch (pieceNumber)
            {
                case 0:
                    piece += Frame & 0xF;
                    break;
                case 1:
                    piece += Frame >> 4;
                    break;
                case 2:
                    piece += Second & 0xF;
                    break;
                case 3:
                    piece += Second >> 4;
                    break;
                case 4:
                    piece += Minute & 0xF;
                    break;
                case 5:
                    piece += Minute >> 4;
                    break;
                case 6:
                    piece += Hour & 0xF;
                    break;
                case 7:
                    piece += Hour >> 4;
                    piece += 0x1 << 1;
                    break;
            }

            return (byte)piece;
        }
    }
}
