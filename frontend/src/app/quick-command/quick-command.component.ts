import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import { Portal, CdkPortal } from '@angular/cdk/portal';
import {Overlay, OverlayConfig, OverlayRef} from '@angular/cdk/overlay';
import {removeDiacritics} from '../shared';
import {QuickAction} from './quick-action.model';
import {QuickCommandService} from './quick-command.service';
import { MatRipple } from '@angular/material/core';
import { MatIcon } from '@angular/material/icon';
import { NgStyle } from '@angular/common';

@Component({
    selector: 'app-quick-command',
    templateUrl: './quick-command.component.html',
    styleUrls: ['./quick-command.component.scss'],
    imports: [CdkPortal, MatRipple, MatIcon, NgStyle]
})
export class QuickCommandComponent implements OnInit {

    @ViewChild('inputElement', {static: false})
    public inputElement: ElementRef;

    @ViewChild('portal', {static: true})
    public portal: Portal<any>;
    public overlayRef?: OverlayRef;

    public actions: QuickAction[] = [];
    public selected?: QuickAction

    selectNext() {
        console.log('next');
        if (!this.selected) {
            this.selected = this.actions[0];
        }

        const i = this.actions.indexOf(this.selected);
        if (i === -1) {
            this.selected = this.actions[0];
        }

        this.selected = this.actions[(i + 1) % this.actions.length];
    }

    selectPrevious() {
        if (!this.selected) {
            this.selected = this.actions[0];
        }

        const i = this.actions.indexOf(this.selected);
        if (i === -1) {
            this.selected = this.actions[0];
        }

        if (i - 1 < 0) {
            this.selected = this.actions[(this.actions.length - 1)];
        } else {
            this.selected = this.actions[(i - 1)];
        }
    }

    constructor(
        private readonly quickCommandService: QuickCommandService,
        private readonly overlay: Overlay
    ) {
    }

    updateFilter(filter?: string) {
        const cleanFilter = removeDiacritics(filter || '').toLowerCase();
        this.actions = this.quickCommandService.getAllActions()
            .filter(x => removeDiacritics(x.displayText).toLowerCase().indexOf(cleanFilter) !== -1)
            .slice(0, 15);
        if (!this.selected) {
            this.selected = this.actions[0];
        }
        const i = this.actions.indexOf(this.selected);
        if (i === -1) {
            this.selected = this.actions[0];
        }

    }

    ngOnInit(): void {
        window.addEventListener('keydown', (event) => {
            if (event.ctrlKey && event.key === 'A') {
                event.preventDefault();
                event.stopImmediatePropagation();
                event.stopPropagation();
                this.open();
            }
            if (this.overlayRef) {
                if (event.key === 'ArrowDown') {
                    this.selectNext();
                    event.preventDefault();
                    event.stopImmediatePropagation();
                    event.stopPropagation();
                } else if (event.key === 'ArrowUp') {
                    this.selectPrevious();
                    event.preventDefault();
                    event.stopImmediatePropagation();
                    event.stopPropagation();
                } else if (event.key === 'Enter') {
                    this.executeAction(this.selected);
                } else if (event.key === 'Escape') {
                    this.executeAction(this.overlayRef?.detach());
                }
            }
        });
    }

    open() {
        let config = new OverlayConfig();

        config.positionStrategy = this.overlay.position()
            .global()
            .centerHorizontally()
            .top('80px');
        config.hasBackdrop = true;
        config.backdropClass = '';

        this.overlayRef = this.overlay.create(config);
        this.overlayRef.attachments().subscribe(() => {
            setTimeout(() => {
                this.inputElement.nativeElement.focus();
            }, 0);
        });
        this.overlayRef.backdropClick().subscribe(() => {
            this.overlayRef?.detach();
        })
        this.overlayRef.detachments().subscribe(() => {
            this.overlayRef = undefined;
        })
        this.overlayRef.attach(this.portal);
    }

    public executeAction(selected?: QuickAction) {
        if (!selected) {
            return;
        }
        selected.action();
        this.overlayRef?.detach();
    }
}
