# Ionic2 App 种子项目

本项目是对Ionic2 App的基本封装，基本的使用方法请查看[http://ionicframework.com](http://ionicframework.com).
 
# Usages

## CoreModule

### Config
配置全局参数；

### HttpClient
封装ng2的Http，实现拦截器、认证等功能，详见[./src/app/core/services/httpclient.ts](./src/app/core/services/httpclient.ts)。没有使用ng2-interceptor，因其不支持AOT。

### 统一导入rxjs的常用操作符
统一导入rxjs的常用操作符，不需要在业务模块中再次导入，详见[./src/app/core/rxjs-operators.ts](./src/app/core/rxjs-operators.ts)。

### MessageBox
封装toast、alert、confirm、prompt方法。

### 微信模式
* 在微信浏览器中运行时，为 body 添加 'wechat' 的样式。

``` html
<body class="wechat" ...
```

* 隐藏 ion-header。
* 通过物理返回键（android）、微信返回（ios）实现页面返回。
* 通过拦截 H5 的 popstate 事件，实现连击返回键退出App的功能。


# Known Issues

## cnpm install后，Lazy Loading的页面无法启动。
* 原因：cnpm 镜像问题
* 解决方法：
    1. 目录下的node_modules; 
    2. npm install
	
## Chrome 调试跨域
* windows: [解决chrome调试时不能跨域的问题](http://www.cnblogs.com/laden666666/p/5544572.html)
* Mac:
``` bash
open -a "Google Chrome" --args --disable-web-security --user-data-dir
```
	
## Android生成时指定 gradle 路径
在终端中(cmd\shell)生成Android时可能需要下载gradle：
``` bash
Downloading http://services.gradle.org/distributions/gradle-2.14.1-all.zip
```
若下载速度慢，可使用迅雷等下载工具把gradle下载后，放到本地服务器；然后通过环境变量指定gradle下载路径：
* windows:
``` bash
set CORDOVA_ANDROID_GRADLE_DISTRIBUTION_URL=http://localhost/gradle/gradle-2.14.1-all.zip
```
* Mac OS X & Linux
``` bash
export CORDOVA_ANDROID_GRADLE_DISTRIBUTION_URL=http://localhost/gradle/gradle-2.14.1-all.zip
```

## Current working directory is not a Cordova-based project
1. 需要在项目根目录运行ionic-cli; 
2. 项目根目录需要有：platforms文件夹，www文件夹。这2个文件夹是编译生成的，没有加入git管理，若没有手动生成即可 。

# License
[MIT](/LICENSE) by yangchao.
