import {Component, Input, Output, EventEmitter, OnChanges} from '@angular/core';

@Component({
    selector: 'value-editor',
    styleUrls: ['./value-editor.component.scss'],
    templateUrl: './value-editor.component.html',
})
export class ValueEditorComponent implements OnChanges {
    @Input() value: number;
    @Input() maxValue: number;
    @Input() title: string;
    @Input() hideMaxValue: boolean;
    @Output() onChange: EventEmitter<number> = new EventEmitter<number>();

    public valueDelta: string | undefined;
    public newValue = 0;
    public displayEditor = false;
    public xOffset = 0;
    public yOffset = 0;

    constructor() {
    }

    computeEditorBoundingBox(element: any, bbox: ClientRect): void {
        for (let i = 0; i < element.children.length; i++) {
            this.computeEditorBoundingBox(element.children[i], bbox);
        }
        let rect = element.getBoundingClientRect();
        bbox.bottom = Math.max(bbox.bottom, rect.bottom);
        bbox.top = Math.min(bbox.top, rect.top);
        bbox.right = Math.max(bbox.right, rect.right);
        bbox.left = Math.min(bbox.left, rect.left);
    }

    showEditor(event: MouseEvent) {
        event.preventDefault();
        event.stopPropagation();
        if (this.displayEditor) {
            this.displayEditor = false;
            return;
        }
        this.xOffset = 0;
        this.yOffset = 0;
        this.displayEditor = true;
    }

    hideEditor() {
        this.displayEditor = false;
    }

    searchVeContainer(elements: HTMLCollectionOf<Element>|HTMLCollection) {
        for (let i = 0; i < elements.length; i++) {
            let element = elements[i];
            if (element.classList.contains('ve-container')) {
                return element;
            }
            let result = this.searchVeContainer(element.children);
            if (result !== null) {
                return result;
            }
        }

        return null;
    }

    onDisplayed() {
        let elements = document.getElementsByClassName('cdk-overlay-container');
        let container = this.searchVeContainer(elements);

        let bbox: ClientRect = {
            bottom: 0,
            height: 0,
            left: 0,
            right: 0,
            top: 0,
            width: 0
        };

        this.computeEditorBoundingBox(container, bbox);

        bbox.right += 50;
        bbox.left += 30;
        bbox.top -= 25;
        bbox.bottom -= 5;
        if (bbox.right > window.innerWidth) {
            this.xOffset = window.innerWidth - bbox.right;
        }
        else if (bbox.left < 0) {
            this.xOffset = -bbox.left;
        }

        if (bbox.bottom > window.innerHeight) {
            this.yOffset = window.innerHeight - bbox.bottom;
        }
        else if (bbox.top < 0) {
            this.yOffset = -bbox.top;
        }
    }

    commitValue() {
        this.displayEditor = false;
        this.onChange.emit(this.newValue);
        this.valueDelta = undefined;
    }

    isRelative(val: string) {
        return val.indexOf('+') === 0 || val.indexOf('-') === 0;
    }

    updateNewValue() {
        if (!this.value) {
            this.value = 0;
        }
        if (this.valueDelta) {
            this.newValue = this.value + parseInt(this.valueDelta, 10);
        } else {
            this.newValue = this.value;
        }
    }

    changeValue(val: string) {
        if (!val) {
            this.valueDelta = val;
        }
        else if (val === 'max') {
            if (this.maxValue) {
                let t = this.maxValue - this.value;
                if (t < 0) {
                    this.valueDelta = t.toString();
                } else {
                    this.valueDelta = '+' + t;
                }
            }
        }
        else if (val === 'zero') {
            let t = -this.value;
            if (t < 0) {
                this.valueDelta = t.toString();
            } else {
                this.valueDelta = '+' + t;
            }
        }
        else if (val === 'reset') {
            this.valueDelta = undefined;
        }
        else if (!this.valueDelta) {
            this.valueDelta = val;
        }
        else if (this.isRelative(val) && this.isRelative(this.valueDelta)) {
            let t = parseInt(this.valueDelta, 10);
            t += parseInt(val, 10);
            if (t < 0) {
                this.valueDelta = t.toString();
            } else {
                this.valueDelta = '+' + t;
            }
        }
        this.updateNewValue();
        return false;
    }

    ngOnChanges() {
        this.updateNewValue();
    }

}
