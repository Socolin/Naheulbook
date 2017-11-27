import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {FlexLayoutModule} from '@angular/flex-layout';

import {SharedModule} from '../shared/shared.module';
import {UsefullDataComponent} from './usefull-data.component';
import {UsefullDataService} from './usefull-data.service';
import {DataArrayComponent} from './data-array.component';
import {ItemTemplateModule} from '../item-template/item-template.module';
import {JobModule} from '../job/job.module';
import {OriginModule} from '../origin/origin.module';
import {SkillModule} from '../skill/skill.module';
import {EffectModule} from '../effect/effect.module';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        FlexLayoutModule,
        SharedModule,
        ItemTemplateModule,
        JobModule,
        OriginModule,
        SkillModule,
        EffectModule
    ],
    declarations: [
        UsefullDataComponent,
        DataArrayComponent,
    ],
    exports: [
        UsefullDataComponent,
    ],
    providers: [UsefullDataService]
})
export class UsefullDataModule {
}
