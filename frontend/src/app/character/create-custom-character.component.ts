import {forkJoin} from 'rxjs';
import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {UntypedFormControl, UntypedFormGroup, Validators} from '@angular/forms';

import {Job, JobService, Speciality} from '../job';
import {Origin, OriginService} from '../origin';
import {Skill, SkillService} from '../skill';
import {CharacterService} from './character.service';
import {CreateCustomCharacterRequest} from '../api/requests';
import {Guid} from '../api/shared/util';

@Component({
    templateUrl: './create-custom-character.component.html',
    styleUrls: ['./create-custom-character.component.scss'],
    standalone: false
})
export class CreateCustomCharacterComponent implements OnInit {
    public form = new UntypedFormGroup({
        name: new UntypedFormControl(undefined, Validators.required),
        experience: new UntypedFormControl(undefined, Validators.required),
        level: new UntypedFormControl(undefined, Validators.required),
        fatePoint: new UntypedFormControl(undefined, Validators.required),
        sex: new UntypedFormControl('Homme', Validators.required),
        stats: new UntypedFormGroup({
            cou: new UntypedFormControl(undefined, Validators.required),
            int: new UntypedFormControl(undefined, Validators.required),
            cha: new UntypedFormControl(undefined, Validators.required),
            ad: new UntypedFormControl(undefined, Validators.required),
            fo: new UntypedFormControl(undefined, Validators.required)
        }),
        basicStatsOverrides: new UntypedFormGroup({
            at: new UntypedFormControl(undefined, Validators.required),
            prd: new UntypedFormControl(undefined, Validators.required),
            ev: new UntypedFormControl(undefined, Validators.required),
            ea: new UntypedFormControl(undefined, Validators.required)
        }),
    });

    public origins: Origin[] = [];
    public jobs: Job[] = [];
    public availableJobs: Job[] = [];
    public skills: Skill[] = [];
    public knownSkills: Skill[] = [];
    public availableSkills: Skill[] = [];
    public selectedOrigin?: Origin;
    public selectedJobs: Job[] = [];
    public selectedSkills: Set<Skill> = new Set<Skill>();
    public specialities: { [jobId: string]: Speciality[] } = {};

    public creating = false;

    constructor(
        private readonly router: Router,
        private readonly jobService: JobService,
        private readonly originService: OriginService,
        private readonly skillService: SkillService,
        private readonly characterService: CharacterService
    ) {
    }

    updateBaseStats(): void {
        if (!(this.form.controls.basicStatsOverrides instanceof UntypedFormGroup)) {
            return;
        }
        let at = 8;
        let prd = 10;
        let ev = 0;
        let ea = 0;

        if (this.selectedOrigin && this.selectedJobs.length && this.selectedJobs[0]) {
            const job = this.selectedJobs[0].getStatData(this.selectedOrigin);
            if (job && job.baseAt) {
                at = job.baseAt;
            }
            if (this.selectedJobs.length && job.basePrd) {
                prd = job.basePrd;
            }
        }
        if (this.selectedOrigin) {
            at += this.selectedOrigin.data.bonusAt || 0;
            prd += this.selectedOrigin.data.bonusPrd || 0;
            ev = this.selectedOrigin.data.baseEv;
        }

        for (let i = 0; i < this.selectedJobs.length; i++) {
            let job = this.selectedJobs[i].getStatData(this.selectedOrigin!);
            if (i === 0) {
                if (job.baseEv) {
                    ev = job.baseEv;
                }
                if (job.factorEv) {
                    ev *= job.factorEv;
                    ev = Math.round(ev);
                }
                if (job.bonusEv) {
                    ev += job.bonusEv;
                }
            }
            if (ea === 0) {
                if (job.baseEa) {
                    ea = job.baseEa;
                }
            }
        }

        this.form.controls.basicStatsOverrides.controls.at.setValue(at);
        this.form.controls.basicStatsOverrides.controls.prd.setValue(prd);
        this.form.controls.basicStatsOverrides.controls.ev.setValue(ev);
        this.form.controls.basicStatsOverrides.controls.ea.setValue(ea);

    }

    removeJob(job: Job) {
        let i = this.selectedJobs.indexOf(job);
        if (i !== -1) {
            this.selectedJobs.splice(i, 1);
            delete this.specialities[job.id];
            this.updateAvailableSkills();
        }
    }

    addJob(job: Job) {
        if (this.isJobSelected(job)) {
            return;
        }
        this.specialities[job.id] = [];
        this.selectedJobs.push(job);
        this.updateAvailableSkills();
    }

    updateAvailableSkills(): void {
        if (!this.selectedOrigin) {
            return;
        }
        let knownSkills: Skill[] = [];
        let availableSkills: Skill[] = [];
        for (let skill of this.skills) {
            let known = false;
            if (this.selectedOrigin.skills.findIndex(s => s.id === skill.id) !== -1) {
                known = true;
            } else {
                for (let job of this.selectedJobs) {
                    if (job.skills.findIndex(s => s.id === skill.id) !== -1) {
                        known = true;
                        break;
                    }
                }
            }

            if (known) {
                knownSkills.push(skill);
            } else {
                availableSkills.push(skill);
            }
        }
        this.knownSkills = knownSkills;
        this.availableSkills = availableSkills;
        this.updateBaseStats();
    }

    updateAvailableJobs(): void {
        if (!this.selectedOrigin) {
            return;
        }
        let availableJobs: Job[] = [];
        for (let job of this.jobs) {
            availableJobs.push(job);
        }
        this.availableJobs = availableJobs;

        for (let i = 0; i < this.selectedJobs.length; i++) {
            let job = this.selectedJobs[i];
            if (availableJobs.findIndex(j => j.id === job.id) === -1) {
                this.selectedJobs.splice(i, 1);
                i--;
            }
        }

        this.updateAvailableSkills();
    }

    ngOnInit(): void {
        forkJoin([
            this.jobService.getJobList(),
            this.originService.getOriginList(),
            this.skillService.getSkills(),
        ]).subscribe(([jobs, origins, skills]) => {
            this.jobs = jobs;
            this.origins = origins;
            this.skills = skills;
        });
    }

    isJobSelected(job: Job) {
        return this.selectedJobs.indexOf(job) !== -1;
    }

    addSpeciality(job: Job, speciality: Speciality) {
        this.specialities[job.id].push(speciality);
    }

    removeSpeciality(job: Job, speciality: Speciality) {
        let i = this.specialities[job.id].findIndex(s => s.id === speciality.id);
        if (i !== -1) {
            this.specialities[job.id].splice(i, 1);
        }
    }

    isSpecialitySelected(job: Job, speciality: Speciality) {
        return this.specialities[job.id].indexOf(speciality) !== -1;
    }

    createCustomCharacter() {
        // FIXME: Replace with assert typescript 3.7
        if (!this.selectedOrigin) {
            return;
        }
        this.creating = true;

        let specialitiesIds: {[jobId: string]: Guid[]} = {};
        for (let jobId in this.specialities) {
            if (!this.specialities.hasOwnProperty(jobId)) {
                continue;
            }
            specialitiesIds[jobId] = [];
            for (const speciality of this.specialities[jobId]) {
                specialitiesIds[jobId].push(speciality.id);
            }
        }

        let creationData: CreateCustomCharacterRequest = {
            ...this.form.value,
            originId: this.selectedOrigin.id,
            jobIds: this.selectedJobs.map(job => job.id),
            skillIds: [...this.selectedSkills].map(skill => skill.id),
            specialityIds: specialitiesIds,
        };
        console.warn(creationData)

        if (this.router.routerState.snapshot.root.queryParams.hasOwnProperty('isNpc')) {
            creationData.isNpc = this.router.routerState.snapshot.root.queryParams['isNpc'];
        }

        if (this.router.routerState.snapshot.root.queryParams.hasOwnProperty('groupId')) {
            creationData.groupId = +this.router.routerState.snapshot.root.queryParams['groupId'];
        }

        this.characterService.createCustomCharacter(creationData).subscribe(
            (res) => {
                if (this.router.routerState.snapshot.root.queryParams.hasOwnProperty('groupId')) {
                    this.router.navigate(['/gm/group', +this.router.routerState.snapshot.root.queryParams['groupId']]);
                } else {
                    this.router.navigate(['player', 'character', 'detail', res.id]);
                }
            },
            () => {
                this.creating = false;
            }
        );
    }

    toggleSkill(skill: Skill, selected: boolean) {
        if (selected) {
            this.selectedSkills.add(skill);
        } else {
            this.selectedSkills.delete(skill);
        }
    }
}
