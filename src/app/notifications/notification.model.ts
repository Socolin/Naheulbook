export class Notification {
    type: string;
    title: string;
    message: string;
    time: Date;

    constructor(type: string, title: string, message: string) {
        this.type = type;
        this.title = title;
        this.message = message;
        this.time = new Date();
    }

    isExpired(): boolean {
        let now = new Date();
        return (now.getTime() - this.time.getTime()) >= 5000;
    }
}
