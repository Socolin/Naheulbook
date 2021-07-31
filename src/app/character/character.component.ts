import {Component, ElementRef, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {MatTabChangeEvent, MatTabGroup} from '@angular/material/tabs';
import {Overlay} from '@angular/cdk/overlay';
import {Subscription} from 'rxjs';

import {NotificationsService} from '../notifications';
import {NhbkDialogService, PromptDialogComponent} from '../shared';
import {Skill} from '../skill';
import {Job} from '../job';

import {WebSocketService} from '../websocket';

import {CharacterService} from './character.service';
import {Character} from './character.model';
import {ItemActionService} from './item-action.service';
import {ChangeSexDialogComponent, ChangeSexDialogData} from './change-sex-dialog.component';
import {OriginPlayerDialogComponent} from './origin-player-dialog.component';
import {JobPlayerDialogComponent} from './job-player-dialog.component';
import {ChangeJobDialogComponent, ChangeJobDialogData, ChangeJobDialogResult} from './change-job-dialog.component';
import {SkillInfoDialogComponent, SkillInfoDialogData} from './skill-info-dialog.component';
import {LevelUpDialogComponent, LevelUpDialogData, LevelUpDialogResult} from './level-up-dialog.component';
import {NhbkMatDialog} from 'app/material-workaround';
import {CommandSuggestionType, QuickAction, QuickCommandService} from '../quick-command';
import {EffectPanelComponent} from './effect-panel.component';
import {filter, map} from 'rxjs/operators';
import {CharacterGroupInviteResponse} from '../api/responses';
import {InventoryPanelComponent} from './inventory-panel.component';

@Component({
    selector: 'character',
    templateUrl: './character.component.html',
    styleUrls: ['./character.component.scss'],
    providers: [ItemActionService],
})
export class CharacterComponent implements OnInit, OnDestroy {
    @Input() id: number;
    @Input() character: Character;

    @ViewChild('mainTabGroup', {static: true})
    private mainTabGroup: MatTabGroup;

    @ViewChild('effectPanel', {static: true})
    private effectPanel: EffectPanelComponent;

    @ViewChild('inventoryPanel', {static: true})
    private inventoryPanel: InventoryPanelComponent;

    @ViewChild('notesEditor', {static: false})
    private notesEditor: ElementRef<HTMLTextAreaElement>;

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
    public editingNotes = false;

    private notificationSub?: Subscription;
    private subscription = new Subscription();

    constructor(
        public readonly itemActionService: ItemActionService,
        private readonly dialog: NhbkMatDialog,
        private readonly characterService: CharacterService,
        private readonly nhbkDialogService: NhbkDialogService,
        private readonly notification: NotificationsService,
        private readonly overlay: Overlay,
        private readonly route: ActivatedRoute,
        private readonly router: Router,
        private readonly websocketService: WebSocketService,
        private readonly quickCommandService: QuickCommandService,
    ) {
    }

    changeStat(stat: string, value: any) {
        this.subscription.add(this.characterService.changeCharacterStat(this.character.id, stat, value).subscribe(
            this.character.onChangeCharacterStat.bind(this.character)
        ));
    }

    setStatBonusAD(id: number, stat: string) {
        if (this.character) {
            this.subscription.add(this.characterService.setStatBonusAD(id, stat).subscribe(
                this.character.onSetStatBonusAD.bind(this.character)
            ));
        }
    }

    changeGmData(key: string, value: any) {
        this.subscription.add(this.characterService.changeGmData(this.character.id, key, value).subscribe(
            change => {
                this.notification.info('Modification', key + ': ' + this.character.gmData[change.key] + ' -> ' + change.value);
                this.character.gmData[change.key] = change.value;
            }
        ));
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

    // Group

    cancelInvite(invite: CharacterGroupInviteResponse) {
        this.subscription.add(this.characterService.cancelInvite(this.character.id, invite.groupId).subscribe(
            res => {
                for (let i = 0; i < this.character.invites.length; i++) {
                    let e = this.character.invites[i];
                    if (e.groupId === res.groupId) {
                        this.character.invites.splice(i, 1);
                        break;
                    }
                }
            }
        ));
        return false;
    }

    acceptInvite(invite: CharacterGroupInviteResponse) {
        this.subscription.add(this.characterService.joinGroup(this.character.id, invite.groupId).subscribe(
            () => {
                this.character.invites = [];
                this.character.group = {id: invite.groupId, name: invite.groupName, config: invite.config};
            }
        ));
        return false;
    }

    // Tab

    getTabIndexFromHash(hash: string): number {
        return this.tabs.findIndex(t => t.hash === hash);
    }

    selectTab(tabChangeEvent: MatTabChangeEvent): boolean {
        this.currentTab = this.tabs[tabChangeEvent.index].hash;
        if (!this.inGroupTab) {
            this.router.navigate(['../', this.character.id], {fragment: this.currentTab, relativeTo: this.route, replaceUrl: true});
        }
        return false;
    }

    openSkillInfoDialog(skill: Skill) {
        this.dialog.open<SkillInfoDialogComponent, SkillInfoDialogData>(SkillInfoDialogComponent, {
            data: {
                skill: skill,
                canViewGmSkillInfo: this.inGroupTab || !!this.character.group?.config.allowPlayersToSeeSkillGmDetails
            },
            autoFocus: false
        });
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
            this.changeStat('sex', result);
        });
    }

    openChangeJobDialog() {
        const dialogRef = this.dialog.openFullScreen<ChangeJobDialogComponent, ChangeJobDialogData, ChangeJobDialogResult>(
            ChangeJobDialogComponent,
            {
                data: {
                    character: this.character
                }
            });

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }
            for (const job of result.addedJobs) {
                this.subscription.add(this.characterService.addJob(this.character.id, job.id).subscribe(addedJob => {
                    this.character.onAddJob(addedJob);
                }));
            }
            for (const job of result.deletedJobs) {
                this.subscription.add(this.characterService.removeJob(this.character.id, job.id).subscribe(removedJob => {
                    this.character.onRemoveJob(removedJob);
                }));
            }
        });
    }

    openJobInfoDialog(job: Job) {
        this.dialog.openFullScreen(JobPlayerDialogComponent, {
            data: {
                job: job
            }
        });
    }

    openOriginInfoDialog() {
        this.dialog.openFullScreen(OriginPlayerDialogComponent, {
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
        this.subscription.unsubscribe();
        this.unregisterQuickActions();
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
            this.route.fragment.pipe(
                filter(fragment => !!fragment),
                map(fragment => fragment.indexOf('?') === -1 ? fragment : fragment.substring(0, fragment.indexOf('?')))
            ).subscribe(tab => {
                this.mainTabGroup.selectedIndex = this.getTabIndexFromHash(tab);
                this.currentTab = tab;
            })
            this.subscription.add(this.route.data.subscribe(data => {
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
            }));

        }
        this.registerQuickActions();
    }


    openLevelUpDialog() {
        const dialogRef = this.dialog.openFullScreen<LevelUpDialogComponent, LevelUpDialogData, LevelUpDialogResult>(
            LevelUpDialogComponent, {
                data: {
                    character: this.character
                }
            });

        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }

            this.characterService.LevelUp(this.character.id, result).subscribe(res => {
                this.character.onLevelUp(res[0], res[1]);
            });
        });
    }

    private unregisterQuickActions(): void {
        this.quickCommandService.unregisterActions('character');
    }

    private registerQuickActions(): void {
        const actions: QuickAction[] = [];

        actions.push({
            type: CommandSuggestionType.Action,
            icon: 'add',
            priority: 50,
            displayText: 'Ajouter un effet',
            canBeUsedInRecent: true,
            action: () => {
                this.effectPanel.openAddEffectDialog();
            },
        })

        this.quickCommandService.registerActions('character', actions);
    }

    startEditNotes() {
        this.editingNotes = true;
        this.notesEditor.nativeElement.value = this.character.notes || '';
    }

    cancelEditNotes() {
        this.editingNotes = false;
    }

    saveNotes() {
        this.changeStat('notes', this.notesEditor.nativeElement.value);
        this.editingNotes = false;
    }

    viewMoneyDetails() {
        this.currentTab = 'inventory';
        if (!this.inGroupTab) {
            this.router.navigate(['../', this.character.id], {fragment: this.currentTab, relativeTo: this.route, replaceUrl: true});
        }
        this.inventoryPanel.changeViewMode('money');
    }
}
