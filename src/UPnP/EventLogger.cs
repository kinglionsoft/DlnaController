/*   
Copyright 2006 - 2010 Intel Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Diagnostics;

namespace OpenSource.Utilities
{

	/// <summary>
	/// This class can be used as the master event logging class for all
	/// of an application and its libraries. The application should set the
	/// system log file upon startup. All exceptions should be sent to the log
	/// using the Log(Exception) method.
	/// </summary>
	public sealed class EventLogger
	{
		public static event EventHandler OnEvent;
		public static bool Enabled = false;
		public static bool ShowAll = false;

		private static string g_logName = null;
        private static string g_sourceName = null;
        private static string g_productVersion = null;
		private static bool   g_onExceptionShowMessage = false;

		/// <summary>
		/// Set the application wide event logging to a Windows event log file.
		/// </summary>
		/// <param name="sourceName">Generaly the name of the application</param>
		/// <param name="logName">The name of the system log file</param>
		/// <param name="productVersion">The application's version string</param>
		public static void SetLog(string sourceName,string logName,string productVersion)
		{
            
		}

		/// <summary>
		/// Set the action to take when an exception is logged into the event
		/// logger. This action will be taken in addition to logging the exception
		/// in the system logs.
		/// </summary>
		/// <param name="showMessageBox">Show a message box with the event</param>
		public static void SetOnExceptionAction(bool showMessageBox)
		{
			g_onExceptionShowMessage = showMessageBox;
		}

		/// <summary>
		/// Stop application wide logging
		/// </summary>
		public static void StopLog()
		{
           
		}
 
		/// <summary>
		/// Log an information string into the system log.
		/// </summary>
		/// <param name="information">Information string to be logged</param>
		public static void Log(string information) 
		{
			Log(new object(), EventLogEntryType.Information ,information);
		}
		public static void Log(object sender, EventLogEntryType LogType, string information) 
		{
            
		}

		public static void Log(Exception exception) 
		{
			Log(exception,"");		
		}

		/// <summary>
		/// Log an exception into the system log.
		/// </summary>
		/// <param name="exception">Exception to be logged</param>
		public static void Log(Exception exception, string additional) 
		{
            
		}

	}
}
