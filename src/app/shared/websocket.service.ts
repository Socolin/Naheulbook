import {Injectable} from '@angular/core';
import {Subject, Observable, Observer} from 'rxjs/Rx';

import {WsMessage, WsEvent} from './websocket.model';

@Injectable()
export class WebSocketService {
    private pendingData: string[] = [];
    private webSocket: WebSocket;
    public registeredElements: {[type: string]: {[id: number]: Subject<WsEvent>}} = {};
    public registerCount: {[type: string]: {[id: number]: number}} = {};
    public registeredObserverElements: {[type: string]: {[id: number]: Observer<WsEvent>}} = {};

    public register(type: string, elementId: number): Subject<WsEvent> {
        if (!this.webSocket) {
            let loc = window.location;
            let uri;
            if (loc.protocol === "https:") {
                uri = "wss:";
            } else {
                uri = "ws:";
            }

            this.create(uri + "//" + window.location.hostname + "/ws/listen");
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

        let observable = Observable.create(function (observer: Observer<WsEvent>) {
            this.registeredObserverElements[type][elementId] = observer;
        }.bind(this)).share();
        let observer = {
            next: (data: any) => {
                let message: WsMessage = {
                    type: type,
                    id: elementId,
                    opcode: data.opcode,
                    data: data.data
                };
                this.sendData(message);
            },
        };
        let subject = Subject.create(observer, observable);
        this.registeredElements[type][elementId] = subject;

        this.sendData({
            opcode: "LISTEN_ELEMENT",
            id: elementId,
            type: type,
            data: null
        });

        return subject;
    }

    public unregister(type: string, elementId: number) {
        if (!(type in this.registeredElements)) {
            return;
        }
        this.registerCount[type][elementId]--;
        if (this.registerCount[type][elementId] == 0) {
            delete this.registeredElements[type][elementId];
            delete this.registeredObserverElements[type][elementId];
            this.sendData({
                opcode: "STOP_LISTEN_ELEMENT",
                id: elementId,
                type: type,
                data: null
            });
        }
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
                reg[message.id].next({id: message.id, opcode: message.opcode, data: message.data});
            }
        }
    }

    private onError(ev: Event) {
        console.log('error ws', ev);
    }

    private onClose(ev: Event) {
        console.log('closed ws', ev);
        this.reconnect();
    }

    private reconnect() {
        setTimeout(() => {
            let loc = window.location;
            let uri;
            if (loc.protocol === "https:") {
                uri = "wss:";
            } else {
                uri = "ws:";
            }

            this.create(uri + "//" + window.location.hostname + "/ws/listen");

            for (let type in this.registeredElements) {
                for (let id in this.registeredElements[type]) {
                    this.sendData({
                        opcode: "LISTEN_ELEMENT",
                        id: +id,
                        type: type,
                        data: null
                    });
                }
            }
        }, 1000);
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
