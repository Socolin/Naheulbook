import {Routes} from '@angular/router';
import {MapComponent} from './map.component';
import {AuthGuard} from '../user/auth-guard';
import {CreateMapComponent} from './create-map.component';
import {MapHomeComponent} from './map-home.component';
import {MapListComponent} from './map-list.component';
import {EditMapComponent} from './edit-map.component';

export const routes: Routes = [
    {
        path: '',
        component: MapHomeComponent,
        children: [
            {
                path: '',
                component: MapListComponent,
            },
            {
                path: 'create',
                component: CreateMapComponent,
                canActivate: [AuthGuard],
                data: {
                    authGuardRedirect: '/login/map'
                },
            },
            {
                path: 'edit/:mapId',
                component: EditMapComponent,
                canActivate: [AuthGuard],
                data: {
                    authGuardRedirect: '/login/map'
                },
            }
        ]
    },
    {
        path: ':mapId',
        children: [
            {
                path: '',
                component: MapComponent,
            },
            {
                path: ':z/:x/:y',
                component: MapComponent,
            }
        ]
    }
];
