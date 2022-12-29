import {Component} from '@angular/core';
import {Router} from '@angular/router';

import {CharacterService} from '../character';
import {GroupService} from './group.service';

@Component({
    templateUrl: './create-group.component.html',
    providers: [CharacterService]
})
export class CreateGroupComponent {
    public groupName: string;

    constructor(
        private readonly groupService: GroupService,
        private readonly router: Router,
    ) {
    }

    create() {
        this.groupService.createGroup(this.groupName).subscribe(
            group => {
                this.router.navigate(['/gm/group/', group.id]);
            }
        );
    }
}
