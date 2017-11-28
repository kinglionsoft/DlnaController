﻿
using System.Net.Http;

namespace SV.UPnPLite.Protocols.UPnP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using SV.UPnPLite.Extensions;
    using SV.UPnPLite.Logging;

    /// <summary>
    ///     The base class for all UPnP device's services.
    /// </summary>
    public class UPnPService
    {
        #region Constants

        private const int MaxConcurentRequests = 10;

        #endregion

        #region Fields

        protected readonly ILogger logger;

        private static readonly XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
        {
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
        };

        private readonly Uri controlUri;

        private readonly Uri eventsUri;

        private readonly Queue<RequestInfo> pendingRequests = new Queue<RequestInfo>();

        private readonly List<RequestInfo> executingRequests = new List<RequestInfo>();

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UPnPService" /> class.
        /// </summary>
        /// <param name="serviceType">
        ///     A type of the service.
        /// </param>
        /// <param name="controlUri">
        ///     An URL for sending commands to the service.
        /// </param>
        /// <param name="eventsUri">
        ///     An URL for subscrinbing to service's events.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="serviceType"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
        ///     <paramref name="controlUri"/> is <c>null</c> -OR-
        ///     <paramref name="eventsUri"/> is <c>null</c>.
        /// </exception>
        public UPnPService(string serviceType, Uri controlUri, Uri eventsUri)
        {
            this.ServiceType = serviceType;
            this.controlUri = controlUri;
            this.eventsUri = eventsUri;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UPnPService" /> class.
        /// </summary>
        /// <param name="serviceType">
        ///     A type of the service.
        /// </param>
        /// <param name="controlUri">
        ///     An URL for sending commands to the service.
        /// </param>
        /// <param name="eventsUri">
        ///     An URL for subscrinbing to service's events.
        /// </param>
        /// <param name="logManager">
        ///     The <see cref="ILogManager"/> to use for logging the debug information.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="serviceType"/> is <c>null</c> or <see cref="string.Empty"/> -OR-
        ///     <paramref name="controlUri"/> is <c>null</c> -OR-
        ///     <paramref name="eventsUri"/> is <c>null</c>.
        /// </exception>
        public UPnPService(string serviceType, Uri controlUri, Uri eventsUri, ILogManager logManager)
            : this(serviceType, controlUri, eventsUri)
        {
            this.LogManager = logManager;

            if (logManager != null)
            {
                this.logger = logManager.GetLogger(this.GetType());
            }
            else
            {
                this.logger = logger.Instance();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets a type of the service.
        /// </summary>
        public string ServiceType { get; internal set; }

        protected ILogManager LogManager { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Invokes an <paramref name="action"/> at the device's service.
        /// </summary>
        /// <param name="action">
        ///     An action to invoke.
        /// </param>
        /// <returns>
        ///     A dictionary with result of the action.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     An <see cref="action"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="WebException">
        ///     An error occurred when sending request to service.
        /// </exception>
        /// <exception cref="FormatException">
        ///     Received result is in a bad format.
        /// </exception>
        /// <exception cref="UPnPServiceException">
        ///     An internal service error occurred when executing request.
        /// </exception>
        public async Task<Dictionary<string, string>> InvokeActionAsync(string action)
        {
            return await this.InvokeActionAsync(action, null);
        }

        /// <summary>
        ///     Invokes an <paramref name="action"/> at the device's service.
        /// </summary>
        /// <param name="action">
        ///     An action to invoke.
        /// </param>
        /// <param name="parameters">
        ///     Invocation parameters.
        /// </param>
        /// <returns>
        ///     A dictionary with result of the action.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     An <see cref="action"/> is <c>null</c> or empty.
        /// </exception>
        /// <exception cref="WebException">
        ///     An error occurred when sending request to service.
        /// </exception>
        /// <exception cref="FormatException">
        ///     Received result is in a bad format.
        /// </exception>
        /// <exception cref="UPnPServiceException">
        ///     An internal service error occurred when executing request.
        /// </exception>
        public Task<Dictionary<string, string>> InvokeActionAsync(string action, Dictionary<string, object> parameters)
        {
            var requestInfo = new RequestInfo
            {
                Action = action,
                Arguments = parameters,
                CompletionSource = new TaskCompletionSource<Dictionary<string, string>>()
            };

            return ProcessRequestAsync(requestInfo);
        }
        
        private async Task<Dictionary<string, string>> ProcessRequestAsync(RequestInfo requestInfo)
        {
            try
            {
                logger.Instance().Trace("Request has been sent: {0}?{1}".F(this.controlUri, requestInfo.Action),
                    requestInfo.Arguments?.ToArray());

                var xml = this.CreateActionRequest(requestInfo.Action, requestInfo.Arguments);

                var headers = new Dictionary<string, string>()
                {
                    {"SOAPACTION", "\"{0}#{1}\"".F(this.ServiceType, requestInfo.Action)}
                };

                var responseStream = await HttpClientHelper.PostXmlAsync(this.controlUri, xml, headers);
                using (responseStream)
                {
                    var result = this.ParseActionResponse(requestInfo.Action, responseStream);
                    return result;
                }

            }
            catch (HttpRequestException ex)
            {
               
                throw ex;
            }
            catch (XmlException ex)
            {
                throw new FormatException(
                    "An error occurred when parsing response for an action '{0}'".F(requestInfo.Action),
                    ex);
            }
        }

        private string CreateActionRequest(string action, Dictionary<string, object> parameters)
        {
            var u = XNamespace.Get(this.ServiceType);
            var encodingStyle = XNamespace.Get("http://schemas.xmlsoap.org/soap/encoding/");

            var actionElement = new XElement(u + action, new XAttribute(XNamespace.Xmlns + "u", u.NamespaceName));

            if (parameters != null && parameters.Any())
            {
                foreach (var parameter in parameters)
                {
                    actionElement.Add(new XElement(parameter.Key, parameter.Value));
                }
            }

            var envelope = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(Namespaces.Envelope + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "s", Namespaces.Envelope.NamespaceName),
                    new XAttribute(Namespaces.Envelope + "encodingStyle", encodingStyle.NamespaceName),
                    new XElement(Namespaces.Envelope + "Body", actionElement)));

            var stEnvelope = envelope.ToStringWithDeclaration();
            return stEnvelope;
        }

        public Dictionary<string, string> ParseActionResponse(string action, Stream response)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (var reader = XmlReader.Create(response, xmlReaderSettings))
            {
                if (reader.ReadToDescendant("{0}Response".F(action), this.ServiceType))
                {
                    // Moving to a first parameter
                    reader.Read();

                    while (!reader.EOF)
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            result[reader.LocalName] = reader.ReadElementContentAsString();
                        }
                        else
                        {
                            reader.Read();
                        }
                    }
                }
                else
                {
                    throw new XmlException("The '{0}Response' element is missing".F(action));
                }
            }

            return result;
        }

        private UPnPServiceException ParseActionError(string action, Stream response, Exception actualException)
        {
            UPnPServiceException exception = null;

            try
            {
                var reader = XmlReader.Create(response, xmlReaderSettings);

                if (reader.ReadToDescendant("UPnPError", Namespaces.Control.NamespaceName))
                {
                    // Moving to a first parameter
                    reader.Read();

                    int? errorCode = null;
                    string errorDescription = null;

                    while (!reader.EOF)
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (StringComparer.OrdinalIgnoreCase.Compare(reader.LocalName, "errorCode") == 0)
                            {
                                errorCode = reader.ReadElementContentAsInt();
                            }
                            else if (StringComparer.OrdinalIgnoreCase.Compare(reader.LocalName, "errorDescription") == 0)
                            {
                                errorDescription = reader.ReadElementContentAsString();
                            }
                            else
                            {
                                reader.Skip();
                            }
                        }
                        else
                        {
                            reader.Read();
                        }
                    }

                    if (errorCode.HasValue)
                    {
                        exception = new UPnPServiceException(errorCode.Value, errorDescription, actualException);
                    }
                    else
                    {
                        this.logger.Instance().Warning("Can't parse '{0}' response with error. The 'errorCode' element is missing".F(action));
                    }
                }
                else
                {
                    this.logger.Instance().Warning("Can't parse '{0}' response with error. The 'UPnPError' element is missing".F(action));
                }
            }
            catch (XmlException ex)
            {
                this.logger.Instance().Warning(ex, "An error occurred when parsing '{0}' response with error".F(action));
            }

            return exception;
        }

        #endregion

        #region Types

        /// <summary>
        ///     Defines some standard XML namespaces.
        /// </summary>
        protected static class Namespaces
        {
            public static XNamespace Control = XNamespace.Get("urn:schemas-upnp-org:control-1-0");

            public static XNamespace Envelope = XNamespace.Get("http://schemas.xmlsoap.org/soap/envelope/");
        }

        private class RequestInfo
        {
            public string Action { get; set; }

            public Dictionary<string, object> Arguments { get; set; }

            public TaskCompletionSource<Dictionary<string, string>> CompletionSource { get; set; }
        }

        #endregion
    }
}
