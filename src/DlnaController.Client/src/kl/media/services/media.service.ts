import { Injectable } from '@angular/core';
import { HttpClient } from 'kl/core';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators/map';
import { of } from 'rxjs/Observable/of';
import { UpnpServer, Video } from './../models';
import { ApiResult } from 'kl/model';

@Injectable()
export class MediaService {

    constructor(private http: HttpClient) { }

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
}
