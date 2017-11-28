﻿
namespace SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory
{
	using System;
	using SV.UPnPLite.Logging;
	using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory.Extensions;

	/// <summary>
	///     Represents a piece of content that, when rendered, generates some still image. It is atomic in the sense that it does not contain other objects in the ContentDirectory. 
	///     It typically has at least 1 <see cref="MediaObject.Resources"/> element.
	/// </summary>
	public class ImageItem : MediaItem
	{
		#region Properties

		/// <summary>
		///     Gets a short description of the media item. Description may include but is not limited to: an abstract, 
		///     a table of contents, a graphical representation, or a free-text account of the resource.
		/// </summary>
		public string Description { get; internal set; }

		/// <summary>
		///     Gets a few lines of description of the media item.
		/// </summary>
		public string LongDescription { get; internal set; }

		/// <summary>
		///     Gets an entity responsible for making the resource available. Examples of a Publisher include a person, an organization, or a service. 
		///     Typically, the name of a Publisher should be used to indicate the entity.
		/// </summary>
		public string Publisher { get; internal set; }

		/// <summary>
		///     Gets a rating of the object’s resource, for ‘parental control’ filtering purposes, such as “R”, “PG-13”, “X”, etc.,.
		/// </summary>
		public string Rating { get; internal set; }

		/// <summary>
		///     Gets an information about rights held in and over the resource.
		/// </summary>
		public string Rights { get; internal set; }

		/// <summary>
		///     Gets a type of storage medium used for the content. Potentially useful for user-interface purposes.
		/// </summary>
		public string StorageMedium { get; internal set; }

		/// <summary>
		///     Gets a date when the item was created.
		/// </summary>
		public DateTime Date { get; internal set; }

		/// <summary>
		///		Gets the reference to album art. 
		/// </summary>
		/// <remarks>
		///		Even though this property is not supported by DLNA protocol, some servers (Windows Media Server) put there a thumbnail of the image.
		/// </remarks>
		public string AlbumArtURI { get; private set; }

		#endregion

		#region Constructors

		/// <summary>
		///		Initializes a new instance of the <see cref="ImageItem"/> class.
		/// </summary>
		/// <param name="logManager">
		///		The log manager to use for logging.
		///	</param>
		public ImageItem(ILogManager logManager = null)
			: base(logManager)
		{
		}

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
		/// <returns>
		///     <c>true</c>, if the value was set; otherwise, <c>false</c>.
		/// </returns>
		protected override void SetValue(string key, string value)
		{
			if (key.Is("StorageMedium"))
			{
				this.StorageMedium = value;
			}
			else if (key.Is("longDescription"))
			{
				this.LongDescription = value;
			}
			else if (key.Is("rating"))
			{
				this.Rating = value;
			}
			else if (key.Is("description"))
			{
				this.Description = value;
			}
			else if (key.Is("publisher"))
			{
				this.Publisher = value;
			}
			else if (key.Is("date"))
			{
				this.Date = ParsingHelper.ParseDate(value);
			}
			else if (key.Is("rights"))
			{
				this.Rights = value;
			}
			else if (key.Is("albumArtURI"))
			{
				this.AlbumArtURI = value;
			}
			else
			{
				base.SetValue(key, value);
			}
		}

		#endregion
	}
}
