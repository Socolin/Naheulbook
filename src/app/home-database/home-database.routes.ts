import {Routes} from '@angular/router';
import {SkillListComponent} from '../skill/skill-list.component';
import {OriginListComponent} from '../origin/origin-list.component';
import {JobListComponent} from '../job/job-list.component';
import {ItemListComponent} from '../item/item-list.component';
import {EffectListComponent} from '../effect/effect-list.component';
import {MonsterListComponent} from '../monster/monster-list.component';
import {LocationListComponent} from '../location/location-list.component';
import {QuestListComponent} from '../quest/quest-list.component';
import {HomeDatabaseComponent} from './home-database.component';
import {EditItemComponent} from '../item/edit-item.component';

export const routes: Routes = [
    {
        path: 'database',
        component: HomeDatabaseComponent,
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
                component: EditItemComponent,
            }
        ]
    }
];
