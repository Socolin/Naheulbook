import {Routes} from '@angular/router';

import {SkillListComponent} from '../skill';
import {OriginListComponent} from '../origin';
import {JobListComponent} from '../job';
import {EditItemTemplateDialogComponent, ItemTemplateListComponent} from '../item-template';
import {MonsterListComponent} from '../monster';
import {CreateEffectComponent, EditEffectComponent, EffectListComponent} from '../effect';
import {LocationListComponent} from '../location';
import {QuestListComponent} from '../quest';
import {HomeDatabaseComponent} from './home-database.component';
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
                component: EditItemTemplateDialogComponent,
            }
        ]
    }
];
