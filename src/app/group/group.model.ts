import {Monster} from '../monster';
import {Character} from '../character';
import {Location} from '../location';
import {NhbkDate} from '../date';
import {Subscription} from 'rxjs/Subscription';
import {Subject} from 'rxjs/Subject';
import {WsRegistrable} from '../websocket/websocket.model';
import {Loot} from '../loot/loot.model';
import {NEvent} from '../event/event.model';
import {date2Timestamp} from '../date/util';
import {WebSocketService} from '../websocket/websocket.service';
import {TargetJsonData} from './target.model';
import {Observable} from 'rxjs/Observable';
import {isNullOrUndefined} from 'util';
import {WsEventServices} from '../websocket';
import {CharacterJsonData} from '../character/character.model';
import {Skill} from '../skill';
import {Job} from '../job/job.model';
import {Origin} from '../origin';

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
    get dmg(): {name: string, damage: string, incompatible?: boolean}[] {
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
    get number(): number|undefined|null {
        return this.fighter.number;
    }
}

export class Fighter {
    stats: FighterStat = new FighterStat(this);
    target: Target|null;

    isMonster: boolean;
    monster: Monster;
    character: Character;

    get onTargetChange(): Observable<TargetJsonData> {
        return this.isMonster ? this.monster.targetChanged : this.character.targetChanged;
    }

    constructor(element: Monster|Character) {
        if (element instanceof Monster) {
            this.isMonster = true;
            this.monster = element;
        }
        else {
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
        }
        else {
            this.target = null;
        }
    }

    updateTime(type: string, data: number | { previous: Fighter; next: Fighter }): any {
        if (this.isMonster) {
            let changes = this.monster.updateTime(type, data);
            if (changes.length) {
                return {isMonster: true, monsterId: this.monster.id, changes: changes};
            }
        } else {
            let changes = this.character.updateTime(type, data);
            if (changes.length) {
                return {isMonster: false, characterId: this.character.id, changes: changes};
            }
        }
        return undefined;
    }

    updateLapDecrement(data: { deleted: Fighter, previous: Fighter; next: Fighter }): any {
        if (this.isMonster) {
            let changes = this.monster.updateLapDecrement(data);
            if (changes.length) {
                return {isMonster: true, monsterId: this.monster.id, changes: changes};
            }
        } else {
            let changes = this.character.updateLapDecrement(data);
            if (changes.length) {
                return {isMonster: false, characterId: this.character.id, changes: changes};
            }
        }
        return undefined;
    }
}

export class GroupData {
    public debilibeuk: number;
    public mankdebol: number;
    public inCombat: boolean;
    public date: NhbkDate;
    public currentFighterIndex = 0;

    public onChange: Subject<any> = new Subject();
    public timestamp = 0;

    static fromJson(jsonData: any): GroupData {
        let groupData = new GroupData();
        if (jsonData) {
            Object.assign(groupData, jsonData, {timestamp: date2Timestamp(jsonData.date)});
        }
        return groupData;
    }

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

export class GroupJsonData {
    id: number;
    name: string;
    data: GroupData;
    location: Location;
    invited: CharacterInviteInfo[];
    invites: CharacterInviteInfo[];
    characters: { id: number }[];
}

export class PartialGroup {
    id: number;
    name: string;
    characterCount: number;
}

export class Group extends WsRegistrable {
    public id: number;
    public name: string;
    public data: GroupData = new GroupData();
    public location: Location;

    public onNotification: Subject<any> = new Subject<any>();

    public invited: CharacterInviteInfo[] = [];
    public invites: CharacterInviteInfo[] = [];

    public characters: Character[] = [];
    public characterAdded: Subject<Character> = new Subject<Character>();
    public characterRemoved: Subject<Character> = new Subject<Character>();
    public characterSubscriptions: {[characterId: number]: {change: Subscription, active: Subscription}} = {};

    public monsters: Monster[] = [];
    public monsterAdded: Subject<Monster> = new Subject<Monster>();
    public monsterRemoved: Subject<Monster> = new Subject<Monster>();
    public monsterSubscriptions: {[monsterId: number]: Subscription} = {};

    public loots: Loot[] = [];
    public lootAdded: Subject<Loot> = new Subject<Loot>();
    public lootRemoved: Subject<Loot> = new Subject<Loot>();

    public events: NEvent[] = [];
    public eventAdded: Subject<NEvent> = new Subject<NEvent>();
    public eventRemoved: Subject<NEvent> = new Subject<NEvent>();

    public fighters: Fighter[] = [];
    public fightersSubscriptions: {[fighterUid: string]: Subscription} = {};

    public pendingModifierChanges: any[]|null;

    get currentFighter(): Fighter|undefined {
        if (this.fighters.length <= this.data.currentFighterIndex) {
            return undefined;
        }
        if (this.data.currentFighterIndex < 0) {
            return undefined;
        }
        return this.fighters[this.data.currentFighterIndex];
    }

    public pastEventCount = 0;
    public futureEventCount = 0;

    static fromJson(jsonData: GroupJsonData): Group {
        let group = new Group();
        Object.assign(group, jsonData, {data: GroupData.fromJson(jsonData.data), characters: []});
        return group;
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
        this.characters.push(addedCharacter);
        this.characterAdded.next(addedCharacter);
        let changeSub = addedCharacter.onUpdate
            .subscribe(() => this.updateFightersOrder());
        let activeSub = addedCharacter.onActiveChange.subscribe((active: number) => {
            if (active) {
                this.addFighter(addedCharacter);
            }
            else {
                this.removeFighter(addedCharacter);
            }
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
     * Add an monster to the group
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
        this.loots.push(addedLoot);
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

    private addFighter(element: Monster|Character) {
        let fighter = new Fighter(element);
        this.fighters.push(fighter);
        this.fightersSubscriptions[fighter.uid] = fighter.onTargetChange.subscribe(() => {
            fighter.updateTarget(this.fighters);
        });
        this.updateFightersOrder();
        this.fighters.forEach(f => f.updateTarget(this.fighters));
    }

    private removeFighter(element: Monster|Character) {
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
        if (this.data.currentFighterIndex > this.fighters.length) {
            this.data.currentFighterIndex = -1;
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
            }
            else if (!first.chercheNoise && second.chercheNoise) {
                return 1;
            }
            else {
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

    public nextFighter(): {modifiersDurationUpdated: any[], fighterIndex: number} | undefined {
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

    public updateTime(type: string, data: number|{previous: Fighter; next: Fighter}): any[] {
        let changes: any[] = [];
        for (let fighter of this.fighters) {
            let fighterChanges = fighter.updateTime(type, data);
            if (fighterChanges) {
                changes.push(fighterChanges);
            }
        }
        return changes;
    }

    public updateLapDecrement(data: { deleted: Fighter, previous: Fighter; next: Fighter }): any[] {
        let changes: any[] = [];
        for (let fighter of this.fighters) {
            let fighterChanges = fighter.updateLapDecrement(data);
            if (fighterChanges) {
                changes.push(fighterChanges);
            }
        }
        return changes;
    }

    public onAddInvite(data: any) {
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

    public onCancelInvite(data: any): void {
        if (data.fromGroup) {
            let i = this.invited.findIndex(d => d.id === data.character.id);
            if (i !== -1) {
                this.invited.splice(i, 1);
            }
        }
        else {
            let i = this.invites.findIndex(d => d.id === data.character.id);
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
    }

    handleWebsocketEvent(opcode: string, data: any, services: WsEventServices) {
        switch (opcode) {
            case 'addLoot': {
                services.skill.getSkillsById().subscribe(skillsById => {
                    this.addLoot(Loot.fromJson(data, skillsById));
                });
                break;
            }
            case 'deleteLoot': {
                this.removeLoot(data.id);
                break;
            }
            case 'addEvent': {
                this.addEvent(NEvent.fromJson(data));
                break;
            }
            case 'deleteEvent': {
                this.removeEvent(data.id);
                break;
            }
            case 'changeData': {
                this.data.changeValue(data.key, data.value);
                if (data.key === 'date') {
                    this.updateEvents();
                }
                break;
            }
            case 'joinCharacter': {
                let characterData: CharacterJsonData = data;
                Observable.forkJoin(
                    services.job.getJobList(),
                    services.origin.getOriginList(),
                    services.skill.getSkillsById()
                ).subscribe(([jobs, origins, skillsById]: [Job[], Origin[], {[skillId: number]: Skill}]) => {
                    let character = Character.fromJson(characterData, origins, jobs, skillsById);

                    try {
                        character.update();
                    } catch (e) {
                        console.log(e, e.stack);
                        throw e;
                    }
                    this.addCharacter(character);
                });

                let i = this.invited.findIndex(d => d.id === characterData.id);
                if (i !== -1) {
                    this.invited.splice(i, 1);
                }
                let j = this.invites.findIndex(d => d.id === characterData.id);
                if (j !== -1) {
                    this.invites.splice(j, 1);
                }
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
}

export interface CharacterInviteInfo {
    id: number;
    jobs: string[];
    name: string;
    origin: string;
}
