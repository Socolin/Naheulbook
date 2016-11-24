import {Injectable} from '@angular/core';
import {NotificationsService} from '../notifications/notifications.service';
import {WebSocketService} from '../shared/websocket.service';
import {GenericWebsocketService} from '../shared/generic.websocket.service';
import {MiscService} from '../shared/misc.service';
import {Observable} from 'rxjs';

@Injectable()
export class GroupWebsocketService extends GenericWebsocketService {
    constructor(notification: NotificationsService
        , miscService: MiscService
        , webSocketService: WebSocketService) {
        super(notification, miscService, webSocketService, 'group');
    }

    registerPacket(opcode: string): Observable<any> {
        return super.registerPacket(opcode).map(res => res.data);
    }
}
