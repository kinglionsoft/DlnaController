import { MediaServer } from './../models';
import { ApiResult } from 'kl/model';
import { Injectable } from '@angular/core';
import { HttpClient } from 'kl/core';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators/map';
import { of } from 'rxjs/Observable/of';

@Injectable()
export class MediaService {

    constructor(private http: HttpClient) { }

    private _mediaServers: MediaServer[];
    /**
     * 获取在线的媒体服务器
     */
    getMediaServers(): Observable<ApiResult<MediaServer[]>> {
        if (this._mediaServers) {
            return of( {
                Code: 0,
                Message: 'ok',
                Data: this._mediaServers
            });
        }
        return this.http.get<MediaServer[]>('/api/media/GetMediaServers')
            .pipe(
                map(r => {
                    if (r.Code >= 0) {
                        this._mediaServers = r.Data;
                    }
                    return r;
                })
            );
    }
}
