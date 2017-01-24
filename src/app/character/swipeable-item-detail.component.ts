import {SwipeService} from './swipe.service';
import {Input, Component} from '@angular/core';
import {Character} from './character.model';
import {Item} from './item.model';

@Component({
    selector: 'swipeable-item-detail',
    templateUrl: './swipeable-item-detail.component.html',
    styleUrls: ['./swipeable-item-detail.component.scss'],
})
export class SwipeableItemDetailComponent {
    @Input() character: Character;
    @Input() item: Item;
    @Input() gmView: boolean;
    @Input() readonly: boolean;

    constructor(public _swipeService: SwipeService) {
    }
}
