using System.Linq;
using Microsoft.Extensions.Logging;

namespace SV.UPnPLite.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>    
    ///     Defines members for controlling a UPnP device.
    /// </summary>
    public abstract class UPnPDevice
	{
		#region Fields

		protected readonly ILogger logger;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UPnPDevice"/> class.
        /// </summary>
        /// <param name="udn">
        ///     A universally-unique identifier for the device.
        /// </param>
        /// <param name="loggerFactory">
        ///     The <see cref="ILoggerFactory"/> to use for logging the debug information.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="udn"/> is <c>nukk</c> or <see cref="string.Empty"/>.
        /// </exception>
        protected UPnPDevice(string udn, ILoggerFactory loggerFactory)
		{
		    udn.EnsureNotNull("udn");

		    this.UDN = udn;
		    this.logger = loggerFactory.CreateLogger(this.GetType().Name);
            this.Icons = new DeviceIcon[0];
		}

		#endregion

		#region Properties

		/// <summary>
		///     Gets a type of the device.
		/// </summary>
		public string DeviceType { get; internal set; }

		/// <summary>
		///     GEts a version of the device.
		/// </summary>
		public UPnPVersion DeviceVersion { get; internal set; }

		/// <summary>
		///     Gets a name of the device.
		/// </summary>
		public string FriendlyName { get; internal set; }

		/// <summary>
		///     Gets a manufacturer's name of the device.
		/// </summary>
		public string Manufacturer { get; internal set; }

		/// <summary>
		///     Gets a universally-unique identifier for the device, whether root or embedded. Must be the same over time for a specific device instance (i.e., must survive reboots).
		/// </summary>
		public string UDN { get; private set; }

		/// <summary>
		///     Gets a network address of the device.
		/// </summary>
		public string Address { get; internal set; }

		/// <summary>
		///     Gets the list of icons to depict device in a UI.
		/// </summary>
		public IEnumerable<DeviceIcon> Icons { get; internal set; }

		internal TimeSpan MaxAge
		{
			get;
			set;
		}

		internal DateTime LastCheckTime
		{
			get;
			set;
		}

        /// <summary>
        /// Get the pretty icon
        /// </summary>
	    public string Icon
	    {
	        get
	        {
	            if (Icons.Any())
	            {
	                foreach (var type in new[] {"image/png", "image/jpeg"})
	                {
	                    var url = Icons.Where(x => x.Type == type).OrderByDescending(x => x.Size.Width).FirstOrDefault()
	                        ?.Url;
	                    if (url != null)
	                    {
	                        return url.ToString();
	                    }
	                }
	            }
	            return string.Empty;
	        }
	    }

		#endregion

	    public override string ToString()
	    {
	        return $"UDN={UDN}, Name={FriendlyName}, Address={Address}";
	    }
	}
}
