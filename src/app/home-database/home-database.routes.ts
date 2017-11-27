import {Routes} from '@angular/router';

import {AuthGuard} from '../user/auth-guard';

import {SkillListComponent} from '../skill/skill-list.component';
import {OriginListComponent} from '../origin/origin-list.component';
import {JobListComponent} from '../job/job-list.component';
import {ItemListComponent} from '../item-template/item-list.component';
import {EffectListComponent} from '../effect/effect-list.component';
import {MonsterListComponent} from '../monster/monster-list.component';
import {LocationListComponent} from '../location/location-list.component';
import {QuestListComponent} from '../quest/quest-list.component';
import {HomeDatabaseComponent} from './home-database.component';
import {EditItemTemplateComponent} from '../item-template/edit-item-template.component';
import {EditEffectComponent} from '../effect/edit-effect.component';
import {CreateEffectComponent} from '../effect/create-effect.component';
import {CreateItemTemplateComponent} from '../item-template/create-item-template.component';
import {DatabaseSectionsComponent} from './database-sections.component';

export const routes: Routes = [
    {
        path: 'database',
        component: HomeDatabaseComponent,
        children: [
            {
                path: '',
                component: DatabaseSectionsComponent
            },
            {
                path: 'skills',
                component: SkillListComponent
            },
            {
                path: 'origins',
                component: OriginListComponent
            },
            {
                path: 'jobs',
                component: JobListComponent
            },
            {
                path: 'items/:id',
                component: ItemListComponent
            },
            {
                path: 'items',
                component: ItemListComponent
            },
            {
                path: 'effects',
                component: EffectListComponent,
            },
            {
                path: 'create-effect',
                component: CreateEffectComponent,
            },
            {
                path: 'edit-effect/:id',
                component: EditEffectComponent,
            },
            {
                path: 'monsters',
                component: MonsterListComponent
            },
            {
                path: 'locations',
                component: LocationListComponent
            },
            {
                path: 'quests',
                component: QuestListComponent
            },
            {
                path: 'edit-item/:id',
                component: EditItemTemplateComponent,
            },
            {
                path: 'create-item',
                canActivate: [AuthGuard],
                data: {
                    authGuardRedirect: '/login/database@create-item'
                },
                component: CreateItemTemplateComponent,
            }
        ]
    }
];
