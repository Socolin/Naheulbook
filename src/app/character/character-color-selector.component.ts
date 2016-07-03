import {Component, EventEmitter} from '@angular/core';

@Component({
    selector: 'character-color-selector',
    templateUrl: 'app/character/character-color-selector.component.html',
    inputs: ["element"],
    outputs: ['onColorChange']
})
export class CharacterColorSelector {
    public element: Object;
    private onColorChange: EventEmitter<string> = new EventEmitter<string>();
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
