import {EventEmitter, Component, Input, Output} from '@angular/core';

import {Monster} from './monster.model';

@Component({
    selector: 'monster-color-selector',
    templateUrl: './monster-color-selector.component.html'
})
export class MonsterColorSelectorComponent {
    @Input() monster: Monster;
    @Output() onColorChange: EventEmitter<string> = new EventEmitter<string>();
    @Output() onNumberChange: EventEmitter<number> = new EventEmitter<number>();

    public showSelector = false;
    public colors: string[] = [
        'ffcc00', 'ff6600', '990033', 'ff99ff',
        '660066', '0066ff', '00802b', '666666',
        '000000', 'dddddd'
    ].map(color => '#' + color);
    public numbers: number[];

    constructor() {
        this.numbers = [];
        for (let i = 0; i < 12; i++) {
            this.numbers.push(i);
        }
    }

    openSelector() {
        this.showSelector = true;
    }
    closeSelector() {
        this.showSelector = false;
    }

    changeColor(color: string) {
        if (color.indexOf('#') === 0) {
            color = color.substring(1);
        }
        this.onColorChange.emit(color);
        this.closeSelector();
    }

    changeNumber(number: number) {
        this.onNumberChange.emit(number);
        this.closeSelector();
    }

}
