import {Component, Input, OnInit} from '@angular/core';
import {ItemModifier} from './item.model';
import {NhbkDateOffset} from '../date/date.model';
import {dateOffset2TimeDuration} from '../date/util';

@Component({
    selector: 'modifier-editor',
    templateUrl: 'modifier-editor.component.html'
})
export class ModifierEditorComponent implements OnInit {
    @Input() modifier: ItemModifier = new ItemModifier();

    private durationDateOffset: NhbkDateOffset = new NhbkDateOffset();

    setTimeDuration(dateOffset: NhbkDateOffset) {
        if (this.modifier.durationType != 'time') {
            throw new Error('Try to set time duration while durationType is not `time` but is `' + this.modifier.durationType + '`');
        }
        this.modifier.duration = dateOffset2TimeDuration(dateOffset);
    }

    setDurationType(durationType: string) {
        if (durationType === 'custom') {
            this.modifier.duration = '';
        }
        else if (durationType === 'combat') {
            this.modifier.duration = 1;
        }
        else if (durationType === 'time') {
            this.modifier.duration = dateOffset2TimeDuration(this.durationDateOffset);
        }
        else {
            throw new Error('Invalid durationType: `' + durationType + '\'');
        }
        this.modifier.durationType = durationType;
    }

    ngOnInit() {
    }
}
