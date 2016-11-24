import {EventEmitter} from '@angular/core';
import {Speciality} from './speciality.model';
import {Item} from './item.model';
import {Origin} from '../origin';
import {Job} from '../job';
import {
    StatModifier
    , IMetadata
    , ItemStatModifier
    , formatModifierValue
} from '../shared';
import {Effect} from '../effect';
import {Skill} from '../skill';

export interface CharacterResume {
    id: number;
    name: string;
    originId: number;
    origin: string;
    jobId: number;
    job: string;
    level: number;
}

export interface CharacterGiveDestination {
    id: number;
    name: string;
    isNpc: boolean;
}

export class CharacterEffect {
    id: number;
    effect: Effect;
    active: boolean;
    reusable: boolean;
    currentCombatCount: number;
    currentTimeDuration: number;
}

export class CharacterModifier {
    id: number;
    name: string;
    values: StatModifier[] = [];
    permanent: boolean;
    duration: string;
    active: boolean;
    combatCount: number;
    currentCombatCount: number;
    reusable: boolean;
    timeDuration: number;
    currentTimeDuration: number;
}

export interface SkillDetail {
    from: string[];
    skillDef: Skill;
    canceled?: boolean;
}

export class StaticDetailShow {
    evea: boolean = false;
    atprd: boolean = false;
    stat: boolean = false;
    other: boolean = false;
    magic: boolean = false;
}
export class StatisticDetail {
    init() {
        this.evea = [];
        this.atprd = [];
        this.stat = [];
        this.magic = [];
        this.other = [];
    }

    static getDetailCategoryForStat(statName: string) {
        if (statName === 'EV' || statName === 'EA') {
            return 'evea';
        }
        if (statName === 'AT' || statName === 'PRD' || statName === 'PR' || statName === 'PR_MAGIC') {
            return 'atprd';
        }
        if (statName === 'COU'
            || statName === 'FO'
            || statName === 'AD'
            || statName === 'CHA'
            || statName === 'INT') {
            return 'stat';
        }
        if (statName === 'MV'
            || statName === 'THROW_MODIFIER'
            || statName === 'DISCRETION_MODIFIER'
            || statName === 'DANSE_MODIFIER'
            || statName === 'PI') {
            return 'other';
        }
        if (statName === 'RESM'
            || statName === 'MPSY'
            || statName === 'MPHYS') {
            return 'magic';
        }
        return 'unk';
    }

    add(name: string, data: {[statName: string]: any}) {
        let categories: {[categoryName: string]: any} = {};
        for (let i in data) {
            if (!data.hasOwnProperty(i)) {
                continue;
            }
            let category = StatisticDetail.getDetailCategoryForStat(i);
            if (!this[category]) {
                this[category] = [];
            }
            categories[category] = 1;
        }
        for (let i in categories) {
            if (!categories.hasOwnProperty(i)) {
                continue;
            }
            if (i === 'evea') {
                this.evea.push({
                    name: name,
                    data: {
                        EV: data['EV'],
                        EA: data['EA']
                    }
                });
            }
            if (i === 'atprd') {
                this.atprd.push({
                    name: name,
                    data: {
                        AT: data['AT'],
                        PRD: data['PRD'],
                        PR: data['PR'],
                        PR_MAGIC: data['PR_MAGIC']
                    }
                });
            }
            if (i === 'stat') {
                this.stat.push({
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
            if (i === 'other') {
                this.other.push({
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
            if (i === 'magic') {
                this.magic.push({
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

    evea: any[] = [];
    atprd: any[] = [];
    stat: any[] = [];
    magic: any[] = [];
    other: any[] = [];
    show: StaticDetailShow = new StaticDetailShow();
}

export class TacticalMovementInfo {
    distance: number;
    maxDuration: number;
    sprintDistance: number;
    sprintMaxDuration: number;
}

export class CharacterComputedData {
    init() {
        this.details.init();

        this.itemsBySlots = {};
        this.itemsBySlotsAll = {};
        this.itemsEquiped = [];
        this.itemSlots = [];
        this.topLevelContainers = [];
        this.containers = [];
        this.skills = [];
        this.nonReusableEffects = [];
        this.reusableEffects = [];
        this.nonReusableModifiers = [];
        this.reusableModifiers = [];
        this.tacticalMovement = new TacticalMovementInfo();
    }

    stats: {[statName: string]: number} = {};
    skills: SkillDetail[] = [];
    containers: Object[];
    details: StatisticDetail = new StatisticDetail();
    selectedItem: Item;

    itemsBySlots = {};
    itemsBySlotsAll = {};
    itemsEquiped: Item[] = [];
    itemSlots = [];
    topLevelContainers = [];
    tacticalMovement: TacticalMovementInfo = new TacticalMovementInfo();

    nonReusableEffects: CharacterEffect[] = [];
    reusableEffects: CharacterEffect[] = [];
    nonReusableModifiers: CharacterModifier[] = [];
    reusableModifiers: CharacterModifier[] = [];

    countExceptionalStats: number = 0;
    countActiveEffect: number = 0;
}

export class Character {
    id: number;
    name: string;
    ev: number;
    ea: number;
    originId: number;
    origin: Origin;
    jobId: number;
    job: Job;
    level: number;
    sex: string;
    experience: number;
    active: number;
    fatePoint: number;
    items: Item[];
    skills: Skill[];
    effects: CharacterEffect[];
    stats: {[statName: string]: number};
    modifiers: CharacterModifier[];
    specialities: Speciality[];
    statBonusAD: string;
    user: Object;
    target: {
        id: number;
        isMonster: boolean;
    };
    color: string;
    gmData: any;
    group: IMetadata;
    invites: IMetadata[];
    isNpc: boolean;

    computedData: CharacterComputedData = new CharacterComputedData();
    onUpdate: EventEmitter<Character> = new EventEmitter<Character>();

    // Concatenate modifiers like [-2 PRD] and [+2 PRD for dwarf]
    private cleanItemModifiers(item: Item): ItemStatModifier[] {
        let cleanModifiers: ItemStatModifier[] = [];
        if (item.template.modifiers) {
            for (let i = 0; i < item.template.modifiers.length; i++) {
                let modifier = item.template.modifiers[i];
                if (modifier.job && modifier.job !== this.job.id) {
                    continue;
                }
                if (modifier.origin && modifier.origin !== this.origin.id) {
                    continue;
                }
                let newModifier = JSON.parse(JSON.stringify(modifier));
                for (let j = 0; j < cleanModifiers.length; j++) {
                    let newMod = cleanModifiers[j];
                    if (newModifier.stat === newMod.stat
                        && newModifier.type === newMod.type
                        && (!newModifier.special || newModifier.special.length === 0)
                        && (!newMod.special || newMod.special.length === 0)) {
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

    private updateInventory() {
        let itemsBySlots = {};
        let itemsBySlotsAll = {};
        let equiped = [];
        let slots = [];
        let containers: Item[] = [];
        let topLevelContainers = [];
        let content: {[itemId: number]: Item[]} = {};
        let itemsById: {[itemId: number]: Item} = {};

        for (let i = 0; i < this.items.length; i++) {
            let item = this.items[i];
            itemsById[item.id] = item;
            if (item.data.equiped || item.container != null) {
                if (item.template.data.container) {
                    if (item.data.equiped) {
                        topLevelContainers.push(item);
                    }
                    containers.push(item);
                }
            }

            for (let s = 0; s < item.template.slots.length; s++) {
                let slot = item.template.slots[s];
                if (!itemsBySlotsAll[slot.id]) {
                    itemsBySlotsAll[slot.id] = [];
                }
                itemsBySlotsAll[slot.id].push(item);

                if (!item.data.equiped) {
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
            if (item.data.equiped) {
                equiped.push(item);
            } else {
                if (item.container) {
                    if (!content[item.container]) {
                        content[item.container] = [];
                    }
                    content[item.container].push(item);
                }
            }
        }

        for (let i = 0; i < this.items.length; i++) {
            let item = this.items[i];
            if (item.container) {
                item.containerInfo = {
                    name: itemsById[item.container].data.name,
                    id: itemsById[item.container].id
                };
            }
            if (item.id in content) {
                item.content = content[item.id];
            }
        }

        for (let i = 0; i < slots.length; i++) {
            let slot = slots[i];
            itemsBySlots[slot.id].sort(function (a: Item, b: Item) {
                if (a.data.equiped === b.data.equiped) {
                    return 0;
                }
                if (a.data.equiped < b.data.equiped) {
                    return 1;
                }
                return -1;
            });
        }

        for (let i = 0; i < containers.length; i++) {
            let container = containers[i];
            if (container.content) {
                container.content.sort((a, b) => {
                    if (a.template.data.container === b.template.data.container) {
                        return 0;
                    }
                    if (a.template.data.container) {
                        return -1;
                    }
                    if (b.template.data.container) {
                        return 1;
                    }
                    return 0;
                });
            }
        }

        this.computedData.itemsEquiped = equiped;
        this.computedData.itemSlots = slots;
        this.computedData.itemsBySlots = itemsBySlots;
        this.computedData.itemsBySlotsAll = itemsBySlotsAll;
        this.computedData.containers = containers;
        this.computedData.topLevelContainers = topLevelContainers;
    }

    private updateStats() {
        this.computedData.countActiveEffect = 0;
        this.computedData.stats = JSON.parse(JSON.stringify(this.stats));
        this.computedData.details.add('Jet de dé initial', this.stats);
        this.computedData.stats['AT'] = 8;
        this.computedData.stats['PRD'] = 10;
        if (this.job && this.job.baseAT) {
            this.computedData.stats['AT'] = this.job.baseAT;
        }
        if (this.job && this.job.basePRD) {
            this.computedData.stats['PRD'] = this.job.basePRD;
        }
        this.computedData.stats['MV'] = 100;
        this.computedData.stats['PR'] = 0;
        this.computedData.stats['PR_MAGIC'] = 0;
        this.computedData.stats['RESM'] = 0;
        this.computedData.stats['PI'] = 0;
        if (this.origin.speedModifier) {
            this.computedData.stats['MV'] += this.origin.speedModifier;
            if (this.origin.speedModifier > 0) {
                this.computedData.details.add('Origine', {MV: '+' + this.origin.speedModifier + '%'});
            } else {
                this.computedData.details.add('Origine', {MV: this.origin.speedModifier + '%'});
            }
        }
        this.computedData.details.add('Valeurs initial', {AT: this.computedData.stats['AT'], PRD: this.computedData.stats['PRD']});
        this.computedData.stats['EV'] = this.origin.baseEV;
        this.computedData.stats['EA'] = null;
        this.computedData.details.add('Origine', {EV: this.origin.baseEV});

        if (this.origin) {
            this.computedData.stats['AT'] += this.origin.bonusAT;
            this.computedData.stats['PRD'] += this.origin.bonusPRD;
            if (this.origin.bonusAT || this.origin.bonusPRD) {
                this.computedData.details.add('Origine', {AT: this.origin.bonusAT, PRD: this.origin.bonusPRD});
            }
        }
        if (this.job) {
            if (this.job.baseEv) {
                this.computedData.stats['EV'] = this.job.baseEv;
                this.computedData.details.add('Métiers (changement de la valeur de base)', {EV: this.origin.baseEV});
            }
            if (this.job.factorEv) {
                this.computedData.stats['EV'] *= this.job.factorEv;
                this.computedData.stats['EV'] = Math.round(this.computedData.stats['EV']);
                this.computedData.details.add('Métiers (% de vie)', {EV: (Math.round((1 - this.job.factorEv) * 100)) + '%'});
            }
            if (this.job.bonusEv) {
                this.computedData.stats['EV'] += this.job.bonusEv;
                this.computedData.details.add('Métiers (bonus de EV)', {EV: this.job.bonusEv});
            }
            if (this.job.baseEa) {
                this.computedData.details.add('Métiers (EA de base)', {EA: this.job.baseEa});
                this.computedData.stats['EA'] = this.job.baseEa;
            }
        }
        for (let i = 0; i < this.specialities.length; i++) {
            let speciality = this.specialities[i];
            let detailData = {};
            if (speciality.modifiers) {
                for (let j = 0; j < speciality.modifiers.length; j++) {
                    let modifier = speciality.modifiers[j];
                    this.computedData.stats[modifier.stat] += modifier.value;
                    detailData[modifier.stat] = modifier.value;
                }
            }
            this.computedData.details.add('Specialite: ' + speciality.name, detailData);
        }

        for (let i = 0; i < this.modifiers.length; i++) {
            let modifier = this.modifiers[i];
            if (modifier.reusable) {
                this.computedData.reusableModifiers.push(modifier);
            }
            else {
                if (modifier.active) {
                    this.computedData.nonReusableModifiers.push(modifier);
                }
            }

            if (!modifier.active) {
                continue;
            }
            if (!modifier.permanent) {
                this.computedData.countActiveEffect++
            }
            let detailData = {};
            for (let j = 0; j < modifier.values.length; j++) {
                let value = modifier.values[j];
                if (value.type === 'ADD') {
                    this.computedData.stats[value.stat] += value.value;
                }
                else if (value.type === 'SET') {
                    this.computedData.stats[value.stat] = value.value;
                }
                else if (value.type === 'DIV') {
                    this.computedData.stats[value.stat] /= value.value;
                }
                else if (value.type === 'MUL') {
                    this.computedData.stats[value.stat] *= value.value;
                }
                else if (value.type === 'PERCENTAGE') {
                    this.computedData.stats[value.stat] *= (value.value / 100);
                }
                detailData[value.stat] = formatModifierValue(value);
            }
            this.computedData.details.add(modifier.name, detailData);
        }

        let canceledSkills = {};
        this.computedData.skills = [];

        this.computedData.stats['THROW_MODIFIER'] = 0;
        this.computedData.stats['DISCRETION_MODIFIER'] = 0;
        this.computedData.stats['DANSE_MODIFIER'] = 0;
        this.computedData.stats['CHA_WITHOUT_MAGIEPSY'] = 0;
        for (let i = 0; i < this.items.length; i++) {
            let item = this.items[i];

            if (!item.data.equiped && item.data.readCount) {
                if (item.data.readCount >= 7) {
                    for (let u = 0; u < item.template.unskills.length; u++) {
                        let skill = item.template.unskills[u];
                        canceledSkills[skill.id] = item;
                    }
                    for (let u = 0; u < item.template.skills.length; u++) {
                        this.computedData.skills.push({
                            skillDef: item.template.skills[u],
                            from: [item.data.name]
                        });
                    }
                }
            }
        }
        for (let i = 0; i < this.computedData.itemsEquiped.length; i++) {
            let item = this.computedData.itemsEquiped[i];
            if (item.template.data.charge) {
                continue;
            }
            if (item.template.data.requireLevel > this.level) {
                // FIXME: Check if we should ignore this item or do something else.
                continue;
            }
            let modifications = {};
            for (let u = 0; u < item.template.unskills.length; u++) {
                let skill = item.template.unskills[u];
                canceledSkills[skill.id] = item;
            }
            for (let u = 0; u < item.template.skills.length; u++) {
                this.computedData.skills.push({
                    skillDef: item.template.skills[u],
                    from: [item.data.name]
                });
            }
            let somethingOver = false;
            for (let s = 0; s < item.template.slots.length; s++) {
                let slot = item.template.slots[s];
                for (let i2 = 0; i2 < this.computedData.itemsBySlots[slot.id].length; i2++) {
                    let item2 = this.computedData.itemsBySlots[slot.id][i2];
                    if (item2.id === item.id) {
                        continue;
                    }
                    if (item.data.equiped < item2.equiped) {
                        somethingOver = true;
                        break;
                    }
                }
            }

            if (item.modifiers) {
                for (let i = 0; i < item.modifiers.length; i++) {
                    let modifier = item.modifiers[i];
                    if (!modifier.active) {
                        continue;
                    }
                    this.computedData.countActiveEffect++;
                    let detailData = {};
                    for (let j = 0; j < modifier.values.length; j++) {
                        let mod = modifier.values[j];
                        if (mod.type === 'ADD') {
                            this.computedData.stats[mod.stat] += mod.value;
                        }
                        else if (mod.type === 'SET') {
                            this.computedData.stats[mod.stat] = mod.value;
                        }
                        else if (mod.type === 'DIV') {
                            this.computedData.stats[mod.stat] /= mod.value;
                        }
                        else if (mod.type === 'MUL') {
                            this.computedData.stats[mod.stat] *= mod.value;
                        }
                        else if (mod.type === 'PERCENTAGE') {
                            this.computedData.stats[mod.stat] *= (mod.value / 100);
                        }
                        detailData[mod.stat] = formatModifierValue(mod);
                    }
                    this.computedData.details.add(item.data.name + '/' + modifier.name, detailData);
                }
            }

            let cleanModifiers = this.cleanItemModifiers(item);
            for (let m = 0; m < cleanModifiers.length; m++) {
                let modifier = cleanModifiers[m];
                if (modifier.job && modifier.job !== this.job.id) {
                    continue;
                }
                if (modifier.origin && modifier.origin !== this.origin.id) {
                    continue;
                }
                let affectStats = true;
                let overrideStatName = modifier.stat;
                if (modifier.special) {
                    if (modifier.special.indexOf('ONLY_IF_NOTHING_ON') >= 0) {
                        if (somethingOver) {
                            continue;
                        }
                    }
                    if (modifier.special.indexOf('AFFECT_ONLY_THROW') >= 0) {
                        overrideStatName = 'THROW_MODIFIER';
                    }
                    if (modifier.special.indexOf('DONT_AFFECT_MAGIEPSY') >= 0) {
                        this.computedData.stats[overrideStatName] += modifier.value;
                        this.computedData.stats['CHA_WITHOUT_MAGIEPSY'] += modifier.value;
                        modifications[overrideStatName] = modifier.value + '(!MPsy)';
                        affectStats = false;
                    }
                    if (modifier.special.indexOf('AFFECT_ONLY_MELEE') >= 0) {
                        // FIXME
                    }
                    if (modifier.special.indexOf('AFFECT_ONLY_MELEE_STAFF') >= 0) {
                        // FIXME
                    }
                    if (modifier.special.indexOf('AFFECT_PR_FOR_ELEMENTS') >= 0) {
                        // FIXME
                    }
                    if (modifier.special.indexOf('AFFECT_DISCRETION') >= 0) {
                        overrideStatName = 'DISCRETION_MODIFIER';
                    }
                    if (modifier.special.indexOf('AFFECT_ONLY_DANSE') >= 0) {
                        overrideStatName = 'DANSE_MODIFIER';
                    }
                }
                if (affectStats) {
                    if (modifier.type === 'ADD') {
                        this.computedData.stats[overrideStatName] += modifier.value;
                    }
                    else if (modifier.type === 'SET') {
                        this.computedData.stats[overrideStatName] = modifier.value;
                    }
                    else if (modifier.type === 'DIV') {
                        this.computedData.stats[overrideStatName] /= modifier.value;
                    }
                    else if (modifier.type === 'MUL') {
                        this.computedData.stats[overrideStatName] *= modifier.value;
                    }
                    else if (modifier.type === 'PERCENTAGE') {
                        this.computedData.stats[overrideStatName] *= (modifier.value / 100);
                    }
                    if (modifications[overrideStatName] == null) {
                        modifications[overrideStatName] = 0;
                    }
                    modifications[overrideStatName] = formatModifierValue(modifier);
                }
            }
            if (item.template.data.protection) {
                modifications['PR'] = item.template.data.protection;
                this.computedData.stats['PR'] += item.template.data.protection;
            }
            if (item.template.data.magicProtection) {
                modifications['PR_MAGIC'] = item.template.data.magicProtection;
                this.computedData.stats['PR_MAGIC'] += item.template.data.magicProtection;
            }
            this.computedData.details.add(item.data.name, modifications);
        }

        if (this.computedData.stats['AD'] > 12 && this.statBonusAD) {
            this.computedData.stats[this.statBonusAD] += 1;
            let detailData = {};
            detailData[this.statBonusAD] = 1;
            this.computedData.details.add('Bonus AD > 12', detailData);
        }
        if (this.computedData.stats['AD'] < 9 && this.statBonusAD) {
            this.computedData.stats[this.statBonusAD] -= 1;
            let detailData = {};
            detailData[this.statBonusAD] = -1;
            this.computedData.details.add('Malus AD < 9', detailData);
        }

        for (let i = 0; i < this.effects.length; i++) {
            let characterEffect = this.effects[i];
            let effect = characterEffect.effect;
            if (characterEffect.reusable) {
                this.computedData.reusableEffects.push(characterEffect);
            }
            else {
                this.computedData.nonReusableEffects.push(characterEffect);
            }
            if (!characterEffect.active) {
                continue;
            }
            this.computedData.countActiveEffect++;
            let detailData = {};
            for (let j = 0; j < effect.modifiers.length; j++) {
                let modifier = effect.modifiers[j];
                if (modifier.type === 'ADD') {
                    this.computedData.stats[modifier.stat] += modifier.value;
                }
                else if (modifier.type === 'SET') {
                    this.computedData.stats[modifier.stat] = modifier.value;
                }
                else if (modifier.type === 'DIV') {
                    this.computedData.stats[modifier.stat] /= modifier.value;
                }
                else if (modifier.type === 'MUL') {
                    this.computedData.stats[modifier.stat] *= modifier.value;
                }
                else if (modifier.type === 'PERCENTAGE') {
                    this.computedData.stats[modifier.stat] *= (modifier.value / 100);
                }
                detailData[modifier.stat] = formatModifierValue(modifier);
            }
            this.computedData.details.add(effect.name, detailData);
        }

        this.computedData.stats['MPHYS'] =
            Math.round(
                (this.computedData.stats['INT'] + this.computedData.stats['AD'])
                / 2
            );
        this.computedData.stats['MPSY'] =
            Math.round((
                    this.computedData.stats['INT'] +
                    (this.computedData.stats['CHA'] - this.computedData.stats['CHA_WITHOUT_MAGIEPSY'])
                )
                / 2
            );
        this.computedData.stats['RESM'] +=
            Math.round((this.computedData.stats['COU']
                + this.computedData.stats['INT']
                + this.computedData.stats['FO'])
                / 3
            );

        this.computedData.details.add('Base', {
            MPHYS: '<sup>(' + this.computedData.stats['INT']
            + ' + ' + this.computedData.stats['AD'] + ')</sup>&frasl;<sub>2</sub>',
            MPSY: '<sup>(' + this.computedData.stats['INT']
            + ' + ' + (this.computedData.stats['CHA']
            - this.computedData.stats['CHA_WITHOUT_MAGIEPSY'])
            + ')</sup>&frasl;<sub>2</sub>',
            RESM: '<sup>(' + this.computedData.stats['COU']
            + ' + ' + this.computedData.stats['INT'] + ' ' +
            '+ ' + this.computedData.stats['FO'] + ')</sup>&frasl;<sub>3</sub>',
        });

        if (this.computedData.stats['FO'] > 12) {
            this.computedData.stats['PI'] += (this.computedData.stats['FO'] - 12);
            this.computedData.details.add('Bonus FO > 12', {'PI': this.computedData.stats['FO'] - 12});
        }
        if (this.computedData.stats['FO'] < 9) {
            this.computedData.stats['PI'] -= 1;
            this.computedData.details.add('Malus FO < 9', {'PI': -1});
        }


        if (this.job) {
            for (let i = 0; i < this.job.skills.length; i++) {
                let skill = this.job.skills[i];
                this.computedData.skills.push({
                    skillDef: skill,
                    from: [this.job.name]
                });
            }
        }
        for (let i = 0; i < this.origin.skills.length; i++) {
            let skill = this.origin.skills[i];
            this.computedData.skills.push({
                skillDef: skill,
                from: [this.origin.name]
            });
        }
        for (let i = 0; i < this.skills.length; i++) {
            let skill = this.skills[i];
            this.computedData.skills.push({
                skillDef: skill,
                from: ['Choisi']
            });
        }
        this.computedData.skills.sort(function (a, b) {
            return a.skillDef.name.localeCompare(b.skillDef.name);
        });

        let prevSkill: SkillDetail = null;
        for (let i = 0; i < this.computedData.skills.length; i++) {
            let skill = this.computedData.skills[i];
            if (skill.skillDef.id in canceledSkills) {
                skill.canceled = canceledSkills[skill.skillDef.id];
            }
            if (prevSkill && skill.skillDef.id === prevSkill.skillDef.id) {
                prevSkill.from.push(skill.from[0]);
                this.computedData.skills.splice(i, 1);
                i--;
            } else {
                prevSkill = skill;
            }
        }

        for (let i = 0; i < this.computedData.skills.length; i++) {
            let skill = this.computedData.skills[i];
            if (!skill.canceled && skill.skillDef.effects && skill.skillDef.effects.length > 0) {
                let detailData = {};
                for (let j = 0; j < skill.skillDef.effects.length; j++) {
                    let modifier = skill.skillDef.effects[j];
                    this.computedData.stats[modifier.stat] += modifier.value;
                    detailData[modifier.stat] = modifier.value;
                }
                this.computedData.details.add(skill.skillDef.name, detailData);
            }
        }

        if (this.ev == null) {
            this.ev = this.computedData.stats['EV'];
        }
        if (this.ea == null && this.computedData.stats['EA'] != null) {
            this.ea = this.computedData.stats['EA'];
        }

        let statToZero = ['CHA', 'FO', 'COU', 'INT', 'AD', 'AT', 'PRD', 'MPHYS', 'MPSY', 'RESM'];
        for (let i = 0; i < statToZero.length; i++) {
            if (this.computedData.stats[statToZero[i]] < 0) {
                this.computedData.stats[statToZero[i]] = 0;
            }
        }

        this.computedData.countExceptionalStats = 0;
        if (this.computedData.stats['AD'] > 12) {
            this.computedData.countExceptionalStats++;
        }
        if (this.computedData.stats['AD'] < 9) {
            this.computedData.countExceptionalStats++;
        }
        if (this.computedData.stats['INT'] > 12) {
            this.computedData.countExceptionalStats++;
        }

        this.computeTacticalMovement();
    }

    private computeTacticalMovement() {
        let distance: number;
        let sprintDistance: number;
        let maxDuration: number;
        let sprintMaxDuration: number;
        let force: number = this.computedData.stats['FO'];
        switch (this.computedData.stats['PR']) {
            case 0:
            case 1:
                distance = 8;
                sprintDistance = 12;
                maxDuration = force * 20;
                sprintMaxDuration = force * 5;
                break;
            case 2:
                distance = 6;
                sprintDistance = 10;
                maxDuration = force * 18;
                sprintMaxDuration = force * 5;
                break;
            case 3:
            case 4:
                distance = 4;
                sprintDistance = 8;
                maxDuration = force * 15;
                sprintMaxDuration = force * 4;
                break;
            case 5:
                distance = 4;
                sprintDistance = 6;
                maxDuration = force * 10;
                sprintMaxDuration = force * 4;
                break;
            case 6:
                distance = 3;
                sprintDistance = 4;
                maxDuration = force * 8;
                sprintMaxDuration = force * 3;
                break;
            case 7:
                distance = 2;
                sprintDistance = 3;
                maxDuration = force * 7;
                sprintMaxDuration = force * 2;
                break;
            default:
                distance = 1;
                sprintDistance = 2;
                maxDuration = force * 2;
                sprintMaxDuration = force * 2;
                break;
        }
        let speedModifier = this.computedData.stats['MV'] / 100;
        this.computedData.tacticalMovement.distance = distance * speedModifier;
        this.computedData.tacticalMovement.sprintDistance = sprintDistance * speedModifier;
        this.computedData.tacticalMovement.maxDuration = maxDuration * speedModifier;
        this.computedData.tacticalMovement.sprintMaxDuration = sprintMaxDuration * speedModifier;
    }

    public update() {
        this.computedData.init();
        this.updateInventory();
        this.updateStats();
        this.onUpdate.emit(this);
    }

    static hasChercherDesNoises(character: Character): boolean {
        return this.hasSkill(character, 14);
    }

    static hasSkill(character: Character, skillId: number): boolean {
        for (let i = 0; i < character.origin.skills.length; i++) {
            let skill = character.origin.skills[i];
            if (skill.id === skillId) {
                return true;
            }
        }
        if (character.job) {
            for (let i = 0; i < character.job.skills.length; i++) {
                let skill = character.job.skills[i];
                if (skill.id === skillId) {
                    return true;
                }
            }
        }
        for (let i = 0; i < character.skills.length; i++) {
            let skill = character.skills[i];
            if (skill.id === skillId) {
                return true;
            }
        }
        return false;
    }
}
