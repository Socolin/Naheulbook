export class Flag {
    type: string;
    data?: any;
}

export class DescribedFlag {
    description: string;
    flags: Flag[] = [];

    static fromJson(jsonData: any): DescribedFlag {
        let describedFlag = new DescribedFlag();
        Object.assign(describedFlag, jsonData);
        if (!describedFlag.flags) {
            describedFlag.flags = [];
        }
        return describedFlag;
    }

    static flagsFromJson(flagsJsonData: any) {
        let flags: DescribedFlag[] = [];

        if (flagsJsonData) {
            for (let flagJsonData of flagsJsonData) {
                flags.push(DescribedFlag.fromJson(flagJsonData));
            }
        }

        return flags;
    }

    hasFlag(flagName: string): boolean {
        let i = this.flags.findIndex(f => f.type === flagName);
        return i !== -1;
    }

    getFlagDatas(flagName: string): any[] {
        let data: any[] = [];

        for (let flag of this.flags) {
            if (flag.type === flagName) {
                if (flag.data) {
                    data.push(flag.data);
                }
            }
        }

        return data;
    }
}
