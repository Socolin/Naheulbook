import {Routes} from '@angular/router';

import {CharacterListComponent, CharacterComponent} from '../character';
import {CharacterResolve} from '../character/character.resolver';
import {HomePlayerComponent} from './home-player.component';
import {CreateCharacterComponent} from '../character/create-character.component';
import {AuthGuard} from '../user/auth-guard';

export const routes: Routes = [
    {
        path: 'player',
        component: HomePlayerComponent,
        canActivate: [AuthGuard],
        data: {
            authGuardRedirect: '/login'
        },
        children: [
            {
                path: '',
                component: CharacterListComponent,
            },
            {
                path: 'character',
                children: [
                    {
                        path: 'create',
                        component: CreateCharacterComponent,
                    },
                    {
                        path: 'list',
                        component: CharacterListComponent,
                    },
                    {
                        path: 'detail/:id',
                        component: CharacterComponent,
                        resolve: {
                            character: CharacterResolve
                        }
                    }
                ]
            }
        ]
    }
];
