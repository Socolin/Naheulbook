export class Quest {
    id: number;
    name: string;
    data: any;
}

export class QuestData {
    description: string;
    giver: string;
    reward: string;
    validator: string;
}

export class QuestTemplate {
    id: number;
    name: string;
    data: QuestData = new QuestData();
}

