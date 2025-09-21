import {forkJoin} from 'rxjs';
import {Component, Input, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import { Portal, CdkPortal } from '@angular/cdk/portal';

import {NhbkDialogService} from '../shared';
import {ItemService} from '../item';
import {Monster, MonsterService, MonsterTemplateService} from '../monster';

import {Group} from './group.model';
import {GroupActionService} from './group-action.service';
import {GroupService} from './group.service';
import {AddMonsterDialogComponent, AddMonsterDialogResult} from './add-monster-dialog.component';
import {CreateFightRequest, CreateMonsterRequest} from '../api/requests';
import {BreakpointObserver, Breakpoints} from '@angular/cdk/layout';
import {NhbkMatDialog} from '../material-workaround';
import {EndCombatDialogComponent, EndCombatDialogResult} from './end-combat-dialog.component';
import {CommandSuggestionType, QuickAction, QuickCommandService} from '../quick-command';
import {Router} from '@angular/router';
import {DeadMonsterResponse} from '../api/responses/dead-monster-response';
import {CreateFightDialogComponent, CreateFightDialogResult} from './create-fight-dialog.component';
import { MatCard, MatCardTitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { MatToolbar } from '@angular/material/toolbar';
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { FighterComponent } from './fighter.component';
import { MatMenuTrigger, MatMenu, MatMenuItem } from '@angular/material/menu';
import { MatDivider } from '@angular/material/list';
import { MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle, MatExpansionPanelDescription } from '@angular/material/expansion';
import { FighterIconComponent } from './fighter-icon.component';

@Component({
    selector: 'fighter-panel',
    templateUrl: './fighter-panel.component.html',
    styleUrls: ['./fighter.component.scss', './fighter-panel.component.scss'],
    imports: [MatCard, MatToolbar, MatButton, MatIcon, FighterComponent, MatIconButton, MatMenuTrigger, MatMenu, MatMenuItem, MatDivider, MatAccordion, MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle, FighterIconComponent, MatExpansionPanelDescription, CdkPortal, MatCardTitle, MatCardContent, MatCardActions]
})
export class FighterPanelComponent implements OnInit, OnDestroy {
    @Input() group: Group;

    public loadingNextLap = false;

    public deadMonsters: DeadMonsterResponse[] = [];
    public allDeadMonstersLoaded = false;

    @ViewChild('deadMonstersDialog', {static: true})
    public deadMonstersDialog: Portal<any>;
    public deadMonstersOverlayRef: OverlayRef | undefined;
    public isMobile: boolean;

    constructor(
        private readonly actionService: GroupActionService,
        private readonly groupService: GroupService,
        private readonly itemService: ItemService,
        private readonly monsterService: MonsterService,
        private readonly monsterTemplateService: MonsterTemplateService,
        private readonly nhbkDialogService: NhbkDialogService,
        private readonly dialog: NhbkMatDialog,
        private readonly router: Router,
        private readonly breakpointObserver: BreakpointObserver,
        private readonly quickCommandService: QuickCommandService,
    ) {
        breakpointObserver.observe([
            Breakpoints.Handset
        ]).subscribe(result => {
            this.isMobile = result.breakpoints[Breakpoints.HandsetPortrait];
        });
    }

    openAddMonsterDialog(fightId?: number) {
        const dialogRef = this.dialog.openFullScreen<AddMonsterDialogComponent, any, AddMonsterDialogResult>(AddMonsterDialogComponent);

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            const {name, items, ...monsterData} = result;
            const request = {
                name: name,
                items: items.map(i => ({itemTemplateId: i.template.id, itemData: i.data})),
                fightId: fightId,
                data: monsterData
            } as CreateMonsterRequest;

            const [color, number] = this.group.getColorAndNumberForNewMonster();
            request.data.number = number;
            request.data.color = color;

            this.monsterService.createMonster(this.group.id, request).subscribe(
                monster => {
                    if (fightId) {
                        let fight = this.group.fights.find(f => f.id === fightId);
                        if (fight)
                            fight.addMonster(monster);
                    } else {
                        this.group.addMonster(monster);
                    }
                    this.group.notify('addMonster', 'Nouveau monstre ajouté: ' + monster.name, monster);
                }
            );
        });
    }

    openCreateFightDialog() {
        const dialogRef = this.dialog.open<CreateFightDialogComponent, any, CreateFightDialogResult>(CreateFightDialogComponent);

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            const {name} = result;
            const request = {
                name: name,
            } as CreateFightRequest;

            this.groupService.createFight(this.group.id, request).subscribe(
                fight => {
                    this.group.addFight(fight);
                    this.group.notify('addFight', 'Nouveau combat ajouté: ' + fight.name);
                }
            );
        });
    }

    startFight(fightId: number) {
        this.groupService.startFight(this.group.id, fightId).subscribe(() => {
            this.group.removeFight(fightId);
            this.group.data.changeValue('inCombat', true);
            this.registerCombatQuickActions();
        })
    }

    deleteFight(fightId: number) {
        this.groupService.deleteFight(this.group.id, fightId).subscribe(() => {
            this.group.removeFight(fightId);
        })
    }

    openDeadMonstersDialog() {
        this.deadMonstersOverlayRef = this.nhbkDialogService.openCenteredBackdropDialog(this.deadMonstersDialog);
    }

    closeDeadMonstersDialog() {
        if (!this.deadMonstersOverlayRef) {
            return;
        }
        this.deadMonstersOverlayRef.detach();
        this.deadMonstersOverlayRef = undefined;
    }

    /**
     * Post request to kill the monster, then monster.dead will contains date of death and loot will be updated
     * @param monster
     */
    killMonster(monster: Monster) {
        this.monsterService.killMonster(monster.id).subscribe(
            () => {
                this.group.removeMonster(monster.id);
                this.deadMonsters.unshift(monster);
                this.group.notify('killMonster', 'Monstre tué: ' + monster.name, monster);
                if (this.group.pendingModifierChanges) {
                    this.groupService.saveChangedTime(this.group.id, this.group.pendingModifierChanges).subscribe();
                    this.group.pendingModifierChanges = undefined;
                }
            }
        );
    }

    /**
     * Fully delete monster, no loot and will not appear in dead monster list
     * @param monster
     */
    deleteMonster(monster: Monster) {
        this.monsterService.deleteMonster(monster.id).subscribe(
            () => {
                this.group.removeMonster(monster.id);
                this.group.notify('deleteMonster', 'Monstre supprimé: ' + monster.name, monster);
                if (this.group.pendingModifierChanges) {
                    this.groupService.saveChangedTime(this.group.id, this.group.pendingModifierChanges).subscribe();
                    this.group.pendingModifierChanges = undefined;
                }
            }
        );
    }

    moveMonsterToFight(monster: Monster, fightId?: number) {
        this.monsterService.moveMonsterToFight(monster.id, {fightId}).subscribe(
            () => {
                this.group.moveMonsterToFight(monster, fightId);
                this.group.notify('moveMonster', 'Monstre déplacé: ' + monster.name, monster);
                if (this.group.pendingModifierChanges) {
                    this.groupService.saveChangedTime(this.group.id, this.group.pendingModifierChanges).subscribe();
                    this.group.pendingModifierChanges = undefined;
                }
            }
        );
    }

    loadMoreDeadMonsters(): boolean {
        this.groupService.loadDeadMonsters(this.group.id, this.deadMonsters.length, 10).subscribe(
            monsters => {
                if (monsters.length === 0) {
                    this.allDeadMonstersLoaded = true;
                } else {
                    this.deadMonsters = this.deadMonsters.concat(monsters);
                }
            }
        );
        return false;
    }

    selectNextFighter() {
        let result = this.group.nextFighter();
        if (!result) {
            return;
        }
        this.loadingNextLap = true;

        forkJoin([
            this.groupService.editGroupValue(this.group.id, 'fighterIndex', result.fighterIndex),
            this.groupService.saveChangedTime(this.group.id, result.modifiersDurationUpdated)
        ]).subscribe(() => {
            this.loadingNextLap = false;
        });
    }

    startCombat() {
        this.groupService.startCombat(this.group.id).subscribe(
            () => {
                this.group.data.changeValue('inCombat', true);
                this.registerCombatQuickActions();
            }
        );
    }

    endCombat(decreaseCombatTimer: boolean) {
        this.groupService.endCombat(this.group.id).subscribe(
            () => {
                this.group.data.changeValue('inCombat', false);
                if (decreaseCombatTimer) {
                    let changes = this.group.updateTime('combat', 1);
                    this.groupService.saveChangedTime(this.group.id, changes).subscribe();
                }
                this.registerCombatQuickActions();
            }
        );
    }

    ngOnDestroy(): void {
        this.quickCommandService.unregisterActions('fighters');
        this.quickCommandService.unregisterActions('combat');
    }

    ngOnInit(): void {
        this.actionService.registerAction('killMonster').subscribe(
            data => {
                this.killMonster(data.data);
            }
        );
        this.actionService.registerAction('moveMonsterToFight').subscribe(
            data => {
                this.moveMonsterToFight(data.data.monster, data.data.fightId);
            }
        );
        this.actionService.registerAction('deleteMonster').subscribe(
            data => {
                this.deleteMonster(data.data);
            }
        );

        this.groupService.loadDeadMonsters(this.group.id, 0, 10).subscribe(
            monsters => {
                this.deadMonsters = monsters;
            }
        );
        this.registerQuickActions();
        this.registerCombatQuickActions();
    }

    openEndCombatDialog() {
        const dialogRef = this.dialog.open<EndCombatDialogComponent, any, EndCombatDialogResult>(EndCombatDialogComponent)
        dialogRef.afterClosed().subscribe(result => {
            if (!result) {
                return;
            }
            this.endCombat(result.decreaseCombatTimer);
        })
    }

    private registerCombatQuickActions() {
        if (!this.group.data.inCombat) {
            this.quickCommandService.registerActions('combat',  [{
                displayText: 'Début combat',
                type: CommandSuggestionType.Action,
                icon: 'play_arrow',
                priority: 40,
                action: () => {
                    this.startCombat(),
                    this.router.navigate([], {fragment: 'combat'})
                },
                canBeUsedInRecent: true
            }]);
        } else {
            this.quickCommandService.registerActions('combat', [{
                displayText: 'Fin combat',
                type: CommandSuggestionType.Action,
                icon: 'stop',
                priority: 40,
                action: () => this.openEndCombatDialog(),
                canBeUsedInRecent: true
            }]);
        }
    }

    private registerQuickActions() {
        const actions: QuickAction[] = [];

        actions.push({
            type: CommandSuggestionType.Action,
            icon: 'add',
            priority: 20,
            displayText: 'Ajouter un monstre',
            canBeUsedInRecent: true,
            action: () => this.openAddMonsterDialog(),
        })

        this.quickCommandService.registerActions('fighter', actions);
    }
}
