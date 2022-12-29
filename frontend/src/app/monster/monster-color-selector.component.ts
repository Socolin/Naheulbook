import {EventEmitter, Component, Input, Output} from '@angular/core';

import {Monster} from './monster.model';
import {tokenColors} from '../shared';

@Component({
    selector: 'monster-color-selector',
    templateUrl: './monster-color-selector.component.html',
    styleUrls: ['./monster-color-selector.component.scss']
})
export class MonsterColorSelectorComponent {
    @Input() monster: Monster;
    @Output() onColorChange: EventEmitter<string> = new EventEmitter<string>();
    @Output() onNumberChange: EventEmitter<number> = new EventEmitter<number>();
    @Output() fontSize = '48px';

    public showSelector = false;
    public colors: string[] = tokenColors.map(color => '#' + color);
    public numbers: number[];

    constructor() {
        this.numbers = [];
        for (let i = 0; i < 12; i++) {
            this.numbers.push(i);
        }
    }

    openSelector(event: Event) {
        event.preventDefault();
        event.stopPropagation();
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
