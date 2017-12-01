import { Injectable } from '@angular/core';
import { HttpClient, StorageService } from 'kl/core';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators/map';
import { of } from 'rxjs/Observable/of';
import { UpnpServer, Video } from './../models';
import { ApiResult } from 'kl/model';

@Injectable()
export class MediaService {

    constructor(private http: HttpClient, private storage: StorageService) { }

    private _mediaServers: UpnpServer[];
    /**
     * 获取在线的媒体服务器
     */
    getMediaServers(): Observable<ApiResult<UpnpServer[]>> {
        if (this._mediaServers) {
            return of({
                Code: 0,
                Message: 'ok',
                Data: this._mediaServers
            });
        }
        return this.http.get<UpnpServer[]>('/api/media/GetMediaServers')
            .pipe(
            map(r => {
                if (r.Code >= 0) {
                    this._mediaServers = r.Data;
                }
                return r;
            })
            );
    }

    private _rendererServers: UpnpServer[];
    /*
    * 获取在线播放服务器
    */
    getRendererServers(): Observable<ApiResult<UpnpServer[]>> {
        if (this._rendererServers) {
            return of({
                Code: 0,
                Message: 'ok',
                Data: this._rendererServers
            });
        }
        return this.http.get<UpnpServer[]>('/api/media/GetMediaRenderers')
            .pipe(
            map(r => {
                if (r.Code >= 0) {
                    this._rendererServers = r.Data;
                }
                return r;
            })
            );
    }

    /**
     * 获取媒体服务器上的视频
     * @param udn
     * @param cache
     */
    getVideos(udn: string, cache: boolean): Observable<ApiResult<Video[]>> {
        return this.http.get<Video[]>('/api/media/GetVideos', {
            udn: udn,
            cache: cache
        });
    }

    saveDefaulMediaServer(server: UpnpServer) {
        this.storage.set('_default_m_svr', server);
    }

    getDefaulMediaServer() {
        return this.storage.get<UpnpServer>('_default_m_svr') || new UpnpServer();
    }

    saveDefaulRendererServer(server: UpnpServer) {
        this.storage.set('_default_r_svr', server);
    }

    getDefaulRendererServer() {
        return this.storage.get<UpnpServer>('_default_r_svr') || new UpnpServer();
    }

    /**
     * 播放视频
     * @param rendererUdn
     * @param mediaId
     * @param mediaUdn
     */
    play(rendererUdn: string, mediaId: string, mediaUdn: string) {
        return this.http.post<ApiResult<any>>('/api/media/play', null, {
            rendererUdn: rendererUdn,
            mediaId: mediaId,
            mediaUdn: mediaUdn
        });
    }
}
