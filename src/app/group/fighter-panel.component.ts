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

@Component({
    selector: 'fighter-panel',
    templateUrl: './fighter-panel.component.html',
    styleUrls: ['./fighter.component.scss', './fighter-panel.component.scss']
})
export class FighterPanelComponent implements OnInit, OnChanges {
    @Input() group: Group;
    @Input() characters: Character[];
    public monsters: Monster[] = [];
    public fighters: Fighter[] = [];
    public currentFighterIndex: number | null = null;
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

    onItemAdded(data: {character: Character, item: Item}) {
        this._itemService.addItemTo('character', data.character.id, data.item.template.id, data.item.data).subscribe();
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
        this.selectedCombatRow = this.fighters.indexOf(fighter);
    }

    /**
     * Using data in monster creation form (bound with `newMonster`), post request to create a new monster
     */
    addMonster(): void {
        this._groupService.createMonster(this.group.id, this.newMonster).subscribe(
            monster => {
                this.monsters.push(monster);
                this.updateOrder();
                this.randomMonsterInventory();
            }
        );
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
                monster.dead = res.dead;
                this.deadMonsters.unshift(monster);
                let idx = this.monsters.findIndex(m => m.id === monster.id);
                if (idx !== -1) {
                    this.monsters.splice(idx, 1);
                }
                this.updateOrder();
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
                let idx = this.monsters.findIndex(m => m.id === monster.id);
                if (idx !== -1) {
                    this.monsters.splice(idx, 1);
                }
                this.updateOrder();
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
        if (this.currentFighterIndex < this.fighters.length - 1) {
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

    updateOrder() {
        let fighters: Fighter[] = [];

        for (let i = 0; i < this.characters.length; i++) {
            let character = this.characters[i];
            if (character.active) {
                let fighter = Fighter.createFromCharacter(character);
                fighter.chercheNoise = Character.hasChercherDesNoises(character);
                fighters.push(fighter);
            }
        }
        for (let i = 0; i < this.monsters.length; i++) {
            let monster = this.monsters[i];
            fighters.push(Fighter.createFromMonster(monster));
        }

        fighters.sort(function (first, second) {
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
        fighters.forEach(f => f.updateTarget(fighters));

        if (this.currentFighterIndex != null && this.fighters) {
            let index = fighters.findIndex(f => f === this.fighters[this.currentFighterIndex]);
            if (index !== -1 && index !== this.currentFighterIndex) {
                this.currentFighterIndex = index;
            }
            if (index === -1) {
                this.currentFighterIndex = Math.min(this.currentFighterIndex, fighters.length - 1);
            }
        }
        this.fighters = fighters;
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

    ngOnChanges(changes: SimpleChanges): void {
        if ('characters' in changes) {
            this.updateOrder();
        }
    }

    ngOnInit(): void {
        this.updateOrder();
        this._actionService.registerAction('reorderFighters').subscribe(
            () => {
                this.updateOrder();
            }
        );
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
                this.createItemComponent.openDialogForCharacter(data.data);
            }
        );

        this._actionService.registerAction('onStartCombat').subscribe(
            () => {
                this.currentFighterIndex = 0;
            }
        );

        this._actionService.registerAction('onStopCombat').subscribe(
            () => {
                this.currentFighterIndex = null;
            }
        );

        if (this.group.data.inCombat) {
            this.currentFighterIndex = 0;
        }

        this._groupService.loadDeadMonsters(this.group.id, 0, 10).subscribe(
            monsters => {
                this.deadMonsters = monsters;
            }
        );

        this._groupService.loadMonsters(this.group.id).subscribe(
            monsters => {
                this.monsters = monsters;
                this.updateOrder();
            }
        );
    }
}
