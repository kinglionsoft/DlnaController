namespace DlnaController.Domain
{
    public class UpnpServerDto
    {
        /// <summary>
        ///     Gets a type of the device.
        /// </summary>
        public string DeviceType { get; set; }
        
        /// <summary>
        ///     Gets a name of the device.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        ///     Gets a manufacturer's name of the device.
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        ///     Gets a universally-unique identifier for the device, whether root or embedded. Must be the same over time for a specific device instance (i.e., must survive reboots).
        /// </summary>
        public string UDN { get; set; }

        /// <summary>
        ///     Gets a network address of the device.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        ///     Gets the list of icons to depict device in a UI.
        /// </summary>
        public string Icon { get; set; }
    }
}