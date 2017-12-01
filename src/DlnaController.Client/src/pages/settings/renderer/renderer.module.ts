import { NgModule } from '@angular/core';
import { IonicPageModule } from 'ionic-angular';
import { RendererPage } from './renderer';

@NgModule({
  declarations: [
    RendererPage,
  ],
  imports: [
    IonicPageModule.forChild(RendererPage),
  ],
})
export class RendererPageModule {}
