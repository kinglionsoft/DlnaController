import { finalize } from 'rxjs/operators/finalize';
import { Injectable } from '@angular/core';
import { Http, Headers, Response, Request, RequestMethod, RequestOptions } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import * as moment from 'moment';
import { LoadingService } from './loading.service';
import { ApiResult } from 'kl/model';
import { map } from 'rxjs/operators/map';
import { catchError } from 'rxjs/operators/catchError';
import { Config } from 'kl/config';

@Injectable()
export class HttpClient {

  constructor(private http: Http, private loading: LoadingService) { }

  get<T>(url: string, params?: { [key: string]: string }, showLoading?: boolean): Observable<ApiResult<T>> {
    return this.request(url + this._formatUrl(params), RequestMethod.Get, null, showLoading);
  }

  post<T>(url: string, body?: any, params?: { [key: string]: any }, showLoading?: boolean): Observable<ApiResult<T>> {
    if (params) {
      url += this._formatUrl(params);
    }
    return this.request(url, RequestMethod.Post, body, showLoading);
  }

  request<T>(url: string, method: RequestMethod, body?: any, showLoading?: boolean): Observable<ApiResult<T>> {
    let headers = new Headers();

    let options = new RequestOptions();
    options.headers = headers;
    options.url = Config.host + url;
    options.method = method;
    options.body = body;
    options.withCredentials = true;

    let request = new Request(options);

    if (showLoading !== false) this.loading.show();

    return this.http.request(request)
      .pipe(
        map(r => r.json() as ApiResult<T>),
        finalize(() => {
          if (showLoading !== false) this.loading.hide();
        }),
        catchError(x => this.handleError(x))
      );
  }

  /**
   * 将字典转为QueryString
   */
  private _formatUrl(params?: { [key: string]: string }): string {
    if (!params) return '';

    let fegment = [];
    for (let k in params) {
      let v: any = params[k];
      if (v instanceof Date) {
        v = moment(v).format('YYYY-MM-DD HH:mm:SS');
      }
      fegment.push(`${k}=${v}`);
    }
    return '?' + fegment.join('&');
  }

  /**
  * 通用异常处理
  */
  private handleError(error: Response | any) {
    let errMsg: string;
    if (error instanceof Response) {
      const body = error.json() || '';
      const err = body.error || JSON.stringify(body);
      errMsg = `${error.status} - ${error.statusText || ''} ${err}`;
    } else {
      errMsg = error ? error.toString() : '服务器发生异常，请稍后再试';
    }
    return Observable.throw(errMsg);
  }
}
