import {Injectable, Pipe, PipeTransform} from '@angular/core';
import {isNullOrUndefined} from 'util';

export function formatModifierValue(modifier) {
    if (isNullOrUndefined(modifier.type) || modifier.type === 'ADD') {
        if (modifier.value <= 0) {
            return modifier.value;
        } else {
            return '+' + modifier.value;
        }
    }
    else if (modifier.type === 'DIV') {
        return '/' + modifier.value;
    }
    else if (modifier.type === 'MUL') {
        return '*' + modifier.value;
    }
    else if (modifier.type === 'SET') {
        return '=' + modifier.value;
    }
    else if (modifier.type === 'PERCENTAGE') {
        return '' + modifier.value + '%';
    }
}

@Pipe({
    name: 'modifier'
})
@Injectable()
export class ModifierPipe implements PipeTransform {
    transform(modifier: any): any {
        let result = modifier.stat + formatModifierValue(modifier);

        if (modifier.special) {
            if (modifier.special.indexOf('AFFECT_ONLY_THROW') !== -1) {
                result += '(lancer)';
            }
            if (modifier.special.indexOf('AFFECT_ONLY_MELEE') !== -1) {
                result += '(c.a.c)';
            }
            if (modifier.special.indexOf('DONT_AFFECT_MAGIEPSY') !== -1) {
                result += '(NoMagPsy)';
            }
            if (modifier.special.indexOf('AFFECT_ONLY_MELEE_STAFF') !== -1) {
                result += '(c.a.c Baton)';
            }
            if (modifier.special.indexOf('AFFECT_PR_FOR_ELEMENTS') !== -1) {
                result += '(element)';
            }
            if (modifier.special.indexOf('AFFECT_DISCRETION') !== -1) {
                result += '(discrétion)';
            }
            if (modifier.special.indexOf('AFFECT_ONLY_DANSE') !== -1) {
                result += '(danse)';
            }
            if (modifier.special.indexOf('ONLY_IF_NOTHING_ON') !== -1) {
                result += '(si rien au dessus)';
            }
        }

        return result;
    }
}
