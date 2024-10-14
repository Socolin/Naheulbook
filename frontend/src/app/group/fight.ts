import {FightResponse} from '../api/responses';
import {Monster} from '../monster';
import {Fighter} from './group.model';
import {SkillDictionary} from '../skill';
import {WebSocketService, WsEventServices, WsRegistrable} from '../websocket';

export class Fight extends WsRegistrable {    id: number;
    name: string;
    monsters: Monster[] = [];
    fighters: Fighter[] = [];

    static fromResponse(response: FightResponse, skillsById: SkillDictionary) {
        const fight = new Fight();
        fight.name = response.name;
        fight.id = response.id;
        fight.monsters = Monster.fromResponses(response.monsters, skillsById);
        fight.fighters = fight.monsters.map(m => new Fighter(m));
        return fight;
    }

    static fightsFromJson(responses: FightResponse[], skillsById: SkillDictionary) {
        return responses.map(response => Fight.fromResponse(response, skillsById));
    }

    addMonster(addedMonster: Monster) {
        let i = this.monsters.findIndex(monster => monster.id === addedMonster.id);
        if (i !== -1) {
            return false;
        }
        this.monsters.push(addedMonster);
        this.fighters.push(new Fighter(addedMonster));
    }

    removeMonster(monsterId: number) {
        let fighterIndex = this.fighters.findIndex(x => x.monster.id === monsterId);
        if (fighterIndex !== -1)
            this.fighters.splice(fighterIndex, 1);
        let monsterIndex = this.monsters.findIndex(x => x.id === monsterId);
        if (monsterIndex !== -1)
            this.monsters.splice(monsterIndex, 1);
    }

    getWsTypeName(): string {
        return 'fight'
    }

    onWsRegister(service: WebSocketService): void {
        for (let monster of this.monsters) {
            service.registerElement(monster);
        }
    }

    onWsUnregister(): void {
        if (!this.wsSubscribtion) {
            return;
        }
        for (let monster of this.monsters) {
            this.wsSubscribtion.service.unregisterElement(monster);
        }
    }

    handleWebsocketEvent(opcode: string, data: any, services: WsEventServices) {
        switch (opcode) {
            case 'addMonster': {
                services.skill.getSkillsById().subscribe(skillsById => {
                    let monster = Monster.fromResponse(data, skillsById);
                    this.addMonster(monster);
                });
                break;
            }
            case 'removeMonster': {
                this.removeMonster(data);
                break;
            }
        }
    }

    dispose() {
    }
}
