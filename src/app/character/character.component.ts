import {Component, EventEmitter, Input, OnInit, Output, OnDestroy} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {Subject, Subscription} from 'rxjs';

import {NotificationsService} from '../notifications';

import {SkillSelectorComponent} from '../skill';
import {Effect, EffectService} from '../effect';
import {ItemService, ItemTemplate} from '../item';
import {
    ModifierPipe,
    removeDiacritics,
    ModifiersEditorComponent,
    PlusMinusPipe,
    ValueEditorComponent
} from "../shared";

import {Item} from "./item.model";
import {SpecialitySelectorComponent} from './speciality-selector.component';
import {CharacterService} from "./character.service";
import {Character, CharacterModifier} from "./character.model";
import {IMetadata} from '../shared/misc.model';
import {ItemDetailComponent} from './item-detail.component';
import {EffectCategory} from '../effect/effect.model';
import {SkillService} from '../skill';
import {WebSocketService} from '../shared/websocket.service';

@Component({
    selector: 'bag-item-view',
    template: `
        <template ngFor let-item [ngForOf]="items"  let-indexItem="index" let-lastItem="last">
            <template ngFor let-l [ngForOf]="ends" let-i="index" let-last="last">
                <div style="float:left;margin-bottom:-1px">
                    <template [ngIf]="lastItem && last">
                        <img src="/img/tree-top-left.png"/>
                    </template>
                    <template [ngIf]="!lastItem && last">
                        <img src="/img/tree-top-bot-left.png"/>
                    </template>
                    
                    <template [ngIf]="!last">
                        <template [ngIf]="ends[i + 1]">
                            <img src="/img/tree-empty.png"/>
                        </template>
                        <template [ngIf]="!ends[i + 1]">
                            <img src="/img/tree-top-bot.png"/>
                        </template>
                    </template>
                </div>
            </template>
            <a href="#" class="list-group-item"
                [style.margin-left]="(level * 20) + 'px'"
                (click)="selectItem(item)"
                [class.active]="selectedItem && selectedItem.id == item.id">
                <span *ngIf="item.data.quantity">{{item.data.quantity}}</span>
                {{item.data.name}}
            </a>
            <template [ngIf]="item.content">
                <bag-item-view
                    [ends]="ends"
                    [level]="level + 1"
                    [end]="lastItem ? 1 : 0"
                    [items]="item.content"
                    [selectedItem]="selectedItem"
                    (itemSelected)="selectItem($event)">
                </bag-item-view>
            </template>
        </template>
    `,
    directives: [BagItemViewComponent],
})
export class BagItemViewComponent implements OnInit {
    @Input() items: Item[];
    @Input() selectedItem: Item;
    @Input() level: number = 0;
    @Input() end: number;
    @Input() ends: number[] = [];
    @Output() itemSelected: EventEmitter<Item> = new EventEmitter<Item>();

    selectItem(item) {
        this.itemSelected.emit(item);
        return false;
    }

    ngOnInit() {
        this.ends = JSON.parse(JSON.stringify(this.ends));
        if (this.end != null) {
            this.ends.push(this.end);
        }
    }
}

class LevelUpInfo {
    EVorEA: string = 'EV';
    EVorEAValue: number;
    targetLevelUp: number;
    statToUp: string;
    skill: any;
    speciality: any;
}

@Component({
    selector: 'character',
    moduleId: module.id,
    templateUrl: 'character.component.html',
    pipes: [PlusMinusPipe, ModifierPipe],
    styles: [
        `.canceled {
            text-decoration: line-through;
        }
        .table-stats > td {
            line-height: 1;
        }
        .stats-detail-name > td {
            padding-top: 0;
            padding-bottom: 0;
        }
        .stats-detail-values > td {
            padding-top: 0;
            border-top: 0;
        }
        `
    ],
    directives: [BagItemViewComponent
        , SkillSelectorComponent
        , SpecialitySelectorComponent
        , ItemDetailComponent
        , ModifiersEditorComponent
        , ValueEditorComponent
    ]
})
export class CharacterComponent implements OnInit, OnDestroy {
    @Input() id: number;
    @Input() character: Character;
    public levelUpInfo: LevelUpInfo = new LevelUpInfo();
    public selectedItem: Item;
    private inGroupTab: boolean = false;

    constructor(private _route: ActivatedRoute
        , private _itemService: ItemService
        , private _effectService: EffectService
        , private _skillService: SkillService
        , private _notification: NotificationsService
        , private _webSocketService: WebSocketService
        , private _characterService: CharacterService) {
    }

    onChangeCharacterStat(change: any) {
        if (this.character[change.stat] !== change.value) {
            if (this.inGroupTab) {
                this._notification.info(this.character.name
                    , change.stat.toUpperCase() + ": " + this.character[change.stat] + ' -> ' + change.value
                    , {isCharacter: true, color: this.character.color}
                );
            } else {
                this._notification.info("Modification"
                    , change.stat.toUpperCase() + ": " + this.character[change.stat] + ' -> ' + change.value
                );
            }
            this.character[change.stat] = change.value;
            this.character.update();
        }
    }

    changeCharacterStat(stat: string, value: any) {
        this._characterService.changeCharacterStat(this.character.id, stat, value).subscribe(
            this.onChangeCharacterStat.bind(this),
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
    }

    changeGmData(key: string, value: any) {
        this._characterService.changeGmData(this.character.id, key, value).subscribe(
            change => {
                this._notification.info("Modification", key + ": " + this.character.gmData[change.key] + ' -> ' + change.value);
                this.character.gmData[change.key] = change.value;
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
    }

    public historyPage: number = 0;
    public currentDay: string = null;
    public history;
    public loadMore: boolean;

    loadHistory(next) {
        if (!next) {
            this.historyPage = 0;
            this.currentDay = null;
            this.history = [];
        }

        this._characterService.loadHistory(this.character.id, this.historyPage).subscribe(
            res => {
                if (res.length === 0) {
                    this.loadMore = false;
                    return;
                }
                this.loadMore = true;
                let logs = [];
                if (this.currentDay) {
                    logs = this.history[this.history.length - 1].logs;
                }
                for (let i = 0; i < res.length; i++) {
                    let l = res[i];
                    l.date = new Date(l.date);

                    let day = l.date.toString().substring(0, 15);
                    if (!this.currentDay || day !== this.currentDay) {
                        this.currentDay = day;
                        logs = [];
                        this.history.push({logs: logs, date: l.date});
                    }
                    logs.push(l);
                }
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
        this.historyPage++;
        return false;
    }

    setStatBonusAD(id: number, stat: string) {
        if (this.character) {
            this._characterService.setStatBonusAD(id, stat).subscribe(
                res => {
                    this.character.statBonusAD = stat;
                    this.character.update();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
    }

    characterHasToken(token: string) {
        if (this.character.origin.specials) {
            if (this.character.origin.specials.indexOf(token) !== -1) {
                return true;
            }
        }
        if (this.character.job) {
            if (this.character.job.specials) {
                if (this.character.job.specials.indexOf(token) !== -1) {
                    return true;
                }
            }
        }
        if (this.character.specialities) {
            for (let i = 0; i < this.character.specialities.length; i++) {
                let speciality = this.character.specialities[i];
                if (speciality.specials) {
                    for (let j = 0; j < speciality.specials.length; j++) {
                        let special = speciality.specials[j];
                        if (special.token === token) {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
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

    initLevelUp() {
        this.levelUpInfo = new LevelUpInfo();
        this.levelUpInfo.EVorEA = 'EV';
        this.levelUpInfo.EVorEAValue = null;
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
            diceLevelUp = this.character.job.diceEaLevelUp;
        }
        this.levelUpInfo.EVorEAValue = Math.ceil(Math.random() * diceLevelUp);
    }

    onLevelUpSelectSkills(skills) {
        this.levelUpInfo.skill = skills[0];
    }

    levelUpShouldSelectSkill() {
        return this.levelUpInfo.targetLevelUp === 3
            || this.levelUpInfo.targetLevelUp === 6
            || this.levelUpInfo.targetLevelUp === 10;
    }

    levelUpShouldSelectSpeciality() {
        return this.characterHasToken('SELECT_SPECIALITY_LVL_5_10')
            && !this.characterHasToken('ONE_SPECIALITY')
            && (this.levelUpInfo.targetLevelUp === 5 || this.levelUpInfo.targetLevelUp === 10);
    }

    levelUpSelectSpeciality(speciality) {
        if (this.levelUpShouldSelectSpeciality()) {
            this.levelUpInfo.speciality = speciality;
        }
    }

    levelUpFormReady() {
        if (!this.levelUpInfo.EVorEAValue) {
            return false;
        }
        if (this.levelUpShouldSelectSpeciality()) {
            if (!this.levelUpInfo.speciality) {
                return false;
            }
        }
        if (this.levelUpShouldSelectSkill()) {
            if (!this.levelUpInfo.skill) {
                return false;
            }
        }
        return true;
    }

    levelUp() {
        if (this.character) {
            this._characterService.LevelUp(this.character.id, this.levelUpInfo).subscribe(
                character => {
                    character.update();
                    this.character = character;
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
    }

    public getItemById(itemId: number): Item {
        for (let i = 0; i < this.character.items.length; i++) {
            let item = this.character.items[i];
            if (item.id === itemId) {
                return item;
            }
        }
        return null;
    }

    public countInOtherTab: number = 0;

    useFatePoint() {
        if (this.character && this.character.fatePoint) {
            let ok = confirm("Utiliser un point de destin");
            if (ok) {
                this.changeCharacterStat('fatePoint', this.character.fatePoint - 1);
            }
        }
    }

    // Effect
    private selectedEffect: Effect;
    private filteredEffects: Effect[];
    private effectCategories: EffectCategory[];
    private effectCategoriesById: {[categoryId: number]: EffectCategory};
    private effectFilterName: string;
    private selectedModifier: CharacterModifier;

    updateFilterEffect() {
        if (this.effectFilterName) {
            this._effectService.searchEffect(this.effectFilterName).subscribe(
                effects => {
                    this.filteredEffects = effects;
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
    }

    addEffect(effect) {
        this._characterService.addEffect(this.character.id, effect.id).subscribe(
            res => {
                this.character.effects.push(res);
                this.character.update();
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
    }

    removeEffect(effect) {
        this._characterService.removeEffect(this.character.id, effect.id).subscribe(
            res => {
                for (let i = 0; i < this.character.effects.length; i++) {
                    let e = this.character.effects[i];
                    if (e.id === res.id) {
                        this.character.effects.splice(i, 1);
                        break;
                    }
                }
                this.selectedEffect = null;
                this.character.update();
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
    }

    selectEffect(effect) {
        this.selectedModifier = null;
        this.selectedEffect = effect;
    }

    // Custom modifier

    public customAddModifier: CharacterModifier = new CharacterModifier();

    addCustomModifier() {
        if (this.customAddModifier.name) {
            this._characterService.addModifier(this.character.id, this.customAddModifier).subscribe(
                res => {
                    this.character.modifiers.push(res);
                    this.character.update();
                    this.customAddModifier = new CharacterModifier();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
    }

    removeModifier(modifiers: CharacterModifier) {
        this._characterService.removeModifier(this.character.id, modifiers.id).subscribe(
            res => {
                for (let i = 0; i < this.character.modifiers.length; i++) {
                    let e = this.character.modifiers[i];
                    if (e.id === res.id) {
                        this.character.modifiers.splice(i, 1);
                        break;
                    }
                }
                this.selectedModifier = null;
                this.character.update();
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
    }

    selectModifier(modifier: CharacterModifier) {
        this.selectedEffect = null;
        this.selectedModifier = modifier;
    }

    // Inventory

    public selectedInventoryTab: string = 'all';
    public filteredItems: ItemTemplate[] = [];
    public itemAddCustomName: string;
    public itemAddCustomDescription: string;
    public selectedAddItem: ItemTemplate;
    public itemAddQuantity: number;

    updateFilterItem() {
        if (this.selectedInventoryTab === 'add') {
            this.updateFilterAddItem();
        }
    }

    updateFilterAddItem() {
        if (this.itemFilterName) {
            this._itemService.searchItem(this.itemFilterName).subscribe(
                items => {
                    this.filteredItems = items;
                    this.selectedAddItem = null;
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
    }

    selectAddItem(item: ItemTemplate) {
        this.selectedAddItem = item;
        this.itemAddCustomName = item.name;
        if (item.data.quantifiable) {
            this.itemAddQuantity = 1;
        } else {
            this.itemAddQuantity = null;
        }
        return false;
    }

    unselectAddItem() {
        this.selectedAddItem = null;
        return false;
    }

    isAddItemSelected(item) {
        return this.selectedAddItem && this.selectedAddItem.id === item.id;
    }

    addItem() {
        if (this.character) {
            this._itemService.addItem(this.character.id
                , this.selectedAddItem.id
                , this.itemAddCustomName
                , this.itemAddCustomDescription
                , this.itemAddQuantity)
                .subscribe(
                    res => {
                        this.character.items.push(res);
                        this.selectedItem = res;
                        this.character.update();
                        this.itemFilterName = "";
                        this.selectedAddItem = null;
                        this.itemAddCustomName = null;
                        this.itemAddCustomDescription = null;
                        this.itemAddQuantity = null;
                    }
                );
        }
    }

    selectItem(item: Item) {
        this.selectedItem = item;
        return false;
    }


    public itemFilterName: string;

    isItemFilteredByName(item: Item): boolean {
        if (!this.itemFilterName) {
            return true;
        }
        let cleanFilter = removeDiacritics(this.itemFilterName).toLowerCase();
        if (removeDiacritics(item.data.name).toLowerCase().indexOf(cleanFilter) > -1) {
            return true;
        }
        if (removeDiacritics(item.template.name).toLowerCase().indexOf(cleanFilter) > -1) {
            return true;
        }
        return false;
    }

    itemAction(event: any, item: Item) {
        if (!this.character) {
            return false;
        }
        if (event.action === 'equip' || event.action === 'unequip') {
            let level = 0;
            if (event.action === 'equip') {
                level = 1;
                if (event.level != null) {
                    level = event.level;
                }
            }
            this._itemService.equipItem(item.id, level).subscribe(
                res => {
                    item.data.equiped = res.data.equiped;
                    this.character.update();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
        else if (event.action === 'delete') {
            this._itemService.deleteItem(item.id).subscribe(
                deletedItem => {
                    for (let i = 0; i < this.character.items.length; i++) {
                        let it = this.character.items[i];
                        if (it.id === deletedItem.id) {
                            this.character.items.splice(i, 1);
                            break;
                        }
                    }
                    this.selectedItem = null;
                    this.character.update();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
        else if (event.action === 'update_quantity') {
            this._itemService.updateQuantity(item.id, event.quantity).subscribe(
                res => {
                    item.data.quantity = res.data.quantity;
                    this.character.update();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
        else if (event.action === 'read_skill_book') {
            this._itemService.readBook(item.id).subscribe(
                res => {
                    item.data.readCount = res.data.readCount;
                    this.character.update();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
        else if (event.action === 'use_charge') {
            this._itemService.updateCharge(item.id, item.data.charge - 1).subscribe(
                res => {
                    item.data.charge = res.data.charge;
                    this.character.update();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
        else if (event.action === 'move_to_container') {
            this._itemService.moveToContainer(item.id, event.container).subscribe(
                res => {
                    if (!res || !res.id) {
                        item.container = null;
                    } else {
                        item.container = res.id;
                    }
                    this.character.update();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
        else if (event.action === 'edit_item_name') {
            let data = {
                name: event.name,
                description: event.description
            };
            this._itemService.updateItem(item.id, data).subscribe(
                res => {

                    for (let key in res) {
                        if (item.hasOwnProperty(key) && res.hasOwnProperty(key)) {
                            item[key] = res[key];
                        }
                    }
                    this.character.update();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
        else {
            console.log(event);
        }
        return false;
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
            },
            err => {
                try {
                    let errJson = err.json();
                    this._notification.error("Erreur", errJson.error_code);
                } catch (e) {
                    console.log(err.stack);
                    this._notification.error("Erreur", "Erreur");
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
            },
            err => {
                try {
                    let errJson = err.json();
                    this._notification.error("Erreur", errJson.error_code);
                } catch (e) {
                    console.log(err.stack);
                    this._notification.error("Erreur", "Erreur");
                }
            }
        );
        return false;
    }

    registerWS() {
        this._webSocketService.register('character', this.character).subscribe(
            res => {
                switch (res.opcode) {
                    case "update": {
                        this.onChangeCharacterStat(res.data);
                        break;
                    }
                }
            }
        );
    }

    ngOnDestroy() {
        if (this.character) {
            this._webSocketService.unregister('character', this.character.id);
        }
    }

    ngOnInit() {
        this._effectService.getCategoryList().subscribe(
            categories => {
                this.effectCategories = categories;
                this.effectCategoriesById = {};
                categories.map(c => this.effectCategoriesById[c.id] = c);
            });

        if (this.character) {
            this.inGroupTab = true;
            this.registerWS();
        } else {
            this._route.params.subscribe(
                param => {
                    let id = this.id;
                    if (!this.id) {
                        id = +param['id'];
                    }
                    this._characterService.getCharacter(id).subscribe(
                        character => {
                            this.character = character;
                            this.registerWS();
                        },
                        err => {
                            console.log(err);
                            this._notification.error("Erreur", "Erreur");
                        }
                    );
                }
            );
        }
    }
}
