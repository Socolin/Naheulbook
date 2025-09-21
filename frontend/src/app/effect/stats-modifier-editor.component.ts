import {Component, Input} from '@angular/core';
import {StatsModifier} from '../shared';
import { MatFormField } from '@angular/material/form-field';
import { MatInput } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { CdkTextareaAutosize } from '@angular/cdk/text-field';
import { DurationSelectorComponent } from '../date/duration-selector.component';
import { MatCheckbox } from '@angular/material/checkbox';
import { MatCardContent } from '@angular/material/card';
import { ModifiersEditorComponent } from './modifiers-editor.component';

@Component({
    selector: 'stats-modifier-editor',
    templateUrl: './stats-modifier-editor.component.html',
    styleUrls: ['./stats-modifier-editor.component.scss'],
    imports: [MatFormField, MatInput, FormsModule, CdkTextareaAutosize, DurationSelectorComponent, MatCheckbox, MatCardContent, ModifiersEditorComponent]
})
export class StatModifierEditorComponent {
    @Input() modifier: StatsModifier;
    @Input() options: {
        noDescription?: boolean,
        noType?: boolean
    } = {};
    @Input() reusableToggle = true;
}
