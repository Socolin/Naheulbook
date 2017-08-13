import {Injectable} from '@angular/core';
import {Notification} from './notification.model';
import {MdSnackBar, MdSnackBarConfig} from '@angular/material';

@Injectable()
export class NotificationsService {
    public notifications: Notification[] = [];

    constructor(private _snackBar: MdSnackBar) {
    }

    addNotification(n: Notification) {
        this.notifications.push(n);
        if (this.notifications.length === 1) {
            let config = new MdSnackBarConfig();
            config.duration = 2500;
            this._snackBar.open(n.title + ' ' + n.message, undefined, config).afterDismissed().subscribe(
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
        let config = new MdSnackBarConfig();
        config.duration = 2500;
        this._snackBar.open(n.title + ' ' + n.message, undefined, config).afterDismissed().subscribe(
            () => {
                this.notifications.shift();
                this.proceedNextNotification();
            }
        );
    }

    error(title: string, message: string, err?: any, data?: any) {
        this.addNotification(new Notification('error', title, message, data));
        console.log('ERROR: ' + title + ':' + message);
        if (err) {
            console.log(err);
        }
    }

    success(title: string, message: string, data?: any) {
        this.addNotification(new Notification('success', title, message, data));
        console.log('SUCCESS: ' + title + ':' + message);
    }

    info(title: string, message: string, data?: any) {
        this.addNotification(new Notification('info', title, message, data));
        console.log('INFO: ' + title + ':' + message);
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
