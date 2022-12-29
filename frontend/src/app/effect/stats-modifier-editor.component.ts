import {Component, Input} from '@angular/core';
import {StatsModifier} from '../shared';

@Component({
    selector: 'stats-modifier-editor',
    templateUrl: './stats-modifier-editor.component.html',
    styleUrls: ['./stats-modifier-editor.component.scss'],
})
export class StatModifierEditorComponent {
    @Input() modifier: StatsModifier;
    @Input() options: {
        noDescription?: boolean,
        noType?: boolean
    } = {};
    @Input() reusableToggle = true;
}
