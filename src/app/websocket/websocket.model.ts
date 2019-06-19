import {Subscription} from 'rxjs';

import {SkillService} from '../skill';
import {JobService} from '../job';
import {OriginService} from '../origin';

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

export interface WsEventServices {
    skill: SkillService;
    job: JobService;
    origin: OriginService
}

export abstract class WsRegistrable {
    public id: number;
    public wsSubscribtion: {sub: Subscription, service: WebSocketService} | undefined;

    public wsRegister(sub: Subscription, service: WebSocketService) {
        this.wsSubscribtion = {
            sub: sub,
            service: service
        };
        this.onWsRegister(service);
    }

    public wsUnregister() {
        if (!this.wsSubscribtion) {
            return;
        }
        this.onWsUnregister();
        this.wsSubscribtion.sub.unsubscribe();
        this.wsSubscribtion = undefined;
    }

    abstract getWsTypeName(): string;
    abstract onWsRegister(service: WebSocketService): void;
    abstract onWsUnregister(): void;
    abstract handleWebsocketEvent(opcode: string, data: any, services: WsEventServices);
}
