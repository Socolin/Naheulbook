import {Component, EventEmitter, Input, Output} from '@angular/core';
import {Character} from './character.model';
import {tokenColors} from '../shared';
import { CdkOverlayOrigin, CdkConnectedOverlay } from '@angular/cdk/overlay';
import { MatRipple } from '@angular/material/core';
import { MatCard } from '@angular/material/card';

@Component({
    selector: 'character-color-selector',
    templateUrl: './character-color-selector.component.html',
    imports: [CdkOverlayOrigin, MatRipple, CdkConnectedOverlay, MatCard]
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
