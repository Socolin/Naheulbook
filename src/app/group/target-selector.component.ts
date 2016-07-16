import {Component, EventEmitter, Input, Output} from '@angular/core';

@Component({
    selector: 'target-selector',
    templateUrl: 'target-selector.component.html',
    moduleId: module.id
})
export class TargetSelectorComponent {
    @Input() element: Object;
    @Input() targets: Object[];
    @Output() onTargetChange: EventEmitter<any> = new EventEmitter<any>();
    private showSelector: boolean = false;
}
