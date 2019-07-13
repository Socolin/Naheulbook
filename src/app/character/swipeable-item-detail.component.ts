import {Input, Component, ElementRef, ViewChild} from '@angular/core';
import {CdkConnectedOverlay, CdkOverlayOrigin} from '@angular/cdk/overlay';

import {Character} from './character.model';
import {Item} from '../item';
import {SwipeService} from './swipe.service';

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
    @Input() origin: CdkOverlayOrigin;

    @ViewChild('currentItemDetail', {read: ElementRef, static: false})
    public currentItemDetail: ElementRef;
    public itemDetailOffsetY = -30;

    constructor(public _swipeService: SwipeService) {
    }

    public updateItemDetailPosition(currentItemDetailOverlay: CdkConnectedOverlay) {
        setTimeout(() => {
            if (!this.currentItemDetail) {
                return;
            }
            if (!currentItemDetailOverlay.overlayRef.overlayElement) {
                return;
            }
            if (!currentItemDetailOverlay.overlayRef.overlayElement.children[0]) {
                return;
            }
            if (!currentItemDetailOverlay.overlayRef.overlayElement.children[0].children[0]) {
                return;
            }

            const previousPosition = this.itemDetailOffsetY;
            let originRect = currentItemDetailOverlay.origin.elementRef.nativeElement.getBoundingClientRect();
            let rect = currentItemDetailOverlay.overlayRef.overlayElement.children[0].children[0].children[0].getBoundingClientRect();
            let bottom = (originRect.bottom + rect.height);
            if (bottom > document.documentElement.clientHeight) {
                this.itemDetailOffsetY = (document.documentElement.clientHeight - bottom);
                currentItemDetailOverlay.offsetY = this.itemDetailOffsetY;
            } else {
                this.itemDetailOffsetY = -30;
                currentItemDetailOverlay.offsetY = -30;
            }
            if (previousPosition !== this.itemDetailOffsetY) {
                currentItemDetailOverlay.overlayRef.updatePosition();
            }
        }, 0);
    }
}
