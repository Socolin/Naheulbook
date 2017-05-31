import {Component} from '@angular/core';
import {Router} from '@angular/router';

import {CharacterService} from '../character/character.service';
import {GroupService} from './group.service';

@Component({
    templateUrl: './create-group.component.html',
    providers: [CharacterService]
})
export class CreateGroupComponent {
    public groupName: string;

    constructor(private _groupService: GroupService
        , private router: Router) {
    }

    create() {
        this._groupService.createGroup(this.groupName).subscribe(
            group => {
                this.router.navigate(['/gm/group/', group.id]);
            }
        );
    }
}
