import {Component, Input, Output, EventEmitter, Renderer, ElementRef, OnChanges} from '@angular/core';

@Component({
    selector: 'value-editor',
    templateUrl: './value-editor.component.html',
})
export class ValueEditorComponent implements OnChanges {
    @Input() value: number;
    @Input() maxValue: number;
    @Input() title: string;
    @Output() onChange: EventEmitter<number> = new EventEmitter<number>();

    private valueDelta: string;
    private newValue: number = 0;
    private displayEditor: boolean = false;
    private xOffset: number = 0;
    private yOffset: number = 0;

    constructor(private _renderer: Renderer
        , private _elementRef: ElementRef) {
    }

    computeEditorBoundingBox(element: any, bbox: ClientRect): void {
        let h = element.hidden;
        element.hidden = false;
        for (let i = 0; i < element.children.length; i++) {
            this.computeEditorBoundingBox(element.children[i], bbox);
        }
        let rect = element.getBoundingClientRect();
        element.hidden = h;
        bbox.bottom = Math.max(bbox.bottom, rect.bottom);
        bbox.top = Math.min(bbox.top, rect.top);
        bbox.right = Math.max(bbox.right, rect.right);
        bbox.left = Math.min(bbox.left, rect.left);
    }

    showEditor() {
        if (this.displayEditor) {
            this.displayEditor = false;
            return;
        }

        let bbox: ClientRect = {
            bottom: 0,
            height: 0,
            left: 0,
            right: 0,
            top: 0,
            width: 0
        };
        this.computeEditorBoundingBox(this._elementRef.nativeElement, bbox);

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

        this.displayEditor = true;
    }

    commitValue() {
        this.displayEditor = false;
        this.onChange.emit(this.newValue);
        this.valueDelta = null;
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
