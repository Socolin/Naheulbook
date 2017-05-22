import {NgModule}      from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {MaterialModule} from '@angular/material';
import {FlexLayoutModule} from '@angular/flex-layout';

import {SharedModule} from '../shared/shared.module';

import {
    SpecialitySelectorComponent, ItemDetailComponent, CharacterComponent,
    CharacterColorSelectorComponent, CharacterListComponent
} from './';
import {
    CharacterService
} from './character.service';
import {SkillModule} from '../skill/skill.module';
import {OriginModule} from '../origin/origin.module';
import {JobModule} from '../job/job.module';
import {EffectModule} from '../effect/effect.module';
import {
    EffectPanelComponent, ModifierDetailComponent, EffectDetailComponent
} from './effect-panel.component';
import {InventoryPanelComponent} from './inventory-panel.component';
import {SwipeableItemDetailComponent} from './swipeable-item-detail.component';
import {BagItemViewComponent} from './bag-item-view.component';
import {DateModule} from '../date/date.module';
import {CharacterLootPanelComponent} from './character-loot-panel.component';
import {CharacterHistoryComponent} from './character-history.component';
import {ActionModule} from '../action/action.module';
import {CharacterResolve} from './character.resolver';
import {CreateCharacterComponent} from './create-character.component';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        MaterialModule,
        FlexLayoutModule.forRoot(),
        SharedModule,
        SkillModule,
        OriginModule,
        JobModule,
        EffectModule,
        DateModule,
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
        EffectDetailComponent,
        EffectPanelComponent,
        InventoryPanelComponent,
        ItemDetailComponent,
        ModifierDetailComponent,
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
