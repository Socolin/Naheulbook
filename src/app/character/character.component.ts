import {Component, ElementRef, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {MatTabChangeEvent, MatTabGroup} from '@angular/material';
import {Overlay, OverlayConfig, OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';
import {Subscription} from 'rxjs';
import {MatDialog} from '@angular/material/dialog';

import {NotificationsService} from '../notifications';
import {NhbkDialogService, PromptDialogComponent} from '../shared';
import {Skill} from '../skill';
import {Job, Speciality} from '../job';
import {Item} from '../item';

import {WebSocketService} from '../websocket';

import {CharacterService} from './character.service';
import {Character, CharacterGroupInvite} from './character.model';
import {InventoryPanelComponent} from './inventory-panel.component';
import {ItemActionService} from './item-action.service';
import {ChangeSexDialogComponent, ChangeSexDialogData} from './change-sex-dialog.component';
import {OriginPlayerDialogComponent} from './origin-player-dialog.component';

export class LevelUpInfo {
    evOrEa = 'EV';
    evOrEaValue: number;
    targetLevelUp: number;
    statToUp: string;
    skill?: Skill;
    specialities: { [jobId: number]: Speciality } = {};
}

@Component({
    selector: 'character',
    templateUrl: './character.component.html',
    styleUrls: ['./character.component.scss'],
    providers: [ItemActionService],
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

    @ViewChild('changeJobDialog', {static: true})
    public changeJobDialog: Portal<any>;
    public changeJobOverlayRef?: OverlayRef;
    public addingJob = false;

    @ViewChild('jobInfoDialog', {static: true})
    public jobInfoDialog: Portal<any>;
    public jobInfoOverlayRef?: OverlayRef;
    public selectedJobInfo?: Job;

    private notificationSub?: Subscription;

    constructor(
        private readonly dialog: MatDialog,
        private readonly itemActionService: ItemActionService,
        private readonly characterService: CharacterService,
        private readonly nhbkDialogService: NhbkDialogService,
        private readonly notification: NotificationsService,
        private readonly overlay: Overlay,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly websocketService: WebSocketService,
    ) {
    }

    changeStat(stat: string, value: any) {
        this.characterService.changeCharacterStat(this.character.id, stat, value).subscribe(
            this.character.onChangeCharacterStat.bind(this.character)
        );
    }

    setStatBonusAD(id: number, stat: string) {
        if (this.character) {
            this.characterService.setStatBonusAD(id, stat).subscribe(
                this.character.onSetStatBonusAD.bind(this.character)
            );
        }
    }

    levelUp() {
        this.closeLevelUpDialog();
        this.characterService.LevelUp(this.character.id, this.levelUpInfo).subscribe(res => {
            this.character.onLevelUp(res[0], res[1]);
        });
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
        this.characterService.changeGmData(this.character.id, key, value).subscribe(
            change => {
                this.notification.info('Modification', key + ': ' + this.character.gmData[change.key] + ' -> ' + change.value);
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

        config.positionStrategy = this.overlay.position()
            .global()
            .centerHorizontally()
            .centerVertically();
        config.hasBackdrop = true;

        let overlayRef = this.overlay.create(config);
        overlayRef.attach(this.levelUpDialog);
        overlayRef.backdropClick().subscribe(() => overlayRef.detach());
        this.levelUpOverlayRef = overlayRef;
    }

    closeLevelUpDialog() {
        this.levelUpOverlayRef.detach();
    }

    initLevelUp() {
        this.levelUpInfo = new LevelUpInfo();
        this.levelUpInfo.evOrEa = 'EV';
        this.levelUpInfo.evOrEaValue = 0;
        this.levelUpInfo.targetLevelUp = this.character.level + 1;
        if (this.levelUpInfo.targetLevelUp % 2 === 0) {
            this.levelUpInfo.statToUp = 'FO';
        }
        else {
            this.levelUpInfo.statToUp = 'AT';
        }
    }

    rollLevelUp() {
        let diceLevelUp = this.character.origin.data.diceEvLevelUp;
        if (this.levelUpInfo.evOrEa === 'EV') {
            if (this.characterHasToken('LEVELUP_DICE_EV_-1')) {
                this.levelUpInfo.evOrEaValue = Math.max(1, Math.ceil(Math.random() * diceLevelUp) - 1);
                return;
            }
        } else {
            let job = this.character.jobs.find(j => !!j.getStatData(this.character.origin).diceEaLevelUp);
            if (job) {
                diceLevelUp = job.getStatData(this.character.origin).diceEaLevelUp!;
            } else {
                diceLevelUp = 6;
            }
        }
        this.levelUpInfo.evOrEaValue = Math.ceil(Math.random() * diceLevelUp);
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
        if (!this.levelUpInfo.evOrEaValue) {
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

    cancelInvite(invite: CharacterGroupInvite) {
        this.characterService.cancelInvite(this.character.id, invite.groupId).subscribe(
            res => {
                for (let i = 0; i < this.character.invites.length; i++) {
                    let e = this.character.invites[i];
                    if (e.groupId === res.groupId) {
                        this.character.invites.splice(i, 1);
                        break;
                    }
                }
            }
        );
        return false;
    }

    acceptInvite(invite: CharacterGroupInvite) {
        this.characterService.joinGroup(this.character.id, invite.groupId).subscribe(
            () => {
                this.character.invites = [];
                this.character.group = {id: invite.groupId, name: invite.groupName};
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
        if (!this.inGroupTab) {
            this.router.navigate(['../', this.character.id], {fragment: this.currentTab, relativeTo: this.route});
        }
        return false;
    }

    // Skill info modal

    openSkillInfoDialog(skill: Skill) {
        this.selectedSkillInfo = skill;
        let config = new OverlayConfig();

        config.positionStrategy = this.overlay.position()
            .global()
            .centerHorizontally()
            .centerVertically();
        config.hasBackdrop = true;

        let overlayRef = this.overlay.create(config);
        overlayRef.attach(this.skillInfoDialog);
        overlayRef.backdropClick().subscribe(() => overlayRef.detach());
        this.skillInfoOverlayRef = overlayRef;
    }

    closeSkillInfoDialog() {
        this.skillInfoOverlayRef.detach();
    }

    openChangeNameDialog() {
        const dialogRef = this.dialog.open(PromptDialogComponent, {
            data: {
                confirmText: 'CHANGER',
                cancelText: 'ANNULER',
                placeholder: 'Nom',
                title: 'Changer de nom',
                initialValue: this.character.name
            }
        });

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            this.changeStat('name', result.text);
        });
    }

    openChangeSexDialog() {
        const dialogRef = this.dialog.open<ChangeSexDialogComponent, ChangeSexDialogData>(
            ChangeSexDialogComponent, {
                autoFocus: false,
                data: {sex: this.character.sex}
            });
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
            this.changeStat('sex',  result);
        });
    }

    openChangeJobDialog() {
        this.addingJob = false;
        this.changeJobOverlayRef = this.nhbkDialogService.openCenteredBackdropDialog(this.changeJobDialog);
    }

    closeChangeJobDialog() {
        this.changeJobOverlayRef!.detach();
        this.changeJobOverlayRef = undefined;
    }

    addJob(job: Job) {
        this.characterService.addJob(this.character.id, job.id).subscribe(addedJob => {
            this.character.onAddJob(addedJob);
        });
        this.addingJob = false;
    }

    removeJob(job: Job) {
        this.characterService.removeJob(this.character.id, job.id).subscribe(removedJob => {
            this.character.onRemoveJob(removedJob);
        });
    }

    openJobInfoDialog(job: Job) {
        this.selectedJobInfo = job;
        this.jobInfoOverlayRef = this.nhbkDialogService.openCenteredBackdropDialog(this.jobInfoDialog);
    }

    closeJobInfoDialog() {
        this.jobInfoOverlayRef!.detach();
        this.jobInfoOverlayRef = undefined;
    }

    openOriginInfoDialog() {
        this.dialog.open(OriginPlayerDialogComponent, {
            autoFocus: false,
            minWidth: '100vw', height: '100vh',
            data: {
                origin: this.character.origin
            }
        });
    }

    ngOnDestroy() {
        if (this.character && !this.inGroupTab) {
            this.websocketService.unregisterElement(this.character);
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
            this.character = this.route.snapshot.data['character'];
            this.websocketService.registerElement(this.character);
            this.notificationSub = this.character.onNotification.subscribe(notificationData => {
                this.notification.info('', notificationData.message);
            });
            let tab = this.route.snapshot.fragment;
            if (tab) {
                this.mainTabGroup.selectedIndex = this.getTabIndexFromHash(tab);
                this.currentTab = tab;
            }
            this.route.data.subscribe(data => {
                if (this.character !== data['character']) {
                    if (this.notificationSub) {
                        this.notificationSub.unsubscribe();
                    }
                    if (this.character) {
                        this.websocketService.unregisterElement(this.character);
                        this.character.dispose();
                    }
                    this.character = data['character'];
                    this.websocketService.registerElement(this.character);
                    this.mainTabGroup.selectedIndex = 0;
                    this.notificationSub = this.character.onNotification.subscribe(notificationData => {
                        this.notification.info('', notificationData.message);
                    });
                }
            });

        }
    }

    unEquipAllAndEquip(item: Item) {
        for (const weaponItem of this.character.computedData.itemsBySlotsAll[1]) {
            if (weaponItem.data.equiped) {
                this.itemActionService.onAction('unequip', weaponItem);
            }
        }
        this.itemActionService.onAction('equip', item)
    }
}
