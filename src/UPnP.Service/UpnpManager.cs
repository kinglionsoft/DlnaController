using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DlnaController.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SV.UPnPLite.Core;

namespace UPnP.Service
{
    public sealed class UpnpManager: IDisposable
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMemoryCache _cache;
        private readonly ILogger _logger;

        private readonly ConcurrentDictionary<string, MediaServer> _mediaServers;

        private readonly ConcurrentDictionary<string, MediaRenderer> _rendererServers;

        public UpnpManager(ILoggerFactory loggerFactory, IMemoryCache cache)
        {
            _loggerFactory = loggerFactory;
            _cache = cache;
            _logger = loggerFactory.CreateLogger(nameof(UpnpManager));

            _mediaServers = new ConcurrentDictionary<string, MediaServer>();
            _rendererServers = new ConcurrentDictionary<string, MediaRenderer>();
        }

        /// <summary>
        /// Starts
        /// </summary>
        public void Start()
        {
            _logger.LogInformation("UPNP manager is started");

            var mediaServersDiscovery = new MediaServersDiscovery(_loggerFactory);

            mediaServersDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Available).Subscribe(e =>
            {
                _logger.LogInformation($"Media Server found: {e.Device}");

                this._mediaServers.TryAdd(e.Device.UDN, e.Device);
            });

            mediaServersDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Gone).Subscribe(e =>
            {
                _logger.LogInformation($"Media Server gone: {e.Device}");
                this._mediaServers.TryRemove(e.Device.UDN, out var gone);
            });


            var mediaRenderersDiscovery = new MediaRenderersDiscovery(_loggerFactory);

            mediaRenderersDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Available).Subscribe(e =>
            {
                _logger.LogInformation($"Renderer Server found: {e.Device}");
                this._rendererServers.TryAdd(e.Device.UDN, e.Device);
            });

            mediaRenderersDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Gone).Subscribe(e =>
            {
                _logger.LogInformation($"Renderer Server gone: {e.Device}");
                this._rendererServers.TryRemove(e.Device.UDN, out var gone);
            });
        }

        #region Servers

        public IList<MediaServer> GetOnlineMediaServers()
        {
            return _mediaServers.Select(x => x.Value).ToList();
        }

        public IList<MediaRenderer> GetOnlineMediaRenderers()
        {
            return _rendererServers.Select(x => x.Value).ToList();
        }

        #endregion

        #region Medias
        
        public async Task<IEnumerable<TMedia>> GetMediasAsync<TMedia>(MediaServer mediaServer, bool cacheFirst = true) where TMedia: MediaItem
        {
            var key = AppConstants.GetMediasCacheKey(typeof(TMedia).Name, mediaServer.UDN);

            if (!cacheFirst || !_cache.TryGetValue<IEnumerable<TMedia>>(key, out var results))
            {
                results = await mediaServer.SearchAsync<TMedia>();
                _cache.Set(key, results);
            }

            return results;
        }
        public Task<IEnumerable<VideoItem>> GetVideosAsync(MediaServer mediaServer, bool cacheFirst = true)
        {
            return GetMediasAsync<VideoItem>(mediaServer, cacheFirst);
        }

        public Task<IEnumerable<VideoItem>> GetVideosAsync(string mediaServerUDN, bool cacheFirst = true)
        {
            if(_mediaServers.TryGetValue(mediaServerUDN, out var server))
            {
                return GetVideosAsync(server, cacheFirst);
            }
            _logger.LogWarning($"Could not find {mediaServerUDN} in found media servers.");
            throw new FriendlyException(-404, "媒体服务器不存在");
        }

        #endregion

        #region Remote Control
        
        public async Task PlayAsync(MediaRenderer renderer, MediaItem media)
        {
            await renderer.OpenAsync(media);
            await renderer.PlayAsync();
        }

        public Task PlayVideoAsync(string rendererUDN, string mediaId, string mediaServerUDN)
        {
            if (_rendererServers.TryGetValue(rendererUDN, out var renderer))
            {
                var key = AppConstants.GetMediasCacheKey(nameof(VideoItem), mediaServerUDN);
                if(_cache.TryGetValue<IEnumerable<VideoItem>>(key, out var videoItems))
                {
                    var media = videoItems.FirstOrDefault(x => x.Id == mediaId);
                    if (media != null)
                    {
                        return PlayAsync(renderer, media);
                    }
                }
                _logger.LogWarning($"Could not find {mediaId} in videos.");
            }
            _logger.LogWarning($"Could not find {rendererUDN} in found renderer servers.");
            throw new FriendlyException(-404, "视频不存在或者已删除");
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用


        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~UpnpManager()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(false);
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}