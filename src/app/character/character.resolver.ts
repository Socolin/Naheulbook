import {Injectable} from '@angular/core';
import {Resolve, ActivatedRouteSnapshot} from '@angular/router';

import {CharacterService} from './character.service';

import {Character} from './character.model';

@Injectable()
export class CharacterResolve implements Resolve<Character> {

    constructor(private _characterService: CharacterService) {
    }

    resolve(route: ActivatedRouteSnapshot) {
        return this._characterService.getCharacter(+route.paramMap.get('id'));
    }
}
