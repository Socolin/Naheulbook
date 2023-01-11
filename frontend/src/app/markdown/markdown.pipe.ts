import {Pipe, PipeTransform} from '@angular/core';
import {marked} from 'marked'

@Pipe({name: 'markdown'})
export class MarkdownPipe implements PipeTransform {
    private renderer = new marked.Renderer();

    constructor() {
        this.renderer.link = function (href, title, _) {
            const link = marked.Renderer.prototype.link.apply(this, arguments);
            return link.replace('<a', '<a target=\'_blank\'');
        };
    }

    transform(text?: string) {
        if (!text) {
            return '';
        }
        try {
            return marked(text, {renderer: this.renderer});
        } catch (e) {
            return 'Invalid markdown: ' + e.message;
        }
    }
}

