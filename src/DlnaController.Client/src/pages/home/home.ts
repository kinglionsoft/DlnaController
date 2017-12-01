import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams, ActionSheetController } from 'ionic-angular';
import { MediaService, Video, UpnpServer } from 'kl/media';
import { MessageBox } from 'kl/core';
import { finalize } from 'rxjs/operators/finalize';

@IonicPage({
  name: 'home'
})
@Component({
  selector: 'page-home',
  templateUrl: 'home.html',
})
export class HomePage {

  videos: Video[];
  mediaServers: UpnpServer[];
  selectedMediaServer: UpnpServer;

  constructor(public navCtrl: NavController,
    public navParams: NavParams,
    public actionSheetCtrl: ActionSheetController,
    private service: MediaService,
    private messageBox: MessageBox
  ) {
    this.videos = [];
    this.mediaServers = [];
    this.selectedMediaServer = this.service.getDefaulMediaServer();
  }

  ionViewDidLoad() {
    if (this.selectedMediaServer.UDN !== '') {
      this.getVidoes(true);
    }
  }

  doRefresh(refresher) {
    if (this.selectedMediaServer.UDN === '') {
      refresher.complete();
      return;
    }
    this.getVidoes(false, refresher);
  }

  getMediaServers() {
    this.mediaServers = [];
    this.service.getMediaServers()
      .subscribe(result => {
        if (result.Code >= 0) {
          if (result.Data && result.Data.length > 0) {
            this.mediaServers = result.Data;
            this.presentActionSheet();
          } else {
            this.messageBox.toast('没有获取到新设备');
          }
        } else {
          this.messageBox.toast(result.Message);
        }
      }, x => this.messageBox.toast(x));
  }

  selectMediaServer(server: UpnpServer) {
    if (this.selectedMediaServer.UDN === server.UDN) return;
    this.selectedMediaServer = server;
    this.service.saveDefaulMediaServer(server);
    this.getVidoes(true);
  }

  getVidoes(cache, refresher?) {
    this.service.getVideos(this.selectedMediaServer.UDN, cache)
      .pipe(finalize(() => {
        if (refresher) refresher.complete();
      }))
      .subscribe(result => {
        if (result.Code >= 0) {
          this.videos = result.Data;
        } else {
          this.messageBox.toast(result.Message);
        }
      }, x => this.messageBox.toast(x));
  }

  play(media: Video) {
    let renderer = this.service.getDefaulRendererServer();
    if (renderer.UDN === '') {
      this.messageBox.toast('请设置播放设备');
      return;
    }
    this.messageBox.confirm(`将【${media.Title}】发送到【${renderer.FriendlyName}】播放？`)
      .then(() => {
        this.service.play(renderer.UDN, media.Id, this.selectedMediaServer.UDN)
          .subscribe(r => {
            if (r.Code < 0) {
              this.messageBox.alert(r.Message);
            } else {
              this.messageBox.toast('发送成功');
            }
          }, x => this.messageBox.alert(x));
      });
  }

  private presentActionSheet() {
    let buttons = [];
    this.mediaServers.forEach(element => {
      buttons.push({
        text: element.FriendlyName,
        handler: () => {
          this.selectMediaServer(element);
        }
      });
    });
    buttons.push({
      text: '取消',
      role: 'cancel',
      handler: () => { }
    });

    let actionSheet = this.actionSheetCtrl.create({
      title: '请选择设备',
      buttons: buttons
    });
    actionSheet.present();
  }
}
