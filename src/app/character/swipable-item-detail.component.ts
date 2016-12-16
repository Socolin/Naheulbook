import {SwipeService} from './swipe.service';
import {Input, Component} from '@angular/core';
import {Character} from './character.model';
import {Item} from './item.model';

@Component({
    selector: 'swipable-item-detail',
    templateUrl: './swipable-item-detail.component.html',
    styles: [`
        .item-detail-swipe-container
        {
            width: 85vw;
            padding: 10px;
            border-bottom-left-radius: 7px;
            border-top-left-radius: 7px;
        }
        .item-detail {
            position: absolute;
            top: 10px;
            right: -17px;
            color: #555;
            z-index: 100;
            width: 20vw;
            touch-action: none;
            -webkit-transition: width 0.5s; /* Safari */
            transition: width 0.5s;
        }
        .item-detail-swipe {
            width: 85vw;
            -webkit-transition: width 0.5s; /* Safari */
            transition: width 0.5s;
        }
        `
    ],
})
export class SwipableItemDetailComponent {
    @Input() character: Character;
    @Input() item: Item;
    @Input() gmView: boolean;
    @Input() readonly: boolean;

    constructor(private _swipeService: SwipeService) {
    }
}
