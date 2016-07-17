import {Component, OnInit} from '@angular/core';

import {MonsterTemplateComponent} from './monster-template.component';
import {MonsterTemplate, MonsterTemplateCategory} from "./monster.model";
import {MonsterService} from "./monster.service";

@Component({
    moduleId: module.id,
    selector: 'monster-list',
    templateUrl: 'monster-list.component.html',
    directives: [MonsterTemplateComponent]
})
export class MonsterListComponent implements OnInit {
    public monsters: MonsterTemplate[];
    public categories: MonsterTemplateCategory[] = [];
    public monsterByCategory: {[categoryId: number]: MonsterTemplate[]} = {};

    constructor(private _monsterService: MonsterService) {
    }

    getMonsters() {
        this._monsterService.getMonsterList().subscribe(
            res => {
                this.monsters = res;
                for (let i = 0; i < this.monsters.length; i++) {
                    let monster = this.monsters[i];
                    if (!(monster.type.id in this.monsterByCategory)) {
                        this.categories.push(monster.type);
                        this.monsterByCategory[monster.type.id] = [];
                    }
                    this.monsterByCategory[monster.type.id].push(monster);
                }
            }
        );
    }

    ngOnInit() {
        this.getMonsters();
    }
}
