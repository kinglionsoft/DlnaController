using Microsoft.Extensions.Logging;

namespace SV.UPnPLite.Core
{
    public class PhotoAlbumContainer : AlbumContainer
	{
        #region Constructors

        /// <summary>
        ///		Initializes a new instance of the <see cref="PhotoAlbumContainer"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        ///		The log manager to use for logging.
        ///	</param>
        public PhotoAlbumContainer(ILoggerFactory loggerFactory)
			: base(loggerFactory)
		{
		}

		#endregion
	}
}
