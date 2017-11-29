import { Injectable } from '@angular/core';
import { Platform, ToastController, Nav, Tab } from 'ionic-angular';
import { TabsPage } from '../../../pages/tabs/tabs';

declare let WeixinJSBridge: any;

@Injectable()
export class PlatformService {
  /**
   * 用于判断返回键是否触发
   *
   * @private
   * @type {boolean}
   * @memberOf PlatformService
   */
  private _backButtonPressed: boolean = false;

  private _rootNav: Nav;

  /**
   * 在首页加载完成后，设置根导航组件
   *
   * @memberOf PlatformService
   */
  public set rootNav(nav: Nav) {
    this._rootNav = nav;
  }

  constructor(
    private platform: Platform,
    private toastCtrl: ToastController
  ) { }

  /**
   * 注册返回键事件
   *
   * @memberOf PlatformService
   */
  registerBackButton() {
    if (this.platform.is('android')) {
      this._registerAndroidExit();
    } else if (this._isWechat()) {
      // 注册返回事件
      this._registerBrowserBack();
    }
  }

  /**
   * 根据UserAgent判断是否微信浏览器
   *
   *
   * @memberOf PlatformService
   */
  private _isWechat() {
    if (this.__isWechat === undefined) {
      let ua = navigator.userAgent.toLowerCase();
      this.__isWechat = /micromessenger/.test(ua);
    }
    return this.__isWechat;
  }
  private __isWechat: boolean = undefined;

  /**
   * 为安卓注册双击返回退出app
   *
   * @private
   *
   * @memberOf MyApp
   */
  private _registerAndroidExit() {
    this.platform.registerBackButtonAction((): any => this._onBackButtonClicked, 101);
  }

  /**
   * 注册浏览器返回事件
   */
  private _registerBrowserBack() {
    window.addEventListener('popstate', () => {
      this._onBackButtonClicked();
    }, false);
  }

  /**
   * 双击退出提示框，这里使用Ionic2的ToastController
   *
   * @private
   *
   * @memberOf MyApp
   */
  private _showExit() {
    if (this._backButtonPressed) {
      // 当触发标志为true时，即2秒内双击返回按键则退出APP
      if (this._isWechat()) {
        WeixinJSBridge.call('closeWindow');
      } else {
        this.platform.exitApp();
      }
    } else {
      let toast = this.toastCtrl.create({
        message: '再按一次退出应用',
        duration: 2000,
        position: 'bottom'
      });
      toast.present();
      this._backButtonPressed = true;
      // 2秒内没有再次点击返回则将触发标志标记为false
      setTimeout(() => {
        this._backButtonPressed = false;
      }, 2000);
    }
  }

  /**
   * 响应返回键点击事件
   *
   * @private
   *
   * @memberOf MyApp
   */
  private _onBackButtonClicked() {
    if (this._isWechat()) {
      if (this._pop) { // 阻止pop()方法引用多次返回
        this._pop = false;
        return;
      }
    }
    let activeVC = this._rootNav.getActive();
    let page = activeVC.instance;
    if (!(page instanceof TabsPage)) {
      if (!this._rootNav.canGoBack()) {
        // 当前页面为tabs，退出APP
        return this._showExit();
      }
      // 当前页面为tabs的子页面，正常返回
      return this._rootNav.pop();
    }
    // 获取主Tab组件
    let tabs = page.tabs;
    let activeNav = tabs.getSelected();
    if (!this._canGoBack(activeNav)) {
      // 当前页面为tab栏，退出APP
      return this._showExit();
    }
    // 当前页面为tab栏的子页面，正常返回
    return activeNav.pop();
  }

  // 解决微信模式下，根页面在页面栈中出现两次而无法返回的问题
  private _canGoBack(tab: Tab): boolean {
    if (!this._isWechat()) {
      return tab.canGoBack();
    }

    const active = tab.getActive();
    if (!active) return false;
    const activeNav = active.getNav();
    if (!activeNav) return false;
    const previous = tab.getPrevious();
    if (!previous) return false;
    if (previous.id === active.id) return false;
    return true;
  }

  private _pop = false;
  pop(nav) {
    this._pop = true;
    nav.pop();
  }
}
