using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Indexer
{
    public class AppEvents
    {
        // Constructors 
        public AppEvents(string logName) :
            this(logName, Process.GetCurrentProcess().ProcessName, ".") { }

        public AppEvents(string logName, string source) : this(logName, source, ".") { }

        public AppEvents(string logName, string source, string machineName)
        {
            this.logName = logName;
            this.source = source;
            this.machineName = machineName;

            if (!EventLog.SourceExists(source, machineName))
            {
                EventSourceCreationData sourceData =
                    new EventSourceCreationData(source, logName);
                sourceData.MachineName = machineName;

                EventLog.CreateEventSource(sourceData);
            }

            log = new EventLog(logName, machineName, source);
            log.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 0);
//            log = new EventLog(logName);
//            log.EnableRaisingEvents = true;
        }

        // Fields 
        private EventLog log = null;
        private string source = "";
        private string logName = "";
        private string machineName = ".";

        // Properties 
        public string Name
        {
            get { return (logName); }
        }

        public string SourceName
        {
            get { return (source); }
        }

        public string Machine
        {
            get { return (machineName); }
        }
        // Methods 
        public void WriteToLog(string message, EventLogEntryType type,
            CategoryType category, EventIDType eventID)
        {
            if (log == null)
            {
                throw (new ArgumentNullException("log",
                    "This Event Log has not been opened or has been closed."));
            }

            log.WriteEntry(message, type, (int)eventID, (short)category);
        }

        public void WriteToLog(string message, EventLogEntryType type,
            CategoryType category, EventIDType eventID, byte[] rawData)
        {
            if (log == null)
            {
                throw (new ArgumentNullException("log",
                    "This Event Log has not been opened or has been closed."));
            }

            log.WriteEntry(message, type, (int)eventID, (short)category, rawData);
        }

        public EventLogEntryCollection GetEntries()
        {
            if (log == null)
            {
                throw (new ArgumentNullException("log",
                    "This Event Log has not been opened or has been closed."));
            }

            return (log.Entries);
        }

        public void ClearLog()
        {
            if (log == null)
            {
                throw (new ArgumentNullException("log",
                    "This Event Log has not been opened or has been closed."));
            }

            log.Clear();
        }

        public void CloseLog()
        {
            if (log == null)
            {
                throw (new ArgumentNullException("log",
                    "This Event Log has not been opened or has been closed."));
            }
            log.Close();
            log = null;
        }

        public void DeleteLog()
        {
            if (EventLog.SourceExists(source, machineName))
            {
                EventLog.DeleteEventSource(source, machineName);
            }

            if (logName != "Application" &&
                logName != "Security" &&
                logName != "System")
            {
                if (EventLog.Exists(logName, machineName))
                {
                    EventLog.Delete(logName, machineName);
                }
            }

            if (log != null)
            {
                log.Close();
                log = null;
            }
        }
    }

    public enum EventIDType
    {
        NA = 0,
        Read = 1,
        Write = 2,
        ExceptionThrown = 3,
        BufferOverflowCondition = 4,
        SecurityFailure = 5,
        SecurityPotentiallyCompromised = 6
    }

    public enum CategoryType : short
    {
        None = 0,
        WriteToDB = 1,
        ReadFromDB = 2,
        WriteToFile = 3,
        ReadFromFile = 4,
        AppStartUp = 5,
        AppShutDown = 6,
        UserInput = 7
    }

}
