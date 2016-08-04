import {Monster} from '../monster';
import {Character} from '../character';
import {Location} from '../location';

export interface Group {
    id: number;
    name: string;
    data: any;
    location: Location;
    monsters: Monster[];
    invited: CharacterInviteInfo[];
    invites: CharacterInviteInfo[];
    characters: {id: number}[];
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
    number: number;
    name: string;
    stats: {
        at: number;
        ad: number;
        prd: number;
        ev: number;
        ea: number;
        maxEv: number;
        maxEa: number;
        pr: string;
        dmg: string;
        cou: number;
        resm: number;
        xp: number;
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
            at: monster.data.at,
            ad: 10, // FIXME: this is used to compute fight order
            prd: monster.data.prd,
            ev: monster.data.ev,
            ea: monster.data.ea,
            maxEv: monster.data.ev,
            maxEa: monster.data.ea,
            pr: monster.data.pr,
            dmg: monster.data.dmg,
            cou: monster.data.cou,
            resm: monster.data.resm,
            xp: monster.data.xp,
        };
        f.chercheNoise = monster.data.chercheNoise;
        f.target = monster.target;
        f.note = monster.data.note;
        f.color = monster.data.color;
        f.number = monster.data.number;
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
            at: character.computedData.stats['AT'],
            ad: character.computedData.stats['AD'],
            prd: character.computedData.stats['PRD'],
            ev: character.ev,
            maxEv: character.computedData.stats['EV'],
            ea: character.ea,
            maxEa:  character.computedData.stats['EA'],
            pr: character.computedData.stats['PR'] + ' (' + character.computedData.stats['PR_MAGIC'] + ')',
            dmg: '',
            cou: character.computedData.stats['COU'],
            resm: character.computedData.stats['RESM'],
            xp: null,
        };
        f.target = character.target;
        f.color = character.color;
        f.number= 0;
        return f;
    }
}
