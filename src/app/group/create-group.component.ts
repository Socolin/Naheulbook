import {Component} from '@angular/core';
import {Router} from '@angular/router';

import {NotificationsService} from '../notifications';

import {CharacterService} from "../character";

@Component({
    moduleId: module.id,
    templateUrl: 'create-group.component.html',
    providers: [CharacterService]
})
export class CreateGroupComponent {
    public groupName: string;

    constructor(private _characterService: CharacterService
        , private _notification: NotificationsService
        , private router: Router) {
    }

    create() {
        this._characterService.createGroup(this.groupName).subscribe(
            group => {
                this.router.navigate(['/character/group/', group.id]);
            },
            err => {
                console.log(err);
                if (err.status === 500) {
                    this._notification.error("Erreur", "Erreur serveur");
                } else {
                    this._notification.error("Erreur", "Erreur requête");
                }
            }
        );
    }
}
