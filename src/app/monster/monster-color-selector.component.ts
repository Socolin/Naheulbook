import {EventEmitter, Component, Input, Output} from '@angular/core';

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
        "ffcc00", "ff6600", "990033", "ff99ff",
        "660066", "0066ff", "00802b", "666666",
        "000000", "dddddd"
    ].map(color => '#' + color);
    private numbers: number[];

    constructor() {
        this.numbers = [];
        for (let i = 0; i < 12; i++) {
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
