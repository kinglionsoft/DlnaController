using Microsoft.Extensions.Logging;

namespace SV.UPnPLite.Core
{
    public class PersonContainer : MediaContainer
	{
        #region Constructors

        /// <summary>
        ///		Initializes a new instance of the <see cref="PersonContainer"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        ///		The log manager to use for logging.
        ///	</param>
        public PersonContainer(ILoggerFactory loggerFactory)
			: base(loggerFactory)
		{
		}

		#endregion
	}
}
