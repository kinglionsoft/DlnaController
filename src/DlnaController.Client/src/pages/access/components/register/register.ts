import { Component } from '@angular/core';
import { IonicPage, NavController, NavParams } from 'ionic-angular';

import { LoginService } from '../../services';

@IonicPage({
  name: 'access-register'
})
@Component({
  selector: 'page-register',
  templateUrl: 'register.html',
})
export class RegisterPage {

  constructor(public navCtrl: NavController, public navParams: NavParams, private service: LoginService) {
  }

  ionViewDidLoad() {
    this.service.register();
  }

  back() {
    this.navCtrl.pop();
  }
}
