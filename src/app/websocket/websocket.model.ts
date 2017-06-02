import {Subscription} from 'rxjs/Subscription';
import {WebSocketService} from './websocket.service';

export interface WsMessage {
    opcode: string;
    type: string;
    id: number;
    data: any;
}

export class WsEvent {
    id: number;
    opcode?: string;
    data: any;
}

export abstract class WsRegistrable {
    public id: number;
    public wsSubscribtion: {sub: Subscription, service: WebSocketService} = null;

    public wsRegister(sub: Subscription, service: WebSocketService) {
        this.wsSubscribtion = {
            sub: sub,
            service: service
        };
        this.onWsRegister(service);
    }

    public wsUnregister() {
        this.onWsUnregister();
        this.wsSubscribtion.sub.unsubscribe();
        this.wsSubscribtion = null;
    }

    abstract getWsTypeName(): string;
    abstract onWsRegister(service: WebSocketService);
    abstract onWsUnregister(): void;
}
