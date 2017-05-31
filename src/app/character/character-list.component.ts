import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {CharacterService} from './character.service';
import {CharacterResume} from './character.model';

@Component({
    templateUrl: './character-list.component.html',
    styleUrls: ['./character-list.component.scss'],
    providers: [CharacterService]
})
export class CharacterListComponent implements OnInit {
    public characters: CharacterResume[];

    constructor(private _router: Router
        , private _route: ActivatedRoute
        , private _characterService: CharacterService) {
    }

    selectCharacter(character: CharacterResume) {
        this._router.navigate(['../detail', character.id], {relativeTo: this._route});
        return false;
    }

    createCharacter() {
        this._router.navigate(['../create'], {relativeTo: this._route});
        return false;
    }

    loadCharacterList() {
        this._characterService.loadList().subscribe(
            characters => {
                this.characters = characters;
            }
        );
    }

    ngOnInit() {
        this.loadCharacterList();
    }
}
