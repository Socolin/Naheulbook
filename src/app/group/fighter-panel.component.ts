
import {forkJoin, from as observableFrom, Observable} from 'rxjs';

import {map} from 'rxjs/operators';
import {Component, Input, OnInit, ViewChild} from '@angular/core';
import {OverlayRef} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';
import {isNullOrUndefined} from 'util';

import {getRandomInt, NhbkDialogService, AutocompleteValue} from '../shared';
import {Character} from '../character';
import {ItemData, Item, ItemService} from '../item';
import {Monster, MonsterTemplate, MonsterService, MonsterTemplateService} from '../monster';

import {CreateItemComponent} from './create-item.component';
import {Group, Fighter} from './group.model';
import {GroupActionService} from './group-action.service';
import {GroupService} from './group.service';

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

    public newMonster: Monster = new Monster();
    public selectedCombatRow = 0;

    public selectedMonsterTemplate: MonsterTemplate;
    public monsterAutocompleteShow = false;
    public autocompleteMonsterListCallback = this.updateMonsterListAutocomplete.bind(this);

    @ViewChild('createItemComponent', {static: true})
    public createItemComponent: CreateItemComponent;

    @ViewChild('addMonsterDialog', {static: true})
    public addMonsterDialog: Portal<any>;
    public addMonsterOverlayRef: OverlayRef | undefined;

    @ViewChild('deadMonstersDialog', {static: true})
    public deadMonstersDialog: Portal<any>;
    public deadMonstersOverlayRef: OverlayRef | undefined;

    constructor(private _actionService: GroupActionService,
                private _groupService: GroupService,
                private _itemService: ItemService,
                private _monsterService: MonsterService,
                private _monsterTemplateService: MonsterTemplateService,
                private _nhbkDialogService: NhbkDialogService) {
    }

    onItemAdded(data: {character: Character, monster: Monster, item: Item}) {
        if (!isNullOrUndefined(data.character)) {
            this._itemService.addItemTo('character', data.character.id, data.item.template.id, data.item.data).subscribe();
        }
        else if (!isNullOrUndefined(data.monster)) {
            this._itemService.addItemTo('monster', data.monster.id, data.item.template.id, data.item.data).subscribe();
        }
    }

    openAddMonsterDialog() {
        this.addMonsterOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.addMonsterDialog);
    }

    closeAddMonsterDialog() {
        if (!this.addMonsterOverlayRef) {
            return;
        }
        this.addMonsterOverlayRef.detach();
        this.addMonsterOverlayRef = undefined;
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
     * Using data in monster creation form (bound with `newMonster`), post request to create a new monster
     */
    addMonster(): void {
        this._monsterService.createMonster(this.group.id, this.newMonster).subscribe(
            monster => {
                this.group.addMonster(monster);
                this.group.notify('addMonster', 'Nouveau monstre ajouté: ' + monster.name, monster);
            }
        );
        this.randomMonsterInventory();
    }

    /**
     * Reset the creation monster form data
     */
    cleanMonster() {
        this.newMonster = new Monster();
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

    updateMonsterListAutocomplete(filter: string): Observable<AutocompleteValue[]> {
        this.newMonster.name = filter;
        if (filter === '') {
            return observableFrom([]);
        }
        return this._monsterTemplateService.searchMonster(filter).pipe(map(
            list => list.map(e => new AutocompleteValue(e, e.name))
        ));
    }

    randomMonsterInventory() {
        let template = this.selectedMonsterTemplate;
        if (template && template.simpleInventory) {
            this.newMonster.items = [];
            for (let i = 0; i < template.simpleInventory.length; i++) {
                let inventoryItem = template.simpleInventory[i];
                let quantity = getRandomInt(inventoryItem.minCount, inventoryItem.maxCount);
                if (!quantity) {
                    continue;
                }
                if (Math.random() > inventoryItem.chance) {
                    continue;
                }
                let item = new Item();
                item.template = inventoryItem.itemTemplate;
                if (inventoryItem.itemTemplate.data.notIdentifiedName) {
                    item.data.name = inventoryItem.itemTemplate.data.notIdentifiedName;
                } else {
                    item.data.name = inventoryItem.itemTemplate.name;
                }
                item.data.notIdentified = true;
                if (item.template.data.useUG) {
                    item.data.ug = 1; // FIXME: minUg/maxUg
                }

                if (item.template.data.quantifiable) {
                    item.data.quantity = quantity;
                    this.newMonster.items.push(item);
                } else {
                    for (let j = 0; j < quantity; j++) {
                        let duplicate = new Item();
                        duplicate.template = item.template;
                        duplicate.data = new ItemData();
                        duplicate.data.name = item.data.name;
                        duplicate.data.notIdentified = item.data.notIdentified;
                        duplicate.data.ug = item.data.ug;
                        this.newMonster.items.push(duplicate);
                    }
                }
            }
        }
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

    selectMonsterInAutocompleteList(monster: MonsterTemplate) {
        this.selectedMonsterTemplate = monster;
        this.monsterAutocompleteShow = false;
        this.newMonster.name = monster.name;
        this.newMonster.data.at = monster.data.at;
        this.newMonster.data.prd = monster.data.prd;
        this.newMonster.data.esq = monster.data.esq;
        this.newMonster.data.ev = monster.data.ev;
        this.newMonster.data.maxEv = monster.data.ev;
        this.newMonster.data.ea = monster.data.ea;
        this.newMonster.data.maxEa = monster.data.ea;
        this.newMonster.data.pr = monster.data.pr;
        this.newMonster.data.pr_magic = monster.data.pr_magic;
        this.newMonster.data.cou = monster.data.cou;
        this.newMonster.data.dmg = monster.data.dmg;
        this.newMonster.data.xp = monster.data.xp;
        this.newMonster.data.page = monster.data.page;
        if (monster.data.resm) {
            this.newMonster.data.resm = monster.data.resm;
        } else {
            this.newMonster.data.resm = 0;
        }
        this.newMonster.data.note = monster.data.note;
        this.randomMonsterInventory();
        return false;
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

        this._actionService.registerAction('openAddItemForm').subscribe(
            data => {
                let fighter: Fighter = data.data;
                if (fighter.isMonster)  {
                    this.createItemComponent.openDialogForMonster(fighter.monster);
                }
                else {
                    this.createItemComponent.openDialogForCharacter(fighter.character);
                }
            }
        );

        this._groupService.loadDeadMonsters(this.group.id, 0, 10).subscribe(
            monsters => {
                this.deadMonsters = monsters;
            }
        );
    }
}
