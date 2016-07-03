import {StatModifier} from "../shared";

export interface Speciality {
    id: number;
    name: string;
    description: string;
    specials: {
        id: number,
        isBonus: boolean;
        name: string;
        description: string;
        token: string;
    }[];
    modifiers: StatModifier[];
}