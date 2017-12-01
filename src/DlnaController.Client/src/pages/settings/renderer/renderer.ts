import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams, Events } from 'ionic-angular';
import { UpnpServer, MediaService } from 'kl/media';
import { MessageBox } from 'kl/core';


@IonicPage({ name: 'renderer' })
@Component({
  selector: 'page-renderer',
  templateUrl: 'renderer.html',
})
export class RendererPage {

  defaultRenderer: UpnpServer;
  renderers: UpnpServer[];

  constructor(public navCtrl: NavController,
    public navParams: NavParams,
    private service: MediaService,
    private messageBox: MessageBox,
    private events: Events
  ) {
    this.defaultRenderer = new UpnpServer();
    this.renderers = [];
  }

  ionViewDidLoad() {
    this.service.getRendererServers()
      .subscribe(r => {
        if (r.Code >= 0) {
          if (r.Data && r.Data.length > 0) {
            this.renderers = r.Data;
            let saved = this.service.getDefaulRendererServer();
            if (saved.UDN === '') return;
            for (let i = 0; i < this.renderers.length; i++) {
              if (saved.UDN === this.renderers[i].UDN) {
                this.defaultRenderer = this.renderers[i];
                break;
              }
            }
          }
        } else {
          this.messageBox.alert(r.Message);
        }
      }, x => this.messageBox.toast(x));
  }

  select(server: UpnpServer) {
    this.defaultRenderer = server;
    this.service.saveDefaulRendererServer(server);
    this.events.publish('settings-update');
  }
}
