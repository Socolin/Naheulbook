import {Component} from '@angular/core';
import {Origin} from "./origin.model";

@Component({
    selector: 'origin',
    templateUrl: 'app/origin/origin.component.html',
    inputs: ['origin']
})
export class OriginComponent {
    public origin: Origin;
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
