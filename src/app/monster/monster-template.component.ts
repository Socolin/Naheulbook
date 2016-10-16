import {Component, Input} from '@angular/core';

import {MonsterTemplate} from "./monster.model";

@Component({
    selector: 'monster-template',
    templateUrl: 'monster-template.component.html'
})
export class MonsterTemplateComponent {
    @Input() monster: MonsterTemplate;
}
