import {provideRouter, RouterConfig} from '@angular/router';

import {CreateCharacterComponent, CharacterComponent, CharacterListComponent} from './character';
import {CreateItemComponent, EditItemComponent, ItemListComponent} from './item';
import {CreateEffectComponent, EffectListComponent, EditEffectComponent} from './effect';
import {OriginListComponent} from './origin';
import {SkillListComponent} from './skill';
import {JobListComponent} from './job';
import {MonsterListComponent} from './monster';
import {CreateGroupComponent, GroupListComponent, GroupComponent} from './group';
import {UserProfileComponent} from './user';
import {LoggedComponent} from './user/logged.component';
import {LocationListComponent, EditLocationComponent} from './location';

export const routes: RouterConfig = [
    {
        path: '',
        component: SkillListComponent
    },
    {
        path: 'home',
        component: SkillListComponent
    },
    {
        path: 'logged',
        component: LoggedComponent,
    },
    {
        path: 'character/create',
        component: CreateCharacterComponent,
    },
    {
        path: 'character/list',
        component: CharacterListComponent,
    },
    {
        path: 'character/detail/:id',
        component: CharacterComponent,
    },
    {
        path: 'character/group/:id',
        component: GroupComponent,
    },
    {
        path: 'profile',
        component: UserProfileComponent,
    },
    {
        path: 'edit-item/:id',
        component: EditItemComponent,
    },
    {
        path: 'edit-location/:id',
        component: EditLocationComponent,
    },
    {
        path: 'edit-effect/:id',
        component: EditEffectComponent,
    },
    {
        path: 'database/skills',
        component: SkillListComponent
    },
    {
        path: 'database/origins',
        component: OriginListComponent
    },
    {
        path: 'database/jobs',
        component: JobListComponent
    },
    {
        path: 'database/items/:id',
        component: ItemListComponent
    },
    {
        path: 'database/items',
        component: ItemListComponent
    },
    {
        path: 'database/effects',
        component: EffectListComponent,
    },
    {
        path: 'database/monsters',
        component: MonsterListComponent
    },
    {
        path: 'database/locations',
        component: LocationListComponent
    },
    {
        path: 'create-group',
        component: CreateGroupComponent
    },
    {
        path: 'group-list',
        component: GroupListComponent
    },
    {
        path: 'create-effect',
        component: CreateEffectComponent
    },
    {
        path: 'create-item',
        component: CreateItemComponent
    }
];

export const APP_ROUTER_PROVIDERS = [
    provideRouter(routes)
];
