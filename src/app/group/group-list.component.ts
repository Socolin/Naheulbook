import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {NotificationsService} from '../notifications';

import {CharacterService} from '../character/character.service';
import {Group} from './group.model';

@Component({
    templateUrl: './group-list.component.html',
    styleUrls: ['./group-list.component.scss']
})
export class GroupListComponent implements OnInit {
    public groups: Object[];

    constructor(private _characterService: CharacterService
        , private _notification: NotificationsService
        , private _router: Router) {
    }

    selectGroup(group: Group) {
        this._router.navigate(['/gm/group/', group.id]);
        return false;
    }

    createGroup(group: Group) {
        this._router.navigate(['/gm/group/create']);
        return false;
    }

    loadGroups() {
        this._characterService.listGroups().subscribe(
            res => {
                this.groups = res;
            },
            err => {
                console.log(err);
                this._notification.error('Erreur', 'Erreur serveur');
            }
        );
    }

    ngOnInit() {
        this.loadGroups();
    }
}
