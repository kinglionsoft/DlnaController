import { Injectable } from '@angular/core';
import { LoadingController, Loading } from 'ionic-angular';

@Injectable()
export class LoadingService {
    private loader: Loading;
    private isShow = false;

    constructor(public loadingCtrl: LoadingController) {
    }

    show() {
        if (this.isShow) return;
        this.loader = this.loadingCtrl.create({
            content: '请稍等...'
        });
        this.isShow = true;
        this.loader.present();
    }

    hide() {
        if (!this.isShow) return;
        this.isShow = false;
        this.loader.dismiss();
    }
}
