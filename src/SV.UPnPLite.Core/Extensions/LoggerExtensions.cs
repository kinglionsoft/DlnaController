
using System.Linq;

namespace SV.UPnPLite.Extensions
{
	using SV.UPnPLite.Logging;
	using System;
	using System.Collections.Generic;

	/// <summary>
	///     Defines extension methods for <see cref="ILogger"/>.
	/// </summary>
	public static class LoggerExtensions
	{
		/// <summary>
		///     Ensures that instance exists. If it doesn't - the stub is returned.
		/// </summary>
		/// <param name="logger"></param>
		/// <returns></returns>
		public static ILogger Instance(this ILogger logger)
		{
			if (logger != null)
			{
				return logger;
			}
			else
			{
				return new StubLogger();
			}
		}

		internal class StubLogger : ILogger
		{
			public void Trace(string message, params KeyValuePair<string, object>[] parameters)
			{
			}

			public void Trace(Exception ex, string message, params KeyValuePair<string, object>[] parameters)
			{
			}

			public void Debug(string message, params KeyValuePair<string, object>[] parameters)
			{
			}

			public void Debug(Exception ex, string message, params KeyValuePair<string, object>[] parameters)
			{
			}

			public void Info(string message, params KeyValuePair<string, object>[] parameters)
			{
			}

			public void Info(Exception ex, string message, params KeyValuePair<string, object>[] parameters)
			{
			}

			public void Warning(string message, params KeyValuePair<string, object>[] parameters)
			{
                Console.WriteLine($"Warning: {message}, {string.Join(",", parameters?.Select(x=> $"{x.Key}={x.Value}"))}");
			}

			public void Warning(Exception ex, string message, params KeyValuePair<string, object>[] parameters)
			{
			    Console.WriteLine($"Warning: {message},{GetInnerError(ex)}, {string.Join(",", parameters?.Select(x => $"{x.Key}={x.Value}"))}");
            }

			public void Error(string message, params KeyValuePair<string, object>[] parameters)
			{
			}

			public void Error(Exception ex, string message, params KeyValuePair<string, object>[] parameters)
			{
			}

			public void Critical(string message, params KeyValuePair<string, object>[] parameters)
			{
			}

			public void Critical(Exception ex, string message, params KeyValuePair<string, object>[] parameters)
			{
			}

		    private static string GetInnerError(Exception ex)
		    {
		        var error = ex.Message;
		        var deep = 3;
		        var innerEx = ex.InnerException;
		        while (innerEx != null && deep > 0)
		        {
		            error += "-->" + innerEx.Message;
		            innerEx = innerEx.InnerException;
		            deep--;
		        }
		        return error;
		    }
        }
	}
}
