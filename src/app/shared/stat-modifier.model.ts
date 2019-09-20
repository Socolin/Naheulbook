import {Effect} from '../effect';
import {Fighter} from '../group';
import {IActiveStatsModifier, IDurable} from '../api/shared';
import {DurationType} from '../api/shared/enums';

export type StatModificationOperand =
    'ADD'
    | 'MUL'
    | 'DIV'
    | 'SET'
    | 'PERCENTAGE';

export class StatModifier {
    stat: string;
    type: StatModificationOperand = 'ADD';
    value: number;
    special?: string[];

    static apply(value: number, mod: StatModifier) {
        if (mod.type === 'ADD') {
            return value + mod.value;
        } else if (mod.type === 'SET') {
            return mod.value;
        } else if (mod.type === 'DIV') {
            return value / mod.value;
        } else if (mod.type === 'MUL') {
            return value * mod.value;
        } else if (mod.type === 'PERCENTAGE') {
            return value * (mod.value / 100);
        }
        throw new Error('Invalid stat modifier')
    }

    static applyInPlace(stats: { [statName: string]: number }, mod: StatModifier) {
        stats[mod.stat] = StatModifier.apply(stats[mod.stat], mod);
    }
}

export class ItemStatModifier implements StatModifier {
    stat: string;
    type: StatModificationOperand;
    value: number;
    special: string[];

    job: number;
    origin: number;
}

export class StatsModifier implements IDurable {
    name: string;

    reusable = false;

    durationType: DurationType = 'combat';
    duration: string;
    combatCount: number;
    lapCount: number;
    timeDuration: number;

    description?: string;
    type?: string;

    values: StatModifier[] = [];
}

export class LapCountDecrement {
    when: 'BEFORE' | 'AFTER';
    fighterId: number;
    fighterIsMonster: boolean;
}

export class ActiveStatsModifier extends StatsModifier {
    id: number;
    permanent: boolean;
    active: boolean;

    currentCombatCount: number;
    currentLapCount: number;
    currentTimeDuration: number;

    lapCountDecrement: LapCountDecrement;

    static fromJson(jsonData: IActiveStatsModifier) {
        let modifier = new ActiveStatsModifier();
        Object.assign(modifier, jsonData);
        return modifier;
    }

    static modifiersFromJson(modifiersJsonData: undefined | null | any[]) {
        let modifiers: ActiveStatsModifier[] = [];

        if (modifiersJsonData) {
            for (let modifierJsonData of modifiersJsonData) {
                modifiers.push(ActiveStatsModifier.fromJson(modifierJsonData));
            }
        }

        return modifiers;
    }

    static fromEffect(effect: Effect, data: any): ActiveStatsModifier {
        let modifier = new ActiveStatsModifier();
        modifier.name = effect.name;
        modifier.description = effect.description;
        modifier.permanent = false;
        modifier.reusable = data.reusable;
        modifier.type = effect.category.name;
        if ('durationType' in data) {
            modifier.durationType = data.durationType;
            switch (data.durationType) {
                case 'combat':
                    modifier.combatCount = data.combatCount;
                    modifier.currentCombatCount = data.combatCount;
                    break;
                case 'time':
                    modifier.timeDuration = data.timeDuration;
                    modifier.currentTimeDuration = data.timeDuration;
                    break;
                case 'lap':
                    modifier.lapCount = data.lapCount;
                    modifier.currentLapCount = data.lapCount;
                    break;
                case 'custom':
                    modifier.duration = data.duration;
                    break;
                case 'forever':
                    break;
            }
        } else {
            modifier.durationType = effect.durationType;
            modifier.combatCount = effect.combatCount;
            modifier.currentCombatCount = effect.combatCount;
            modifier.lapCount = effect.lapCount;
            modifier.currentLapCount = effect.lapCount;
            modifier.timeDuration = effect.timeDuration;
            modifier.currentTimeDuration = effect.timeDuration;
            modifier.duration = effect.duration;
        }
        if (effect.modifiers) {
            modifier.values = JSON.parse(JSON.stringify(effect.modifiers));
        }
        return modifier;
    }

    public updateDuration(durationType: string, data: number | { previous: Fighter, next: Fighter }): boolean {
        if (!this.active) {
            return false;
        }
        if (durationType === 'combat' && this.durationType === 'lap') {
            this.currentLapCount = 0;
            this.active = false;
            return true;
        }

        if (durationType !== this.durationType) {
            return false;
        }

        switch (this.durationType) {
            case 'combat': {
                if (this.currentCombatCount > 0 && typeof (data) === 'number') {
                    this.currentCombatCount -= data;
                    if (this.currentCombatCount <= 0) {
                        this.currentCombatCount = 0;
                        this.active = false;
                    }
                    return true;
                }
                break;
            }
            case 'time': {
                if (this.currentTimeDuration > 0 && typeof (data) === 'number') {
                    this.currentTimeDuration -= data;
                    if (this.currentTimeDuration <= 0) {
                        this.currentTimeDuration = 0;
                        this.active = false;
                    }
                    return true;
                }
                break;
            }
            case 'lap': {
                if (this.currentLapCount > 0) {
                    let testFighter: Fighter;
                    if (!this.lapCountDecrement) {
                        return false;
                    }

                    let lapDecrement: { previous: Fighter, next: Fighter };
                    if (typeof (data) !== 'number') {
                        lapDecrement = data;
                    } else {
                        return false;
                    }

                    if (this.lapCountDecrement.when === 'AFTER') {
                        testFighter = lapDecrement.previous;
                    } else if (this.lapCountDecrement.when === 'BEFORE') {
                        testFighter = lapDecrement.next;
                    } else {
                        return false;
                    }

                    if (testFighter.id === this.lapCountDecrement.fighterId
                        && testFighter.isMonster === this.lapCountDecrement.fighterIsMonster) {
                        this.currentLapCount--;
                        if (this.currentLapCount <= 0) {
                            this.currentLapCount = 0;
                            this.active = false;
                        }
                        return true;
                    }
                }
                break;
            }
        }

        return false;
    }

    updateLapDecrement(data: { deleted: Fighter; previous: Fighter; next: Fighter }): boolean {
        if (this.durationType !== 'lap') {
            return false;
        }
        if (!this.lapCountDecrement) {
            return false;
        }

        if (this.lapCountDecrement.fighterId === data.deleted.id
            && this.lapCountDecrement.fighterIsMonster === data.deleted.isMonster) {
            if (this.lapCountDecrement.when === 'BEFORE') {
                this.lapCountDecrement.when = 'AFTER';
                this.lapCountDecrement.fighterId = data.previous.id;
                this.lapCountDecrement.fighterIsMonster = data.previous.isMonster;
            } else {
                this.lapCountDecrement.when = 'BEFORE';
                this.lapCountDecrement.fighterId = data.next.id;
                this.lapCountDecrement.fighterIsMonster = data.next.isMonster;
            }
            return true;
        }
        return false;
    }
}
