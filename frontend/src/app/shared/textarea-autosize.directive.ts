import {Directive, HostListener, ElementRef, OnInit} from '@angular/core';


@Directive({
    selector: 'textarea[autosize]',
    standalone: false
})
export class TextareaAutosizeDirective implements OnInit {
    constructor(public element: ElementRef) {
    }

    updateSize() {
        this.element.nativeElement.style.height = 'auto';
        this.element.nativeElement.style.height = this.element.nativeElement.scrollHeight + 'px';
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
