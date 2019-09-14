import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';

import {SharedModule} from '../shared/shared.module';
import {UsefulDataComponent} from './useful-data.component';
import {UsefulDataService} from './useful-data.service';
import {DataArrayComponent} from './data-array.component';
import {ItemTemplateModule} from '../item-template/item-template.module';
import {JobModule} from '../job/job.module';
import {OriginModule} from '../origin/origin.module';
import {SkillModule} from '../skill/skill.module';
import {EffectModule} from '../effect/effect.module';
import {EpicFailsCriticalSuccessDialogComponent} from './dialogs/epic-fails-critical-success-dialog.component';
import {UsefulDataDialogsService} from './useful-data-dialogs.service';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        SharedModule,
        ItemTemplateModule,
        JobModule,
        OriginModule,
        SkillModule,
        EffectModule
    ],
    declarations: [
        UsefulDataComponent,
        DataArrayComponent,
        EpicFailsCriticalSuccessDialogComponent,
    ],
    entryComponents: [
        EpicFailsCriticalSuccessDialogComponent,
    ],
    exports: [
        UsefulDataComponent,
    ],
    providers: [UsefulDataService, UsefulDataDialogsService]
})
export class UsefulDataModule {
}
