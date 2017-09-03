import {Flag, StatModifier} from '../shared';
import {FlagData} from '../shared/flag.model';

export class Speciality {
    id: number;
    name: string;
    description: string;
    specials: {
        id: number,
        isBonus: boolean;
        name: string;
        description: string;
        flags?: Flag[];
    }[];
    modifiers: StatModifier[];

    static fromJson(specialityData: any): Speciality {
        let speciality = new Speciality();

        Object.assign(speciality, specialityData, {skills: [], availableSkills: []});

        if (!speciality.specials) {
            speciality.specials = [];
        }

        return speciality;
    }

    static specialitiesFromJson(specialitiesJsonData: undefined|null|any[]) {
        let specialities: Speciality[] = [];

        if (specialitiesJsonData) {
            for (let specialityJsonData of specialitiesJsonData) {
                specialities.push(Speciality.fromJson(specialityJsonData));
            }
        }

        return specialities;
    }

    hasFlag(flagName: string): boolean {
        for (let special of this.specials) {
            if (!special.flags) {
                continue;
            }
            let i = special.flags.findIndex(f => f.type === flagName);
            if (i !== -1) {
                return true;
            }
        }

        return false;
    }

    getFlagsDatas(data: {[flagName: string]: FlagData[]}): void {
        for (let special of this.specials) {
            if (!special.flags) {
                continue;
            }
            for (let flag of special.flags) {
                if (!(flag.type in data)) {
                    data[flag.type] = [];
                }
                data[flag.type].push({data: flag.data, source: {type: 'speciality', name: this.name}});
            }
        }
    }
}
