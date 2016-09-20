import {Component, EventEmitter, Input, OnInit, Output, OnDestroy} from '@angular/core';
import {ActivatedRoute} from "@angular/router";

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

import {Item, ItemData, PartialItem} from "./item.model";
import {SpecialitySelectorComponent} from './speciality-selector.component';
import {CharacterService} from "./character.service";
import {Character, CharacterModifier} from "./character.model";
import {IMetadata} from '../shared/misc.model';
import {ItemDetailComponent} from './item-detail.component';
import {EffectCategory} from '../effect/effect.model';
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
    styles: [`
        .canceled {
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
        , private _notification: NotificationsService
        , private _webSocketService: WebSocketService
        , private _characterService: CharacterService) {
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
            }
        );
        this.historyPage++;
        return false;
    }

    notifyChange(message: string) {
        if (this.inGroupTab) {
            this._notification.info(this.character.name
                , message
                , {isCharacter: true, color: this.character.color}
            );
        } else {
            this._notification.info("Modification", message);
        }
    }

    registerWS() {
        this._webSocketService.register('character', this.character).subscribe(
            res => {
                try {
                    switch (res.opcode) {
                        case "update":
                            this.onChangeCharacterStat(res.data);
                            break;
                        case "statBonusAd":
                            this.onSetStatBonusAD(res.data);
                            break;
                        case "levelUp":
                            this.onLevelUp(res.data);
                            break;
                        case "addEffect":
                            this.onAddEffect(res.data);
                            break;
                        case "removeEffect":
                            this.onRemoveEffect(res.data);
                            break;
                        case "addModifier":
                            this.onAddModifier(res.data);
                            break;
                        case "removeModifier":
                            this.onRemoveModifier(res.data);
                            break;
                        case "equipItem":
                            this.onEquipItem(res.data);
                            break;
                        case "addItem":
                            this.onAddItem(res.data);
                            break;
                        case "deleteItem":
                            this.onDeleteItem(res.data);
                            break;
                        case "identifyItem":
                            this.onIdentifyItem(res.data);
                            break;
                        case "useCharge":
                            this.onUseItemCharge(res.data);
                            break;
                        case "changeContainer":
                            this.onChangeContainer(res.data);
                            break;
                        case "updateItemName":
                            this.onUpdateItemName(res.data);
                            break;
                        case "changeQuantity":
                            this.onUpdateQuantity(res.data);
                            break;
                    }
                }
                catch (err) {
                    this._notification.error("Erreur", "Erreur WS");
                    this._characterService.postJson('/api/debug/report', err).subscribe();
                    console.log(err);
                }
            }
        );
    }

    changeCharacterStat(stat: string, value: any) {
        this._characterService.changeCharacterStat(this.character.id, stat, value).subscribe(
            this.onChangeCharacterStat.bind(this)
        );
    }

    onChangeCharacterStat(change: any) {
        if (this.character[change.stat] !== change.value) {
            this.notifyChange(change.stat.toUpperCase() + ": " + this.character[change.stat] + ' -> ' + change.value);
            this.character[change.stat] = change.value;
            this.character.update();
        }
    }

    setStatBonusAD(id: number, stat: string) {
        if (this.character) {
            this._characterService.setStatBonusAD(id, stat).subscribe(
                this.onSetStatBonusAD.bind(this)
            );
        }
    }

    onSetStatBonusAD(bonusStat: any) {
        if (this.character.statBonusAD !== bonusStat) {
            this.notifyChange("Stat bonus/malus AD défini sur " + bonusStat);
            this.character.statBonusAD = bonusStat;
            this.character.update();
        }
    }

    levelUp() {
        if (this.character) {
            this._characterService.LevelUp(this.character.id, this.levelUpInfo).subscribe(
                this.onLevelUp.bind(this)
            );
        }
    }

    onLevelUp(result: any) {
        if (this.character.level !== result.level) {
            this.notifyChange("Levelup ! " + this.character.level + '->' + result.level);
            this.character.level = result.level;
            this._characterService.getCharacter(result.id).subscribe(
                character => {
                    character.onUpdate = this.character.onUpdate;
                    this.character = character;
                    character.update();
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

    changeGmData(key: string, value: any) {
        this._characterService.changeGmData(this.character.id, key, value).subscribe(
            change => {
                this._notification.info("Modification", key + ": " + this.character.gmData[change.key] + ' -> ' + change.value);
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

    setLevelUpStatToUp(stat: string) {
        this.levelUpInfo.statToUp = stat;
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
            this.onAddEffect.bind(this)
        );
    }

    onAddEffect(effect: Effect) {
        for (let i = 0; i < this.character.effects.length; i++) {
            if (this.character.effects[i].id === effect.id) {
                return;
            }
        }

        this.notifyChange('Ajout de l\'effet: ' + effect.name);
        this.character.effects.push(effect);
        this.character.update();
    }

    removeEffect(effect) {
        this.selectedEffect = null;
        this._characterService.removeEffect(this.character.id, effect.id).subscribe(
            this.onRemoveEffect.bind(this)
        );
    }

    onRemoveEffect(effect: Effect) {
        for (let i = 0; i < this.character.effects.length; i++) {
            let e = this.character.effects[i];
            if (e.id === effect.id) {
                this.notifyChange('Suppression de l\'effetde: ' + effect.name);
                this.character.effects.splice(i, 1);
                this.character.update();
                return;
            }
        }
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
                this.onAddModifier.bind(this)
            );
        }
    }

    onAddModifier(modifier: CharacterModifier) {
        for (let i = 0; i < this.character.modifiers.length; i++) {
            if (this.character.modifiers[i].id === modifier.id) {
                return;
            }
        }
        this.character.modifiers.push(modifier);
        this.character.update();
        this.customAddModifier = new CharacterModifier();
        this.notifyChange('Ajout du modificateur: ' + modifier.name);
    }

    removeModifier(modifier: CharacterModifier) {
        this.selectedModifier = null;
        this._characterService.removeModifier(this.character.id, modifier.id).subscribe(
            this.onRemoveModifier.bind(this)
        );
    }

    onRemoveModifier(modifier: CharacterModifier) {
        for (let i = 0; i < this.character.modifiers.length; i++) {
            let e = this.character.modifiers[i];
            if (e.id === modifier.id) {
                this.character.modifiers.splice(i, 1);
                this.character.update();
                this.notifyChange('Suppression du modificateur: ' + modifier.name);
                return;
            }
        }
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

    onAddItem(item: Item) {
        for (let i = 0; i < this.character.items.length; i++) {
            if (this.character.items[i].id === item.id) {
                return;
            }
        }

        this.notifyChange("Ajout de l'objet: " + item.data.name);
        this.character.items.push(item);
        this.character.update();
    }

    addItem() {
        if (this.character) {
            let itemData = new ItemData();
            itemData['name'] = this.itemAddCustomName;
            itemData['description'] = this.itemAddCustomDescription;
            itemData['quantity'] = this.itemAddQuantity;
            this._itemService.addItem(this.character.id
                , this.selectedAddItem.id
                , itemData)
                .subscribe(
                    item => {
                        this.onAddItem(item);
                        this.selectedItem = item;
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

    private itemFilterName: string;
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

    private sortType = 'none';
    sortInventory(type: string) {
        if (type === 'not_identified_first') {
            this.sortType = type;
            this.character.items.sort((a, b) =>
                {
                    if (a.data.notIdentified && b.data.notIdentified) {
                        return 0;
                    }
                    if (a.data.notIdentified) {
                        return -1;
                    }
                    if (b.data.notIdentified) {
                        return 1;
                    }
                    return a.data.name.localeCompare(b.data.name);
                }
            );

        }
        else {
            if (this.sortType !== 'asc') {
                this.sortType = 'asc';
                this.character.items.sort((a, b) =>
                    {
                        return a.data.name.localeCompare(b.data.name);
                    }
                );
            }
            else {
                this.sortType = 'desc';
                this.character.items.sort((a, b) =>
                    {
                        return 2 - a.data.name.localeCompare(b.data.name);
                    }
                );
            }
            this.character.update();
        }
    }

    onEquipItem(it: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let item = this.character.items[i];
            if (item.id === it.id) {
                if (item.data.equiped === it.data.equiped) {
                    return;
                }
                item.data.equiped = it.data.equiped;
                if (it.data.equiped) {
                    this.notifyChange('Equipe ' + item.data.name);
                } else {
                    this.notifyChange('Déséquipe ' + item.data.name);
                }
                this.character.update();
                return;
            }
        }
    }

    onDeleteItem(item: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                this.character.items.splice(i, 1);
                this.character.update();
                this.notifyChange("Suppression de l'objet: " + item.data.name);
                break;
            }
        }
    }

    onUseItemCharge(item: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                it.data.charge = item.data.charge;
                this.character.update();
                this.notifyChange("Utilisation d'une charge de l'objet: " + item.data.name);
                break;
            }
        }
    }

    onChangeContainer(item: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                it.container = item.container;
                this.character.update();
                break;
            }
        }
    }

    onIdentifyItem(item: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                it.data.name = item.data.name;
                it.data.notIdentified = item.data.notIdentified;
                this.character.update();
                this.notifyChange("Identification de l'objet: " + item.data.name);
                break;
            }
        }
    }

    onUpdateItemName(item: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                it.data = item.data;
                this.character.update();
                break;
            }
        }
    }

    onUpdateQuantity(item: PartialItem) {
        for (let i = 0; i < this.character.items.length; i++) {
            let it = this.character.items[i];
            if (it.id === item.id) {
                if (it.data.quantity !== item.data.quantity) {
                    this.notifyChange("Modification de la quantité de l'objet: " + item.data.name + ": " + item.data.quantity + " ->" + it.data.quantity);
                    it.data.quantity = item.data.quantity;
                    this.character.update();
                }
                break;
            }
        }
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
                this.onEquipItem.bind(this)
            );
        }
        else if (event.action === 'delete') {
            this._itemService.deleteItem(item.id).subscribe(
                deletedItem => {
                    this.onDeleteItem(deletedItem);
                    this.selectedItem = null;
                }
            );
        }
        else if (event.action === 'update_quantity') {
            this._itemService.updateQuantity(item.id, event.quantity).subscribe(
                res => {
                    this.onUpdateQuantity(res);
                }
            );
        }
        else if (event.action === 'read_skill_book') {
            this._itemService.readBook(item.id).subscribe(
                res => {
                    item.data.readCount = res.data.readCount;
                    this.character.update();
                }
            );
        }
        else if (event.action === 'identify') {
            this._itemService.identify(item.id).subscribe(
                this.onIdentifyItem.bind(this)
            );
        }
        else if (event.action === 'use_charge') {
            this._itemService.updateCharge(item.id, item.data.charge - 1).subscribe(
                this.onUseItemCharge.bind(this)
            );
        }
        else if (event.action === 'move_to_container') {
            this._itemService.moveToContainer(item.id, event.container).subscribe(
                this.onChangeContainer.bind(this)
            );
        }
        else if (event.action === 'edit_item_name') {
            let data = {
                name: event.name,
                description: event.description
            };
            this._itemService.updateItem(item.id, data).subscribe(
                this.onUpdateItemName.bind(this)
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
            }
        );
        return false;
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
                        }
                    );
                }
            );
        }
    }
}
