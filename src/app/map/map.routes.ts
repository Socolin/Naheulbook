import {Routes} from '@angular/router';
import {MapComponent} from './map.component';
import {AuthGuard} from '../user/auth-guard';
import {CreateMapComponent} from './create-map.component';

export const routes: Routes = [
    {
        path: 'create',
        component: CreateMapComponent,
        canActivate: [AuthGuard],
        data: {
            authGuardRedirect: '/login/map'
        },

    },
    {
        path: ':mapId',
        component: MapComponent,
    }
];
