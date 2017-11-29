import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { map } from 'rxjs/operators/map';
import { Storage } from '@ionic/storage';
import { HttpClient } from './httpclient';
import { User } from 'kl/model';
import { Subject } from 'rxjs/Subject';

@Injectable()
export class AuthService {

  private _user: User;

  constructor(private http: HttpClient, private storage: Storage) { }

  /**
   * 登录
   *
   * @param {string} userName
   * @param {string} password
   * @returns {Observable<boolean>}
   *
   * @memberOf AuthService
   */
  login(userName: string, password: string): Observable<boolean> {
    return this.http.post<User>('', null, {
      UserName: userName,
      Password: password
    })
    .pipe(
      map(result => {
        if (result.Code >= 0 && result.Data) {
          this._user = result.Data;
          this.storage.set('user', result.Data).then(() => console.log('user info saved.'));
          return true;
        }
        return false;
      })
    );
  }

  /**
   * 获取当前登录用户
   *
   * @returns {Observable<User>}
   *
   * @memberOf AuthService
   */
  getUser(): Observable<User> {
    let emitter = new Subject<User>();
    if (this._user) {
      setTimeout(() => emitter.next(this._user), 0); // mac os 中直接next无法获取数据
    } else {
      this.storage.get('user').then(u => {
        this._user = u;
        emitter.next(u);
      });
    }
    return emitter.asObservable();
  }

  /**
   * 自动登录
   *
   * @returns {Observable<boolean>}
   *
   * @memberOf AuthService
   */
  autoLogin(): Observable<boolean> {
    let emitter = new Subject<boolean>();
    this.storage.get('user').then((u: User) => {
      if (!u) {
        setTimeout(() => emitter.next(false), 0);
      } else {
        this.login(u.Name, u.Password)
          .subscribe(res => emitter.next(res));
      }
    });
    return emitter.asObservable();
  }


  /**
   * 删除用户数据
   *
   * @memberOf AuthService
   */
  public delete() {
    this._user = new User();
    this.storage.remove('user');
  }

  /**
   * 更新用户
   *
   * @param {string} [key]
   * @param {*} [value]
   *
   * @memberOf AuthService
   */
  save(key?: string, value?: any) {
    if (key) {
      this._user[key] = value;
    }
    this.storage.set('user', this._user);
  }

}
