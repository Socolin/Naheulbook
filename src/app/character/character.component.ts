import {Component, Input, OnInit, OnDestroy, ViewChild} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {MdTabChangeEvent, Portal, OverlayRef, OverlayState, Overlay} from "@angular/material";

import {NotificationsService} from '../notifications';

import {CharacterService} from './character.service';
import {Character} from './character.model';
import {IMetadata} from '../shared';
import {CharacterWebsocketService} from './character-websocket.service';
import {SwipeService} from './swipe.service';
import {ItemActionService} from './item-action.service';
import {Item} from './item.model';
import {Skill} from '../skill/skill.model';

export class LevelUpInfo {
    EVorEA: string = 'EV';
    EVorEAValue: number;
    targetLevelUp: number;
    statToUp: string;
    skill: Skill;
    speciality: any;
}

@Component({
    selector: 'character',
    templateUrl: './character.component.html',
    styleUrls: ['./character.component.scss'],
    providers: [CharacterWebsocketService, SwipeService, ItemActionService],
})
export class CharacterComponent implements OnInit, OnDestroy {
    @Input() id: number;
    @Input() character: Character;

    @ViewChild('levelUpDialog')
    public levelUpDialog: Portal<any>;
    public levelUpOverlayRef: OverlayRef;

    public levelUpInfo: LevelUpInfo = new LevelUpInfo();
    public inGroupTab: boolean = false;
    public selectedItem: Item;
    public currentTab: string = 'infos';
    public currentTabIndex: number = 0;
    public tabs: any[] = [
        {hash: 'infos'},
        {hash: 'combat'},
        {hash: 'statistics'},
        {hash: 'inventory'},
        {hash: 'effects'},
        {hash: 'loot'},
        {hash: 'other'},
        {hash: 'history'},
    ];

    constructor(private _route: ActivatedRoute
        , private _notification: NotificationsService
        , private _characterWebsocketService: CharacterWebsocketService
        , private _overlay: Overlay
        , private _characterService: CharacterService) {
    }


    notifyChange(message: string) {
        if (this.inGroupTab) {
            this._notification.info(this.character.name
                , message
                , {isCharacter: true, color: this.character.color}
            );
        } else {
            this._notification.info('Modification', message);
        }
    }

    registerWS() {
        this._characterWebsocketService.register(this.character);
        this._characterWebsocketService.registerNotifyFunction(this.notifyChange.bind(this));

        this._characterWebsocketService.registerPacket('update').subscribe(this.onChangeCharacterStat.bind(this));
        this._characterWebsocketService.registerPacket('statBonusAd').subscribe(this.onSetStatBonusAD.bind(this));
        this._characterWebsocketService.registerPacket('levelUp').subscribe(this.onLevelUp.bind(this));
   }

    changeCharacterStat(stat: string, value: any) {
        this._characterService.changeCharacterStat(this.character.id, stat, value).subscribe(
            this.onChangeCharacterStat.bind(this)
        );
    }

    onChangeCharacterStat(change: any) {
        if (this.character[change.stat] !== change.value) {
            this.notifyChange(change.stat.toUpperCase() + ': ' + this.character[change.stat] + ' -> ' + change.value);
            this.character[change.stat] = change.value;
            this.character.update();
        }
    }

    setStatBonusAD(id: number, stat: string) {
        if (this.character) {
            this._characterService.setStatBonusAD(id, stat).subscribe(
                this.onSetStatBonusAD.bind(this)
            );
        }
    }

    onSetStatBonusAD(bonusStat: any) {
        if (this.character.statBonusAD !== bonusStat) {
            this.notifyChange('Stat bonus/malus AD défini sur ' + bonusStat);
            this.character.statBonusAD = bonusStat;
            this.character.update();
        }
    }

    levelUp() {
        this.closeLevelUpDialog();
        this._characterService.LevelUp(this.character.id, this.levelUpInfo).subscribe(this.onLevelUp.bind(this));
    }

    onLevelUp(result: any) {
        if (this.character.level !== result.level) {
            this.notifyChange('Levelup ! ' + this.character.level + '->' + result.level);
            this.character.level = result.level;
            this._characterService.getCharacter(result.id).subscribe(
                character => {
                    character.onUpdate = this.character.onUpdate;
                    this.character = character;
                    character.update();
                }
            );
        }
    }

    characterHasToken(token: string) {
        if (this.character.origin.specials) {
            if (this.character.origin.specials.indexOf(token) !== -1) {
                return true;
            }
        }
        if (this.character.job) {
            if (this.character.job.specials) {
                if (this.character.job.specials.indexOf(token) !== -1) {
                    return true;
                }
            }
        }
        if (this.character.specialities) {
            for (let i = 0; i < this.character.specialities.length; i++) {
                let speciality = this.character.specialities[i];
                if (speciality.specials) {
                    for (let j = 0; j < speciality.specials.length; j++) {
                        let special = speciality.specials[j];
                        if (special.token === token) {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    changeGmData(key: string, value: any) {
        this._characterService.changeGmData(this.character.id, key, value).subscribe(
            change => {
                this._notification.info('Modification', key + ': ' + this.character.gmData[change.key] + ' -> ' + change.value);
                this.character.gmData[change.key] = change.value;
            }
        );
    }

    canLevelUp(): boolean {
        return this.character.level < this.getLevelForXp();
    }

    getXpForNextLevel() {
        let level = 1;
        let totalXp = 0;
        let xp = this.character.experience;
        while (xp >= level * 100) {
            xp -= level * 100;
            totalXp += level * 100;
            level++;
        }

        totalXp += level * 100;

        return totalXp;
    }

    getLevelForXp(): number {
        let level = 1;
        let xp = this.character.experience;
        while (xp >= level * 100) {
            xp -= level * 100;
            level++;
        }
        return level;
    }

    openLevelUpDialog() {
        this.initLevelUp();

        let config = new OverlayState();

        config.positionStrategy = this._overlay.position()
            .global()
            .centerHorizontally()
            .centerVertically();
        config.hasBackdrop = true;

        let overlayRef = this._overlay.create(config);
        overlayRef.attach(this.levelUpDialog);
        overlayRef.backdropClick().subscribe(() => overlayRef.detach());
        this.levelUpOverlayRef = overlayRef;
    }

    closeLevelUpDialog() {
        this.levelUpOverlayRef.detach();
    }

    initLevelUp() {
        this.levelUpInfo = new LevelUpInfo();
        this.levelUpInfo.EVorEA = 'EV';
        this.levelUpInfo.EVorEAValue = null;
        this.levelUpInfo.targetLevelUp = this.character.level + 1;
        if (this.levelUpInfo.targetLevelUp % 2 === 0) {
            this.levelUpInfo.statToUp = 'FO';
        }
        else {
            this.levelUpInfo.statToUp = 'AT';
        }
    }

    rollLevelUp() {
        let diceLevelUp = this.character.origin.diceEVLevelUp;
        if (this.levelUpInfo.EVorEA === 'EV') {
            if (this.characterHasToken('LEVELUP_DICE_EV_-1')) {
                this.levelUpInfo.EVorEAValue = Math.max(1, Math.ceil(Math.random() * diceLevelUp) - 1);
                return;
            }
        } else {
            diceLevelUp = this.character.job.diceEaLevelUp;
        }
        this.levelUpInfo.EVorEAValue = Math.ceil(Math.random() * diceLevelUp);
    }

    onLevelUpSelectSkills(skills: Skill[]) {
        this.levelUpInfo.skill = skills[0];
    }

    levelUpShouldSelectSkill() {
        return this.levelUpInfo.targetLevelUp === 3
            || this.levelUpInfo.targetLevelUp === 6
            || this.levelUpInfo.targetLevelUp === 10;
    }

    levelUpShouldSelectSpeciality() {
        return this.characterHasToken('SELECT_SPECIALITY_LVL_5_10')
            && !this.characterHasToken('ONE_SPECIALITY')
            && (this.levelUpInfo.targetLevelUp === 5 || this.levelUpInfo.targetLevelUp === 10);
    }

    levelUpSelectSpeciality(speciality) {
        if (this.levelUpShouldSelectSpeciality()) {
            this.levelUpInfo.speciality = speciality;
        }
    }

    levelUpFormReady() {
        if (!this.levelUpInfo.EVorEAValue) {
            return false;
        }
        if (this.levelUpShouldSelectSpeciality()) {
            if (!this.levelUpInfo.speciality) {
                return false;
            }
        }
        if (this.levelUpShouldSelectSkill()) {
            if (!this.levelUpInfo.skill) {
                return false;
            }
        }
        return true;
    }

    // Group

    cancelInvite(group) {
        this._characterService.cancelInvite(this.character.id, group.id).subscribe(
            res => {
                for (let i = 0; i < this.character.invites.length; i++) {
                    let e = this.character.invites[i];
                    if (e.id === res.group.id) {
                        this.character.invites.splice(i, 1);
                        break;
                    }
                }
            }
        );
        return false;
    }

    acceptInvite(group: IMetadata) {
        this._characterService.joinGroup(this.character.id, group.id).subscribe(
            res => {
                this.character.invites = [];
                this.character.group = res.group;
            }
        );
        return false;
    }

    // Tab

    getTabIndexFromHash(hash: string): number {
        return this.tabs.findIndex(t => t.hash === hash);
    }

    selectTab(tabChangeEvent: MdTabChangeEvent): boolean {
        this.currentTab = this.tabs[tabChangeEvent.index].hash;
        if (!this.inGroupTab) {
            window.location.hash = this.currentTab;
        }
        return false;
    }

    ngOnDestroy() {
        if (this.character) {
            this._characterWebsocketService.unregister();
        }
    }

    ngOnInit() {
        if (this.character) {
            this.inGroupTab = true;
            this.registerWS();
        } else {
            this._route.params.subscribe(
                param => {
                    let id = this.id;
                    if (!this.id) {
                        id = +param['id'];
                    }
                    this._characterService.getCharacter(id).subscribe(
                        character => {
                            this.character = character;
                            this.registerWS();
                        }
                    );
                }
            );
        }
        if (!this.inGroupTab) {
            this._route.fragment.subscribe(value => {
                if (value) {
                    this.currentTabIndex = this.getTabIndexFromHash(value);
                    this.currentTab = value;
                }
            });
        }
    }
}
