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

import {
    AddItemDialogComponent,
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
import {EditItemDialogComponent} from './edit-item-dialog.component';
import { AddItemModifierDialogComponent } from './add-item-modifier-dialog.component';
import { CharacterItemDialogComponent } from './character-item-dialog.component';
import { ItemLineComponent } from './item-line.component';
import {WebsocketModule} from '../websocket/websocket.module';

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
        AddItemDialogComponent,
        EditItemDialogComponent,
        AddItemModifierDialogComponent,
        CharacterItemDialogComponent,
        ItemLineComponent,
    ],
    entryComponents: [
        AddItemDialogComponent,
        CharacterItemDialogComponent,
        EditItemDialogComponent,
        AddItemModifierDialogComponent,
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
