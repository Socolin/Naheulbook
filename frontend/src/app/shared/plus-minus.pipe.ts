import {Injectable, Pipe, PipeTransform} from '@angular/core';

@Pipe({
    name: 'plusminus',
    standalone: false
})
@Injectable()
export class PlusMinusPipe implements PipeTransform {
    transform(value: any): any {
        if (value <= 0) {
            return value;
        }
        if (value > 0) {
            return '+' + value;
        }
        return value;
    }
}
