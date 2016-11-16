import {NgModule}      from '@angular/core';
import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";

import {SharedModule} from "../shared/shared.module";

import {
    SpecialitySelectorComponent, ItemDetailComponent, CharacterComponent,
    CharacterColorSelectorComponent, CharacterListComponent, CreateCharacterComponent
} from "./";
import {
    CharacterService
} from "./character.service";
import {SkillModule} from "../skill/skill.module";
import {OriginModule} from "../origin/origin.module";
import {JobModule} from "../job/job.module";
import {EffectModule} from "../effect/effect.module";
import {
    EffectPanelComponent, ModifierDetailComponent, EffectDetailComponent
} from "./effect-panel.component";
import {InventoryPanelComponent} from "./inventory-panel.component";
import {SwipableItemDetail} from "./swipable-item-detail.component";
import {BagItemViewComponent} from "./bag-item-view.component";
import {DateModule} from "../date/date.module";
import {ModifierEditorComponent} from "./modifier-editer.component";
import {CharacterLootPanelComponent} from "./character-loot-panel.component";

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule,
        SkillModule,
        OriginModule,
        JobModule,
        EffectModule,
        DateModule,
    ],
    declarations: [
        BagItemViewComponent
        , CharacterComponent
        , CharacterColorSelectorComponent
        , CharacterListComponent
        , CreateCharacterComponent
        , ItemDetailComponent
        , SpecialitySelectorComponent
        , EffectPanelComponent
        , SwipableItemDetail
        , InventoryPanelComponent
        , EffectDetailComponent
        , ModifierDetailComponent
        , ModifierEditorComponent
        , CharacterLootPanelComponent
    ],
    exports: [
        CharacterComponent
        , CharacterListComponent
        , CreateCharacterComponent
        , CharacterColorSelectorComponent
        , EffectModule
        , SkillModule
    ],
    providers: [
        CharacterService,
    ],
})
export class CharacterModule {
}
