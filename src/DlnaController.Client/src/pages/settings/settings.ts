import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams, Events } from 'ionic-angular';
import { UpnpServer, MediaService } from 'kl/media';

@IonicPage({
  name: 'settings'
})
@Component({
  selector: 'page-settings',
  templateUrl: 'settings.html',
})
export class SettingsPage {


  renderer: UpnpServer;

  constructor(public navCtrl: NavController,
    public navParams: NavParams,
    private service: MediaService,
    event: Events
  ) {
    this.renderer = this.service.getDefaulRendererServer();
    event.subscribe('settings-update', () => {
      this.ionViewDidLoad();
    });
  }

  ionViewDidLoad(): void {
    this.renderer = this.service.getDefaulRendererServer();
  }

  go(page) {
    this.navCtrl.push(page);
  }
}
