
import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
    moduleId: module.id,
    selector: 'character-color-selector',
    templateUrl: 'character-color-selector.component.html'
})
export class CharacterColorSelectorComponent {
    @Input() element: Object;
    @Output() onColorChange: EventEmitter<string> = new EventEmitter<string>();

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
}
