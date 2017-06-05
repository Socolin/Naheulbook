import {Injectable} from '@angular/core';
import {Subject, Observable, Observer} from 'rxjs/Rx';

import {WsMessage, WsEvent, WsRegistrable} from './websocket.model';
import {NotificationsService} from '../notifications/notifications.service';
import {Monster} from '../monster/monster.model';
import {Character} from '../character/character.model';
import {Loot} from '../loot/loot.model';
import {Item, PartialItem} from '../character/item.model';
import {SkillService} from '../skill/skill.service';
import {MiscService} from '../shared/misc.service';
import {Group} from '../group/group.model';
import {NEvent} from '../event/event.model';
import {ActiveStatsModifier} from '../shared/stat-modifier.model';

@Injectable()
export class WebSocketService {
    private pendingData: string[] = [];
    private webSocket: WebSocket;
    public registeredElements: {[type: string]: {[id: number]: Subject<WsEvent>}} = {};
    public registerCount: {[type: string]: {[id: number]: number}} = {};
    public registeredObserverElements: {[type: string]: {[id: number]: Observer<WsEvent>}} = {};

    constructor(private _notification: NotificationsService
        , private _skillService: SkillService
        , private _miscService: MiscService) {
    }

    public register(type: string, elementId: number): Subject<WsEvent> {
        if (!this.webSocket) {
            let loc = window.location;
            let uri;
            if (loc.protocol === 'https:') {
                uri = 'wss:';
            } else {
                uri = 'ws:';
            }

            this.create(uri + '//' + window.location.hostname + '/ws/listen');
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
            opcode: 'LISTEN_ELEMENT',
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
        if (this.registerCount[type][elementId] === 0) {
            delete this.registeredElements[type][elementId];
            delete this.registeredObserverElements[type][elementId];
            this.sendData({
                opcode: 'STOP_LISTEN_ELEMENT',
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
            if (loc.protocol === 'https:') {
                uri = 'wss:';
            } else {
                uri = 'ws:';
            }

            this.create(uri + '//' + window.location.hostname + '/ws/listen');

            for (let type in this.registeredElements) {
                if (!this.registeredElements.hasOwnProperty(type)) {
                    continue;
                }
                for (let id in this.registeredElements[type]) {
                    if (!this.registeredElements[type].hasOwnProperty(id)) {
                        continue;
                    }
                    this.sendData({
                        opcode: 'LISTEN_ELEMENT',
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

    public registerElement(registrable: WsRegistrable) {
        if (registrable.wsSubscribtion) {
            throw new Error('Element is already subscribed to websocket: ' + registrable);
        }
        let sub = this.register(registrable.getWsTypeName(), registrable.id).subscribe(
            res => {
                try {
                    if (registrable instanceof Character) {
                        this.handleWebsocketCharacterEvent(registrable, res.opcode, res.data);
                    }
                    else if (registrable instanceof Group) {
                        this.handleWebsocketGroupEvent(registrable, res.opcode, res.data);
                    }
                    else if (registrable instanceof Monster) {
                        this.handleWebsocketMonsterEvent(registrable, res.opcode, res.data);
                    }
                    else if (registrable instanceof Loot) {
                        this.handleWebsocketLootEvent(registrable, res.opcode, res.data);
                    }
                    else {
                        console.error('Invalid registrable type', registrable);
                    }
                } catch (err) {
                    this._notification.error('Erreur', 'Erreur WS');
                    this._miscService.postJson('/api/debug/report', err, true).subscribe();
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


    public handleWebsocketMonsterEvent(monster: Monster, opcode: string, data: any) {
        switch (opcode) {
            case 'addItem': {
                this._skillService.getSkillsById().subscribe(skillsById => {
                    monster.addItem(Item.fromJson(data, skillsById));
                });
                break;
            }
            case 'deleteItem': {
                let item = PartialItem.fromJson(data);
                monster.removeItem(item.id);
                break;
            }
            case 'tookItem': {
                let takenItem = PartialItem.fromJson(data.item);
                let leftItem = null;
                if (data.leftItem) {
                    leftItem = PartialItem.fromJson(data.leftItem);
                }
                monster.takeItem(leftItem, takenItem, data.character);
                break;
            }
            case 'changeName': {
                monster.name = data;
                break;
            }
            case 'changeTarget': {
                monster.changeTarget(data);
                break;
            }
            case 'changeData': {
                monster.changeData(data.fieldName, data.value);
                break;
            }
            case 'addModifier': {
                monster.onAddModifier(ActiveStatsModifier.fromJson(data));
                break;
            }
            case 'removeModifier': {
                monster.onRemoveModifier(ActiveStatsModifier.fromJson(data));
                break;
            }
            case 'updateModifier': {
                monster.onUpdateModifier(ActiveStatsModifier.fromJson(data));
                break;
            }
            case 'equipItem': {
                monster.equipItem(PartialItem.fromJson(data));
                break;
            }
            default: {
                console.warn('Opcode not handle: `' + opcode + '`');
                break;
            }
        }
    }

    public handleWebsocketGroupEvent(group: Group, opcode: string, data: any) {
        switch (opcode) {
            case 'addLoot': {
                this._skillService.getSkillsById().subscribe(skillsById => {
                    group.addLoot(Loot.fromJson(data, skillsById));
                });
                break;
            }
            case 'deleteLoot': {
                group.removeLoot(data.id);
                break;
            }
            case 'addEvent': {
                group.addEvent(NEvent.fromJson(data));
                break;
            }
            case 'deleteEvent': {
                group.removeEvent(data.id);
                break;
            }
            case 'changeData': {
                group.data.changeValue(data.key, data.value);
                break;
            }
            default: {
                console.warn('Opcode not handle: `' + opcode + '`');
                break;
            }
        }
    }

    public handleWebsocketCharacterEvent(character: Character, opcode: string, data: any): void {
        switch (opcode) {
            case 'showLoot': {
                this._skillService.getSkillsById().subscribe(skillsById => {
                    character.addLoot(Loot.fromJson(data, skillsById));
                });
                break;
            }
            case 'hideLoot': {
                character.removeLoot(data.id);
                break;
            }
            case 'update': {
                character.onChangeCharacterStat(data);
                break;
            }
            case 'statBonusAd': {
                character.onSetStatBonusAD(data);
                break;
            }
            case 'levelUp': {
                character.onPartialLevelUp(data);
                break;
            }
            case 'equipItem': {
                character.onEquipItem(data);
                break;
            }
            case 'addItem': {
                this._skillService.getSkillsById().subscribe(skillsById => {
                    character.onAddItem(Item.fromJson(data, skillsById));

                });
                break;
            }
            case 'deleteItem': {
                character.onDeleteItem(data);
                break;
            }
            case 'identifyItem': {
                this._skillService.getSkillsById().subscribe(skillsById => {
                    character.onIdentifyItem(Item.fromJson(data, skillsById));

                });
                break;
            }
            case 'useCharge': {
                character.onUseItemCharge(data);
                break;
            }
            case 'changeContainer': {
                character.onChangeContainer(data);
                break;
            }
            case 'updateItem': {
                character.onUpdateItem(data);
                break;
            }
            case 'changeQuantity': {
                character.onUpdateQuantity(data);
                break;
            }
            case 'updateItemModifiers': {
                character.onUpdateModifiers(data);
                break;
            }
            case 'addEffect': {
                character.onAddEffect(data);
                break;
            }
            case 'removeEffect': {
                character.onRemoveEffect(data);
                break;
            }
            case 'updateEffect': {
                character.onUpdateEffect(data);
                break;
            }
            case 'addModifier': {
                character.onAddModifier(data);
                break;
            }
            case 'removeModifier': {
                character.onRemoveModifier(data);
                break;
            }
            case 'updateModifier': {
                character.onUpdateModifier(data);
                break;
            }
            case 'active': {
                character.changeActive(data);
                break;
            }
            case 'changeColor': {
                character.color = data;
                break;
            }
            case 'changeTarget': {
                character.changeTarget(data);
                break;
            }
            default:
                console.warn('Opcode not handle: `' + opcode + '`');
                break;
        }
    }

    public handleWebsocketLootEvent(loot: Loot, opcode: string, data: any): void {
        switch (opcode) {
            case 'addItem': {
                this._skillService.getSkillsById().subscribe(skillsById => {
                    loot.addItem(Item.fromJson(data, skillsById));
                });
                break;
            }
            case 'deleteItem': {
                let item = PartialItem.fromJson(data);
                loot.removeItem(item.id);
                break;
            }
            case 'addMonster': {
                this._skillService.getSkillsById().subscribe(skillsById => {
                    let monster = Monster.fromJson(data, skillsById);
                    loot.addMonster(monster);
                });
                break;
            }
            case 'deleteMonster': {
                this._skillService.getSkillsById().subscribe(skillsById => {
                    let monster = Monster.fromJson(data, skillsById);
                    loot.removeMonster(monster.id);
                });
                break;
            }
            case 'updateLoot': {
                loot.visibleForPlayer = data.visibleForPlayer;
                break;
            }
            case 'tookItem': {
                let takenItem = PartialItem.fromJson(data.item);
                let leftItem = null;
                if (data.leftItem) {
                    leftItem = PartialItem.fromJson(data.leftItem);
                }
                loot.takeItem(leftItem, takenItem, data.character);
                break;
            }
        }
    }
}
