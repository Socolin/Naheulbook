import {Observable, Subject, Subscription} from 'rxjs';

import {WebSocketService, WsEventServices, WsRegistrable} from '../websocket';

import {Monster} from '../monster';
import {Character} from '../character';
import {NhbkDate} from '../date';
import {Loot} from '../loot';
import {NEvent} from '../event';
import {date2Timestamp} from '../date/util';
import {TargetJsonData} from './target.model';
import {FighterDurationChanges, tokenColors} from '../shared';
import {DeleteInviteResponse, GroupResponse, NpcResponse} from '../api/responses';
import {IGroupConfig, IGroupData, INpcData} from '../api/shared';
import {Fight} from './fight';

export class FighterStat {
    private fighter: Fighter;

    get at(): number {
        return this.fighter.isMonster ? this.fighter.monster.computedData.at : this.fighter.character.computedData.stats['AT'];
    }

    get ad(): number {
        return this.fighter.isMonster ? 10 : this.fighter.character.computedData.stats['AD'];
    }

    get prd(): number {
        return this.fighter.isMonster ? this.fighter.monster.computedData.prd : this.fighter.character.computedData.stats['PRD'];
    }

    get esq(): number {
        return this.fighter.isMonster ? this.fighter.monster.computedData.esq : this.fighter.character.computedData.stats['AD'];
    }

    get ev(): number {
        return this.fighter.isMonster ? this.fighter.monster.data.ev : this.fighter.character.ev;
    }

    get ea(): number {
        return this.fighter.isMonster ? this.fighter.monster.data.ea : this.fighter.character.ea;
    }

    get maxEv(): number {
        return this.fighter.isMonster ? this.fighter.monster.data.maxEv : this.fighter.character.computedData.stats['EV'];
    }

    get maxEa(): number {
        return this.fighter.isMonster ? this.fighter.monster.data.maxEa : this.fighter.character.computedData.stats['EA'];
    }

    get pr(): number {
        return this.fighter.isMonster ? this.fighter.monster.computedData.pr : this.fighter.character.computedData.stats['PR'];
    }

    get pr_magic(): number {
        return this.fighter.isMonster ? this.fighter.monster.computedData.pr_magic : this.fighter.character.computedData.stats['PR_MAGIC'];
    }

    get dmg(): { name: string, damage: string, incompatible?: boolean }[] {
        return this.fighter.isMonster ? this.fighter.monster.computedData.dmg : this.fighter.character.computedData.weaponsDamages;
    }

    get cou(): number {
        return this.fighter.isMonster ? this.fighter.monster.computedData.cou : this.fighter.character.computedData.stats['COU'];
    }

    get resm(): number {
        return this.fighter.isMonster ? this.fighter.monster.computedData.resm : this.fighter.character.computedData.stats['RESM'];
    }

    constructor(fighter: Fighter) {
        this.fighter = fighter;
    }
}

export class Target {
    fighter: Fighter;

    constructor(fighter: Fighter) {
        this.fighter = fighter;
    }

    get id(): number {
        return this.fighter.id;
    }

    get isMonster(): boolean {
        return this.fighter.isMonster;
    }

    get name(): string {
        return this.fighter.name;
    }

    get color(): string {
        return this.fighter.color;
    }

    get number(): number | undefined | null {
        return this.fighter.number;
    }
}

export class Fighter {
    stats: FighterStat = new FighterStat(this);
    target: Target | null;

    isMonster: boolean;
    monster: Monster;
    character: Character;

    get typeName(): 'monster' | 'character' {
        return this.isMonster ? 'monster' : 'character';
    }

    get onTargetChange(): Observable<TargetJsonData> {
        return this.isMonster ? this.monster.targetChanged : this.character.targetChanged;
    }

    constructor(element: Monster | Character) {
        if (element instanceof Monster) {
            this.isMonster = true;
            this.monster = element;
        } else {
            this.isMonster = false;
            this.character = element;
        }
        Object.freeze(this.isMonster);
    }

    get chercheNoise(): boolean {
        return this.isMonster ? this.monster.data.chercheNoise : this.character.hasFlag('ATTACK_FIRST');
    }

    get id(): number {
        return this.isMonster ? this.monster.id : this.character.id;
    }

    get uid(): string {
        return this.isMonster ? 'm_' + this.monster.id : 'c_' + this.character.id;
    }

    get active(): boolean {
        return this.isMonster ? true : this.character.active === 1;
    }

    get name(): string {
        return this.isMonster ? this.monster.name : this.character.name;
    }

    get color(): string {
        return this.isMonster ? this.monster.data.color : this.character.color;
    }

    get number(): number {
        return this.isMonster ? this.monster.data.number : 0;
    }

    get isCharacter(): boolean {
        return !this.isMonster;
    }

    changeTarget(target: TargetJsonData) {
        if (this.isMonster) {
            this.monster.changeTarget(target);
        } else {
            this.character.changeTarget(target);
        }
    }

    changeColor(color: string) {
        if (this.isMonster) {
            this.monster.data.color = color;
        } else {
            this.character.color = color;
        }
    }

    changeNumber(number: number) {
        if (this.isMonster) {
            this.monster.data.number = number;
        } else {
            return;
        }
    }

    updateTarget(fighters: Fighter[]) {
        let targetInfo: any = null;
        if (this.isMonster) {
            if (!this.monster.target) {
                this.target = null;
                return;
            }
            targetInfo = this.monster.target;
        } else {
            if (!this.character.target) {
                this.target = null;
                return;
            }
            targetInfo = this.character.target;
        }

        let fighterIndex = fighters.findIndex(f => f.id === targetInfo.id && f.isMonster === targetInfo.isMonster);
        if (fighterIndex !== -1) {
            let fighter = fighters[fighterIndex];
            this.target = new Target(fighter);
        } else {
            this.target = null;
        }
    }

    updateTime(type: string, data: number | { previous: Fighter; next: Fighter }): FighterDurationChanges | undefined {
        if (this.isMonster) {
            let changes = this.monster.updateTime(type, data);
            if (changes.length) {
                return {monsterId: this.monster.id, changes: changes};
            }
        } else {
            let changes = this.character.updateTime(type, data);
            if (changes.length) {
                return {characterId: this.character.id, changes: changes};
            }
        }
        return undefined;
    }

    updateLapDecrement(data: { deleted: Fighter, previous: Fighter; next: Fighter }): FighterDurationChanges | undefined {
        if (this.isMonster) {
            let changes = this.monster.updateLapDecrement(data);
            if (changes.length) {
                return {monsterId: this.monster.id, changes: changes};
            }
        } else {
            let changes = this.character.updateLapDecrement(data);
            if (changes.length) {
                return {characterId: this.character.id, changes: changes};
            }
        }
        return undefined;
    }
}

export class GroupConfig implements IGroupConfig {
    public allowPlayersToSeeSkillGmDetails: boolean;
    public allowPlayersToAddObject: boolean;
    public allowPlayersToSeeGemPriceWhenIdentified: boolean;
    public autoIncrementMonsterNumber: boolean;
    public autoIncrementMonsterColor: boolean;

    static fromResponse(response: IGroupConfig): GroupConfig {
        const groupConfig = new GroupConfig();
        groupConfig.allowPlayersToAddObject = response.allowPlayersToAddObject;
        groupConfig.allowPlayersToSeeSkillGmDetails = response.allowPlayersToSeeSkillGmDetails;
        groupConfig.allowPlayersToSeeGemPriceWhenIdentified = response.allowPlayersToSeeGemPriceWhenIdentified;
        groupConfig.autoIncrementMonsterNumber = response.autoIncrementMonsterNumber;
        groupConfig.autoIncrementMonsterColor = response.autoIncrementMonsterColor;
        return groupConfig;
    }
}

export class GroupData implements IGroupData {
    public debilibeuk: number;
    public mankdebol: number;
    public inCombat: boolean;
    public date: NhbkDate;
    public currentFighterIndex = 0;

    public onChange: Subject<any> = new Subject();
    public timestamp = 0;

    static fromResponse(response?: IGroupData): GroupData {
        let groupData = new GroupData();
        if (response) {
            Object.assign(groupData, response, {timestamp: date2Timestamp(response.date)});
        }
        return groupData;
    }

    // TODO: Improve typing based on string
    changeValue(key: string, value: any): boolean {
        switch (key) {
            case 'debilibeuk':
                if (value === this.debilibeuk) {
                    return false;
                }
                this.debilibeuk = value;
                break;
            case 'mankdebol':
                if (value === this.mankdebol) {
                    return false;
                }
                this.mankdebol = value;
                break;
            case 'inCombat':
                if (value === this.inCombat) {
                    return false;
                }
                this.inCombat = value;
                if (value) {
                    this.currentFighterIndex = 0;
                }
                break;
            case 'date':
                if (value === this.date) {
                    return false;
                }
                this.date = value;
                this.timestamp = date2Timestamp(this.date);
                break;
        }
        this.onChange.next({key: key, value: value});
        return true;
    }
}

export class Group extends WsRegistrable {
    public id: number;
    public name: string;
    public data: GroupData = new GroupData();
    public config: GroupConfig = new GroupConfig();

    public onNotification: Subject<any> = new Subject<any>();

    public invited: GroupInvite[] = [];
    public invites: GroupInvite[] = [];

    public characters: Character[] = [];
    public charactersById: {[characterId: number]: Character} = {}
    public characterJoining: Subject<number> = new Subject<number>();
    public characterAdded: Subject<Character> = new Subject<Character>();
    public characterRemoved: Subject<Character> = new Subject<Character>();
    public characterSubscriptions: { [characterId: number]: { change: Subscription, active: Subscription } } = {};

    public monsters: Monster[] = [];
    public monsterAdded: Subject<Monster> = new Subject<Monster>();
    public monsterRemoved: Subject<Monster> = new Subject<Monster>();
    public monsterSubscriptions: { [monsterId: number]: Subscription } = {};

    public fights: Fight[] = [];
    public fightAdded: Subject<Fight> = new Subject<Fight>();
    public fightRemoved: Subject<Fight> = new Subject<Fight>();

    public loots: Loot[] = [];
    public lootAdded: Subject<Loot> = new Subject<Loot>();
    public lootRemoved: Subject<Loot> = new Subject<Loot>();

    public events: NEvent[] = [];
    public eventAdded: Subject<NEvent> = new Subject<NEvent>();
    public eventRemoved: Subject<NEvent> = new Subject<NEvent>();

    public fighters: Fighter[] = [];
    public fightersSubscriptions: { [fighterUid: string]: Subscription } = {};

    public pendingModifierChanges?: FighterDurationChanges[];
    public characterIdWithShownItem: Set<number> = new Set<number>();

    public pastEventCount = 0;
    public futureEventCount = 0;

    static fromResponse(response: GroupResponse): Group {
        const group = new Group();
        group.id = response.id;
        group.name = response.name;
        group.data = GroupData.fromResponse(response.data);
        group.config = GroupConfig.fromResponse(response.config);
        group.invited = response.invites.filter(i => i.fromGroup);
        group.invites = response.invites.filter(i => !i.fromGroup);
        return group;
    }

    get currentFighter(): Fighter | undefined {
        if (this.fighters.length <= this.data.currentFighterIndex) {
            return undefined;
        }
        if (this.data.currentFighterIndex < 0) {
            return undefined;
        }
        return this.fighters[this.data.currentFighterIndex];
    }

    public notify(type: string, message: string, data?: any) {
        this.onNotification.next({type: type, message: message, data: data});
    }

    /**
     * Add an character to the group
     * @param addedCharacter The character to add
     * @returns {boolean} true if the character has been added (false if the character was already in)
     */
    public addCharacter(addedCharacter: Character): boolean {
        let i = this.characters.findIndex(character => character.id === addedCharacter.id);
        if (i !== -1) {
            return false;
        }
        this.charactersById[addedCharacter.id] = addedCharacter;
        this.characters.push(addedCharacter);
        this.characterAdded.next(addedCharacter);
        let changeSub = addedCharacter.onUpdate
            .subscribe((character) => {
                this.updateFightersOrder();
                this.updateCharacterShownItems(character);
            });
        let activeSub = addedCharacter.onActiveChange.subscribe((active: number) => {
            if (active) {
                this.addFighter(addedCharacter);
            } else {
                this.removeFighter(addedCharacter);
            }
            this.updateCharacterShownItems(addedCharacter);
            this.updateFightersOrder();
        });
        if (addedCharacter.active) {
            this.addFighter(addedCharacter);
        }
        this.characterSubscriptions[addedCharacter.id] = {
            active: activeSub,
            change: changeSub
        };
        if (this.wsSubscribtion) {
            this.wsSubscribtion.service.registerElement(addedCharacter);
        }
        this.updateShownItems();
        return true;
    }

    /**
     * Delete an character from the group
     * @param removedCharacterId The id of the character to remove
     * @returns {boolean} true if character was removed (false if character was not present)
     */
    public removeCharacter(removedCharacterId: number): boolean {
        let i = this.characters.findIndex(character => character.id === removedCharacterId);
        if (i !== -1) {
            let removedCharacter = this.characters[i];
            delete this.charactersById[removedCharacter.id];
            this.characters.splice(i, 1);
            this.characterRemoved.next(removedCharacter);
            this.characterSubscriptions[removedCharacter.id].active.unsubscribe();
            this.characterSubscriptions[removedCharacter.id].change.unsubscribe();
            delete this.characterSubscriptions[removedCharacter.id];
            this.removeFighter(removedCharacter);
            if (this.wsSubscribtion) {
                this.wsSubscribtion.service.unregisterElement(removedCharacter);
            }
            return true;
        }
        return false;
    }

    /**
     * Add a monster to the group
     * @param addedMonster The monster to add
     * @returns {boolean} true if the monster has been added (false if the monster was already in)
     */
    public addMonster(addedMonster: Monster): boolean {
        let i = this.monsters.findIndex(monster => monster.id === addedMonster.id);
        if (i !== -1) {
            return false;
        }
        this.monsters.push(addedMonster);
        this.monsterAdded.next(addedMonster);
        this.monsterSubscriptions[addedMonster.id] = addedMonster.onChange
            .subscribe(() => this.updateFightersOrder());
        this.addFighter(addedMonster);
        this.updateFightersOrder();
        if (this.wsSubscribtion) {
            this.wsSubscribtion.service.registerElement(addedMonster);
        }
        return true;
    }

    addFight(addedFight: Fight) {
        let i = this.fights.findIndex(fight => fight.id === addedFight.id);
        if (i !== -1) {
            return false;
        }
        this.fights.push(addedFight);
        this.fightAdded.next(addedFight);
        if (this.wsSubscribtion) {
            this.wsSubscribtion.service.registerElement(addedFight);
        }
        return true;
    }

    removeFight(fightId: number) {
        let i = this.fights.findIndex(fight => fight.id === fightId);
        if (i === -1) {
            return false;
        }
        let removedFight = this.fights.splice(i, 1)[0];
        this.fightRemoved.next(removedFight);

        return true;
    }

    /**
     * Delete an monster from the group
     * @param monsterId The id of the monster to remove
     * @returns {boolean} true if monster was removed (false if monster was not present)
     */
    public removeMonster(monsterId: number): boolean {
        let i = this.monsters.findIndex(monster => monster.id === monsterId);
        if (i !== -1) {
            let removedMonster = this.monsters[i];
            this.monsters.splice(i, 1);
            this.monsterRemoved.next(removedMonster);
            this.monsterSubscriptions[removedMonster.id].unsubscribe();
            delete this.monsterSubscriptions[removedMonster.id];
            this.removeFighter(removedMonster);
            if (this.wsSubscribtion) {
                this.wsSubscribtion.service.unregisterElement(removedMonster);
            }
            return true;
        }
        return false;
    }

    /**
     * Add an loot to the group
     * @param addedLoot The loot to add
     * @returns {boolean} true if the loot has been added (false if the loot was already in)
     */
    public addLoot(addedLoot: Loot): boolean {
        let i = this.loots.findIndex(loot => loot.id === addedLoot.id);
        if (i !== -1) {
            return false;
        }
        this.loots.unshift(addedLoot);
        this.lootAdded.next(addedLoot);
        if (this.wsSubscribtion) {
            this.wsSubscribtion.service.registerElement(addedLoot);
        }
        return true;
    }

    /**
     * Delete an loot from the group
     * @param lootId The id of the loot to remove
     * @returns {boolean} true if loot was removed (false if loot was not present)
     */
    public removeLoot(lootId: number): boolean {
        let i = this.loots.findIndex(loot => loot.id === lootId);
        if (i !== -1) {
            let removedLoot = this.loots[i];
            this.loots.splice(i, 1);
            this.lootRemoved.next(removedLoot);
            if (this.wsSubscribtion) {
                this.wsSubscribtion.service.unregisterElement(removedLoot);
            }
            return true;
        }
        return false;
    }

    /**
     * Add an event to the group
     * @param addedEvent The event to add
     * @returns {boolean} true if the event has been added (false if the event was already in)
     */
    public addEvent(addedEvent: NEvent): boolean {
        let i = this.events.findIndex(event => event.id === addedEvent.id);
        if (i !== -1) {
            return false;
        }
        this.events.push(addedEvent);
        this.eventAdded.next(addedEvent);
        this.updateEvents();
        return true;
    }

    /**
     * Delete an event from the group
     * @param eventId The id of the event to remove
     * @returns {boolean} true if event was removed (false if event was not present)
     */
    public removeEvent(eventId: number): boolean {
        let i = this.events.findIndex(event => event.id === eventId);
        if (i !== -1) {
            let removedEvent = this.events[i];
            this.events.splice(i, 1);
            this.eventRemoved.next(removedEvent);
            this.updateEvents();
            return true;
        }
        return false;
    }

    public updateEvents() {
        this.events.sort((a: NEvent, b: NEvent) => {
            return a.timestamp - b.timestamp;
        });

        let pastEventCount = 0;
        let futureEventCount = 0;
        for (let i = 0; i < this.events.length; i++) {
            let event = this.events[i];
            if (event.timestamp <= this.data.timestamp) {
                pastEventCount++;
            } else {
                futureEventCount++;
            }
        }
        this.pastEventCount = pastEventCount;
        this.futureEventCount = futureEventCount;
    }

    private addFighter(element: Monster | Character) {
        let fighter = new Fighter(element);
        this.fighters.push(fighter);
        this.fightersSubscriptions[fighter.uid] = fighter.onTargetChange.subscribe(() => {
            fighter.updateTarget(this.fighters);
        });
        this.updateFightersOrder();
        this.fighters.forEach(f => f.updateTarget(this.fighters));
    }

    private removeFighter(element: Monster | Character) {
        let isMonster = element instanceof Monster;
        let i = this.fighters.findIndex(f => f.isMonster === isMonster && f.id === element.id);
        if (i !== -1) {
            let fighter = this.fighters[i];
            let pi = i === 0 ? this.fighters.length - 1 : i - 1;
            let ni = i === this.fighters.length - 1 ? 0 : i + 1;
            let previousFighter = this.fighters[pi];
            let nextFighter = this.fighters[ni];
            this.fightersSubscriptions[fighter.uid].unsubscribe();
            this.fighters.splice(i, 1);
            this.pendingModifierChanges = this.updateLapDecrement({
                deleted: fighter,
                previous: previousFighter,
                next: nextFighter
            });
        }
        this.fighters.forEach(f => f.updateTarget(this.fighters));
        if (this.data.currentFighterIndex >= this.fighters.length) {
            this.nextLap();
        }
    }

    /**
     * Update order of fighters, order is not updated during combat except if param force is at true
     * @param force to ignore chekc if group is in combat
     */
    public updateFightersOrder(force?: boolean) {
        if (this.data.inCombat && !force) {
            return;
        }
        this.fighters.sort((first: Fighter, second: Fighter) => {
            if (first.chercheNoise && !second.chercheNoise) {
                return -1;
            } else if (!first.chercheNoise && second.chercheNoise) {
                return 1;
            } else {
                let cou1 = first.stats.cou;
                let cou2 = second.stats.cou;

                if (cou1 > cou2) {
                    return -1;
                } else if (cou1 < cou2) {
                    return 1;
                } else {
                    let ad1 = first.stats.ad;
                    let ad2 = second.stats.ad;

                    if (ad1 > ad2) {
                        return -1;
                    } else if (ad1 < ad2) {
                        return 1;
                    } else {
                        return 0;
                    }
                }
            }
        });
    }

    public nextFighter(): { modifiersDurationUpdated: any[], fighterIndex: number } | undefined {
        if (this.fighters.length < 2) {
            return undefined;
        }

        let previousFighter: Fighter;
        if (this.data.currentFighterIndex === -1) {
            previousFighter = this.fighters[this.fighters.length - 1];
        } else {
            previousFighter = this.fighters[this.data.currentFighterIndex];
        }
        this.data.currentFighterIndex++;
        if (this.data.currentFighterIndex >= this.fighters.length) {
            this.nextLap();
        }

        let changes = this.updateTime('lap', {
            previous: previousFighter,
            next: this.fighters[this.data.currentFighterIndex]
        });

        return {modifiersDurationUpdated: changes, fighterIndex: this.data.currentFighterIndex};
    }

    public nextLap() {
        this.updateFightersOrder(true);
        this.data.currentFighterIndex = 0;
    }

    public updateTime(type: string, data: number | { previous: Fighter; next: Fighter }): any[] {
        let changes: any[] = [];
        for (let fighter of this.fighters) {
            let fighterChanges = fighter.updateTime(type, data);
            if (fighterChanges) {
                changes.push(fighterChanges);
            }
        }
        return changes;
    }

    public updateLapDecrement(data: { deleted: Fighter, previous: Fighter; next: Fighter }): FighterDurationChanges[] {
        let changes: FighterDurationChanges[] = [];
        for (let fighter of this.fighters) {
            let fighterChanges = fighter.updateLapDecrement(data);
            if (fighterChanges) {
                changes.push(fighterChanges);
            }
        }
        return changes;
    }

    public onAddInvite(data: GroupInvite) {
        if (data.fromGroup) {
            if (this.invited.findIndex(d => d.id === data.id) === -1) {
                this.invited.push(data);
            }
        } else {
            if (this.invites.findIndex(d => d.id === data.id) === -1) {
                this.invites.push(data);
            }
        }
    }

    public onCancelInvite(data: DeleteInviteResponse): void {
        if (data.fromGroup) {
            let i = this.invited.findIndex(d => d.id === data.characterId);
            if (i !== -1) {
                this.invited.splice(i, 1);
            }
        } else {
            let i = this.invites.findIndex(d => d.id === data.characterId);
            if (i !== -1) {
                this.invites.splice(i, 1);
            }
        }
    }

    public getWsTypeName(): string {
        return 'group';
    }

    public onWsRegister(service: WebSocketService) {
        for (let loot of this.loots) {
            service.registerElement(loot);
        }
        for (let monster of this.monsters) {
            service.registerElement(monster);
        }
        for (let character of this.characters) {
            service.registerElement(character);
        }
        for (let fight of this.fights) {
            service.registerElement(fight);
        }
    }

    public onWsUnregister(): void {
        if (!this.wsSubscribtion) {
            return;
        }
        for (let loot of this.loots) {
            this.wsSubscribtion.service.unregisterElement(loot);
        }
        for (let monster of this.monsters) {
            this.wsSubscribtion.service.unregisterElement(monster);
        }
        for (let character of this.characters) {
            this.wsSubscribtion.service.unregisterElement(character);
        }
        for (let fight of this.fights) {
            this.wsSubscribtion.service.unregisterElement(fight);
        }
    }

    handleWebsocketEvent(opcode: string, data: any, services: WsEventServices) {
        switch (opcode) {
            case 'addLoot': {
                services.skill.getSkillsById().subscribe(skillsById => {
                    this.addLoot(Loot.fromResponse(data, skillsById));
                });
                break;
            }
            case 'deleteLoot': {
                this.removeLoot(data);
                break;
            }
            case 'changeConfig': {
                this.updateConfig(GroupConfig.fromResponse(data));
                break;
            }
            case 'addEvent': {
                this.addEvent(NEvent.fromResponse(data));
                break;
            }
            case 'deleteEvent': {
                this.removeEvent(data.id);
                break;
            }
            case 'changeData': {
                for (let key in data) {
                    if (!data.hasOwnProperty(key)) {
                        continue;
                    }
                    this.data.changeValue(key, data[key]);
                }
                this.updateEvents();
                break;
            }
            case 'joinCharacter': {
                this.characterJoining.next(data);
                let i = this.invited.findIndex(d => d.id === data);
                if (i !== -1) {
                    this.invited.splice(i, 1);
                }
                let j = this.invites.findIndex(d => d.id === data);
                if (j !== -1) {
                    this.invites.splice(j, 1);
                }
                break;
            }
            case 'removeCharacter': {
                this.removeCharacter(data.characterId);
                break;
            }
            case 'groupInvite': {
                this.onAddInvite(data);
                break;
            }
            case 'cancelInvite': {
                this.onCancelInvite(data);
                break;
            }
            case 'addMonster': {
                services.skill.getSkillsById().subscribe(skillsById => {
                    this.addMonster(Monster.fromResponse(data, skillsById));
                });
                break;
            }
            case 'killMonster': {
                this.removeMonster(data);
                break;
            }
            case 'deleteMonster': {
                this.removeMonster(data);
                break;
            }
            case 'deleteFight': {
                this.removeFight(data);
                break;
            }
            case 'addFight': {
                services.skill.getSkillsById().subscribe(skillsById => {
                    let fight = Fight.fromResponse(data, skillsById);
                    this.addFight(fight);
                });
                break;
            }
            default: {
                console.warn('Opcode not handle: `' + opcode + '`');
                break;
            }
        }
    }

    dispose() {
        for (let character of this.characters) {
            if (character.id in this.characterSubscriptions) {
                this.characterSubscriptions[character.id].active.unsubscribe();
                this.characterSubscriptions[character.id].change.unsubscribe();
            }
            character.dispose();
        }

        for (let monster of this.monsters) {
            if (monster.id in this.monsterSubscriptions) {
                this.monsterSubscriptions[monster.id].unsubscribe();
            }
            monster.dispose();
        }

        for (let fight of this.fights) {
            fight.dispose();
        }

        for (let fighter of this.fighters) {
            if (fighter.uid in this.fightersSubscriptions) {
                this.fightersSubscriptions[fighter.uid].unsubscribe();
            }
        }

        this.characterAdded.unsubscribe();
        this.characterRemoved.unsubscribe();

        this.monsterAdded.unsubscribe();
        this.monsterRemoved.unsubscribe();

        this.lootAdded.unsubscribe();
        this.lootRemoved.unsubscribe();

        this.eventAdded.unsubscribe();
        this.eventRemoved.unsubscribe();
    }

    private updateShownItems() {
        for (let character of this.characters) {
            this.updateCharacterShownItems(character);
        }
    }

    private updateCharacterShownItems(character: Character) {
        if (character.active && character.computedData.shownItemsToGm.length) {
            this.characterIdWithShownItem.add(character.id);
        } else {
            this.characterIdWithShownItem.delete(character.id);
        }
    }

    private updateConfig(config: GroupConfig) {
        this.config = config;
    }

    getColorAndNumberForNewMonster(previousMonster?: Monster): [string, number] {
        if (!previousMonster && this.monsters.length) {
            previousMonster = this.monsters[this.monsters.length - 1];
        }
        if (!previousMonster) {
            if (this.config.autoIncrementMonsterNumber) {
                return ['000000', 1];
            }
            return ['000000', 0];
        }

        if (this.config.autoIncrementMonsterNumber && this.config.autoIncrementMonsterColor) {
            return this.getNextFreeMonsterColorAndNumber(previousMonster.data.color, previousMonster.data.number);
        }
        else if (this.config.autoIncrementMonsterNumber) {
            const number = this.getNextFreeMonsterNumber(previousMonster.data.color, previousMonster.data.number);
            return [previousMonster?.data?.color || '000000', number];
        }
        else if (this.config.autoIncrementMonsterNumber) {
            return [this.getNextFreeMonsterColor(previousMonster.data.color), 0];
        }
        return [previousMonster?.data?.color || '000000', previousMonster?.data?.number || 0];
    }


    private getNextFreeMonsterNumber(color: string, fromNumber: number): number {
        let monstersSameColor = this.monsters.filter(x => x.data.color === color);
        for (let i = 1; i < 12; i++) {
            let number = ((fromNumber + i) % 12)
            if (number === 0) {
                continue;
            }
            if (monstersSameColor.find(m => m.data.number === number)) {
                continue
            }
            return number;
        }
        return 0;
    }

    private getNextFreeMonsterColorAndNumber(fromColor: string, fromNumber: number): [string, number] {
        let currentColorIndex = tokenColors.indexOf(fromColor);
        if (currentColorIndex === -1) {
            currentColorIndex = 0;
        }

        for (let c = 0; c < tokenColors.length; c++) {
            let color = tokenColors[(c + currentColorIndex) % tokenColors.length];
            for (let i = 0; i < 12; i++) {
                let number = ((fromNumber + i) % 12)
                if (number === 0) {
                    break;
                }
                if (this.monsters.find(x => x.data.color === color && x.data.number === number)) {
                    continue;
                }
                return [color, number];
            }
            fromNumber = 1;
        }

        return [tokenColors[tokenColors.length - 1], 0];
    }

    private getNextFreeMonsterColor(fromColor: string): string {
        let currentColorIndex = tokenColors.indexOf(fromColor);
        if (currentColorIndex === -1) {
            currentColorIndex = 0;
        }
        for (let c = 0; c < tokenColors.length; c++) {
            let color = tokenColors[(c + currentColorIndex) % tokenColors.length];

            if (this.monsters.find(x => x.data.color === color)) {
                continue;
            }
            return color;
        }
        return '000000';
    }

    moveMonsterToFight(monster: Monster, fightId?: number) {
        if (monster.fightId === fightId)
            return;

        let actualFight = this.fights.find(f => f.id === monster.fightId);
        let targetFight = this.fights.find(f => f.id === fightId)
        if (actualFight) {
            monster.fightId = fightId;
            if (!targetFight) {
                // Move to active combat
                actualFight.removeMonster(monster.id);
                this.addMonster(monster);
            }
            else {
                // Move to another fight
                actualFight.removeMonster(monster.id);
                targetFight.addMonster(monster);
            }
        }
        else if (targetFight) {
            // Move from active combat to another fight
            monster.fightId = fightId;
            this.removeMonster(monster.id);
            targetFight.addMonster(monster);
        }
    }
}

export class NpcData implements INpcData {
    public location?: string;
    public note?: string;
    public sex?: string;
    public originName?: string;

    static fromResponse(response: INpcData): NpcData {
        const npcData = new NpcData();

        npcData.location = response.location;
        npcData.note = response.note;
        npcData.sex = response.sex;
        npcData.originName = response.originName;

        return npcData;
    }
}

export class Npc {
    public id: number;
    public name: string;
    public data: NpcData;

    static fromResponse(response: NpcResponse): Npc {
        const npc = new Npc();
        npc.data = NpcData.fromResponse(response.data);
        npc.name = response.name;
        npc.id = response.id;
        return npc;
    }

    static fromResponses(responses: NpcResponse[]): Npc[] {
        return responses.map(response => Npc.fromResponse(response));
    }
}

export interface GroupInviteResponse {
    id: number;
    jobs: string[];
    name: string;
    origin: string;
    fromGroup: boolean;
    groupId: number;
    groupName: string;
}

export interface GroupInvite {
    id: number;
    jobs: string[];
    name: string;
    origin: string;
    fromGroup: boolean;
}

