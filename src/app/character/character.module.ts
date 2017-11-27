import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {FlexLayoutModule} from '@angular/flex-layout';

import {NhbkMaterialModule} from '../nhbk-material.module';
import {SharedModule} from '../shared/shared.module';
import {DateModule} from '../date/date.module';
import {SkillModule} from '../skill/skill.module';
import {OriginModule} from '../origin/origin.module';
import {JobModule} from '../job/job.module';
import {EffectModule} from '../effect/effect.module';
import {ActionModule} from '../action/action.module';
import {ItemTemplateModule} from '../item-template/item-template.module';
import {ItemModule} from '../item/item.module';

import {
    BagItemViewComponent,
    CharacterColorSelectorComponent,
    CharacterComponent,
    CharacterHistoryComponent,
    CharacterListComponent,
    CharacterLootPanelComponent,
    CreateCharacterComponent,
    CreateCustomCharacterComponent,
    EffectPanelComponent,
    InventoryPanelComponent,
    ItemDetailComponent,
    SkillSelectorComponent,
    SpecialitySelectorComponent,
    SwipeableItemDetailComponent,
} from './';

import {CharacterService} from './character.service';
import {CharacterResolve} from './character.resolver';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        FlexLayoutModule,
        SharedModule,
        DateModule,
        SkillModule,
        OriginModule,
        JobModule,
        EffectModule,
        ActionModule,
        ItemTemplateModule,
        ItemModule,
    ],
    declarations: [
        BagItemViewComponent,
        CharacterColorSelectorComponent,
        CharacterComponent,
        CharacterHistoryComponent,
        CharacterListComponent,
        CharacterLootPanelComponent,
        CreateCharacterComponent,
        CreateCustomCharacterComponent,
        EffectPanelComponent,
        InventoryPanelComponent,
        ItemDetailComponent,
        SkillSelectorComponent,
        SpecialitySelectorComponent,
        SwipeableItemDetailComponent,
    ],
    exports: [
        CharacterColorSelectorComponent,
        CharacterComponent,
        CharacterListComponent,
        CreateCharacterComponent,
        CreateCustomCharacterComponent,
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
