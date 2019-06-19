import {Component, ElementRef, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {MatTabChangeEvent, MatTabGroup} from '@angular/material';
import {Overlay, OverlayConfig, OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';
import {Subscription} from 'rxjs';

import {NotificationsService} from '../notifications';
import {smoothScrollBy, IMetadata, NhbkDialogService} from '../shared';
import {Skill} from '../skill';
import {Job, Speciality} from '../job';
import {Item} from '../item';

import {WebSocketService} from '../websocket';

import {CharacterService} from './character.service';
import {Character} from './character.model';
import {InventoryPanelComponent} from './inventory-panel.component';
import {ItemActionService} from './item-action.service';
import {SwipeService} from './swipe.service';
import {CharacterLootPanelComponent} from './character-loot-panel.component';

export class LevelUpInfo {
    EVorEA = 'EV';
    EVorEAValue: number | undefined;
    targetLevelUp: number;
    statToUp: string;
    skill: Skill;
    specialities: { [jobId: number]: Speciality } = {};
}

@Component({
    selector: 'character',
    templateUrl: './character.component.html',
    styleUrls: ['./character.component.scss'],
    providers: [SwipeService, ItemActionService],
})
export class CharacterComponent implements OnInit, OnDestroy {
    @Input() id: number;
    @Input() character: Character;

    @ViewChild('combatWeaponDetail', {static: true})
    private combatWeaponDetailElement: ElementRef;

    @ViewChild('mainTabGroup', {static: true})
    private mainTabGroup: MatTabGroup;

    @ViewChild('inventoryPanel', {static: true})
    private inventoryPanel: InventoryPanelComponent;

    @ViewChild('lootPanel', {static: true})
    private lootPanel: CharacterLootPanelComponent;

    @ViewChild('levelUpDialog', {static: true})
    public levelUpDialog: Portal<any>;
    public levelUpOverlayRef: OverlayRef;
    public levelUpInfo: LevelUpInfo = new LevelUpInfo();

    @ViewChild('skillInfoDialog', {static: true})
    public skillInfoDialog: Portal<any>;
    public skillInfoOverlayRef: OverlayRef;
    public selectedSkillInfo: Skill;
    public viewGmSkillInfo = false;

    public inGroupTab = false;
    public selectedItem: Item;
    public currentTab = 'infos';
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

    @ViewChild('changeNameDialog', {static: true})
    public changeNameDialog: Portal<any>;
    public changeNameOverlayRef?: OverlayRef;
    public newCharacterName: string;

    @ViewChild('changeSexDialog', {static: true})
    public changeSexDialog: Portal<any>;
    public changeSexOverlayRef?: OverlayRef;
    public newCharacterSex: string;

    @ViewChild('changeJobDialog', {static: true})
    public changeJobDialog: Portal<any>;
    public changeJobOverlayRef?: OverlayRef;
    public addingJob = false;

    @ViewChild('jobInfoDialog', {static: true})
    public jobInfoDialog: Portal<any>;
    public jobInfoOverlayRef?: OverlayRef;
    public selectedJobInfo?: Job;

    @ViewChild('originInfoDialog', {static: true})
    public originInfoDialog: Portal<any>;
    public originInfoOverlayRef?: OverlayRef;

    private notificationSub?: Subscription;

    constructor(private _route: ActivatedRoute
        , private _router: Router
        , private _notification: NotificationsService
        , private _websocketService: WebSocketService
        , private _nhbkDialogService: NhbkDialogService
        , private _overlay: Overlay
        , private _characterService: CharacterService) {
    }

    changeStat(stat: string, value: any) {
        this._characterService.changeCharacterStat(this.character.id, stat, value).subscribe(
            this.character.onChangeCharacterStat.bind(this.character)
        );
    }

    setStatBonusAD(id: number, stat: string) {
        if (this.character) {
            this._characterService.setStatBonusAD(id, stat).subscribe(
                this.character.onSetStatBonusAD.bind(this.character)
            );
        }
    }

    levelUp() {
        this.closeLevelUpDialog();
        this._characterService.LevelUp(this.character.id, this.levelUpInfo).subscribe(this.character.onLevelUp.bind(this.character));
    }

    characterHasToken(flagName: string) {
        if (this.character.origin.hasFlag(flagName)) {
            return true
        }
        for (let job of this.character.jobs) {
            if (job.hasFlag(flagName)) {
                return true;
            }
        }
        if (this.character.specialities) {
            for (let speciality of this.character.specialities) {
                if (speciality.hasFlag(flagName)) {
                    return true;
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

        let config = new OverlayConfig();

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
        this.levelUpInfo.EVorEAValue = undefined;
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
            let job = this.character.jobs.find(j => j.diceEaLevelUp > 0);
            if (job) {
                diceLevelUp = job.diceEaLevelUp;
            } else {
                diceLevelUp = 6;
            }
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

    levelUpShouldSelectSpeciality(job: Job): boolean {
        if (!job.specialities) {
            return false;
        }
        let specialities = this.character.getJobsSpecialities(job);
        for (let speciality of specialities) {
            if (speciality.hasFlag('ONE_SPECIALITY')) {
                return false;
            }
        }
        return job.hasFlag('SELECT_SPECIALITY_LVL_5_10')
            && !job.hasFlag('ONE_SPECIALITY')
            && (this.levelUpInfo.targetLevelUp === 5 || this.levelUpInfo.targetLevelUp === 10);
    }

    levelUpSelectSpeciality(job: Job, speciality: Speciality) {
        if (this.levelUpShouldSelectSpeciality(job)) {
            this.levelUpInfo.specialities[job.id] = speciality;
        }
    }

    levelUpFormReady() {
        if (!this.levelUpInfo.EVorEAValue) {
            return false;
        }
        for (let job of this.character.jobs) {
            if (this.levelUpShouldSelectSpeciality(job)) {
                if (this.levelUpInfo.specialities[job.id] == null) {
                    return false;
                }
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

    selectTab(tabChangeEvent: MatTabChangeEvent): boolean {
        this.currentTab = this.tabs[tabChangeEvent.index].hash;
        this.inventoryPanel.deselectItem();
        this.lootPanel.deselectItem();
        if (!this.inGroupTab) {
            this._router.navigate(['../', this.character.id], {fragment: this.currentTab, relativeTo: this._route});
        }
        return false;
    }

    // Skill info modal

    openSkillInfoDialog(skill: Skill) {
        this.selectedSkillInfo = skill;
        let config = new OverlayConfig();

        config.positionStrategy = this._overlay.position()
            .global()
            .centerHorizontally()
            .centerVertically();
        config.hasBackdrop = true;

        let overlayRef = this._overlay.create(config);
        overlayRef.attach(this.skillInfoDialog);
        overlayRef.backdropClick().subscribe(() => overlayRef.detach());
        this.skillInfoOverlayRef = overlayRef;
    }

    closeSkillInfoDialog() {
        this.skillInfoOverlayRef.detach();
    }

    selectItem(item: Item) {
        this.selectedItem = item;
        smoothScrollBy(0, this.combatWeaponDetailElement.nativeElement.getBoundingClientRect().bottom, 400);
    }

    openChangeNameDialog() {
        this.newCharacterName = this.character.name;
        this.changeNameOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.changeNameDialog);
    }

    closeChangeNameDialog() {
        this.changeNameOverlayRef.detach();
        this.changeNameOverlayRef = undefined;
    }

    changeName() {
        this.changeStat('name', this.newCharacterName);
        this.closeChangeNameDialog();
    }

    openChangeSexDialog() {
        this.newCharacterSex = this.character.sex;
        this.changeSexOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.changeSexDialog);
    }

    closeChangeSexDialog() {
        this.changeSexOverlayRef.detach();
        this.changeSexOverlayRef = undefined;
    }

    changeSex() {
        this.changeStat('sex', this.newCharacterSex);
        this.closeChangeSexDialog();
    }

    openChangeJobDialog() {
        this.addingJob = false;
        this.changeJobOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.changeJobDialog);
    }

    closeChangeJobDialog() {
        this.changeJobOverlayRef!.detach();
        this.changeJobOverlayRef = undefined;
    }

    addJob(job: Job) {
        this._characterService.addJob(this.character.id, job.id).subscribe(addedJob => {
            this.character.onAddJob(addedJob);
        });
        this.addingJob = false;
    }

    removeJob(job: Job) {
        this._characterService.removeJob(this.character.id, job.id).subscribe(removedJob => {
            this.character.onRemoveJob(removedJob);
        });
    }

    openJobInfoDialog(job: Job) {
        this.selectedJobInfo = job;
        this.jobInfoOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.jobInfoDialog);
    }

    closeJobInfoDialog() {
        this.jobInfoOverlayRef!.detach();
        this.jobInfoOverlayRef = undefined;
    }

    openOriginInfoDialog() {
        this.originInfoOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.originInfoDialog);
    }

    closeOriginInfoDialog() {
        this.originInfoOverlayRef!.detach();
        this.originInfoOverlayRef = undefined;
    }

    ngOnDestroy() {
        if (this.character && !this.inGroupTab) {
            this._websocketService.unregisterElement(this.character);
            this.character.dispose();
        }
        if (this.notificationSub) {
            this.notificationSub.unsubscribe();
        }
    }

    ngOnInit() {
        if (this.character) {
            this.inGroupTab = true;
        } else {
            this.character = this._route.snapshot.data['character'];
            this._websocketService.registerElement(this.character);
            this.notificationSub = this.character.onNotification.subscribe(notificationData => {
                this._notification.info('', notificationData.message);
            });
            let tab = this._route.snapshot.fragment;
            if (tab) {
                this.mainTabGroup.selectedIndex = this.getTabIndexFromHash(tab);
                this.currentTab = tab;
            }
            this._route.data.subscribe(data => {
                if (this.character !== data['character']) {
                    if (this.notificationSub) {
                        this.notificationSub.unsubscribe();
                    }
                    if (this.character) {
                        this._websocketService.unregisterElement(this.character);
                        this.character.dispose();
                    }
                    this.character = data['character'];
                    this._websocketService.registerElement(this.character);
                    this.mainTabGroup.selectedIndex = 0;
                    this.notificationSub = this.character.onNotification.subscribe(notificationData => {
                        this._notification.info('', notificationData.message);
                    });
                }
            });

        }
    }
}
