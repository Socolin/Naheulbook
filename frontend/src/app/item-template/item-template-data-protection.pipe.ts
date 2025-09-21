import {Pipe, PipeTransform} from '@angular/core';
import {ItemTemplate} from './item-template.model';

@Pipe({ name: 'itemTemplateDataProtection' })
export class ItemTemplateDataProtectionPipe implements PipeTransform {

    transform(itemTemplate: ItemTemplate, ...args: unknown[]): unknown {
        let result = '';

        if (itemTemplate.data.protection) {
            result += itemTemplate.data.protection;
        }
        if (itemTemplate.data.protection && itemTemplate.data.magicProtection) {
            result += '+';
        }
        if (itemTemplate.data.magicProtection) {
            result += itemTemplate.data.magicProtection + ' (magique)';
        }
        if ((itemTemplate.data.protection || itemTemplate.data.magicProtection) && itemTemplate.data.protectionAgainstMagic) {
            result += '/'
        }
        if (itemTemplate.data.protectionAgainstMagic) {
            result += itemTemplate.data.protectionAgainstMagic;
            if (itemTemplate.data.protectionAgainstType) {
                result += '(' + itemTemplate.data.protectionAgainstType + ')';
            } else {
                result += '(mag./myst.)';
            }
        }
        return result;
    }

}
