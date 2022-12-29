import {NgModule}      from '@angular/core';
import {CreateQuestTemplateComponent, QuestEditorComponent, QuestListComponent} from './';
import {CommonModule} from '@angular/common';
import {SharedModule} from '../shared/shared.module';
import {FormsModule} from '@angular/forms';

@NgModule({
    imports: [
        CommonModule,
        FormsModule,
        SharedModule
    ],
    declarations: [
        CreateQuestTemplateComponent,
        QuestEditorComponent,
        QuestListComponent,
    ],
})
export class QuestModule {
}
