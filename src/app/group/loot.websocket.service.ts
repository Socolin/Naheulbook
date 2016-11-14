import {Injectable} from "@angular/core";
import {NotificationsService} from "../notifications/notifications.service";
import {WebSocketService} from "../shared/websocket.service";
import {GenericWebsocketService} from "../shared/generic.websocket.service";
import {MiscService} from "../shared/misc.service";

@Injectable()
export class LootWebsocketService extends GenericWebsocketService {
    constructor(notification: NotificationsService
        , miscService: MiscService
        , webSocketService: WebSocketService) {
        super(notification, miscService, webSocketService, 'loot');
    }
}
