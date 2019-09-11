import {forkJoin} from 'rxjs';
import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

import {NhbkDialogService} from '../shared';
import {ItemService} from '../item';
import {Monster, MonsterService, MonsterTemplateService} from '../monster';

import {Fighter, Group} from './group.model';
import {GroupActionService} from './group-action.service';
import {GroupService} from './group.service';
import {MatDialog} from '@angular/material/dialog';
import {AddMonsterDialogComponent, AddMonsterDialogResult} from './add-monster-dialog.component';
import {CreateMonsterRequest} from '../api/requests';

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

    public selectedCombatRow = 0;

    @ViewChild('deadMonstersDialog', {static: true})
    public deadMonstersDialog: Portal<any>;
    public deadMonstersOverlayRef: OverlayRef | undefined;

    constructor(private _actionService: GroupActionService,
                private _groupService: GroupService,
                private _itemService: ItemService,
                private _monsterService: MonsterService,
                private _monsterTemplateService: MonsterTemplateService,
                private _nhbkDialogService: NhbkDialogService,
                private dialog: MatDialog,
    ) {
    }

    openAddMonsterDialog() {
        const dialogRef = this.dialog.open<AddMonsterDialogComponent, any, AddMonsterDialogResult>(AddMonsterDialogComponent, {
            minWidth: '100vw', height: '100vh'
        });

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
            this._monsterService.createMonster(this.group.id, request).subscribe(
                monster => {
                    this.group.addMonster(monster);
                    this.group.notify('addMonster', 'Nouveau monstre ajouté: ' + monster.name, monster);
                }
            );
        });
    }

    openDeadMonstersDialog() {
        this.deadMonstersOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.deadMonstersDialog);
    }

    closeDeadMonstersDialog() {
        if (!this.deadMonstersOverlayRef) {
            return;
        }
        this.deadMonstersOverlayRef.detach();
        this.deadMonstersOverlayRef = undefined;
    }

    selectFighter(fighter: Fighter) {
        this.selectedCombatRow = this.group.fighters.indexOf(fighter);
    }

    /**
     * Post request to kill the monster, then monster.dead will contains date of death and loot will be updated
     * @param monster
     */
    killMonster(monster: Monster) {
        this._monsterService.killMonster(monster.id).subscribe(
            () => {
                this.group.removeMonster(monster.id);
                this.deadMonsters.unshift(monster);
                this.group.notify('killMonster', 'Monstre tué: ' + monster.name, monster);
                if (this.group.pendingModifierChanges) {
                    this._groupService.saveChangedTime(this.group.id, this.group.pendingModifierChanges);
                    this.group.pendingModifierChanges = null;
                }
            }
        );
    }

    /**
     * Fully delete monster, no loot and will not appear in dead monster list
     * @param monster
     */
    deleteMonster(monster: Monster) {
        this._monsterService.deleteMonster(monster.id).subscribe(
            () => {
                this.group.removeMonster(monster.id);
                this.group.notify('deleteMonster', 'Monstre supprimé: ' + monster.name, monster);
                if (this.group.pendingModifierChanges) {
                    this._groupService.saveChangedTime(this.group.id, this.group.pendingModifierChanges);
                    this.group.pendingModifierChanges = null;
                }
            }
        );
    }

    loadMoreDeadMonsters(): boolean {
        this._groupService.loadDeadMonsters(this.group.id, this.deadMonsters.length, 10).subscribe(
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
            this._groupService.editGroupValue(this.group.id, 'fighterIndex', result.fighterIndex),
            this._groupService.saveChangedTime(this.group.id, result.modifiersDurationUpdated)
        ]).subscribe(() => {
            this.loadingNextLap = false;
        });
    }

    startCombat() {
        this._groupService.startCombat(this.group.id).subscribe(
            () => {
                this.group.data.changeValue('inCombat', true);
            }
        );
    }

    endCombat() {
        this._groupService.endCombat(this.group.id).subscribe(
            () => {
                this.group.data.changeValue('inCombat', false);
                let changes = this.group.updateTime('combat', 1);
                this._groupService.saveChangedTime(this.group.id, changes);
            }
        );
    }

    ngOnInit(): void {
        this._actionService.registerAction('killMonster').subscribe(
            data => {
                this.killMonster(data.data);
            }
        );
        this._actionService.registerAction('deleteMonster').subscribe(
            data => {
                this.deleteMonster(data.data);
            }
        );

        this._groupService.loadDeadMonsters(this.group.id, 0, 10).subscribe(
            monsters => {
                this.deadMonsters = monsters;
            }
        );
    }
}
