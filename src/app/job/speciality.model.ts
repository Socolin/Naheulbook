import {Flag, StatModifier} from '../shared';

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

    getFlagDatas(flagName: string): any[] {
        let data: any[] = [];

        for (let special of this.specials) {
            if (!special.flags) {
                continue;
            }
            for (let flag of special.flags) {
                if (flag.type === flagName) {
                    if (flag.data) {
                        data.push(flag.data);
                    }
                }
            }
        }

        return data;
    }
}
