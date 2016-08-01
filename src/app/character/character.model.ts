import {Speciality} from './speciality.model';
import {Item} from "./item.model";
import {Origin} from "../origin";
import {Job} from "../job";
import {StatModifier, IMetadata} from "../shared";
import {Effect} from "../effect";
import {Skill} from '../skill/skill.model';

export interface CharacterResume {
    id: number;
    name: string;
    originId: number;
    origin: string;
    jobId: number;
    job: string;
    level: number;
}

export class CharacterModifier {
    name: string;
    values: StatModifier[] = [];
    permanent: boolean;
    duration: string;
    id: number;
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
    experience: number;
    active: number;
    fatePoint: number;
    items: Item[];
    skills: Skill[];
    effects: Effect[];
    stats: {[statName: string]: number};
    modifiers: CharacterModifier[];
    specialities: Speciality[];
    statBonusAD: string;
    user: Object;
    target: {
        id: number;
        isMonster: number;
        name: string;
        color: string;
    };
    statsCache: any;
    color: string;
    gmData: any;
    group: IMetadata;
    invites: IMetadata[];

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
