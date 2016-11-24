import {Injectable} from '@angular/core';

@Injectable()
export class SwipeService {
    public swipedLeft: boolean = false;
    public swipedRight: boolean = true;

    constructor() {
    }

    onSwipeLeft() {
        this.swipedLeft = true;
        this.swipedRight = false;
    }

    onSwipeRight() {
        this.swipedLeft = false;
        this.swipedRight = true;
    }
}
