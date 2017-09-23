import {Injectable} from '@angular/core';
import {Overlay, OverlayRef, OverlayConfig} from '@angular/cdk/overlay';
import {Portal} from '@angular/cdk/portal';

@Injectable()
export class NhbkDialogService {
    constructor(private _overlay: Overlay) {
    }

    openCenteredBackdropDialog(portal: Portal<any>, noBackdropEvent?: boolean): OverlayRef {
        let config = new OverlayConfig();

        config.positionStrategy = this._overlay.position()
            .global()
            .centerHorizontally()
            .centerVertically();
        config.hasBackdrop = true;

        let overlayRef = this._overlay.create(config);
        overlayRef.attach(portal);
        if (!noBackdropEvent) {
            overlayRef.backdropClick().subscribe(() => overlayRef.detach());
        }
        return overlayRef;
    }

    openTopCenteredBackdropDialog(portal: Portal<any>, noBackdropEvent?: boolean): OverlayRef {
        let config = new OverlayConfig();

        config.positionStrategy = this._overlay.position()
            .global()
            .centerHorizontally()
            .top('40px');
        config.hasBackdrop = true;

        let overlayRef = this._overlay.create(config);
        overlayRef.attach(portal);
        if (!noBackdropEvent) {
            overlayRef.backdropClick().subscribe(() => overlayRef.detach());
        }
        return overlayRef;
    }
}
