export type NotificationType = 'error' | 'success' | 'info';

export class Notification {
    type: NotificationType;
    title: string;
    message: string;
    data: any;
    time: Date;

    constructor(type: NotificationType, title: string, message: string, data?: any) {
        this.type = type;
        this.title = title;
        this.message = message;
        this.time = new Date();
        this.data = data;
    }

    isExpired(): boolean {
        let now = new Date();
        return (now.getTime() - this.time.getTime()) >= 5000;
    }
}
