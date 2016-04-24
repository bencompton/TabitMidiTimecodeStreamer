using System;
using System.Collections.Generic;

namespace TabitMtcStreamer.MtcStreaming
{
    public class NoteTimecodeReceiver
    {
        private List<int> InitialNotes;
        private DateTime _LastNoteReceived;

        public NoteTimecodeReceiver()
        {
            InitialNotes = new List<int>();
        }

        public DateTime LastNoteReceived
        {
            get
            {
                lock (InitialNotes)
                {
                    return _LastNoteReceived;
                }
            }
        }
        
        public int? StartBeatNumber
        {
            get
            {
                lock (InitialNotes)
                { 
                    if (InitialNotes.Count >= 3)
                    {
                        int ones = 0, tens = 0, hundreds = 0;
                        
                        for (int i = 0; i < 3; i++)
                        {
                            int noteNumber = InitialNotes[i];

                            if (noteNumber < 10)
                            {
                                ones = noteNumber;
                            }
                            else if (noteNumber < 20)
                            {
                                tens = noteNumber - 10;
                            }
                            else if (noteNumber < 200)
                            {
                                hundreds = noteNumber - 100;
                            }                            
                        }

                        return ones + (tens * 10) + (hundreds * 100);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            
        }

        public void AddNote(int noteNumber)
        {
            lock (InitialNotes)
            {
                if (StartBeatNumber == null)
                {
                    InitialNotes.Add(noteNumber);
                }

                _LastNoteReceived = DateTime.Now;
            }
        }
    }
}
