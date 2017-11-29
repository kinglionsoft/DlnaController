import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

/**
 * 1. 将UTC时间，改为本地时间：2017-1-1T12:01
 * 2. 将json时间，改为本地时间：‘/Date(1472608668000)/’
 */
@Pipe({
    name: 'date'
})

export class LocalDatePipe implements PipeTransform {
    constructor(private date: DatePipe) { }
    transform(value: any, pattern?: string): string {
        if (value instanceof Date) {
            return this.date.transform(value, pattern);
        }
        if (value.indexOf('T') >= 0) {
            return this.date.transform(value.replace('T', ' '), pattern);
        }
        let myRex = /^\/Date\(\d*\)\/$/;
        if (myRex.test(value)) {
            value = new Date(parseInt(value.replace(/^\/Date\(/, '').replace(/\)\/$/, ''), 10));
        }
        return this.date.transform(value, pattern);
    }
}
