import {Component, Input, OnInit} from '@angular/core';
import {Group, Fighter} from "./group.model";
import {Monster, MonsterTemplate} from "../monster/monster.model";
import {GroupService} from "./group.service";
import {ItemData, Item} from "../character/item.model";
import {ItemTemplate} from "../item/item-template.model";
import {Observable} from "rxjs";
import {AutocompleteValue} from "../shared/autocomplete-input.component";
import {MonsterService} from "../monster/monster.service";
import {Character} from "../character/character.model";
import {getRandomInt} from "../shared/random";
import {NotificationsService} from "../notifications/notifications.service";
import {GroupActionService} from "./group-action.service";
import {CharacterService} from "../character/character.service";

@Component({
    selector: 'fighter-panel',
    templateUrl: 'fighter-panel.component.html',
    styles: [`
        .even_row {
            background-color: #f9f9f9;
        }
        .combat_row_selected {
            background-color: #e3e3e3!important;
        }
    `],
})
export class FighterPanelComponent implements OnInit{
    @Input() group: Group;
    @Input() characters: Character[];
    public monsters: Monster[] = [];
    public deadMonsters: Monster[] = [];
    public charAndMonsters: Fighter[] = [];

    public newMonster: Monster = new Monster();
    private selectedCombatRow: number = 0;
    private addItemTarget: Character;

    constructor(private _groupService: GroupService
        , private _actionService: GroupActionService
        , private _characterService: CharacterService
        , private _notification: NotificationsService
        , private _monsterService: MonsterService) {
    }

    addItemTo(character: Character) {
        this.addItemTarget = character;
    }

    onItemAdded() {
        this.addItemTarget = null;
    }

    selectCombatRow(i: number) {
        this.selectedCombatRow = i;
    }

    itemHasSlot(template: ItemTemplate, slot: string) {
        return ItemTemplate.hasSlot(template, slot);
    }

    /**
     * Using data in monster creation form (bound with `newMonster`), post request to create a new monster
     */
    addMonster(): void {
        this._groupService.createMonster(this.group.id, this.newMonster).subscribe(
            monster => {
                this.monsters.push(monster);
                this.updateOrder();
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
                this.deadMonsters.push(monster);
                let idx = this.monsters.findIndex(m => m.id == monster.id);
                if (idx != -1)
                    this.monsters.splice(idx, 1);
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
                let idx = this.monsters.findIndex(m => m.id == monster.id);
                if (idx != -1)
                    this.monsters.splice(idx, 1);
                this.updateOrder();
            }
        );
    }

    private selectedMonsterTemplate: MonsterTemplate;
    private monsterAutocompleteShow = false;
    private autocompleteMonsterListCallback = this.updateMonsterListAutocomplete.bind(this);

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
                item.template = <ItemTemplate>inventoryItem.itemTemplate;
                item.data.name = inventoryItem.itemTemplate.name;
                item.data.notIdentified = true;
                if (item.template.data.useUG) {
                    item.data.ug = 1; //FIXME: minUg/maxUg
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


    changeTarget(element, target) {
        if (element.isMonster) {
            this._groupService.updateMonster(element.id, 'target', {id: target.id, isMonster: target.isMonster})
                .subscribe(
                    () => {
                        element.changeTarget(target);
                        element.updateTarget(this.charAndMonsters);
                        this._notification.info("Monstre", "Cible changée");
                    }
                );
        } else {
            this._characterService.changeGmData(element.id, 'target', {
                id: target.id,
                isMonster: target.isMonster
            }).subscribe(
                change => {
                    element.changeTarget(change.value);
                    element.updateTarget(this.charAndMonsters);
                    this._notification.info("Joueur", "Cible changée");
                }
            );
        }
    }

    changeColor(element: Fighter, color: string) {
        if (color.indexOf('#') === 0) {
            color = color.substring(1);
        }
        if (element.isMonster) {
            this._groupService.updateMonster(element.id, 'color', color)
                .subscribe(
                    res => {
                        element.changeColor(res.value);
                        this.charAndMonsters.forEach(f => f.updateTarget(this.charAndMonsters));
                        this._notification.info("Monstre", "Couleur changé");
                    }
                );
        } else {
            this._characterService.changeGmData(element.id, 'color', color).subscribe(
                change => {
                    element.changeColor(change.value);
                    this.charAndMonsters.forEach(f => f.updateTarget(this.charAndMonsters));
                    this._notification.info("Joueur", "Couleur changée");
                }
            );
        }
    }

    updateMonsterField(monster: Monster, fieldName: string, newValue: any) {
        this._groupService.updateMonster(monster.id, fieldName, newValue)
            .subscribe(
                res => {
                    this._notification.info('Monstre: ' + monster.name
                        , 'Modification: ' + fieldName.toUpperCase() + ': '
                        + monster.data[fieldName] + ' -> ' + res.value);
                    monster.data[fieldName] = res.value;
                }
            );
    }

    changeNumber(element: Fighter, number: number) {
        if (element.isMonster) {
            this._groupService.updateMonster(element.id, 'number', number)
                .subscribe(
                    res => {
                        element.changeNumber(res.value);
                        this.charAndMonsters.forEach(f => f.updateTarget(this.charAndMonsters));
                        this._notification.info("Monstre", "Couleur changé");
                    }
                );
        }
    }


    updateOrder() {
        let deadMonsters = [];
        let charAndMonsters: Fighter[] = [];
        for (let i = 0; i < this.characters.length; i++) {
            let character = this.characters[i];
            if (character.active) {
                let fighter = Fighter.createFromCharacter(character);
                fighter.chercheNoise = Character.hasChercherDesNoises(character);
                charAndMonsters.push(fighter);
            }
        }
        for (let i = 0; i < this.monsters.length; i++) {
            let monster = this.monsters[i];
            if (monster.dead) {
                deadMonsters.push(monster);
            } else {
                charAndMonsters.push(Fighter.createFromMonster(monster));
            }
        }
        deadMonsters.sort((first, second) => {
            let a = new Date(first.dead);
            let b = new Date(second.dead);
            return a > b ? -1 : a < b ? 1 : 0;
        });

        while (deadMonsters.length > 10) {
            deadMonsters.pop();
        }

        charAndMonsters.sort(function (first, second) {
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
        charAndMonsters.forEach(f => f.updateTarget(charAndMonsters));
        this.charAndMonsters = charAndMonsters;
        this.deadMonsters = deadMonsters;
    }

    selectMonsterInAutocompleteList(monster: MonsterTemplate) {
        this.selectedMonsterTemplate = monster;
        this.monsterAutocompleteShow = false;
        this.newMonster.name = monster.name;
        this.newMonster.data.at = monster.data.at;
        this.newMonster.data.prd = monster.data.prd;
        this.newMonster.data.esq= monster.data.esq;
        this.newMonster.data.ev = monster.data.ev;
        this.newMonster.data.maxEv = monster.data.ev;
        this.newMonster.data.ea = monster.data.ea;
        this.newMonster.data.maxEa= monster.data.ev;
        this.newMonster.data.pr = monster.data.pr;
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



    ngOnChanges() {
        this.updateOrder();
    }

    ngOnInit(): void {
        this._actionService.registerAction('reorderFighters').subscribe(
            () => {
                this.updateOrder();
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
