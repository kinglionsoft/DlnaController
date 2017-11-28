
namespace SV.UPnPLite.Core
{
	using System;

	public static class StringExtensions
	{
		public static string F(this string st, params object[] args)
		{
			return String.Format(st, args);
		}

		public static string[] SplitIntoLines(this string text)
		{
			return text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		}

		public static string[] SplitIntoLines(this string text, int maxNumberOfLines)
		{
			return text.Split(new char[] { '\r', '\n' }, maxNumberOfLines, StringSplitOptions.RemoveEmptyEntries);
		}

		public static bool ToBool(this string source)
		{
			return source != null && (string.Compare(source, "true", StringComparison.OrdinalIgnoreCase) == 0 || source == "1");
		}

	    /// <summary>
	    ///     Checks whether the string is euqal to <see cref="compareTo"/> with ignoring case.
	    /// </summary>
	    /// <param name="st">
	    ///     The original string.
	    /// </param>
	    /// <param name="compareTo">
	    ///     The string compare to.
	    /// </param>
	    /// <returns>
	    ///     <c>true</c>, if <paramref name="st"/> and <paramref name="compareTo"/> are equal; otherwise, <c>false</c>.
	    /// </returns>
	    public static bool Is(this string st, string compareTo)
	    {
	        return StringComparer.OrdinalIgnoreCase.Compare(st, compareTo) == 0;
	    }
    }
}
