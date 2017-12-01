import { Injectable } from '@angular/core';
import { AlertController, AlertOptions, ToastController } from 'ionic-angular';

@Injectable()
export class MessageBox {

    constructor(private alertCtrl: AlertController, private toastCtrl: ToastController) { }

    /**
     * Toast提示框
     */
    toast(msg: string, css: string = '') {
        let toast = this.toastCtrl.create({
            message: msg,
            duration: 3000,
            cssClass: css
        });

        toast.present();
    }

    /** 提示框 */
    alert(msg: string) {
        let options: AlertOptions = {
            title: '',
            subTitle: msg,
            buttons: [
                {
                    text: '',
                },
                {
                    text: '确定',
                }
            ]
        };
        this._present(options);
    }

    /**
     * 确认框
     */
    confirm(
        msg: string,
        okText = '确定',
        cancelText = '取消'
    ): Promise<any> {
        return new Promise((resolve, reject) => {
            let options: AlertOptions = {
                title: '',
                message: msg,
                buttons: [
                    {
                        text: cancelText,
                        role: 'cancel',
                        handler: () => { }
                    },
                    {
                        text: okText,
                        role: 'ok',
                        handler: () => resolve()
                    }
                ]
            };
            this._present(options);
        });
    }

    /**
     * 输入框
     */
    prompt(
        msg: string,
        inputName: string,
        inputPlaceholer: string,
        onOk: (data: any) => void,
        onCancel = (data: any) => { },
        okText = '确定',
        cancelText = ''
    ) {
        this._present({
            title: '请输入',
            subTitle: msg,
            inputs: [
                {
                    name: inputName,
                    placeholder: inputPlaceholer
                }
            ],
            buttons: [
                {
                    text: cancelText,
                    role: 'cancel',
                    handler: onCancel
                },
                {
                    text: okText,
                    role: 'ok',
                    handler: onOk
                }
            ]
        });
    }

    private _present(options: AlertOptions) {
        options.cssClass = 'messagebox';
        let alert = this.alertCtrl.create(options);
        alert.present();
    }
}
