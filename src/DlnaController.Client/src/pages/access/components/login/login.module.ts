import { NgModule } from '@angular/core';
import { IonicPageModule } from 'ionic-angular';
import { LoginPage } from './login';
import { AccessModule } from '../../access.module';

@NgModule({
  declarations: [
    LoginPage,
  ],
  imports: [
    AccessModule,
    IonicPageModule.forChild(LoginPage),
  ],
})
export class LoginPageModule { }
