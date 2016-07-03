import {Component} from '@angular/core';
import {HTTP_PROVIDERS} from '@angular/http';
import {Router} from '@angular/router';

import {NotificationsService} from '../notifications';
import {CharacterService} from "./character.service";
import {CharacterResume} from "./character.model";

@Component({
    selector: 'character-list',
    templateUrl: 'app/character/character-list.component.html',
    providers: [HTTP_PROVIDERS, CharacterService]
})
export class CharacterListComponent {
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
            res => {
                this.characters = res;
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
