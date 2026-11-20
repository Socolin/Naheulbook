import {Pipe, PipeTransform} from '@angular/core';
import {IItemTemplateConfinsDuGivreData} from '../api/shared';
import {zgarugCurrencyByCode} from './confins-du-givre-currency';

@Pipe({name: 'zgarugPrice'})
export class ZgarugPricePipe implements PipeTransform {
    transform(data?: IItemTemplateConfinsDuGivreData): string {
        if (!data?.price || !data.currency) {
            return '';
        }
        return data.price + ' ' + (zgarugCurrencyByCode[data.currency]?.shortName ?? data.currency.toUpperCase());
    }
}
