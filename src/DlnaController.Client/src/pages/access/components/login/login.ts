import { Component } from '@angular/core';
import { IonicPage, NavController } from 'ionic-angular';

import { LoginService } from '../../services';

@IonicPage({
  name: 'access-login'
})
@Component({
  selector: 'page-login',
  templateUrl: 'login.html',
})
export class LoginPage {

  constructor(
    public navCtrl: NavController,
    private service: LoginService) {
  }

  ionViewDidLoad() {
    this.service.login();
  }

  register() {
    this.navCtrl.push('access-register');
  }
}
