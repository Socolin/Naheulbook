import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

import {SharedModule} from '../shared/shared.module';
import {DateModule} from '../date/date.module';
import {SkillModule} from '../skill/skill.module';
import {OriginModule} from '../origin/origin.module';
import {JobModule} from '../job/job.module';
import {EffectModule} from '../effect/effect.module';
import {ActionModule} from '../action/action.module';

import {
    BagItemViewComponent,
    CharacterColorSelectorComponent,
    CharacterComponent,
    CharacterListComponent,
    CharacterLootPanelComponent,
    EffectPanelComponent,
    InventoryPanelComponent,
    ItemDetailComponent,
    SpecialitySelectorComponent,
    SwipeableItemDetailComponent,
    CharacterHistoryComponent,
    CreateCharacterComponent,
} from './';

import {CharacterService} from './character.service';
import {CharacterResolve} from './character.resolver';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        MaterialModule,
        FlexLayoutModule,
        SharedModule,
        DateModule,
        SkillModule,
        OriginModule,
        JobModule,
        EffectModule,
        ActionModule,
    ],
    declarations: [
        BagItemViewComponent,
        CharacterColorSelectorComponent,
        CharacterComponent,
        CharacterHistoryComponent,
        CharacterListComponent,
        CharacterLootPanelComponent,
        CreateCharacterComponent,
        EffectPanelComponent,
        InventoryPanelComponent,
        ItemDetailComponent,
        SpecialitySelectorComponent,
        SwipeableItemDetailComponent,
    ],
    exports: [
        CharacterColorSelectorComponent,
        CharacterComponent,
        CharacterListComponent,
        CreateCharacterComponent,
        EffectModule,
        ItemDetailComponent,
        SkillModule,
    ],
    providers: [
        CharacterService,
        CharacterResolve,
    ],
})
export class CharacterModule {
}
