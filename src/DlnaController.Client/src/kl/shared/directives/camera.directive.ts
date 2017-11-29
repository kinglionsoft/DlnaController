import { Directive, Input, Output, HostListener, EventEmitter } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';
import { Camera, CameraOptions } from '@ionic-native/camera';
import { ActionSheetController } from 'ionic-angular';
/**
 * 拍照或者从相册中选取一张图片
 * 用法：<span [camera]="options" (completed)="onCameraCompleted($event)"><span>
 *
 * 问题：
 * 1. 安卓机上，completed的绑定方法里设置页面的img的src绑定属性无效，暂时使用js赋值的方式解决
 * onCameraCompleted(fileUrl:string) {
 *  (document.getElementById('img_test') as any).src=fileUrl;
 * }
 */
@Directive({ selector: '[camera]' })
export class CameraDirective {

  private _default: CameraOptions;

  private _options: CameraOptions;

  constructor(
    private actionSheetCtrl: ActionSheetController,
    private cameraPlugin: Camera
  ) {
    this._default = {
      quality: 100,
      destinationType: this.cameraPlugin.DestinationType.FILE_URI,
      sourceType: this.cameraPlugin.PictureSourceType.CAMERA,
      allowEdit: true,
      targetWidth: 100,
      targetHeight: 100,
      mediaType: this.cameraPlugin.MediaType.PICTURE,
      correctOrientation: true,
      saveToPhotoAlbum: false,
      cameraDirection: this.cameraPlugin.Direction.BACK
    };
  }

  @Output() completed = new EventEmitter<string>();

  @Input()
  set camera(options: CameraOptions) {
    this._options = this._extend(options, this._default);
  }

  @HostListener('click')
  onClick() {
    this._presentActionSheet()
      .subscribe(
      type => {
        this._options.sourceType = type;
        this.cameraPlugin.getPicture(this._options).then((imageData) => {
          this.completed.emit(imageData);
        }, (err) => {
          console.error('获取图片失败:' + JSON.stringify(err));
        });
      },
      error => {
        console.error('选择获取头像方式失败：' + error);
      });
  }

  private _extend<T>(left: T, right: T): T {
    if (!(left && left instanceof Object)) return right;

    if (right && right instanceof Object) {
      for (let k in right) {
        if (right[k] && right[k] !== left[k]) {
          left[k] = right[k];
        }
      }
    }
    return left;
  }

  private _presentActionSheet(): Observable<number> {
    let sub = new Subject<number>();
    let actionSheet = this.actionSheetCtrl.create({
      title: '选择头像',
      buttons: [
        {
          text: '拍照',
          role: '1',
          handler: () => sub.next(1)
        },
        {
          text: '相册',
          role: '0',
          handler: () => sub.next(0)
        },
        {
          text: '取消',
          role: 'cancel',
          handler: () => sub.error(-1)
        }
      ]
    });

    actionSheet.present();
    return sub.asObservable();
  }
}
