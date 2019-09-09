import {Routes} from '@angular/router';
import {CreateCharacterComponent} from '../character/create-character.component';
import {CharacterListComponent} from '../character/character-list.component';
import {CharacterComponent} from '../character/character.component';
import {CharacterResolve} from '../character/character.resolver';
import {GroupComponent} from '../group/group.component';
import {SkillListComponent} from '../skill/skill-list.component';
import {OriginListComponent} from '../origin/origin-list.component';
import {JobListComponent} from '../job/job-list.component';
import {ItemTemplateListComponent} from '../item-template/item-template-list.component';
import {EffectListComponent} from '../effect/effect-list.component';
import {MonsterListComponent} from '../monster/monster-list.component';
import {LocationListComponent} from '../location/location-list.component';
import {QuestListComponent} from '../quest/quest-list.component';
import {CreateGroupComponent} from '../group/create-group.component';
import {GroupListComponent} from '../group/group-list.component';
import {AuthGuard} from '../user/auth-guard';
import {HomeGmComponent} from './home-gm.component';

export const routes: Routes = [
    {
        path: 'gm',
        component: HomeGmComponent,
        canActivate: [AuthGuard],
        data: {
            authGuardRedirect: '/login/gm'
        },
        children: [
            {
                path: '',
                redirectTo: 'group/list',
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
            },
            {
                path: 'group',
                children: [
                    {
                        path: 'list',
                        component: GroupListComponent,
                    },
                    {
                        path: 'create',
                        component: CreateGroupComponent,
                    },
                    {
                        path: ':id',
                        component: GroupComponent,
                    }
                ]
            },
            {
                path: 'database',
                children: [
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
                        component: ItemTemplateListComponent
                    },
                    {
                        path: 'items',
                        component: ItemTemplateListComponent
                    },
                    {
                        path: 'effects',
                        component: EffectListComponent,
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
                    }
                ]
            }
        ]
    }
];
