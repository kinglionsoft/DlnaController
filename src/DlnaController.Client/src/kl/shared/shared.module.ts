import { NgModule } from '@angular/core';
import { CameraDirective } from './directives/camera.directive';
import { LocalDatePipe } from './pipes/localdate.pipe';
import { DatePipe } from '@angular/common';

const _PROVIDDERS = [
  DatePipe,
  LocalDatePipe
];

const _DECLARATIONS = [
  CameraDirective,
  LocalDatePipe
];

@NgModule({
  imports: [],
  exports: [
    ..._DECLARATIONS
  ],
  declarations: [
    ..._DECLARATIONS
  ],
  providers: [
    ..._PROVIDDERS
  ],
})
export class KlSharedModule { }
