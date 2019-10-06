import {Routes} from '@angular/router';

import {LoggedComponent} from './user';
import {CreditComponent} from './home/credit.component';
import {HomeComponent} from './home/home.component';
import {UserProfileComponent} from './user/user-profile.component';
import {LoginComponent} from './user/login.component';
import {LogoutComponent} from './user/logout.component';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent
    },
    {
        path: 'gm',
        loadChildren: () => import('./home-gm/home-gm.module').then(m => m.GmHomeModule)
    },
    {
        path: 'database',
        loadChildren: () => import('./home-database/home-database.module').then(m => m.DatabaseHomeModule)
    },
    {
        path: 'player',
        loadChildren: () => import('./home-player/home-player.module').then(m => m.PlayerHomeModule)
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
