import {Component, EventEmitter} from '@angular/core';
import {ActivatedRoute} from "@angular/router";
import {NotificationsService} from '../notifications';

import {Skill, SkillSelectorComponent} from '../skill';
import {Effect, EffectService} from '../effect';
import {ItemService, ItemTemplate, ItemDetailComponent} from '../item';
import {
    ModifierPipe,
    formatModifierValue,
    removeDiacritics,
    ModifiersEditorComponent,
    PlusMinusPipe,
    ItemStatModifier
} from "../shared";

import {Item} from "./item.model";
import {SpecialitySelectorComponent} from './speciality-selector.component';
import {CharacterService} from "./character.service";
import {Character, CharacterModifier} from "./character.model";

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
                <span *ngIf="item.quantity">{{item.quantity}}</span>
                {{item.name}}
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
    inputs: ['items', 'selectedItem', 'end', 'ends', 'level'],
    outputs: ['itemSelected'],
    directives: [BagItemView],
})
export class BagItemView {
    public items: Item[];
    private itemSelected: EventEmitter<Item> = new EventEmitter<Item>();
    private level: number = 0;
    public ends: number[] = [];
    public end: number;

    selectItem(item) {
        this.itemSelected.emit(item);
        return false;
    }

    ngOnInit() {
        this.ends = JSON.parse(JSON.stringify(this.ends));
        if (this.end != null) {
            this.ends.push(this.end);
            console.log(this.ends);
        }
    }
}

class StatisticDetail {
    evea: any[];
    atprd: any[];
    stat: any[];
    magic: any[];
    other: any[];
    show: {
        evea: boolean;
        atprd: boolean;
        stat: boolean;
        other: boolean;
        magic: boolean;
    };
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
    templateUrl: 'app/character/character.component.html',
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
    inputs: ['id'],
    directives: [BagItemView, SkillSelectorComponent, SpecialitySelectorComponent, ItemDetailComponent, ModifiersEditorComponent]
})
export class CharacterComponent {
    public id: number;
    public character: Character;
    public stats: {[statName: string]: number};
    public skills: {from: string[], skill: Skill, canceled?: boolean}[];
    public containers: Object[];
    public levelUpInfo: LevelUpInfo = new LevelUpInfo();
    public details: StatisticDetail;
    public selectedItem: Item;

    constructor(private _route: ActivatedRoute
        , private _itemService: ItemService
        , private _effectService: EffectService
        , private _notification: NotificationsService
        , private _characterService: CharacterService) {
    }

    changeCharacterStat(stat: string, value: any) {
        this._characterService.changeCharacterStat(this.character.id, stat, value).subscribe(
            change => {
                this._notification.info("Modification", stat.toUpperCase() + ": " + this.character[change.stat] + ' -> ' + change.value);
                this.character[change.stat] = change.value;
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
                if (res.length == 0) {
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
                    if (!this.currentDay || day != this.currentDay) {
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

    private evEditField: string;

    modifyEv() {
        if (this.evEditField == null) {
            return;
        }
        if (this.evEditField == 'max') {
            this.changeCharacterStat('ev', this.stats['EV']);
            this.evEditField = null;
            return;
        }
        if (isNaN(parseInt(this.evEditField))) {
            this.evEditField = null;
            return;
        }
        if (this.evEditField.lastIndexOf('+', 0) == 0 || this.evEditField.lastIndexOf('-', 0) == 0) {
            this.changeCharacterStat('ev', this.character.ev + parseInt(this.evEditField));
        } else {
            this.changeCharacterStat('ev', parseInt(this.evEditField));
        }
        this.evEditField = null;
    }

    private eaEditField: string;

    modifyEa() {
        if (this.eaEditField == null) {
            return;
        }
        if (this.eaEditField == 'max') {
            this.changeCharacterStat('ea', this.stats['EA']);
            this.eaEditField = null;
            return;
        }
        if (isNaN(parseInt(this.eaEditField))) {
            this.eaEditField = null;
            return;
        }
        if (this.eaEditField.lastIndexOf('+', 0) == 0 || this.eaEditField.lastIndexOf('-', 0) == 0) {
            this.changeCharacterStat('ea', this.character.ea + parseInt(this.eaEditField));
        } else {
            this.changeCharacterStat('ea', parseInt(this.eaEditField));
        }
        this.eaEditField = null;
    }

    setStatBonusAD(id: number, stat: string) {
        if (this.character) {
            this._characterService.setStatBonusAD(id, stat).subscribe(
                res => {
                    this.character.statBonusAD = stat;
                    this.updateStats();
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
            if (this.character.origin.specials.indexOf(token) != -1) {
                return true;
            }
        }
        if (this.character.job) {
            if (this.character.job.specials) {
                if (this.character.job.specials.indexOf(token) != -1) {
                    return true;
                }
            }
        }
        if (this.character.specialities) {
            for (let i = 0; i < this.character.specialities.length; i++) {
                var speciality = this.character.specialities[i];
                if (speciality.specials) {
                    for (let j = 0; j < speciality.specials.length; j++) {
                        var special = speciality.specials[j];
                        if (special.token == token) {
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
        var level = 1;
        var xp = this.character.experience;
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
        if (this.levelUpInfo.targetLevelUp % 2 == 0) {
            this.levelUpInfo.statToUp = 'FO';
        }
        else {
            this.levelUpInfo.statToUp = 'AT';
        }
    }

    rollLevelUp() {
        var diceLevelUp = this.character.origin.diceEVLevelUp;
        if (this.levelUpInfo.EVorEA == 'EV') {
            if (this.characterHasToken('LEVELUP_DICE_EV_-1')) {
                this.levelUpInfo.EVorEAValue = Math.max(1, Math.ceil(Math.random() * diceLevelUp) - 1);
                return;
            }
        } else {
            diceLevelUp = this.character.job.diceEALevelUp;
        }
        this.levelUpInfo.EVorEAValue = Math.ceil(Math.random() * diceLevelUp);
    }

    onLevelUpSelectSkills(skills) {
        this.levelUpInfo.skill = skills[0];
    }

    levelUpShouldSelectSkill() {
        return this.levelUpInfo.targetLevelUp == 3
            || this.levelUpInfo.targetLevelUp == 6
            || this.levelUpInfo.targetLevelUp == 10;
    }

    levelUpShouldSelectSpeciality() {
        return this.characterHasToken('SELECT_SPECIALITY_LVL_5_10')
            && !this.characterHasToken('ONE_SPECIALITY')
            && (this.levelUpInfo.targetLevelUp == 5 || this.levelUpInfo.targetLevelUp == 10);
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
                res => {
                    this.character = res.json();
                    this.updateStats();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
    }

    initDetails() {
        let details: StatisticDetail = {
            evea: [],
            atprd: [],
            stat: [],
            magic: [],
            other: [],
            show: {
                evea: false, atprd: false, stat: false, other: false, magic: false
            }
        };
        if (this.details && this.details.show) {
            details.show = this.details.show;
        }
        this.details = details;
    }

    getDetailCategoryForStat(statName: string) {
        if (statName == 'EV' || statName == 'EA') {
            return 'evea';
        }
        if (statName == 'AT' || statName == 'PRD' || statName == 'PR' || statName == 'PR_MAGIC') {
            return 'atprd';
        }
        if (statName == 'COU'
            || statName == 'FO'
            || statName == 'AD'
            || statName == 'CHA'
            || statName == 'INT') {
            return 'stat';
        }
        if (statName == 'MV'
            || statName == 'THROW_MODIFIER'
            || statName == 'DISCRETION_MODIFIER'
            || statName == 'DANSE_MODIFIER'
            || statName == 'PI') {
            return 'other';
        }
        if (statName == 'RESM'
            || statName == 'MPSY'
            || statName == 'MPHYS') {
            return 'magic';
        }
        return 'unk';
    }

    addDetails(name: string, data: {[statName: string]: any}) {
        let categories: {[categoryName: string]: any} = {};
        for (let i in data) {
            if (!data.hasOwnProperty(i)) {
                continue;
            }
            var category = this.getDetailCategoryForStat(i);
            if (!this.details[category]) {
                this.details[category] = [];
            }
            categories[category] = 1;
        }
        for (var i in categories) {
            if (i == 'evea') {
                this.details.evea.push({
                    name: name,
                    data: {
                        EV: data['EV'],
                        EA: data['EA']
                    }
                });
            }
            if (i == 'atprd') {
                this.details.atprd.push({
                    name: name,
                    data: {
                        AT: data['AT'],
                        PRD: data['PRD'],
                        PR: data['PR'],
                        PR_MAGIC: data['PR_MAGIC']
                    }
                });
            }
            if (i == 'stat') {
                this.details.stat.push({
                    name: name,
                    data: {
                        COU: data['COU']
                        , FO: data['FO']
                        , CHA: data['CHA']
                        , AD: data['AD']
                        , INT: data['INT']
                    }
                });
            }
            if (i == 'other') {
                this.details.other.push({
                    name: name,
                    data: {
                        MV: data['MV']
                        , THROW_MODIFIER: data['THROW_MODIFIER']
                        , DISCRETION_MODIFIER: data['DISCRETION_MODIFIER']
                        , DANSE_MODIFIER: data['DANSE_MODIFIER']
                        , PI: data['PI']
                    }
                });
            }
            if (i == 'magic') {
                this.details.magic.push({
                    name: name,
                    data: {
                        MPHYS: data['MPHYS']
                        , MPSY: data['MPSY']
                        , RESM: data['RESM']
                    }
                });
            }
        }
    }

    // Concatenate modifiers like [-2 PRD] and [+2 PRD for dwarf]
    cleanItemModifiers(item: Item) {
        var cleanModifiers: ItemStatModifier[] = [];
        if (item.template.modifiers) {
            for (let i = 0; i < item.template.modifiers.length; i++) {
                let modifier = item.template.modifiers[i];
                if (modifier.job && modifier.job != this.character.job.id) {
                    continue;
                }
                if (modifier.origin && modifier.origin != this.character.origin.id) {
                    continue;
                }
                let newModifier = JSON.parse(JSON.stringify(modifier));
                for (let j = 0; j < cleanModifiers.length; j++) {
                    let newMod = cleanModifiers[j];
                    if (newModifier.stat == newMod.stat
                        && newModifier.type == newMod.type
                        && (!newModifier.special || newModifier.special.length == 0)
                        && (!newMod.special || newMod.special.length == 0)) {
                        newMod.value += newModifier.value;
                        newModifier = null;
                        break;
                    }
                }
                if (newModifier) {
                    cleanModifiers.push(newModifier);
                }
            }
        }
        return cleanModifiers;
    }

    public countInOtherTab: number = 0;

    updateStats() {
        this.initDetails();
        this.stats = JSON.parse(JSON.stringify(this.character.stats));
        this.addDetails('Jet de dé initial', this.character.stats);
        this.stats['AT'] = 8;
        this.stats['PRD'] = 10;
        this.stats['MV'] = 100;
        this.stats['PR'] = 0;
        this.stats['PR_MAGIC'] = 0;
        this.stats['RESM'] = 0;
        this.stats['PI'] = 0;
        if (this.character.origin.speedModifier) {
            this.stats['MV'] += this.character.origin.speedModifier;
            if (this.character.origin.speedModifier > 0) {
                this.addDetails('Origine', {MV: '+' + this.character.origin.speedModifier + '%'});
            } else {
                this.addDetails('Origine', {MV: this.character.origin.speedModifier + '%'});
            }
        }
        this.addDetails('Valeurs initial', {AT: 8, PRD: 10});
        this.stats['EV'] = this.character.origin.baseEV;
        this.stats['EA'] = null;
        this.addDetails('Origine', {EV: this.character.origin.baseEV});

        if (this.character.origin) {
            this.stats['AT'] += this.character.origin.bonusAT;
            this.stats['PRD'] += this.character.origin.bonusPRD;
            if (this.character.origin.bonusAT || this.character.origin.bonusPRD) {
                this.addDetails('Origine', {AT: this.character.origin.bonusAT, PRD: this.character.origin.bonusPRD});
            }
        }
        if (this.character.job) {
            if (this.character.job.baseEV) {
                this.stats['EV'] = this.character.job.baseEV;
                this.addDetails('Métiers (changement de la valeur de base)', {EV: this.character.origin.baseEV});
            }
            if (this.character.job.factorEV) {
                this.stats['EV'] *= this.character.job.factorEV;
                this.stats['EV'] = Math.round(this.stats['EV']);
                this.addDetails('Métiers (% de vie)', {EV: (Math.round((1 - this.character.job.factorEV) * 100)) + '%'});
            }
            if (this.character.job.bonusEV) {
                this.stats['EV'] += this.character.job.bonusEV;
                this.addDetails('Métiers (bonus de EV)', {EV: this.character.job.bonusEV});
            }
            if (this.character.job.baseEA) {
                this.addDetails('Métiers (EA de base)', {EA: this.character.job.baseEA});
                this.stats['EA'] = this.character.job.baseEA;
            }
        }
        for (let i = 0; i < this.character.specialities.length; i++) {
            let speciality = this.character.specialities[i];
            let detailData = {};
            if (speciality.modifiers) {
                for (let j = 0; j < speciality.modifiers.length; j++) {
                    var modifier = speciality.modifiers[j];
                    this.stats[modifier.stat] += modifier.value;
                    detailData[modifier.stat] = modifier.value;
                }
            }
            this.addDetails('Specialite: ' + speciality.name, detailData);
        }
        for (let i = 0; i < this.character.modifiers.length; i++) {
            let modifier = this.character.modifiers[i];
            let detailData = {};
            for (let j = 0; j < modifier.values.length; j++) {
                var value = modifier.values[j];
                if (value.type == 'ADD') {
                    this.stats[value.stat] += value.value;
                }
                else if (value.type == 'SET') {
                    this.stats[value.stat] = value.value;
                }
                else if (value.type == 'DIV') {
                    this.stats[value.stat] /= value.value;
                }
                else if (value.type == 'MUL') {
                    this.stats[value.stat] *= value.value;
                }
                else if (value.type == 'PERCENTAGE') {
                    this.stats[value.stat] *= (value.value / 100);
                }
                detailData[value.stat] = formatModifierValue(value);
            }
            this.addDetails(modifier.name, detailData);
        }

        let canceledSkills = {};
        this.skills = [];

        this.stats['THROW_MODIFIER'] = 0;
        this.stats['DISCRETION_MODIFIER'] = 0;
        this.stats['DANSE_MODIFIER'] = 0;
        this.stats['CHA_WITHOUT_MAGIEPSY'] = 0;
        for (let i = 0; i < this.itemsEquiped.length; i++) {
            let item = this.itemsEquiped[i];
            if (item.template.charge) {
                continue;
            }
            let modifications = {};
            for (let u = 0; u < item.template.unskills.length; u++) {
                let skill = item.template.unskills[u];
                canceledSkills[skill.id] = item;
            }
            for (let u = 0; u < item.template.skills.length; u++) {
                let skill = item.template.skills[u];
                this.skills.push({
                    skill: JSON.parse(JSON.stringify(skill)),
                    from: [item.name]
                });
            }
            let somethingOver = false;
            for (let s = 0; s < item.template.slots.length; s++) {
                let slot = item.template.slots[s];
                for (let i2 = 0; i2 < this.itemsBySlots[slot.id].length; i2++) {
                    let item2 = this.itemsBySlots[slot.id][i2];
                    if (item2.id == item.id) {
                        continue;
                    }
                    if (item.equiped < item2.equiped) {
                        somethingOver = true;
                        break;
                    }
                }
            }

            let cleanModifiers = this.cleanItemModifiers(item);
            for (let m in cleanModifiers) {
                let modifier = cleanModifiers[m];
                if (modifier.job && modifier.job != this.character.job.id) {
                    continue;
                }
                if (modifier.origin && modifier.origin != this.character.origin.id) {
                    continue;
                }
                let affectStats = true;
                let overrideStatName = modifier.stat;
                if (modifier.special) {
                    if (modifier.special.indexOf("ONLY_IF_NOTHING_ON") >= 0) {
                        if (somethingOver) {
                            continue;
                        }
                    }
                    if (modifier.special.indexOf("AFFECT_ONLY_THROW") >= 0) {
                        overrideStatName = 'THROW_MODIFIER';
                    }
                    if (modifier.special.indexOf("DONT_AFFECT_MAGIEPSY") >= 0) {
                        this.stats[overrideStatName] += modifier.value;
                        this.stats['CHA_WITHOUT_MAGIEPSY'] += modifier.value;
                        modifications[overrideStatName] = modifier.value + '(!MPsy)';
                        affectStats = false;
                    }
                    if (modifier.special.indexOf("AFFECT_ONLY_MELEE") >= 0) {
                        //FIXME
                    }
                    if (modifier.special.indexOf("AFFECT_ONLY_MELEE_STAFF") >= 0) {
                        //FIXME
                    }
                    if (modifier.special.indexOf("AFFECT_PR_FOR_ELEMENTS") >= 0) {
                        //FIXME
                    }
                    if (modifier.special.indexOf("AFFECT_DISCRETION") >= 0) {
                        overrideStatName = 'DISCRETION_MODIFIER';
                    }
                    if (modifier.special.indexOf("AFFECT_ONLY_DANSE") >= 0) {
                        overrideStatName = 'DANSE_MODIFIER';
                    }
                }
                if (affectStats) {
                    if (modifier.type == 'ADD') {
                        this.stats[overrideStatName] += modifier.value;
                    }
                    else if (modifier.type == 'SET') {
                        this.stats[overrideStatName] = modifier.value;
                    }
                    else if (modifier.type == 'DIV') {
                        this.stats[overrideStatName] /= modifier.value;
                    }
                    else if (modifier.type == 'MUL') {
                        this.stats[overrideStatName] *= modifier.value;
                    }
                    else if (modifier.type == 'PERCENTAGE') {
                        this.stats[overrideStatName] *= (modifier.value / 100);
                    }
                    if (modifications[overrideStatName] == null) {
                        modifications[overrideStatName] = 0;
                    }
                    modifications[overrideStatName] = formatModifierValue(modifier);
                }
            }
            if (item.template.protection) {
                modifications['PR'] = item.template.protection;
                this.stats['PR'] += item.template.protection;
            }
            if (item.template.magicProtection) {
                modifications['PR_MAGIC'] = item.template.magicProtection;
                this.stats['PR_MAGIC'] += item.template.magicProtection;
            }
            this.addDetails(item.name, modifications);
        }

        if (this.stats['AD'] > 12 && this.character.statBonusAD) {
            this.stats[this.character.statBonusAD] += 1;
            let detailData = {};
            detailData[this.character.statBonusAD] = 1;
            this.addDetails('Bonus AD > 12', detailData);
        }
        if (this.stats['AD'] < 9 && this.character.statBonusAD) {
            this.stats[this.character.statBonusAD] -= 1;
            let detailData = {};
            detailData[this.character.statBonusAD] = -1;
            this.addDetails('Malus AD < 9', detailData);
        }

        for (let i = 0; i < this.character.effects.length; i++) {
            let effect = this.character.effects[i];
            let detailData = {};
            for (let j = 0; j < effect.modifiers.length; j++) {
                var modifier = effect.modifiers[j];
                if (modifier.type == 'ADD') {
                    this.stats[modifier.stat] += modifier.value;
                }
                else if (modifier.type == 'SET') {
                    this.stats[modifier.stat] = modifier.value;
                }
                else if (modifier.type == 'DIV') {
                    this.stats[modifier.stat] /= modifier.value;
                }
                else if (modifier.type == 'MUL') {
                    this.stats[modifier.stat] *= modifier.value;
                }
                else if (modifier.type == 'PERCENTAGE') {
                    this.stats[modifier.stat] *= (modifier.value / 100);
                }
                detailData[modifier.stat] = formatModifierValue(modifier);
            }
            this.addDetails(effect.name, detailData);
        }

        this.stats['MPHYS'] = Math.round((this.stats['INT'] + this.stats['AD']) / 2);
        this.stats['MPSY'] = Math.round((this.stats['INT'] + (this.stats['CHA'] - this.stats['CHA_WITHOUT_MAGIEPSY'])) / 2);
        this.stats['RESM'] += Math.round((this.stats['COU'] + this.stats['INT'] + this.stats['FO']) / 3);
        this.addDetails('Base', {
            MPHYS: "<sup>(" + this.stats['INT'] + " + " + this.stats['AD '] + ")</sup>&frasl;<sub>2</sub>",
            MPSY: "<sup>(" + this.stats['INT'] + " + " + (this.stats['CHA'] - this.stats['CHA_WITHOUT_MAGIEPSY']) + ")</sup>&frasl;<sub>2</sub>",
            RESM: "<sup>(" + this.stats['COU'] + " + " + this.stats['INT'] + " + " + this.stats['FO'] + ")</sup>&frasl;<sub>3</sub>",
        });

        if (this.stats['FO'] > 12) {
            this.stats['PI'] += (this.stats['FO'] - 12);
            this.addDetails('Bonus FO > 12', {'PI': this.stats['FO'] - 12});
        }
        if (this.stats['FO'] < 9) {
            this.stats['PI'] += (this.stats['FO'] - 9);
            this.addDetails('Malus FO < 9', {'PI': this.stats['FO'] - 9});
        }


        if (this.character.job) {
            for (var i in this.character.job.skills) {
                var skill = this.character.job.skills[i];
                this.skills.push({
                    skill: JSON.parse(JSON.stringify(skill)),
                    from: [this.character.job.name]
                });
            }
        }
        for (let i = 0; i < this.character.origin.skills.length; i++) {
            let skill = this.character.origin.skills[i];
            this.skills.push({
                skill: JSON.parse(JSON.stringify(skill)),
                from: [this.character.origin.name]
            });
        }
        for (let i = 0; i < this.character.skills.length; i++) {
            let skill = this.character.skills[i];
            this.skills.push({
                skill: JSON.parse(JSON.stringify(skill)),
                from: ["Choisi"]
            });
        }
        this.skills.sort(function (a, b) {
            return a.skill.name.localeCompare(b.skill.name);
        });

        var prevSkill = null;
        for (let i = 0; i < this.skills.length; i++) {
            let skill = this.skills[i];
            if (skill.skill.id in canceledSkills) {
                skill.canceled = canceledSkills[skill.skill.id];
            }
            if (prevSkill && skill.skill.id == prevSkill.skill.id) {
                prevSkill.from.push(skill.from[0]);
                this.skills.splice(i, 1);
                i--;
            } else {
                prevSkill = skill;
            }
        }

        for (let i = 0; i < this.skills.length; i++) {
            let skill = this.skills[i];
            if (!skill.canceled && skill.skill.effects && skill.skill.effects.length > 0) {
                let detailData = {};
                for (var j = 0; j < skill.skill.effects.length; j++) {
                    let modifier = skill.skill.effects[j];
                    this.stats[modifier.stat] += modifier.value;
                    detailData[modifier.stat] = modifier.value;
                }
                this.addDetails(skill.skill.name, detailData);
            }
        }

        if (this.character.ev == null) {
            this.character.ev = this.stats['EV'];
        }
        if (this.character.ea == null && this.stats['EA'] != null) {
            this.character.ea = this.stats['EA'];
        }

        let statToZero = ['CHA', 'FO', 'COU', 'INT', 'AD', 'AT', 'PRD', 'MPHYS', 'MPSY', 'RESM'];
        for (let i = 0; i < statToZero.length; i++) {
            if (this.stats[statToZero[i]] < 0) {
                this.stats[statToZero[i]] = 0;
            }
        }

        this.countInOtherTab = 0;
        if (this.stats['AD'] > 12) {
            this.countInOtherTab++;
        }
        if (this.stats['AD'] < 9) {
            this.countInOtherTab++;
        }
        if (this.stats['INT'] > 12) {
            this.countInOtherTab++;
        }
        this._characterService.saveStatsCache(this.character.id, this.stats).subscribe(
            () => {
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
    }

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
                this.updateStats();
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
                    if (e.id == res.id) {
                        this.character.effects.splice(i, 1);
                        break;
                    }
                }
                this.selectedEffect = null;
                this.updateStats();
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
                    this.updateStats();
                    this.customAddModifier = new CharacterModifier();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
    }

    removeModifier(modifiers) {
        this._characterService.removeModifier(this.character.id, modifiers.id).subscribe(
            res => {
                for (let i = 0; i < this.character.modifiers.length; i++) {
                    let e = this.character.modifiers[i];
                    if (e.id == res.id) {
                        this.character.modifiers.splice(i, 1);
                        break;
                    }
                }
                this.selectedModifier = null;
                this.updateStats();
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
    }

    selectModifier(modifier) {
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
        if (this.selectedInventoryTab == 'add') {
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
        if (item.quantifiable) {
            this.itemAddQuantity = 1;
        } else {
            delete this.itemAddQuantity;
        }
        return false;
    }

    unselectAddItem() {
        this.selectedAddItem = null;
        return false;
    }

    isAddItemSelected(item) {
        return this.selectedAddItem && this.selectedAddItem.id == item.id;
    }

    addItem() {
        if (this.character) {
            this._itemService.addItem(this.character.id, this.selectedAddItem.id, this.itemAddCustomName, this.itemAddCustomDescription, this.itemAddQuantity).subscribe(
                res => {
                    this.character.items.push(res);
                    this.selectedItem = res;
                    this.updateInventory();
                    this.updateStats();
                    this.itemFilterName = "";
                    delete this.selectedAddItem;
                    delete this.itemAddCustomName;
                    delete this.itemAddCustomDescription;
                    delete this.itemAddQuantity;
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
        var cleanFilter = removeDiacritics(this.itemFilterName).toLowerCase();
        if (removeDiacritics(item.name).toLowerCase().indexOf(cleanFilter) > -1) {
            return true;
        }
        if (removeDiacritics(item.template.name).toLowerCase().indexOf(cleanFilter) > -1) {
            return true;
        }
        return false;
    }

    itemAction(event, item: Item) {
        if (!this.character) {
            return false;
        }
        if (event.action == 'equip' || event.action == 'unequip') {
            let level = 0;
            if (event.action == 'equip') {
                level = 1;
                if (event.level != null) {
                    level = event.level;
                }
            }
            this._itemService.equipItem(item.id, level).subscribe(
                res => {
                    item.equiped = res.equiped;
                    this.updateInventory();
                    this.updateStats();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
        else if (event.action == 'delete') {
            this._itemService.deleteItem(item.id).subscribe(
                deletedItem => {
                    for (let i = 0; i < this.character.items.length; i++) {
                        let it = this.character.items[i];
                        if (it.id == deletedItem.id) {
                            this.character.items.splice(i, 1);
                            break;
                        }
                    }
                    this.selectedItem = null;
                    this.updateInventory();
                    this.updateStats();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
        else if (event.action == 'update_quantity') {
            let quantity = item.quantity;

            if (event.quantity.startsWith('+') || event.quantity.startsWith('-')) {
                quantity = quantity + parseInt(event.quantity);
            } else {
                quantity = parseInt(event.quantity);
            }

            this._itemService.updateQuantity(item.id, quantity).subscribe(
                res => {
                    item.quantity = res.quantity;
                    this.updateInventory();
                    this.updateStats();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
        else if (event.action == 'use_charge') {
            this._itemService.updateCharge(item.id, item.charge - 1).subscribe(
                res => {
                    item.charge = res.charge;
                    this.updateInventory();
                    this.updateStats();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
        else if (event.action == 'move_to_container') {
            this._itemService.moveToContainer(item.id, event.container).subscribe(
                res => {
                    if (!res || !res.id) {
                        item.container = null;
                    } else {
                        item.container = res;
                    }
                    this.updateInventory();
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
        }
        else if (event.action == 'edit_item_name') {
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
                    this.updateInventory();
                    this.updateStats();
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

    public itemsBySlots = {};
    public itemsBySlotsAll = {};
    public itemsEquiped: Item[] = [];
    public itemSlots = [];
    public topLevelContainers = [];

    updateInventory() {
        let itemsBySlots = {};
        let itemsBySlotsAll = {};
        let equiped = [];
        let slots = [];
        let containers = [];
        let topLevelContainers = [];
        let content: {[itemId: number]: Item[]} = {};

        for (let i = 0; i < this.character.items.length; i++) {
            let item = this.character.items[i];
            if (item.equiped != null || item.container != null) {
                if (item.template.container) {
                    if (item.equiped) {
                        topLevelContainers.push(item);
                    }
                    containers.push(item);
                }
            }

            for (var s in item.template.slots) {
                let slot = item.template.slots[s];
                if (!itemsBySlotsAll[slot.id]) {
                    itemsBySlotsAll[slot.id] = [];
                }
                itemsBySlotsAll[slot.id].push(item);

                if (!item.equiped) {
                    continue;
                }

                if (!itemsBySlots[slot.id]) {
                    itemsBySlots[slot.id] = [];
                }
                if (!itemsBySlots[slot.id].length) {
                    slots.push(slot);
                }
                itemsBySlots[slot.id].push(item);
            }
            if (item.equiped != null) {
                equiped.push(item);
            } else {
                if (item.container) {
                    if (!content[item.container.id]) {
                        content[item.container.id] = [];
                    }
                    content[item.container.id].push(item);
                }
            }
        }

        for (let i = 0; i < this.character.items.length; i++) {
            let item = this.character.items[i];
            if (item.id in content) {
                item.content = content[item.id];
            }
        }

        for (let i = 0; i < slots.length; i++) {
            let slot = slots[i];
            itemsBySlots[slot.id].sort(function (a: Item, b: Item) {
                if (a.equiped == b.equiped) {
                    return 0;
                }
                if (a.equiped < b.equiped) {
                    return 1;
                }
                return -1;
            })
        }

        this.itemsEquiped = equiped;
        this.itemSlots = slots;
        this.itemsBySlots = itemsBySlots;
        this.itemsBySlotsAll = itemsBySlotsAll;
        this.containers = containers;
        this.topLevelContainers = topLevelContainers;
    }

    // Group

    cancelInvite(group) {
        this._characterService.cancelInvite(this.character.id, group.id).subscribe(
            res => {
                for (let i = 0; i < this.character.invites.length; i++) {
                    let e = this.character.invites[i];
                    if (e.id == res.group.id) {
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

    acceptInvite(group) {
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

    ngOnInit() {
        this._route.params.subscribe(
            param => {
                let id = this.id;
                if (!this.id) {
                    id = +param['id'];
                }

                this._characterService.getCharacter(id).subscribe(
                    character => {
                        this.character = character;
                        try {
                            this.updateInventory();
                            this.updateStats();
                        } catch (e) {
                            console.log(e.stack);
                            this._notification.error("Erreur", "Erreur JS");
                        }
                    },
                    err => {
                        try {
                            let errJson = err.json();
                            this._notification.error("Erreur", errJson.error_code, {timeOut: -1});
                        } catch (e) {
                            console.log(e);
                            console.log(err);
                            this._notification.error("Erreur", "Erreur");
                        }
                    }
                );
            }
        );
    }
}
