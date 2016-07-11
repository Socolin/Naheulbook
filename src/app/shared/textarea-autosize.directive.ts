import {Directive, HostListener, ElementRef} from '@angular/core';


@Directive({
    selector: 'textarea[autosize]'
})
export class TextareaAutosizeDirective {
    constructor(public element: ElementRef) {
    }

    updateSize() {
        this.element.nativeElement.style.height = 'auto';
        this.element.nativeElement.style.height = this.element.nativeElement.scrollHeight + "px";
    }

    @HostListener('input', ['$event.target'])
    onInput(t: HTMLTextAreaElement) {
        this.updateSize();
    }

    ngOnInit() {
        this.element.nativeElement.style.overflow = 'hidden';
        this.updateSize();
    }
}
