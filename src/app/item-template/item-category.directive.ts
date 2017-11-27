import {Directive, ElementRef, Input} from '@angular/core';
import {ItemCategory} from './item-template.model';

@Directive({
    selector: '[itemCategory]'
})
export class ItemCategoryDirective {
    @Input() category: ItemCategory;
    constructor(public elementRef: ElementRef) {
    }
}
