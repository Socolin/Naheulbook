import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Character} from './character.model';

@Component({
    selector: 'character-color-selector',
    templateUrl: 'character-color-selector.component.html'
})
export class CharacterColorSelectorComponent {
    @Input() character: Character;
    @Output() onColorChange: EventEmitter<string> = new EventEmitter<string>();

    public showSelector = false;
    public colors: string[] = [
        'ffcc00', 'ff6600', '990033', 'ff99ff',
        '660066', '0066ff', '00802b', '666666',
        '000000', 'dddddd'
    ].map(color => '#' + color);

    toggleSelector() {
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
