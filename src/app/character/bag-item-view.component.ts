import {Component, Input} from '@angular/core';
import {Item} from '../item';

import {Character} from './character.model';

@Component({
    selector: 'bag-item-view',
    templateUrl: './bag-item-view.component.html',
    styleUrls: ['./bag-item-view.component.scss'],
})
export class BagItemViewComponent {
    @Input() items: Item[];
    @Input() character: Character;
    @Input() gmView: boolean;
}
