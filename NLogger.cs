using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NLogger
{
    /// <summary>
    /// LogType is an enum representing what kind of log you're using.
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// There is no log type.
        /// </summary>
        None=1,
        /// <summary>
        /// Everything is fine, but this should be known
        /// </summary>
        Info=2,
        /// <summary>
        /// Something is out of the ordinary and should have attention.
        /// </summary>
        Warning=4,
        /// <summary>
        /// Something has gone wrong, but the program may be able to recover.
        /// </summary>
        Error=8,
        /// <summary>
        /// Something has gone very wrong and the program is unable to recover.
        /// </summary>
        Critical=16,
        /// <summary>
        /// This is a message intended to debug. This kind of message should not be in a production release.
        /// </summary>
        Debug=32
    }
    /// <summary>
    /// Logger is a class implimentation of a log. It is in charge of logging errors and information to console and also streams.
    /// </summary>
    public class Logger
    {
        private enum LogStyle
        {
            Console = 1,
            File = 2,
            Steam = 4
        }
        private readonly int type;
        private readonly string location;

        /// <summary>
        /// The Seperator property is placed after every argument in a log. It is used to seperate arguments in text.
        /// </summary>
        public char Seperator = '\t'; // TAB is default
        /// <summary>
        /// The Verbose property will only display logs from Info and None when it is set to true.
        /// When Verbose is set to false, it  will only display Warnings, Errors or Critical logs.
        /// </summary>
        public bool Verbose = false; // Show Info/Warnings
        /// <summary>
        /// When the Debug property is set to true, the logger will log Debug messages
        /// </summary>
        public bool Debug = false; // Show Debug
        /// <summary>
        /// When the AddTime property is set to true, the logger will add Date Time information to the log
        /// </summary>
        public bool AddTime = false; // Display time information

        /// <summary>
        /// The default constructor for constructing a logger.
        /// The logger will only log to console.
        /// </summary>
        public Logger()
        {
            type += (int)LogStyle.Console;
        }
        /// <summary>
        /// This constructor creates a Logger which logs to Console, and a specified file location.
        /// </summary>
        /// <param name="file_location">The file location to log to.</param>
        public Logger(string file_location)
        {
            type += (int)LogStyle.File;
            type += (int)LogStyle.Console;
            location = file_location;
        }
        /// <summary>
        /// This method will log any number of arguments to the log.
        /// If the first argument is a <see cref="LogType"/>, use that type for the log.
        /// </summary>
        /// <param name="objs">Any number of objects, parsed as an object array, or as any number of parameters.</param>
        /// <example>
        /// This shows how to log multiple variables to the logger.
        /// <code>
        ///     Logger logger = new Logger()
        /// 
        ///     int Test1 = 1
        ///     float Test2 = 5.4
        ///     string Test3 = "Hello World!"
        ///     
        ///     logger.Log("This is a test:", Test1, Test2, Test3)
        /// </code>
        /// This will output <c>[DEBUG] This is a test: 1   5.4 Hello World!</c> to the log.
        /// In this case, it only logs to console.
        /// </example>
        public void Log(params object[] objs)
        {
            // The eventual output message
            string LogMessage = "";
            // The message type of message is debug by default
            LogType logType = LogType.Debug;
            // If the first argument is a logtype, then set logType to that argument
            if (objs[0] is LogType)
            {
                logType = (LogType)objs[0];
            }
            // Loop through all the objects
            foreach (object obj in objs)
            {
                // If the argument is a logtype, we assume it's the first argument
                // If it's not the first argument, it will do multiple unneccesary checks
                if (obj is LogType)
                {
                    if ((LogType)obj == LogType.Debug & !Debug)
                        return;
                    if (((int)obj & 7)>0 & !Verbose)
                        return;

                    continue;
                }
                // Add the object's string value to the message, followed by the seperator
                LogMessage += obj.ToString()+Seperator;
            }
            // We support windows here. Carriage Return, New line.
            LogMessage += "\r\n";

            // Build the end message
            LogMessage = $"{logType.ToString()}] " + LogMessage;
            // If the time flag is set, include the current time
            if (AddTime)
                LogMessage = $"{DateTime.Now}, " + LogMessage;
            LogMessage = "[" + LogMessage;


            if ((type & (int)LogStyle.Console) > 0)
                Console.WriteLine(LogMessage);
            // If the type contains the File LogStyle then write the message to a file
            if ((type & (int)LogStyle.File) > 0)
            {
                using (FileStream stream = new FileStream(location, FileMode.Append))
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(LogMessage);
                }

            }
            
        }

    }
}
