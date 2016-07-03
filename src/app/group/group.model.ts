import {Monster} from '../monster';
import {Character} from '../character';

export interface Group {
    id: number;
    name: string;
    monsters: Monster[];
    invited: CharacterInviteInfo[];
    invites: CharacterInviteInfo[];
    characters: Character[];
}

export interface CharacterInviteInfo {
    id: number;
    job: string;
    name: string;
    origin: string;
}

export class Fighter {
    id: number;
    active: number;
    color: string;
    name: string;
    stats: {
        at: number;
        ad: number;
        prd: number;
        ev: number;
        ea: number;
        pr: string;
        dmg: string;
        cou: number;
        resm: number;
        classeXP: number;
    };
    note: string;
    target: {
        id: number;
        isMonster: number;
        name: string;
        color: string;
    };
    isMonster: boolean;
    chercheNoise: boolean;
    monster: Monster;
    character: Character;

    static createFromMonster(monster: Monster): Fighter {
        let f = new Fighter();
        f.name = monster.name;
        f.id = monster.id;
        f.monster = monster;
        f.isMonster = true;
        f.active = 1;
        f.stats = {
            at: monster.at,
            ad: 10, // FIXME: this is used to compute fight order
            prd: monster.prd,
            ev: monster.ev,
            ea: monster.ea,
            pr: monster.pr,
            dmg: monster.dmg,
            cou: monster.cou,
            resm: monster.resm,
            classeXP: monster.classeXP,
        };
        f.chercheNoise = monster.chercheNoise;
        f.target = monster.target;
        f.note = monster.note;
        f.color = monster.color;
        return f;
    }

    static createFromCharacter(character: Character): Fighter {
        let f = new Fighter();
        f.name = character.name;
        f.id = character.id;
        f.character = character;
        f.isMonster = false;
        f.active = character.active;

        f.stats = {
            at: character.statsCache.AT,
            ad: character.statsCache.AD,
            prd: character.statsCache.PRD,
            ev: character.statsCache.EV,
            ea: character.statsCache.EA,
            pr: character.statsCache.PR,
            dmg: '',
            cou: character.statsCache.COU,
            resm: character.statsCache.RESM,
            classeXP: null,
        };
        f.target = character.target;
        f.color = character.color;
        return f;
    }
}
