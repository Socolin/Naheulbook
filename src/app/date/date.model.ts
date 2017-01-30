export interface CalendarDate {
    id: number;
    name: string;
    startDay: number;
    endDay: number;
    note: string;
}

export class NhbkDate {
    public minute: number = 0;
    public hour: number = 0;
    public day: number = 0;
    public year: number = 1498;

    constructor(minute?: number, hour?: number, day?: number, year?: number) {
        this.minute = minute;
        this.hour = hour;
        this.day = day;
        this.year = year;
    }
}

export class NhbkDateOffset {
    public minute: number = 0;
    public hour: number = 0;
    public day: number = 0;
    public week: number = 0;
    public year: number = 0;
    public second: number = 0;
}

