import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Character} from './character.model';
import {tokenColors} from '../shared';

@Component({
    selector: 'character-color-selector',
    templateUrl: './character-color-selector.component.html'
})
export class CharacterColorSelectorComponent {
    @Input() character: Character;
    @Output() onColorChange: EventEmitter<string> = new EventEmitter<string>();

    public showSelector = false;
    public colors: string[] = tokenColors.map(color => '#' + color);

    toggleSelector(event: Event) {
        event.preventDefault();
        event.stopPropagation();
        this.showSelector = !this.showSelector;
    }

    hideSelector() {
        this.showSelector = false;
    }

    changeColor(color: string) {
        this.onColorChange.emit(color);
        this.showSelector = false;
    }
}
