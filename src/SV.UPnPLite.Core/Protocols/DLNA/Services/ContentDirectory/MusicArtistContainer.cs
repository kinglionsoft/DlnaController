﻿using Microsoft.Extensions.Logging;

namespace SV.UPnPLite.Core
{
    public class MusicArtistContainer : PersonContainer
	{
        #region Constructors

        /// <summary>
        ///		Initializes a new instance of the <see cref="MusicArtistContainer"/> class.
        /// </summary>
        /// <param name="loggerFactory">
        ///		The log manager to use for logging.
        ///	</param>
        public MusicArtistContainer(ILoggerFactory loggerFactory)
			: base(loggerFactory)
		{
		}

		#endregion

		#region Properties

		/// <summary>
		///		Gets the reference to artist discography.
		/// </summary>
		public string ArtistDiscographyURI { get; private set; }

		/// <summary>
		///		Gets the genre of the artist.
		/// </summary>
		public string Genre { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		///     Sets a value read from an object's metadata XML.
		/// </summary>
		/// <param name="key">
		///     The key of the property read from XML.
		/// </param>
		/// <param name="value">
		///     The value of the property read from XML.
		/// </param>
		protected override void SetValue(string key, string value)
		{
			if (key.Is("artistDiscographyURI"))
			{
				this.ArtistDiscographyURI = value;
			}
			else if (key.Is("genre"))
			{
				this.Genre = value;
			}
			else
			{
				base.SetValue(key, value);
			}
		}

		#endregion
	}
}
