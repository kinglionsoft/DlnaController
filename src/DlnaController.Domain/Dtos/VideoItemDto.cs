using System;

namespace DlnaController.Domain
{
    public class VideoItemDto
    {
        /// <summary>
        ///     Gets an identifier for the object. The value of each object id property must be unique with respect to the Content Directory.
        /// </summary>
        public string Id { get; set; }

        public string Title { get; set; }

        /// <summary>
        ///     Gets an id property of object’s parent. The parentID of the Content Directory ‘root’ container must be set to the reserved value of  “-1”.  No other 
        ///     parentID attribute of any other Content Directory object may take this value. 
        /// </summary>
        public string ParentId { get; internal set; }

        /// <summary>
		///     Gets a genre of the media item.
		/// </summary>
		public string Genre { get; set; }

        /// <summary>
        ///     Gets a short description of the media item. Description may include but is not limited to: an abstract, 
        ///     a table of contents, a graphical representation, or a free-text account of the resource.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets an entity responsible for making the resource available. Examples of a Publisher include a person, an organization, or a service. 
        ///     Typically, the name of a Publisher should be used to indicate the entity.
        /// </summary>
        public string Publisher { get; set; }

        /// <summary>
        ///     Gets a name of producer of e.g., a movie or CD.
        /// </summary>
        public string Producer { get; set; }

        /// <summary>
        ///     Gets a rating of the object’s resource, for ‘parental control’ filtering purposes, such as “R”, “PG-13”, “X”, etc.,.
        /// </summary>
        public string Rating { get; set; }

        /// <summary>
        ///     Gets a name of an actor appearing in a video item.
        /// </summary>
        public string Actor { get; set; }

        /// <summary>
        ///     Gets a name of the director of the video content item (e.g., the movie).
        /// </summary>
        public string Director { get; set; }

        /// <summary>
        ///     Gets a language of the media item.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        ///     Gets a URL to a album cover.
        /// </summary>
        public string AlbumArtUri { get; set; }

        /// <summary>
        ///     Gets size in bytes of the resource.
        /// </summary>
        public string Size { get; set; }

        /// <summary>
        ///     Gets a time duration of the playback of the resource, at normal speed.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        ///     Gets resolution of the resource in pixels (typically image item or video item).
        /// </summary>
        public string Resolution { get; set; }
    }
}