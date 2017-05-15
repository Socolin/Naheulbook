import {Routes} from '@angular/router';

import {LoggedComponent} from './user';
import {CreditComponent} from './home/credit.component';
import {HomeComponent} from './home/home.component';
import {UserProfileComponent} from './user/user-profile.component';
import {LoginComponent} from './user/login.component';

export const routes: Routes = [
    {
        path: '',
        component: HomeComponent
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
        path: 'credit',
        component: CreditComponent,
    },
    {
        path: 'profile',
        component: UserProfileComponent,
    }
];
