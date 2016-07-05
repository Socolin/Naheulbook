import {Component, Input} from '@angular/core';
import {Origin} from "./origin.model";

@Component({
    moduleId: module.id,
    selector: 'origin',
    templateUrl: 'origin.component.html',
})
export class OriginComponent {
    @Input() origin: Origin;
    public folded: boolean;

    constructor() {
        this.folded = true;
    }

    toggleFold(event: Event) {
        event.preventDefault();
        event.stopPropagation();
        this.folded = !this.folded;
    }
}
