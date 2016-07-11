import {Pipe, PipeTransform} from '@angular/core';

@Pipe({name: 'textFormatter'})
export class TextFormatterPipe implements PipeTransform {
    private tagsInfo = {
        b: {
            open: '<strong>',
            close: '</strong>'
        },
        i: {
            open: '<i>',
            close: '</i>'
        },
        hr: {
            open: '<hr>',
            close: '</hr>'
        },
        list: {
            open: '<ul>',
            close: '</ul>'
        },
        '*': {
            open: '<li>',
            close: '</li>',
            closeOnNewLine: true
        }
    };

    transform(text: string) {
        let result = '';

        let tags: string[] = [];
        let openTag = false;
        let lastChar: string = null;
        let currentTag: string = null;
        let isClosingTag = false;
        let currentTagInfo: any = null;
        for (let i = 0; i < text.length; i++) {
            let currentChar = text.charAt(i);
            if (currentChar === '\n') {
                if (currentTagInfo && currentTagInfo.closeOnNewLine) {
                    tags.pop();
                    result += currentTagInfo.close;
                    if (tags.length) {
                        currentTag = tags[tags.length - 1];
                        currentTagInfo = this.tagsInfo[currentTag];
                    } else {
                        currentTag = null;
                        currentTagInfo = null;
                    }
                } else {
                    result += '<br>';
                }
            }
            else if (lastChar !== '\'' && currentChar === '[') {
                if (openTag) {
                    console.log('Error in text at char: ' + i);
                }
                currentTag = '';
                isClosingTag = false;
                openTag = true;
            }
            else if (currentChar === ']') {
                openTag = false;
                if (isClosingTag) {
                    let lastOpenTag = tags.pop();
                    if (lastOpenTag !== currentTag) {
                        console.log('Error, found closing tag `' + currentTag + '\' at pos: ' + i
                            + ' but expecting tag: `' + lastOpenTag + '\'');
                    }
                    if (currentTag in this.tagsInfo) {
                        if (this.tagsInfo[currentTag].close) {
                            result += this.tagsInfo[lastOpenTag].close;
                        }
                    } else {
                        console.log('unknown closing tag: ' + currentTag + 'at pos: ' + i);
                    }
                } else {
                    tags.push(currentTag);
                    if (currentTag in this.tagsInfo) {
                        currentTagInfo = this.tagsInfo[currentTag];
                        if (this.tagsInfo[currentTag].open) {
                            result += this.tagsInfo[currentTag].open;
                        }
                    } else {
                        console.log('unknown opening tag: ' + currentTag + 'at pos: ' + i);
                    }
                }
            }
            else {
                if (openTag) {
                    if (currentTag.length === 0 && currentChar === '/') {
                        isClosingTag = true;
                    } else {
                        currentTag += currentChar;
                    }
                } else {
                    result += currentChar;
                }
            }
            lastChar = currentChar;
        }
        return result;
    }
}
