using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Microsoft.Extensions.Logging;

namespace SV.UPnPLite.Core
{
    /// <summary>
    ///     A Desktop implementation of the SSDP protocol.
    /// </summary>
    internal class SSDPServer : ISSDPServer, IDisposable
	{
		#region Constants

		private const string MulticastAddress = "239.255.255.250";

		private const int MulticastPort = 1900;

		private const string MSearchRequestFormattedString =
			"M-SEARCH * HTTP/1.1" + "\r\n" +
			"HOST: 239.255.255.250:1900" + "\r\n" +
			"MAN: \"ssdp:discover\"" + "\r\n" +
			"ST: {0}" + "\r\n" +
			"MX: {1}" + "\r\n" +
			"Content-Length: 0" + "\r\n\r\n";

		#endregion

		#region Fields

		private static readonly object instanceSyncObject = new object();

		private static ISSDPServer instance;

		private static ILogger logger;

		private readonly Subject<NotifyMessage> notifyMessages = new Subject<NotifyMessage>();

		private UdpClient server;

		private bool isDisposed;

        private readonly string[] _localIps;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SSDPServer" /> class.
        /// </summary>
        /// <param name="logManager">
        ///     The <see cref="ILoggerFactory"/> to use for logging the debug information
        /// </param>
        private SSDPServer(ILoggerFactory logManager, string[] localIps)
		{
		    logger = logManager.CreateLogger(nameof(SSDPServer));
            _localIps = localIps;

            this.StartNotificationsListening();
        }

        #endregion

        #region Properties

        /// <summary>
        ///     An observable collection which contains notifications from devices.
        /// </summary>
        public IObservable<NotifyMessage> NotifyMessages { get { return this.notifyMessages; } }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets a singletone instance of the <see cref="SSDPServer"/>.
        /// </summary>
        /// <param name="loggerFactory">
        ///     The <see cref="ILoggerFactory"/> to use for logging the debug information.
        /// </param>
        /// <returns>
        ///     A singletone instance of the <see cref="SSDPServer"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="logManager"/> is <c>null</c>.
        /// </exception>
        public static ISSDPServer GetInstance(ILoggerFactory loggerFactory, string[] localIps = null)
		{
			lock (instanceSyncObject)
			{
				if (instance == null)
				{
					instance = new SSDPServer(loggerFactory, localIps);
				}
			}

			return instance;
		}

        private IEnumerable<IPEndPoint> GetLocalEndPoints()
        {
            if(_localIps == null)
            {
                return GetLocalAddresses()
                        .Where(a => a.AddressFamily == AddressFamily.InterNetwork)
                        .Select(a => new IPEndPoint(a, 0));
            }

            return _localIps.Select(a => new IPEndPoint(IPAddress.Parse(a), 0));
        }

		/// <summary>
		///     Searches for an available devices of specified type.
		/// </summary>
		/// <param name="searchTarget">
		///     The type of the devices to search for.
		/// </param>
		/// <param name="timeForResponse">
		///     The time (in seconds) of a search.
		/// </param>
		/// <returns>
		///     An observable collection which contains search results.
		/// </returns>
		public IObservable<SearchResponseMessage> Search(string searchTarget, int timeForResponse)
		{
			return Observable.Create<SearchResponseMessage>(
				observer =>
				{
					var clients = new List<IDisposable>();

                    var ipV4LocalEndpoints = GetLocalEndPoints();

                    foreach (var localEndPoint in ipV4LocalEndpoints)
					{
						var responses = Search(localEndPoint, searchTarget, timeForResponse);
						var subscription = responses.Subscribe(observer);

						clients.Add(subscription);
					}

					return () => clients.ForEach(c => c.Dispose());
				});
		}

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
		}

		/// <summary>
		///     Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="isDisposing">
		///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
		/// </param>
		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposed == false)
			{
				if (isDisposing)
				{
					this.server.Close();
				}

				isDisposed = true;
			}
		}

		private static IObservable<SearchResponseMessage> Search(IPEndPoint localEndPoint, string searchTarget, int timeForResponse)
		{
			return Observable.Create<SearchResponseMessage>(
				observer =>
				{
					var multicastEndPoint = new IPEndPoint(IPAddress.Parse(MulticastAddress), MulticastPort);
					var request = MSearchRequestFormattedString.F(searchTarget, timeForResponse);
					var buffer = Encoding.UTF8.GetBytes(request);

					UdpClient searchClient = null;
					IDisposable timerSubscription = null;

					try
					{
						searchClient = new UdpClient(localEndPoint);

						// Stop listening for a devices when timeout for responses is expired
						timerSubscription = Observable.Timer(TimeSpan.FromSeconds(timeForResponse)).Subscribe(s =>
							{
								searchClient?.Close();
								observer.OnCompleted();
							});

                        var responses = GetIncommingMessagesSequence(searchClient);
                        responses.Subscribe(message => HandleSearchResponseMessage(message, observer));

                        searchClient.SendAsync(buffer, buffer.Length, multicastEndPoint);

                        logger.LogDebug("Sent M-Search request from local endpoint '{0}' to a multicast endpoint '{1}' with search target '{2}'.".F(localEndPoint, multicastEndPoint, searchTarget));
					}
					catch (SocketException ex)
					{
						logger.LogWarning(ex, "Failed to send M-Search request from local endpoint '{0}' to a multicast endpoint '{1}'.".F(localEndPoint, multicastEndPoint));
					}
					catch (ObjectDisposedException)
					{
						// Can happen when subscription is canceled before the search message is sent. No handling is required.
					}

					return () =>
						{
						    timerSubscription?.Dispose();

						    searchClient?.Close();
						};
				});
		}

		private static IObservable<string> GetIncommingMessagesSequence(UdpClient socket)
		{
            var searchStream = Observable
                                // cause high usage of cpu
                                // .Create<UdpReceiveResult>(obs => Observable.FromAsync(socket.ReceiveAsync).Subscribe(obs))
                                .Create<UdpReceiveResult>(obs => Observable.FromAsync(socket.ReceiveAsync).Subscribe(obs.OnNext))
                                .Repeat()
                                .Retry()
                                .Publish()
                                .RefCount();
            
            var searchHttpMessages = from receiveResult in searchStream
									 select Encoding.UTF8.GetString(receiveResult.Buffer);

			return searchHttpMessages;
		}

		private static IEnumerable<IPAddress> GetLocalAddresses()
		{
			var result = new List<IPAddress>();

			var interfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (var networkInterface in interfaces)
			{
				if (networkInterface.IsReceiveOnly == false && networkInterface.OperationalStatus == OperationalStatus.Up && networkInterface.SupportsMulticast)
				{
					var ipProperties = networkInterface.GetIPProperties();
					foreach (var address in ipProperties.UnicastAddresses)
					{
						if (result.Contains(address.Address) == false && address.Address.Equals(IPAddress.IPv6Loopback) == false)
						{
							result.Add(address.Address);
						}
					}
				}
			}

			return result;
		}

		private static void HandleSearchResponseMessage(string message, IObserver<SearchResponseMessage> observer)
		{
			logger.LogTrace("Received search response message", "Message".As(message));

			try
			{
				var responseMessage = SearchResponseMessage.Create(message);
				observer.OnNext(responseMessage);
			}
			catch (ArgumentException ex)
			{
				logger.LogWarning(ex, "The received M-Search response has been ignored.", "Message".As(message));
			}
		}

		private static void HandleNotifyMessage(string message, IObserver<NotifyMessage> observer)
		{
			logger.LogTrace("Received notification message", "Message".As(message));

			try
			{
				var notifyMessage = NotifyMessage.Create(message);
				observer.OnNext(notifyMessage);
			}
			catch (ArgumentException ex)
			{
				logger.LogWarning(ex, "The received notification message has been discarded.", "Message".As(message));
			}
		}

		private void StartNotificationsListening()
		{
            var localEndPoint = new IPEndPoint(IPAddress.Any, 1900);

            try
            {
                this.server = new UdpClient();
                this.server.ExclusiveAddressUse = false;
                this.server.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                this.server.Client.Bind(localEndPoint);
                this.server.JoinMulticastGroup(IPAddress.Parse(MulticastAddress));

                var incommingMessages = GetIncommingMessagesSequence(this.server);
                incommingMessages.Subscribe(message => HandleNotifyMessage(message, this.notifyMessages));

                logger.LogInformation("Started listening for notification messages at {0}".F(localEndPoint));
            }
            catch (SocketException ex)
            {
                logger.LogError(ex, "Can't bind to a {0} for listening for notification messages".F(localEndPoint));
            }
        }

		#endregion
	}
}
