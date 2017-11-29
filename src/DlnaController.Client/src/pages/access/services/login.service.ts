import { Injectable } from '@angular/core';

@Injectable()
export class LoginService {

  constructor() { }

  login() {
    console.log('login for demo');
  }

  register() {
    console.log('register for demo');
  }
}
