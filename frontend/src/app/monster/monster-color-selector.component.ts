import {Component, EventEmitter, Input, Output} from '@angular/core';

import {Monster} from './monster.model';
import {tokenColors} from '../shared';

@Component({
    selector: 'monster-color-selector',
    templateUrl: './monster-color-selector.component.html',
    styleUrls: ['./monster-color-selector.component.scss'],
    standalone: false
})
export class MonsterColorSelectorComponent {
    @Input() monster: Monster;
    @Output() colorChanged: EventEmitter<string> = new EventEmitter<string>();
    @Output() numberChanged: EventEmitter<number> = new EventEmitter<number>();
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
        this.colorChanged.emit(color);
        this.closeSelector();
    }

    changeNumber(number: number) {
        this.numberChanged.emit(number);
        this.closeSelector();
    }

}
