import {Injectable} from "@angular/core";
import {Notification} from './notification.model';

@Injectable()
export class NotificationsService {
    public notifications: Notification[] = [];

    constructor() {
    }

    addNotification(n: Notification) {
        this.notifications.push(n);
        setTimeout(() => {
            this.update();
        }, 5100);
    }

    error(title: string, message: string, err?: any) {
        this.addNotification(new Notification('error', title, message));
        console.log('ERROR: ' + title + ':' + message);
        if (err) {
            console.log(err);
        }
    }

    success(title: string, message: string) {
        this.addNotification(new Notification('success', title, message));
        console.log('SUCCESS: ' + title + ':' + message);
    }

    info(title: string, message: string) {
        this.addNotification(new Notification('info', title, message));
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
