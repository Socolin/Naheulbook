import {Injectable, EventEmitter} from '@angular/core';
import {Subject, Observable, Observer} from 'rxjs/Rx';

import {WsMessage, WsEvent} from './websocket.model';

@Injectable()
export class WebSocketService {
    private pendingData: string[] = [];
    private webSocket: WebSocket;
    public registeredElements: {[type: string]: {[id: number]: Subject<WsEvent>}} = {};
    public registeredObserverElements: {[type: string]: {[id: number]: Observer<WsEvent>}} = {};

    public register(type: string, element: any): Subject<WsEvent> {
        if (!this.webSocket) {
            this.create("ws://naheulbouc.fr/ws/listen");
        }

        if (!(type in this.registeredElements)) {
            this.registeredElements[type] = {};
        }
        if (!(type in this.registeredObserverElements)) {
            this.registeredObserverElements[type] = {};
        }

        let observable = Observable.create(function (observer: Observer<WsEvent>) {
            this.registeredObserverElements[type][element.id] = observer;
        }.bind(this));
        let observer = {
            next: (data: any) => {
                let message: WsMessage = {
                    type: type,
                    id: element.id,
                    opcode: data.opcode,
                    data: data.data
                };
                this.sendData(message);
            },
        };
        let subject = Subject.create(observer, observable);
        this.registeredElements[type][element.id] = subject;

        this.sendData({
            opcode: "LISTEN_ELEMENT",
            id: element.id,
            type: 'character',
            data: null
        });

        return subject;
    }

    public unregister(type: string, element: any) {
        if (!(type in this.registeredElements)) {
            return;
        }
        delete this.registeredElements[type][element.id];
        delete this.registeredObserverElements[type][element.id];
    }

    private onConnected(ev: Event) {
        for (let i = 0; i < this.pendingData.length; i++) {
            let data = this.pendingData[i];
            this.webSocket.send(data);
        }
    }

    private onMessage(data: MessageEvent) {
        let message: WsMessage = JSON.parse(data.data);
        if (message.type in this.registeredObserverElements) {
            let reg = this.registeredObserverElements[message.type];
            if (message.id in reg) {
                reg[message.id].next({opcode: message.opcode, data: message.data});
            }
        }
    }

    private onError(ev: Event) {
        console.log('error ws', ev);
    }

    private onClose(ev: Event) {
        console.log('closed ws', ev);
    }

    private sendData(message: WsMessage) {
        if (this.webSocket.readyState === WebSocket.OPEN) {
            this.webSocket.send(JSON.stringify(message));
        } else if (this.webSocket.readyState === WebSocket.CONNECTING) {
            this.pendingData.push(JSON.stringify(message));
        }
    }

    private create(url) {
        let ws = new WebSocket(url);

        ws.onopen = this.onConnected.bind(this);
        ws.onmessage = this.onMessage.bind(this);
        ws.onerror = this.onError.bind(this);
        ws.onclose = this.onClose.bind(this);

        this.webSocket = ws;
    }
}
