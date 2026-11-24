import {Routes} from '@angular/router';
import {AuthGuard} from '../user/auth-guard';
import {AdminComponent} from './admin.component';
import {AdminUsersComponent} from './admin-users.component';

export const routes: Routes = [
    {
        path: '',
        component: AdminComponent,
        canActivate: [AuthGuard],
        data: {
            authGuardRedirect: '/login/admin'
        },
        children: [
            {
                path: '',
                redirectTo: 'users',
                pathMatch: 'full'
            },
            {
                path: 'users',
                component: AdminUsersComponent
            },
        ]
    },
];
