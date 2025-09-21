import {Component} from '@angular/core';
import {Router} from '@angular/router';

import {CharacterService} from '../character';
import {GroupService} from './group.service';
import { MatCard, MatCardTitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';

@Component({
    templateUrl: './create-group.component.html',
    providers: [CharacterService],
    imports: [MatCard, MatCardTitle, MatCardContent, MatFormField, MatInput, FormsModule, MatCardActions, MatButton]
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
