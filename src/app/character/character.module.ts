import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';

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
import {WebsocketModule} from '../websocket/websocket.module';

import {CharacterService} from './character.service';
import {CharacterResolve} from './character.resolver';

import {
    AddItemDialogComponent,
    AddItemModifierDialogComponent,
    BagItemViewComponent,
    ChangeJobDialogComponent,
    ChangeSexDialogComponent,
    CharacterColorSelectorComponent,
    CharacterComponent,
    CharacterHistoryComponent,
    CharacterItemDialogComponent,
    CharacterListComponent,
    CharacterLootPanelComponent,
    CreateCharacterComponent,
    CreateCustomCharacterComponent,
    EditItemDialogComponent,
    EffectPanelComponent,
    GiveItemDialogComponent,
    InventoryPanelComponent,
    ItemLifetimeEditorDialogComponent,
    ItemLineComponent,
    JobPlayerDialogComponent,
    OriginPlayerDialogComponent,
    SkillInfoDialogComponent,
    SkillSelectorComponent,
    SpecialitySelectorComponent,
    TakeLootDialogComponent,
} from './';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        NhbkMaterialModule,
        SharedModule,
        DateModule,
        SkillModule,
        OriginModule,
        JobModule,
        EffectModule,
        ActionModule,
        ItemTemplateModule,
        ItemModule,
        WebsocketModule,
        ReactiveFormsModule,
    ],
    declarations: [
        AddItemDialogComponent,
        AddItemModifierDialogComponent,
        BagItemViewComponent,
        ChangeJobDialogComponent,
        ChangeSexDialogComponent,
        CharacterColorSelectorComponent,
        CharacterComponent,
        CharacterHistoryComponent,
        CharacterItemDialogComponent,
        CharacterListComponent,
        CharacterLootPanelComponent,
        CreateCharacterComponent,
        CreateCustomCharacterComponent,
        EditItemDialogComponent,
        EffectPanelComponent,
        GiveItemDialogComponent,
        InventoryPanelComponent,
        ItemLifetimeEditorDialogComponent,
        ItemLineComponent,
        SkillSelectorComponent,
        SpecialitySelectorComponent,
        SkillInfoDialogComponent,
        TakeLootDialogComponent,
        OriginPlayerDialogComponent,
        JobPlayerDialogComponent,
    ],
    entryComponents: [
        AddItemDialogComponent,
        AddItemModifierDialogComponent,
        ChangeJobDialogComponent,
        ChangeSexDialogComponent,
        CharacterItemDialogComponent,
        EditItemDialogComponent,
        GiveItemDialogComponent,
        ItemLifetimeEditorDialogComponent,
        JobPlayerDialogComponent,
        OriginPlayerDialogComponent,
        SkillInfoDialogComponent,
        TakeLootDialogComponent,
    ],
    exports: [
        CharacterColorSelectorComponent,
        CharacterComponent,
        CharacterListComponent,
        CreateCharacterComponent,
        CreateCustomCharacterComponent,
        EffectModule,
        SkillModule,
    ],
    providers: [
        CharacterService,
        CharacterResolve,
    ],
})
export class CharacterModule {
}
