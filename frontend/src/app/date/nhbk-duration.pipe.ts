import {Pipe, Injectable, PipeTransform} from '@angular/core';
import {NhbkDateOffset} from './date.model';
import {duration2shortText, duration2text} from './util';

@Pipe({
    name: 'nhbkDuration',
    standalone: false
})
@Injectable()
export class NhbkDateDurationPipe implements PipeTransform {

    transform(duration?: number | NhbkDateOffset, ...args: any[]): any {
        return duration2text(duration);
    }
}

@Pipe({
    name: 'nhbkShortDuration',
    standalone: false
})
@Injectable()
export class NhbkDateShortDurationPipe implements PipeTransform {

    transform(duration: number | NhbkDateOffset): any {
        return duration2shortText(duration);
    }
}
