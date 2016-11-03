import {Component, Input, OnInit} from '@angular/core';

import {MonsterTemplate, MonsterTrait} from "./monster.model";
import {MonsterService} from "./monster.service";
import {NotificationsService} from "../notifications/notifications.service";

@Component({
    selector: 'monster-template',
    templateUrl: 'monster-template.component.html'
})
export class MonsterTemplateComponent implements OnInit {
    @Input() monster: MonsterTemplate;
    private traisById: {[id: number]: MonsterTrait};
    private editing: boolean;

    constructor(private _monsterService: MonsterService
        , private _notifications: NotificationsService) {
    }

    saveMonster() {
        this.editing = false;
        this._monsterService.editMonster(this.monster).subscribe(res => {
            this._notifications.success("Monster", "Monstre editer");
        })
    }

    editMonster() {
        this.editing = true;
    }

    ngOnInit(): void {
        this._monsterService.getMonsterTraitsById().subscribe(
            traitsById => {
                this.traisById = traitsById
            }
        );
    }
}
