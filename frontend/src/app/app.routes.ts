import {Routes} from '@angular/router';

import {CreditComponent, HomeComponent} from './home';
import {LogoutComponent} from './user/logout.component';
import {LoginComponent} from './user/login.component';
import {LoggedComponent} from './user/logged.component';
import {UserProfileComponent} from './user/user-profile.component';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent
    },
    {
        path: 'gm',
        loadChildren: () => import('./home-gm/home-gm.routes').then(m => m.routes)
    },
    {
        path: 'database',
        loadChildren: () => import('./home-database/home-database.routes').then(m => m.routes)
    },
    {
        path: 'player',
        loadChildren: () => import('./home-player/home-player.routes').then(m => m.routes)
    },
    {
        path: 'map',
        loadChildren: () => import('./map/map.routes').then(m => m.routes)
    },
    {
        path: 'login/:redirect',
        component: LoginComponent,
    },
    {
        path: 'logged/:redirect',
        component: LoggedComponent,
    },
    {
        path: 'logged',
        component: LoggedComponent,
    },
    {
        path: 'logout',
        component: LogoutComponent,
    },
    {
        path: 'credit',
        component: CreditComponent,
    },
    {
        path: 'profile',
        component: UserProfileComponent,
    }
];
