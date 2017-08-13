import {Injectable} from '@angular/core';
import {Resolve, ActivatedRouteSnapshot} from '@angular/router';
import {Observable} from 'rxjs/Observable';

import {Character} from './character.model';
import {CharacterService} from './character.service';

@Injectable()
export class CharacterResolve implements Resolve<Character> {

    constructor(private _characterService: CharacterService) {
    }

    resolve(route: ActivatedRouteSnapshot): Observable<Character> {
        let characterId = route.paramMap.get('id');
        if (!characterId) {
            throw new Error('resolve: No parameter `id` in route param');
        }
        if (!+characterId) {
            throw new Error('resolve: Invalid `id` not a number. value=`' + characterId + '');
        }
        return this._characterService.getCharacter(+characterId);
    }
}
