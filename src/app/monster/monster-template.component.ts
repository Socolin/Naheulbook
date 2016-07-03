import {Component} from '@angular/core';

import {MonsterTemplate} from "./monster.model";

@Component({
    selector: 'monster-template',
    templateUrl: 'app/monster/monster-template.component.html',
    inputs: ['monster']
})
export class MonsterTemplateComponent {
    public monster: MonsterTemplate;
}
