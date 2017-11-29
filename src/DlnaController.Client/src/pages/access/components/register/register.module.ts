import { NgModule } from '@angular/core';
import { IonicPageModule } from 'ionic-angular';
import { RegisterPage } from './register';
import { AccessModule } from '../../access.module';

@NgModule({
  declarations: [
    RegisterPage,
  ],
  imports: [
    AccessModule,
    IonicPageModule.forChild(RegisterPage),
  ],
})
export class RegisterPageModule { }
