import {Routes} from '@angular/router';

import {SkillListComponent} from '../skill';
import {OriginListComponent} from '../origin';
import {JobListComponent} from '../job';
import {MonsterListComponent} from '../monster';
import {EffectListComponent} from '../effect';
import {QuestListComponent} from '../quest';
import {HomeDatabaseComponent} from './home-database.component';
import {DatabaseSectionsComponent} from './database-sections.component';
import {AptitudeListComponent} from '../aptitude/aptitude-list.component';
import {AptitudeGroupComponent} from '../aptitude/aptitude-group.component';
import {EditItemTemplateDialogComponent} from '../item-template/edit-item-template-dialog.component';
import {ItemTemplateListComponent} from '../item-template/item-template-list.component';

export const routes: Routes = [
    {
        path: '',
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
                path: 'items/:categoryId/:subCategoryId',
                component: ItemTemplateListComponent
            },
            {
                path: 'items/:categoryId',
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
            },
            {
                path: 'edit-item/:id',
                component: EditItemTemplateDialogComponent,
            },
            {
                path: 'aptitudes/:aptitudeGroupId',
                component: AptitudeGroupComponent
            },
            {
                path: 'aptitudes',
                component: AptitudeListComponent
            }
        ]
    }
];
