import {Component, Input, OnInit} from '@angular/core';
import {ItemModifier} from './item.model';
import {NhbkDateOffset} from '../date/date.model';
import {dateOffset2TimeDuration} from '../date/util';

@Component({
    selector: 'modifier-editor',
    templateUrl: './modifier-editor.component.html'
})
export class ModifierEditorComponent {
    @Input() modifier: ItemModifier = new ItemModifier();

    public durationDateOffset: NhbkDateOffset = new NhbkDateOffset();

    setTimeDuration(dateOffset: NhbkDateOffset) {
        if (this.modifier.durationType !== 'time') {
            throw new Error('Try to set time duration while durationType is not `time` but is `' + this.modifier.durationType + '`');
        }
        this.modifier.duration = dateOffset2TimeDuration(dateOffset);
    }

    updateDurationType() {
        if (this.modifier.durationType === 'custom') {
            this.modifier.duration = '';
        }
        else if (this.modifier.durationType === 'combat') {
            this.modifier.duration = 1;
        }
        else if (this.modifier.durationType === 'forever') {
            this.modifier.duration = null;
        }
        else if (this.modifier.durationType === 'time') {
            this.modifier.duration = dateOffset2TimeDuration(this.durationDateOffset);
        }
        else {
            throw new Error('Invalid durationType: `' + this.modifier.durationType + '\'');
        }
    }
}
