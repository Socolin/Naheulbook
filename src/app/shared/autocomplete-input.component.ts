import {Component, Input, Output, EventEmitter} from '@angular/core';

export class AutocompleteValue {
    constructor(value: any, text: string) {
        this.value = value;
        this.text = text;
    }

    value: any;
    text: string;
}

@Component({
    moduleId: module.id,
    selector: 'autocomplete-input',
    templateUrl: 'autocomplete-input.component.html'
})
export class AutocompleteInputComponent {
    @Input() callback: Function;
    @Input() value: string;
    @Output() onSelect: EventEmitter<any> = new EventEmitter<any>();
    private values: AutocompleteValue[];

    selectValue(value: any) {
        this.onSelect.emit(value.value);
        this.value = value.text;
        this.values = null;
        return false;
    }

    updateList() {
        this.callback(this.value).subscribe(
            values => {
                this.values = values;
            },
            err => {
                console.log(err);
            }
        )
        ;
    }
}
