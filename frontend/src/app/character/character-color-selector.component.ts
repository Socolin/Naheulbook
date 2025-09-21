import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Character} from './character.model';
import {tokenColors} from '../shared';

@Component({
    selector: 'character-color-selector',
    templateUrl: './character-color-selector.component.html',
    standalone: false
})
export class CharacterColorSelectorComponent {
    @Input() character: Character;
    @Output() colorChanged: EventEmitter<string> = new EventEmitter<string>();

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
        this.colorChanged.emit(color);
        this.showSelector = false;
    }
}
