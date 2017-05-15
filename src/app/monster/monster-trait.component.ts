import {Component, Input} from '@angular/core';

import {MonsterTrait, TraitInfo} from './monster.model';
import {removeDiacritics} from '../shared/remove_diacritics';

@Component({
    selector: 'monster-trait',
    styleUrls: ['./monster-trait.component.scss'],
    templateUrl: './monster-trait.component.html'
})
export class MonsterTraitComponent {
    @Input() trait: MonsterTrait;
    @Input() traitInfo: TraitInfo;

    constructor() {
    }

    imageUrl() {
        return 'assets/img/monster-traits/' + removeDiacritics(this.trait.name).split(' ').join('_').toLowerCase() + '.png';
    }
}
