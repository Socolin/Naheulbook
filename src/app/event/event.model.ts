export class NEvent {
    id: number;
    name: string;
    description: string;
    timestamp: number;

    static fromJson(jsonData: any): NEvent {
        let event = new NEvent();
        Object.assign(event, jsonData);
        return event;
    }

    static eventsFromJson(jsonData: object[]): NEvent[] {
        let events: NEvent[] = [];
        for (let eventData of jsonData) {
            events.push(NEvent.fromJson(eventData));
        }
        return events;
    }
}
