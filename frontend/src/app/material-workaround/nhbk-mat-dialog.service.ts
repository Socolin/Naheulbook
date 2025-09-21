import {Injectable, TemplateRef} from '@angular/core';
import {MatDialog, MatDialogConfig, MatDialogRef} from '@angular/material/dialog';
import {ComponentType} from '@angular/cdk/overlay';
import {Location} from '@angular/common';
import {ActivatedRoute, Router} from '@angular/router';

@Injectable({providedIn: 'root'})
export class NhbkMatDialog {
    private dialogCount = 0;

    constructor(
        private readonly dialog: MatDialog,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly location: Location
    ) {
    }

    open<T, D = any, R = any>(
        componentOrTemplateRef: ComponentType<T> | TemplateRef<T>,
        config?: MatDialogConfig<D>
    ): MatDialogRef<T, R> {
        return this.dialog.open(componentOrTemplateRef, {...config, maxWidth: (config && config.maxWidth) || '960px'});
    }

    openFullScreen<T, D = any, R = any>(
        componentOrTemplateRef: ComponentType<T> | TemplateRef<T>,
        config?: MatDialogConfig<D>
    ): MatDialogRef<T, R> {
        const dialogCount = this.dialogCount++;

        if (!(this.location.getState() && 'isDialog' in (this.location.getState() as any))) {
            this.router.navigate([], {
                fragment: location.hash.substring(1) || undefined,
                queryParams: {dialogOpen: this.dialogCount},
                relativeTo: this.route,
                state: {isDialog: true, dialogCount: dialogCount}
            }).then(() => {
                this.location.replaceState(location.pathname + location.hash, location.search, {
                    isDialog: true,
                    dialogCount: dialogCount
                });
            });
        }

        const dialogRef = this.dialog.open(componentOrTemplateRef, {
            ...config,
            minWidth: '100vw',
            height: '100vh',
            panelClass: 'nhbk-full-screen-dialog',
            autoFocus: config === undefined || config.autoFocus === undefined ? false : config.autoFocus
        });
        dialogRef.afterClosed().subscribe(() => {
            if (this.location.getState() && 'isDialog' in (this.location.getState() as any)) {
                if ((this.location.getState() as any)['dialogCount'] === dialogCount) {
                    this.location.back();
                }
            }
        });
        return dialogRef;
    }
}
