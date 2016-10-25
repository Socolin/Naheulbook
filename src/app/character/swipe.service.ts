import {Injectable} from "@angular/core";
import {WebSocketService} from "../shared/websocket.service";
import {NotificationsService} from "../notifications/notifications.service";
import {Character} from "./character.model";
import {CharacterService} from "./character.service";
import {Observable, Observer} from "rxjs";

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
