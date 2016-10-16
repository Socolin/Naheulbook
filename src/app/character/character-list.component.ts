import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {NotificationsService} from '../notifications';
import {CharacterService} from "./character.service";
import {CharacterResume} from "./character.model";

@Component({
    selector: 'character-list',
    templateUrl: 'character-list.component.html',
    providers: [CharacterService]
})
export class CharacterListComponent implements OnInit {
    public characters: CharacterResume[];

    constructor(private _router: Router
        , private _characterService: CharacterService
        , private _notification: NotificationsService) {
    }

    selectCharacter(character: CharacterResume) {
        this._router.navigate(['/character/detail', character.id]);
        return false;
    }

    loadCharacterList() {
        this._characterService.loadList().subscribe(
            characters => {
                this.characters = characters;
            },
            err => {
                console.log(err);
                this._notification.error("Erreur", "Erreur serveur");
            }
        );
    }

    ngOnInit() {
        this.loadCharacterList();
    }
}
