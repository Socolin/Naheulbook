import {Flag, FlagData, StatModifier} from '../shared';
import {SpecialityResponse} from '../api/responses';

export class Speciality {
    id: number;
    name: string;
    description: string;
    specials: {
        id: number,
        isBonus: boolean;
        description: string;
        flags: Flag[];
    }[];
    modifiers: StatModifier[];
    flags: Flag[];

    static fromResponse(response: SpecialityResponse): Speciality {
        const speciality = new Speciality();
        speciality.id = response.id;
        speciality.name = response.name;
        speciality.description = response.description;
        speciality.modifiers = response.modifiers;
        speciality.specials = (response.specials || []).map(s => ({
            ...s,
            flags: s.flags || []
        }));
        speciality.flags = response.flags || [];
        return speciality;
    }

    static fromResponses(responses: SpecialityResponse[]) {
        return responses.map(response => Speciality.fromResponse(response));
    }

    hasFlag(flagName: string): boolean {
        let i = this.flags.findIndex(f => f.type === flagName);
        if (i !== -1) {
            return true;
        }

        for (let special of this.specials) {
            let j = special.flags.findIndex(f => f.type === flagName);
            if (j !== -1) {
                return true;
            }
        }

        return false;
    }

    getFlagsDatas(data: {[flagName: string]: FlagData[]}): void {
        for (let flag of this.flags) {
            if (!(flag.type in data)) {
                data[flag.type] = [];
            }
            data[flag.type].push({data: flag.data, source: {type: 'speciality', name: this.name}});
        }

        for (let special of this.specials) {
            for (let flag of special.flags) {
                if (!(flag.type in data)) {
                    data[flag.type] = [];
                }
                data[flag.type].push({data: flag.data, source: {type: 'speciality', name: this.name}});
            }
        }
    }
}
