import {Pipe, PipeTransform} from '@angular/core';

@Pipe({
    name: 'itemPrice',
    standalone: false
})
export class ItemPricePipe implements PipeTransform {

    transform(price?: number, ...args: unknown[]): unknown {
        if (!price) {
            return '';
        }
        price = Math.round(price * 100) / 100;
        if (price >= 1) {
            return price + ' P.O.';
        }
        if (price >= 0.1) {
            return (price * 10) + ' P.A.';
        }
        return (price * 100) + ' P.C.';
    }

}
