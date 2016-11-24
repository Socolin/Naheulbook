import {Pipe, Injectable, PipeTransform} from '@angular/core';
import {timeDuration2DateOffset2} from './util';
import {NhbkDateOffset} from './date.model';

@Pipe({
    name: 'nhbkDuration'
})
@Injectable()
export class NhbkDateDurationPipe implements PipeTransform {

    transform(duration: number|NhbkDateOffset): any {
        if (!duration) {
            return '';
        }

        let dateOffset;
        if (typeof duration === 'number') {
            dateOffset = timeDuration2DateOffset2(duration);
        } else {
            dateOffset = duration;
        }

        let result = '';
        if (dateOffset.year > 1) {
            result += dateOffset.year + ' ans ';
        }
        else if (dateOffset.year === 1) {
            result += dateOffset.year + ' an ';
        }

        if (dateOffset.week > 1) {
            result += dateOffset.week + ' semaines ';
        }
        else if (dateOffset.week === 1) {
            result += dateOffset.week + ' semaine ';
        }

        if (dateOffset.day > 1) {
            result += dateOffset.day + ' jours ';
        }
        else if (dateOffset.day === 1) {
            result += dateOffset.day + ' jour ';
        }

        if (dateOffset.hour > 1) {
            result += dateOffset.hour + ' heures ';
        }
        else if (dateOffset.hour === 1) {
            result += dateOffset.hour + ' heure ';
        }

        if (dateOffset.minute > 1) {
            result += dateOffset.minute + ' minutes ';
        }
        else if (dateOffset.minute === 1) {
            result += dateOffset.minute + ' minute ';
        }

        if (dateOffset.second > 1) {
            result += dateOffset.second + ' secondes ';
        }
        else if (dateOffset.second === 1) {
            result += dateOffset.second + ' seconde ';
        }

        result = result.trim();

        return result;
    }
}
