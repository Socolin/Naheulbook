import {Routes} from '@angular/router';
import {CharacterComponent, CharacterListComponent, CharacterResolve, CreateCharacterComponent} from '../character';
import {CreateGroupComponent, GroupComponent, GroupListComponent} from '../group';
import {SkillListComponent} from '../skill';
import {OriginListComponent} from '../origin';
import {JobListComponent} from '../job';
import {EffectListComponent} from '../effect';
import {MonsterListComponent} from '../monster';
import {QuestListComponent} from '../quest';
import {AuthGuard} from '../user/auth-guard';
import {HomeGmComponent} from './home-gm.component';
import {ItemTemplateListComponent} from '../item-template/item-template-list.component';

export const routes: Routes = [
    {
        path: '',
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
                        path: 'quests',
                        component: QuestListComponent
                    }
                ]
            }
        ]
    }
];
