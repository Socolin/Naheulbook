import {Injectable} from '@angular/core';
import {Notification} from './notification.model';
import {MatSnackBar, MatSnackBarConfig} from '@angular/material';
import {ErrorReportService} from '../error-report.service';
import {ErrorDetailsDialogComponent} from './error-details-dialog.component';
import {NhbkMatDialog} from '../material-workaround';

@Injectable()
export class NotificationsService {
    public notifications: Notification[] = [];

    constructor(
        private readonly snackBar: MatSnackBar,
        private readonly errorReportService: ErrorReportService,
        private readonly dialog: NhbkMatDialog,
    ) {
        errorReportService.notifyError.subscribe(msg => {
            this.addNotification(new Notification('error', msg.message, '', msg.error));
            console.error('==== ERROR ==== ', msg.message, msg.error);
        });
    }

    addNotification(notification: Notification) {
        this.notifications.push(notification);
        if (this.notifications.length === 1) {
            let config = new MatSnackBarConfig();
            config.duration = notification.type === 'error' ? 5000 : 2500;
            const action = notification.type === 'error' ? 'DETAILS' : undefined;
            let snackBarRef = this.snackBar.open(notification.title + ' ' + notification.message, action, config);
            snackBarRef.onAction().subscribe(() => {
                this.dialog.open(ErrorDetailsDialogComponent, {data: notification.data});
            });
            snackBarRef.afterDismissed().subscribe(
                () => {
                    this.notifications.shift();
                    this.proceedNextNotification();
                }
            );
        }
    }

    proceedNextNotification() {
        if (this.notifications.length === 0) {
            return;
        }

        let n = this.notifications[0];
        let config = new MatSnackBarConfig();
        config.duration = 2500;
        this.snackBar.open(n.title + ' ' + n.message, undefined, config).afterDismissed().subscribe(
            () => {
                this.notifications.shift();
                this.proceedNextNotification();
            }
        );
    }

    error(message: string, err?: any) {
        this.addNotification(new Notification('error', 'Erreur', message, err));
        // tslint:disable-next-line:no-console
        console.info('ERROR: ' + 'Erreur' + ':' + message);
        if (err instanceof Error) {
            console.error(err);
        } else {
            console.log('Error data:', err);
        }
    }

    success(title: string, message: string, data?: any) {
        this.addNotification(new Notification('success', title, message, data));
        // tslint:disable-next-line:no-console
        console.info('SUCCESS: ' + title + ':' + message);
    }

    info(title: string, message: string, data?: any) {
        this.addNotification(new Notification('info', title, message, data));
        // tslint:disable-next-line:no-console
        console.info('INFO: ' + title + ':' + message);
    }

    update() {
        for (let i = 0; i < this.notifications.length; i++) {
            if (this.notifications[i].isExpired()) {
                this.notifications.splice(i, 1);
                i--;
            }
        }
    }
}
