import {Component, Input} from '@angular/core';
import {StatsModifier} from '../shared/stat-modifier.model';

@Component({
    selector: 'stats-modifier-editor',
    templateUrl: './stats-modifier-editor.component.html',
    styleUrls: ['./stats-modifier-editor.component.scss'],
})
export class CharacterModifierEditorComponent {
    @Input() modifier: StatsModifier;
    @Input() reusableToggle: boolean = true;
}
