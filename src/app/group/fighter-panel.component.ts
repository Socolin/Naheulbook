import {forkJoin} from 'rxjs';
import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {NhbkDialogService} from '../shared';
import {ItemService} from '../item';
import {Monster, MonsterService, MonsterTemplateService} from '../monster';

import {Group} from './group.model';
import {GroupActionService} from './group-action.service';
import {GroupService} from './group.service';
import {AddMonsterDialogComponent, AddMonsterDialogResult} from './add-monster-dialog.component';
import {CreateMonsterRequest} from '../api/requests';
import {BreakpointObserver, Breakpoints} from '@angular/cdk/layout';
import {NhbkMatDialog} from '../material-workaround';

@Component({
    selector: 'fighter-panel',
    templateUrl: './fighter-panel.component.html',
    styleUrls: ['./fighter.component.scss', './fighter-panel.component.scss']
})
export class FighterPanelComponent implements OnInit {
    @Input() group: Group;

    public loadingNextLap = false;

    public deadMonsters: Monster[] = [];
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
        private readonly breakpointObserver: BreakpointObserver,
    ) {
        breakpointObserver.observe([
            Breakpoints.Handset
        ]).subscribe(result => {
            this.isMobile = result.breakpoints[Breakpoints.HandsetPortrait];
        });
    }

    openAddMonsterDialog() {
        const dialogRef = this.dialog.openFullScreen<AddMonsterDialogComponent, any, AddMonsterDialogResult>(AddMonsterDialogComponent);

        dialogRef.afterClosed().subscribe((result) => {
            if (!result) {
                return;
            }

            const {name, items, ...monsterData} = result;
            const request = {
                name: name,
                items: items.map(i => ({itemTemplateId: i.template.id, itemData: i.data})),
                data: monsterData
            } as CreateMonsterRequest;
            this.monsterService.createMonster(this.group.id, request).subscribe(
                monster => {
                    this.group.addMonster(monster);
                    this.group.notify('addMonster', 'Nouveau monstre ajouté: ' + monster.name, monster);
                }
            );
        });
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
                    this.groupService.saveChangedTime(this.group.id, this.group.pendingModifierChanges);
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
                    this.groupService.saveChangedTime(this.group.id, this.group.pendingModifierChanges);
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
            }
        );
    }

    endCombat() {
        this.groupService.endCombat(this.group.id).subscribe(
            () => {
                this.group.data.changeValue('inCombat', false);
                let changes = this.group.updateTime('combat', 1);
                this.groupService.saveChangedTime(this.group.id, changes);
            }
        );
    }

    ngOnInit(): void {
        this.actionService.registerAction('killMonster').subscribe(
            data => {
                this.killMonster(data.data);
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
    }
}
