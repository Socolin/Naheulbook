import {Injectable} from '@angular/core';
import {ActivatedRouteSnapshot, Resolve} from '@angular/router';
import {Observable} from 'rxjs';

import {Character} from './character.model';
import {CharacterService} from './character.service';

@Injectable()
export class CharacterResolve implements Resolve<Character> {

    constructor(
        private readonly characterService: CharacterService
    ) {
    }

    resolve(route: ActivatedRouteSnapshot): Observable<Character> {
        let characterId = route.paramMap.get('id');
        if (!characterId) {
            throw new Error('resolve: No parameter `id` in route param');
        }
        if (!+characterId) {
            throw new Error('resolve: Invalid `id` not a number. value=`' + characterId + '');
        }
        return this.characterService.getCharacter(+characterId);
    }
}
