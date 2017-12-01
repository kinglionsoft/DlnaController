import { Injectable } from '@angular/core';

@Injectable()
export class StorageService {

    constructor() { }

    set(key: string, value: any) {
        localStorage.setItem(key, value ? JSON.stringify(value) : '');
    }

    getString(key: string): string {
        return localStorage.getItem(key);
    }

    get<T>(key: string): T {
        let value = localStorage.getItem(key);
        if (value && value !== '') {
            return JSON.parse(value) as T;
        }
        return null;
    }
}
