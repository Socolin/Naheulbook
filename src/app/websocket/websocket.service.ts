import {Injectable} from '@angular/core';
import {Subject, Observable, Observer} from 'rxjs';
import * as signalR from '@aspnet/signalr';

import {MiscService} from '../shared';
import {NotificationsService} from '../notifications';
import {SkillService} from '../skill';
import {JobService} from '../job';
import {OriginService} from '../origin';

import {WsMessage, WsEvent, WsRegistrable} from './websocket.model';
import {share} from 'rxjs/operators';
import {LoginService} from '../user';

@Injectable()
export class WebSocketService {
    private pendingMessages: { methodName: string, args: any[] }[] = [];
    public registeredElements: { [type: string]: { [id: number]: Subject<WsEvent> } } = {};
    public registerCount: { [type: string]: { [id: number]: number } } = {};
    public registeredObserverElements: { [type: string]: { [id: number]: Observer<WsEvent> } } = {};
    private _hubConnection?: signalR.HubConnection;
    private connecting = false;

    constructor(private _notification: NotificationsService
        , private _loginService: LoginService
        , private _skillService: SkillService
        , private _jobService: JobService
        , private _originService: OriginService
        , private _miscService: MiscService) {
    }

    public registerElement(registrable: WsRegistrable) {
        if (registrable.wsSubscribtion) {
            throw new Error('Element is already subscribed to websocket: ' + registrable);
        }
        let sub = this.register(registrable.getWsTypeName(), registrable.id).subscribe(
            res => {
                try {
                    if (!res.opcode) {
                        console.error('Invalid packet, no opcode', res);
                    }
                    let services = {
                        skill: this._skillService,
                        job: this._jobService,
                        origin: this._originService,
                    };
                    registrable.handleWebsocketEvent(res.opcode, res.data, services);
                } catch (err) {
                    this._notification.error('Erreur', 'Erreur WS');
                    this._miscService.postError('/api/debug/report', err).subscribe();
                    console.error(err);
                }
            }
        );
        registrable.wsRegister(sub, this);
    }

    public unregisterElement(registrable: WsRegistrable) {
        if (!registrable.wsSubscribtion) {
            throw new Error('Element not registered to websocket: ' + registrable);
        }
        this.unregister(registrable.getWsTypeName(), registrable.id);
        registrable.wsUnregister();
    }

    private register(type: string, elementId: number): Subject<WsEvent> {
        if (!this._hubConnection) {
            this.connect();
        }

        if (!(type in this.registeredElements)) {
            this.registeredElements[type] = {};
        }
        if (!(type in this.registeredObserverElements)) {
            this.registeredObserverElements[type] = {};
        }
        if (!(type in this.registerCount)) {
            this.registerCount[type] = {};
        }
        if (elementId in this.registeredElements[type]) {
            this.registerCount[type][elementId]++;
            return this.registeredElements[type][elementId];
        }

        this.registerCount[type][elementId] = 1;

        let observable = new Observable((obs: Observer<WsEvent>) => {
            this.registeredObserverElements[type][elementId] = obs;
        }).pipe(share());

        let observer = {
            next: (data: any) => {
                let message: WsMessage = {
                    type: type,
                    id: elementId,
                    opcode: data.opcode,
                    data: data.data
                };
                this.sendData('Event' + type, message);
            },
        };
        let subject = Subject.create(observer, observable);
        this.registeredElements[type][elementId] = subject;

        this.sendSubscribe(type, elementId);

        return subject;
    }

    private connect(reconnect?: boolean) {
        if (this.connecting) {
            return;
        }

        this.connecting = true;
        this._hubConnection = new signalR.HubConnectionBuilder()
            .withUrl('/ws/listen', {accessTokenFactory: () => this._loginService.currentJwt})
            .configureLogging(signalR.LogLevel.Information)
            .build();
        this._hubConnection.on('event', (data: string) => {
            let message: WsMessage = JSON.parse(data);
            if (message.type in this.registeredObserverElements) {
                let reg = this.registeredObserverElements[message.type];
                if (message.id in reg) {
                    reg[message.id].next({id: message.id, opcode: message.opcode, data: message.data});
                }
            }
            console.log(data);
        });
        this._hubConnection.onclose(err => {
            console.error(err);
            setTimeout(() => this.connect(true), 2000);
        });
        this._hubConnection.start()
            .then(async () => {
                this.connecting = false;
                console.log('start');
                if (reconnect) {
                    for (let type in this.registeredElements) {
                        if (!this.registeredElements.hasOwnProperty(type)) {
                            continue;
                        }
                        for (let id in this.registeredElements[type]) {
                            if (!this.registeredElements[type].hasOwnProperty(id)) {
                                continue;
                            }
                            await this.sendSubscribe(type, +id);
                        }
                    }
                }
                for (const pendingMessage of this.pendingMessages) {
                    await this._hubConnection.send.apply(this._hubConnection, [pendingMessage.methodName].concat(pendingMessage.args));
                }
            })
            .catch((err) => {
                this.connecting = false;
                console.error(err);
                setTimeout(() => this.connect(true), 2000);
            });
    }

    private unregister(type: string, elementId: number): Promise<void> {
        if (!(type in this.registeredElements)) {
            return Promise.resolve();
        }
        if (!(elementId in this.registeredElements[type])) {
            return Promise.resolve();
        }

        this.registerCount[type][elementId]--;
        if (this.registerCount[type][elementId] === 0) {
            delete this.registeredElements[type][elementId];
            delete this.registeredObserverElements[type][elementId];
            return this.sendUnsubscribe(type, elementId);
        }
    }

    private sendSubscribe(type: string, id: number): Promise<void> {
        const methodName = 'Subscribe' + type.charAt(0).toUpperCase() + type.slice(1);
        return this.sendData(methodName, id);
    }

    private sendUnsubscribe(type: string, id: number): Promise<void> {
        const methodName = 'Unsubscribe' + type.charAt(0).toUpperCase() + type.slice(1);
        return this.sendData(methodName, id);
    }

    private sendData(methodName: string, ...args: any[]): Promise<void> {
        if (this._hubConnection.state === signalR.HubConnectionState.Connected) {
            return this._hubConnection.send.apply(this._hubConnection, [methodName].concat(args));
        } else {
            this.pendingMessages.push({methodName, args});
            return Promise.resolve();
        }
    }
}
