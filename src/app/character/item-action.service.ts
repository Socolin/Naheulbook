import {Injectable} from "@angular/core";
import {WebSocketService} from "../shared/websocket.service";
import {NotificationsService} from "../notifications/notifications.service";
import {Character} from "./character.model";
import {CharacterService} from "./character.service";
import {Observable, Observer} from "rxjs";
import {Item} from "./item.model";

@Injectable()
export class ItemActionService {
    private actionObservers: {[actionName: string]: Observer<{item: Item, data: any}>} = {}

    public registerAction(action: string): Observable<{item: Item, data: any}> {
        return Observable.create((function (observer) {
            this.actionObservers[action] = observer;
        }).bind(this));
    }

    public onAction(actionName: string, item: Item, data?: any) {
        if (!(actionName in this.actionObservers)) {
            throw new Error('action: `' + actionName + '\' was not registered');
        }
        this.actionObservers[actionName].next({item: item, data: data});
        return false;
    }

}
