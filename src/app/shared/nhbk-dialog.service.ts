import {Injectable} from '@angular/core';
import {Overlay, Portal, OverlayRef, OverlayState} from '@angular/material';

@Injectable()
export class NhbkDialogService {
    constructor(private _overlay: Overlay) {
    }

    openCenteredBackdropDialog(portal: Portal<any>): OverlayRef {
        let config = new OverlayState();

        config.positionStrategy = this._overlay.position()
            .global()
            .centerHorizontally()
            .centerVertically();
        config.hasBackdrop = true;

        let overlayRef = this._overlay.create(config);
        overlayRef.attach(portal);
        overlayRef.backdropClick().subscribe(() => overlayRef.detach());
        return overlayRef;
    }
}
