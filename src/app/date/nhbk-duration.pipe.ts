import {Pipe, Injectable, PipeTransform} from '@angular/core';
import {NhbkDateOffset} from './date.model';
import {duration2text} from './util';

@Pipe({
    name: 'nhbkDuration'
})
@Injectable()
export class NhbkDateDurationPipe implements PipeTransform {

    transform(duration: number|NhbkDateOffset): any {
        return duration2text(duration);
    }
}
