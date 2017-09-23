import {Routes} from '@angular/router';

import {
    CharacterListComponent,
    CharacterComponent,
    CharacterResolve,
    CreateCharacterComponent,
    CreateCustomCharacterComponent,
} from '../character';
import {AuthGuard} from '../user/auth-guard';
import {HomePlayerComponent} from './home-player.component';

export const routes: Routes = [
    {
        path: 'player',
        canActivate: [AuthGuard],
        component: HomePlayerComponent,
        data: {
            authGuardRedirect: '/login/player'
        },
        children: [
            {
                path: '',
                redirectTo: 'character/list',
                pathMatch: 'full'
            },
            {
                path: 'character',
                children: [
                    {
                        path: 'create',
                        component: CreateCharacterComponent,
                    },
                    {
                        path: 'create-custom',
                        component: CreateCustomCharacterComponent,
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
