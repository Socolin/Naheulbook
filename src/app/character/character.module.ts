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
import {ItemModule} from '../item/item.module';

import {
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
    SkillSelectorComponent,
    SpecialitySelectorComponent,
    SwipeableItemDetailComponent,
} from './';

import {CharacterService} from './character.service';
import {CharacterResolve} from './character.resolver';
import {ItemService} from './item.service';

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
        EffectModule,
        ItemDetailComponent,
        SkillModule,
    ],
    providers: [
        CharacterService,
        CharacterResolve,
        ItemService,
    ],
})
export class CharacterModule {
}
