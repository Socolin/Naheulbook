import {Routes} from '@angular/router';
import {CreateCharacterComponent} from '../character/create-character.component';
import {CharacterListComponent} from '../character/character-list.component';
import {CharacterComponent} from '../character/character.component';
import {CharacterResolve} from '../character/character.resolver';
import {GroupComponent} from '../group/group.component';
import {UserProfileComponent} from '../user/user-profile.component';
import {EditItemComponent} from '../item/edit-item.component';
import {EditLocationComponent} from '../location/edit-location.component';
import {EditEffectComponent} from '../effect/edit-effect.component';
import {SkillListComponent} from '../skill/skill-list.component';
import {OriginListComponent} from '../origin/origin-list.component';
import {JobListComponent} from '../job/job-list.component';
import {ItemListComponent} from '../item/item-list.component';
import {EffectListComponent} from '../effect/effect-list.component';
import {MonsterListComponent} from '../monster/monster-list.component';
import {LocationListComponent} from '../location/location-list.component';
import {QuestListComponent} from '../quest/quest-list.component';
import {CreateGroupComponent} from '../group/create-group.component';
import {GroupListComponent} from '../group/group-list.component';
import {CreateEffectComponent} from '../effect/create-effect.component';
import {CreateItemComponent} from '../item/create-item.component';
import {CreateQuestTemplateComponent} from '../quest/create-quest-template.component';

export const routes: Routes = [
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
        resolve: {
            character: CharacterResolve
        }
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
        path: 'database/quests',
        component: QuestListComponent
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
    },
    {
        path: 'create-quest',
        component: CreateQuestTemplateComponent
    }
];
