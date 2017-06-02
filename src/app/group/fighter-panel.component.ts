import {Component, Input, OnInit, OnChanges, SimpleChanges, ViewChild} from '@angular/core';
import {OverlayRef, Portal} from '@angular/material';

import {Group, Fighter} from './group.model';
import {Monster, MonsterTemplate} from '../monster/monster.model';
import {GroupService} from './group.service';
import {ItemData, Item} from '../character/item.model';
import {Observable} from 'rxjs';
import {AutocompleteValue} from '../shared/autocomplete-input.component';
import {MonsterService} from '../monster/monster.service';
import {Character} from '../character/character.model';
import {getRandomInt} from '../shared/random';
import {GroupActionService} from './group-action.service';
import {ItemService} from '../item/item.service';
import {NhbkDialogService} from '../shared/nhbk-dialog.service';
import {CreateItemComponent} from './create-item.component';
import {isNullOrUndefined} from 'util';

@Component({
    selector: 'fighter-panel',
    templateUrl: './fighter-panel.component.html',
    styleUrls: ['./fighter.component.scss', './fighter-panel.component.scss']
})
export class FighterPanelComponent implements OnInit {
    @Input() group: Group;

    public currentFighterIndex = 0;
    public loadingNextLap = false;

    public deadMonsters: Monster[] = [];
    public allDeadMonstersLoaded = false;

    public newMonster: Monster = new Monster();
    public selectedCombatRow = 0;

    public selectedMonsterTemplate: MonsterTemplate;
    public monsterAutocompleteShow = false;
    public autocompleteMonsterListCallback = this.updateMonsterListAutocomplete.bind(this);

    @ViewChild('createItemComponent')
    public createItemComponent: CreateItemComponent;

    @ViewChild('addMonsterDialog')
    public addMonsterDialog: Portal<any>;
    public addMonsterOverlayRef: OverlayRef;

    @ViewChild('deadMonstersDialog')
    public deadMonstersDialog: Portal<any>;
    public deadMonstersOverlayRef: OverlayRef;

    constructor(private _groupService: GroupService
        , private _actionService: GroupActionService
        , private _itemService: ItemService
        , private _nhbkDialogService: NhbkDialogService
        , private _monsterService: MonsterService) {
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
        this.addMonsterOverlayRef.detach();
        this.addMonsterOverlayRef = null;
    }

    openDeadMonstersDialog() {
        this.deadMonstersOverlayRef = this._nhbkDialogService.openCenteredBackdropDialog(this.deadMonstersDialog);
    }

    closeDeadMonstersDialog() {
        this.deadMonstersOverlayRef.detach();
        this.deadMonstersOverlayRef = null;
    }

    selectFighter(fighter: Fighter) {
        this.selectedCombatRow = this.group.fighters.indexOf(fighter);
    }

    /**
     * Using data in monster creation form (bound with `newMonster`), post request to create a new monster
     */
    addMonster(): void {
        this._groupService.createMonster(this.group.id, this.newMonster).subscribe(
            monster => {
                this.group.addMonster(monster);
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
        this._groupService.killMonster(monster.id).subscribe(
            res => {
                this.group.removeMonster(monster.id);
                this.deadMonsters.unshift(monster);
            }
        );
    }

    /**
     * Fully delete monster, no loot and will not appear in dead monster list
     * @param monster
     */
    deleteMonster(monster: Monster) {
        this._groupService.deleteMonster(monster.id).subscribe(
            () => {
                this.group.removeMonster(monster.id);
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
            return Observable.from([]);
        }
        return this._monsterService.searchMonster(filter).map(
            list => list.map(e => new AutocompleteValue(e, e.name))
        );
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
        if (this.currentFighterIndex == null) {
            return;
        }
        if (this.currentFighterIndex < this.group.fighters.length - 1) {
            this.currentFighterIndex++;
        } else {
            this.loadingNextLap = true;
            this._groupService.nextLap(this.group.id).subscribe(
                () => {
                    this.loadingNextLap = false;
                    this.currentFighterIndex = 0;
                },
                () => {
                    this.loadingNextLap = false;
                }
            );
        }
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

        this.group.data.onChange.subscribe((v: {key: string, value: any}) => {
           if (v.key === 'inCombat') {
               this.currentFighterIndex = 0;
           }
        });


        if (this.group.data.inCombat) {
            this.currentFighterIndex = 0;
        }

        this._groupService.loadDeadMonsters(this.group.id, 0, 10).subscribe(
            monsters => {
                this.deadMonsters = monsters;
            }
        );
    }
}
