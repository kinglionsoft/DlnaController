﻿using DlnaController.Abstractions;
using Microsoft.AspNetCore.Mvc;
using SV.UPnPLite.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UPnP.Service;
using Microsoft.Extensions.Logging;

namespace DlnaController.Web.Controllers
{
    public class MediaController: BaseController
    {
        private readonly UpnpManager _upnpManager;
        private readonly ILogger _logger;

        public MediaController(UpnpManager upnpManager, ILoggerFactory loggerFactory)
        {
            _upnpManager = upnpManager;
            _logger = loggerFactory.CreateLogger(nameof(MediaController));
        }

        public ApiResult<IList<MediaServer>> GetMediaServers()
        {
            return new ApiResult<IList<MediaServer>>(_upnpManager.GetOnlineMediaServers());
        }

        public ApiResult<IList<MediaRenderer>> GetMediaRenderers()
        {
            return new ApiResult<IList<MediaRenderer>>(_upnpManager.GetOnlineMediaRenderers());
        }

        /// <summary>
        /// Gets videos from the specific Media Server
        /// </summary>
        /// <param name="udn">udn of media server</param>
        /// <param name="cache">weather getting from cache</param>
        /// <returns></returns>
        public async Task<ApiResult<IEnumerable<VideoItem>>> GetVideos(string udn, bool cache)
        {
            var videos = await _upnpManager.GetVideosAsync(udn, cache);
            return new ApiResult<IEnumerable<VideoItem>>(videos);
        }

        /// <summary>
        /// Sends a video to the specific Media Renderer and starts to play
        /// </summary>
        /// <param name="rendererUdn"></param>
        /// <param name="mediaId"></param>
        /// <param name="mediaUdn"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult> Play(string rendererUdn, string mediaId, string mediaUdn)
        {
            try
            {
                await _upnpManager.PlayVideoAsync(rendererUdn, mediaId, mediaUdn);
                return new ApiResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"play failed, renderer={rendererUdn}, media server={mediaUdn}, media={mediaId}");
                return new ApiResult(-500, "播放失败");
            }
        }
    }
}
