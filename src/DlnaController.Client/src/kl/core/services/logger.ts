import { Injectable } from '@angular/core';
import { ToastController } from 'ionic-angular';

@Injectable()
export class Logger {

    constructor(private toastCtrl: ToastController) { }

    /**
     * 显示通知
     * @param msg 显示的内容
     * @param extro 将被保存到日志
     */
    info(msg: string, extro?: any) {
        this._show(msg, 'secondary');
        this.save(extro);
    }

    /**
     * 显示警告
     * @param msg 警告的内容
     * @param extro 将被保存到日志
     */
    warn(msg: string, extro?: any) {
        this._show(msg, 'danger');
        this.save(extro);
    }

    /**
     * 显示错误
     * @param msg 错误的内容
     * @param extro 将被保存到日志
     */
    error(msg: string, extro?: any) {
        this._show(msg, 'danger');
        this.save(extro);
    }

    /**
     * 保存日志
     * @param msg 错误的内容
     * @param extro 将被保存为本地日志
     */
    save(extro?: any) {
        if (!extro) return;
        console.log(JSON.stringify(extro));
    }

    private _show(msg: string, css: string) {
        let toast = this.toastCtrl.create({
            message: msg,
            duration: 3000,
            cssClass: css
        });

        toast.present();
    }
}

