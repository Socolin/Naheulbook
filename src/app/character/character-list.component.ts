import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {CharacterSummary} from '../shared';
import {CharacterService} from './character.service';

@Component({
    templateUrl: './character-list.component.html',
    styleUrls: ['./character-list.component.scss'],
    providers: [CharacterService]
})
export class CharacterListComponent implements OnInit {
    public characters: CharacterSummary[];
    public loading = true;
    public loadingCharacterId?: number;

    constructor(private _router: Router
        , private _route: ActivatedRoute
        , private _characterService: CharacterService) {
    }

    selectCharacter(character: CharacterSummary) {
        if (this.loadingCharacterId) {
            return false;
        }
        this.loadingCharacterId = character.id;
        this._router.navigate(['../detail', character.id], {relativeTo: this._route})
            .catch((e) => {
                this.loadingCharacterId = undefined
                throw e;
            });
        return false;
    }

    createCharacter() {
        this._router.navigate(['../create'], {relativeTo: this._route});
        return false;
    }

    createCustomCharacter() {
        this._router.navigate(['../create-custom'], {relativeTo: this._route});
        return false;
    }


    loadCharacterList() {
        this._characterService.loadList().subscribe(
            characters => {
                this.characters = characters;
                this.loading = false;
            }
        );
    }

    ngOnInit() {
        this.loadCharacterList();
    }
}
