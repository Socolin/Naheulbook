import {Pipe, PipeTransform} from '@angular/core';

/**
 * Format a value change from an history entry: `28 ➜ 278 (+250)`.
 * The difference is only shown when both values are numbers.
 */
@Pipe({name: 'valueChange'})
export class ValueChangePipe implements PipeTransform {
    transform(data: { oldValue?: any, newValue?: any } | undefined): string {
        if (!data) {
            return '';
        }
        const oldValue = data.oldValue ?? '?';
        const newValue = data.newValue ?? '?';
        let result = `${oldValue} ➜ ${newValue}`;

        if (typeof data.oldValue === 'number' && typeof data.newValue === 'number') {
            const diff = data.newValue - data.oldValue;
            if (diff > 0) {
                result += ` (+${diff})`;
            } else if (diff < 0) {
                result += ` (−${-diff})`;
            }
        }

        return result;
    }
}
