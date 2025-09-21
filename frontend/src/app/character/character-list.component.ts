import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';

import {CharacterService} from './character.service';
import {CharacterSummaryResponse} from '../api/responses';

@Component({
    templateUrl: './character-list.component.html',
    styleUrls: ['./character-list.component.scss'],
    providers: [CharacterService],
    standalone: false
})
export class CharacterListComponent implements OnInit {
    public characters: CharacterSummaryResponse[];
    public loading = true;
    public loadingCharacterId?: number;

    constructor(
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly characterService: CharacterService
    ) {
    }

    selectCharacter(character: CharacterSummaryResponse) {
        if (this.loadingCharacterId) {
            return false;
        }
        this.loadingCharacterId = character.id;
        this.router.navigate(['../detail', character.id], {relativeTo: this.route})
            .catch((e) => {
                this.loadingCharacterId = undefined;
                throw e;
            });
        return false;
    }

    createCharacter() {
        this.router.navigate(['../create'], {relativeTo: this.route});
        return false;
    }

    createCustomCharacter() {
        this.router.navigate(['../create-custom'], {relativeTo: this.route});
        return false;
    }


    loadCharacterList() {
        this.characterService.loadList().subscribe(
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
