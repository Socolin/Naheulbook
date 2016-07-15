import {Component, Input} from '@angular/core';

import {GroupService} from '../group/group.service';
import {NotificationsService} from '../notifications';

import {Monster} from './monster.model';

@Component({
    moduleId: module.id,
    selector: 'monster-color-selector',
    templateUrl: 'monster-color-selector.component.html'
})
export class MonsterColorSelectorComponent {
    @Input() monster: Monster;

    private showSelector = false;
    private colors: string[] = [
        "ffcc00", "ff6600",
        "ff3300", "990033",
        "ff3399", "cc33ff",
        "0099cc", "0033cc",
        "009999", "00cc66",
        "006600",
        "669900", "996633",
        "999966", "595959",
        "000000",
    ].map(color => '#' + color);

    constructor(private _groupService: GroupService
        , private _notification: NotificationsService) {
    }

    changeColor(color: string) {
        if (color.indexOf('#') === 0) {
            color = color.substring(1);
        }
        this._groupService.updateMonster(this.monster.id, 'color', color)
            .subscribe(
                () => {
                    this.monster.data.color = color;
                    this._notification.info("Monstre", "Couleur changé");
                },
                err => {
                    console.log(err);
                    this._notification.error("Erreur", "Erreur serveur");
                }
            );
    }

}
