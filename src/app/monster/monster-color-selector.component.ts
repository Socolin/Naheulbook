import {EventEmitter, Component, Input, Output} from '@angular/core';

import {GroupService} from '../group/group.service';
import {NotificationsService} from '../notifications';

import {Monster} from './monster.model';

@Component({
    moduleId: module.id,
    selector: 'monster-color-selector',
    templateUrl: 'monster-color-selector.component.html'
})
export class MonsterColorSelectorComponent {
    @Input() monster: Monster;
    @Output() onColorChange: EventEmitter<string> = new EventEmitter<string>();
    @Output() onNumberChange: EventEmitter<number> = new EventEmitter<number>();

    private showSelector = false;
    private colors: string[] = [
        "ffcc00", "ff6600",
        "ff3300", "990033",
        "ff3399", "cc33ff",
        "0099cc", "0033cc",
        "009999", "00cc66",
        "006600",
        "669900", "996633",
        "999966", "595959",
        "000000",
    ].map(color => '#' + color);
    private numbers: number[];

    constructor() {
        this.numbers = [];
        for (let i = 0; i < 16; i++) {
            this.numbers.push(i);
        }
    }

    changeColor(color: string) {
        if (color.indexOf('#') === 0) {
            color = color.substring(1);
        }
        this.onColorChange.emit(color);
    }

    changeNumber(number: number) {
        this.onNumberChange.emit(number);
    }

}
