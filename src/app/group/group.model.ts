import {Monster} from '../monster';
import {Character} from '../character';
import {Location} from '../location';
import {NhbkDate} from '../date';

export class GroupData {
    debilibeuk: number;
    mankdebol: number;
    inCombat: boolean;
    date: NhbkDate;
}

export class Group{
    id: number;
    name: string;
    data: GroupData;
    location: Location;
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
        isMonster: boolean;
        name: string;
        color: string;
        number?: number;
    };
    isMonster: boolean;
    chercheNoise: boolean;
    monster: Monster;
    character: Character;

    static createFromMonster(monster: Monster): Fighter {
        let f = new Fighter();
        f.isMonster = true;
        Object.freeze(f.isMonster);
        f.updateSource(monster);
        return f;
    }

    static createFromCharacter(character: Character): Fighter {
        let f = new Fighter();
        f.isMonster = false;
        Object.freeze(f.isMonster);
        f.updateSource(character);
        return f;
    }

    changeTarget(target: {id: number, isMonster: boolean}) {
        if (this.isMonster) {
            this.monster.target = target;
        } else {
            this.character.target = target;
        }
        this.updateSource();
    }

    changeColor(color: string) {
        if (this.isMonster) {
            this.monster.data.color = color;
        } else {
            this.character.color = color;
        }
        this.updateSource();
    }

    changeNumber(number: number) {
        if (this.isMonster) {
            this.monster.data.number = number;
        } else {
            return;
        }
        this.updateSource();
    }

    updateSource(source?: any) {
        if (source == null) {
            if (this.isMonster) {
                this.updateSource(this.monster);
            } else {
                this.updateSource(this.character);
            }
            return;
        }
        if (this.isMonster) {
            let monster: Monster = source;
            this.name = monster.name;
            this.id = monster.id;
            this.monster = monster;
            this.active = 1;

            this.stats = {
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
            this.chercheNoise = monster.data.chercheNoise;
            this.note = monster.data.note;
            this.color = monster.data.color;
            this.number = monster.data.number;
        } else {
            let character: Character = source;
            this.name = character.name;
            this.id = character.id;
            this.character = character;
            this.active = character.active;
            this.stats = {
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
            this.color = character.color;
            this.number = null;
        }
    }
    updateTarget(fighters: Fighter[]) {
        let targetInfo: any = null;
        if (this.isMonster) {
            if (!this.monster.target) {
                this.target = null;
                return;
            }
            targetInfo = this.monster.target;
        } else {
            if (!this.character.target) {
                this.target = null;
                return;
            }
            targetInfo = this.character.target;
        }

        let fighterIndex = fighters.findIndex(f => f.id === targetInfo.id && f.isMonster === targetInfo.isMonster);
        if (fighterIndex !== -1) {
            let fighter = fighters[fighterIndex];
            this.target = {
                id: fighter.id,
                isMonster: fighter.isMonster,
                name: fighter.name,
                color: fighter.color,
                number: fighter.number
            };
        }
        else {
            this.target = null;
        }
    }
}
