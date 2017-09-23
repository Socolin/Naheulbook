import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {FlexLayoutModule} from '@angular/flex-layout';

import {SharedModule} from '../shared/shared.module';
import {UsefullDataComponent} from './usefull-data.component';
import {UsefullDataService} from './usefull-data.service';
import {DataArrayComponent} from './data-array.component';
import {ItemModule} from '../item/item.module';
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
        ItemModule,
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
