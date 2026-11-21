import {Injectable} from '@angular/core';
import {Observable, Observer, Subject} from 'rxjs';
import * as signalR from '@microsoft/signalr';

import {MiscService} from '../shared';
import {NotificationsService} from '../notifications';
import {SkillService} from '../skill';
import {JobService} from '../job';
import {OriginService} from '../origin';

import {WsEvent, WsMessage, WsRegistrable} from './websocket.model';
import {share} from 'rxjs/operators';
import {LoginService} from '../user';

@Injectable({ providedIn: 'root'})
export class WebSocketService {
    private pendingMessages: { methodName: string, args: any[] }[] = [];
    public registeredElements: { [type: string]: { [id: number]: Subject<WsEvent> } } = {};
    public registerCount: { [type: string]: { [id: number]: number } } = {};
    public registeredObserverElements: { [type: string]: { [id: number]: Observer<WsEvent> } } = {};
    private hubConnection?: signalR.HubConnection;
    private connecting = false;
    private reconnectAttempts = 0;
    private reconnectTimeout?: ReturnType<typeof setTimeout>;
    private static readonly minReconnectDelayMs = 2000;
    private static readonly maxReconnectDelayMs = 60000;

    constructor(
        private readonly jobService: JobService,
        private readonly loginService: LoginService,
        private readonly miscService: MiscService,
        private readonly notification: NotificationsService,
        private readonly originService: OriginService,
        private readonly skillService: SkillService,
    ) {
    }

    public registerElement(registrable: WsRegistrable) {
        if (registrable.wsSubscribtion) {
            throw new Error('Element is already subscribed to websocket: ' + registrable);
        }
        let sub = this.register(registrable.getWsTypeName(), registrable.id).subscribe(
            res => {
                try {
                    let services = {
                        skill: this.skillService,
                        job: this.jobService,
                        origin: this.originService,
                    };
                    registrable.handleWebsocketEvent(res.opcode, res.data, services);
                } catch (err) {
                    console.error(err);
                    let error = new Error('An error occured while handling websocket event `' + res.opcode + '\' on '
                        + registrable.getWsTypeName()
                    );
                    error['innerError'] = err;
                    error['data'] = res.data;
                    throw error;
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
        if (!this.hubConnection) {
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
        const hubConnection = new signalR.HubConnectionBuilder()
            .withUrl('/ws/listen', {accessTokenFactory: () => this.loginService.currentJwt!})
            .configureLogging(signalR.LogLevel.Information)
            .build();
        hubConnection.on('event', (data: string) => {
            let message: WsMessage = JSON.parse(data);
            if (message.type in this.registeredObserverElements) {
                let reg = this.registeredObserverElements[message.type];
                if (message.id in reg) {
                    reg[message.id].next({id: message.id, opcode: message.opcode, data: message.data});
                }
            }
        });
        hubConnection.onclose(err => {
            console.error(err);
            this.scheduleReconnect();
        });
        hubConnection.start()
            .then(async () => {
                this.connecting = false;
                this.reconnectAttempts = 0;
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
                const pendingMessages = this.pendingMessages;
                this.pendingMessages = [];
                for (const pendingMessage of pendingMessages) {
                    await hubConnection.send.apply(hubConnection, [pendingMessage.methodName].concat(pendingMessage.args));
                }
            })
            .catch((err) => {
                this.connecting = false;
                console.error(err);
                const unauthorized = WebSocketService.isUnauthorizedError(err);
                if (unauthorized) {
                    // The JWT is probably expired (e.g. the tab was asleep and the renewal timer did not run).
                    // Ask for a fresh one using the session: the next attempt reads it via accessTokenFactory.
                    this.loginService.checkLogged();
                }
                this.scheduleReconnect(unauthorized);
            });
        this.hubConnection = hubConnection;
    }

    private scheduleReconnect(unauthorized = false) {
        if (this.reconnectTimeout) {
            return;
        }
        const delay = Math.min(
            WebSocketService.minReconnectDelayMs * Math.pow(2, this.reconnectAttempts),
            WebSocketService.maxReconnectDelayMs
        );
        this.reconnectAttempts++;
        this.reconnectTimeout = setTimeout(() => {
            this.reconnectTimeout = undefined;
            if (unauthorized && !this.loginService.currentJwt) {
                // Session expired too: not logged anymore: stop retrying, the next register() will open a new connection.
                this.hubConnection = undefined;
                this.reconnectAttempts = 0;
                return;
            }
            this.connect(true);
        }, delay);
    }

    private static isUnauthorizedError(err: any): boolean {
        // The negotiate failure is wrapped in a FailedToNegotiateWithServerError which only keeps the status code in the message.
        if (err instanceof signalR.HttpError) {
            return err.statusCode === 401;
        }
        return typeof err?.message === 'string' && err.message.includes('Status code \'401\'');
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
        return Promise.resolve();
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
        if (this.hubConnection && this.hubConnection.state === signalR.HubConnectionState.Connected) {
            return this.hubConnection.send.apply(this.hubConnection, [methodName].concat(args));
        } else {
            this.pendingMessages.push({methodName, args});
            return Promise.resolve();
        }
    }
}
