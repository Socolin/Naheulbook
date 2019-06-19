
import {forkJoin} from 'rxjs';
import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

import {Job, JobService} from '../job';
import {Origin, OriginService} from '../origin';
import {Skill, SkillService} from '../skill';
import {CharacterService} from './character.service';
import {Speciality} from '../job/speciality.model';
import {IMetadata} from '../shared';

@Component({
    templateUrl: './create-custom-character.component.html',
    styleUrls: ['./create-custom-character.component.scss'],
})
export class CreateCustomCharacterComponent implements OnInit {
    public cou: number;
    public int: number;
    public cha: number;
    public ad: number;
    public fo: number;

    public xp = 0;
    public level = 1;
    public fatePoint = 0;
    public sex = 'Homme';
    public name = '';

    public origins: Origin[] = [];
    public jobs: Job[] = [];
    public availableJobs: Job[] = [];
    public skills: Skill[] = [];
    public knownSkills: Skill[] = [];
    public availableSkills: Skill[] = [];
    public selectedOrigin?: Origin;
    public selectedJobs: Job[] = [];
    public selectedSkills: Skill[] = [];
    public specialities: {[jobId: number]: Speciality[]} = {};
    public ea = 0;
    public ev = 0;
    public at = 8;
    public prd = 10;

    public creating = false;

    constructor(private _router: Router
        , private _jobService: JobService
        , private _originService: OriginService
        , private _skillService: SkillService
        , private _characterService: CharacterService) {
    }

    updateBaseStats(): void {
        this.at = 8;
        this.prd = 10;

        if (this.selectedJobs.length && this.selectedJobs[0].baseAT) {
            this.at = this.selectedJobs[0].baseAT;
        }
        if (this.selectedJobs.length && this.selectedJobs[0].basePRD) {
            this.prd = this.selectedJobs[0].basePRD;
        }
        if (this.selectedOrigin) {
            this.at += this.selectedOrigin.bonusAT;
            this.prd += this.selectedOrigin.bonusPRD;
        }

        this.ev = this.selectedOrigin.baseEV;
        this.ea = 0;

        for (let i = 0; i < this.selectedJobs.length; i++) {
            let job = this.selectedJobs[i];
            if (i === 0) {
                if (job.baseEv) {
                    this.ev = job.baseEv;
                }
                if (job.factorEv) {
                    this.ev *= job.factorEv;
                    this.ev = Math.round(this.ev);
                }
                if (job.bonusEv) {
                    this.ev += job.bonusEv;
                }
            }
            if (this.ea === 0) {
                if (job.baseEa) {
                    this.ea = job.baseEa;
                }
            }
        }
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
            }
            else {
                for (let job of this.selectedJobs) {
                    if (job.skills.findIndex(s => s.id === skill.id) !== -1) {
                        known = true;
                        break;
                    }
                }
            }

            if (known) {
                knownSkills.push(skill);
            }
            else {
                availableSkills.push(skill);
            }
        }
        this.knownSkills = knownSkills;
        this.availableSkills = availableSkills;
        this.updateBaseStats();
    }

    updateAvailableJobs(): void {
        let availableJobs: Job[] = [];
        for (let job of this.jobs) {
            if (job.originsWhitelist && job.originsWhitelist.length) {
                if (job.originsWhitelist.findIndex(w => w.id === this.selectedOrigin.id) !== -1) {
                    availableJobs.push(job);
                }
            }
            else if (job.originsBlacklist && job.originsBlacklist.length) {
                if (job.originsBlacklist.findIndex(w => w.id === this.selectedOrigin.id) === -1) {
                    availableJobs.push(job);
                }
            }
            else {
                availableJobs.push(job);
            }
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
            this._jobService.getJobList(),
            this._originService.getOriginList(),
            this._skillService.getSkills(),
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

    private getIds(elements: IMetadata[]) {
        let ids = [];
        for (let e of elements) {
            ids.push(e.id);
        }
        return ids;
    }

    createCustomCharacter() {
        this.creating = true;

        let specialitiesIds = {};
        for (let jobId in this.specialities) {
            if (!this.specialities.hasOwnProperty(jobId)) {
                continue;
            }
            specialitiesIds[jobId] = [];
            for (let spe of this.specialities[jobId]) {
                specialitiesIds[jobId].push(spe.id);
            }
        }
        let creationData = {
            stats: {
                cou: this.cou,
                int: this.int,
                cha: this.cha,
                ad: this.ad,
                fo: this.fo,
            },
            name: this.name,
            fatePoint: this.fatePoint,
            sex: this.sex,
            xp: this.xp,
            level: this.level,
            at: this.at,
            prd: this.prd,
            ev: this.ev,
            ea: this.ea,
            originId: this.selectedOrigin.id,
            jobsIds: this.getIds(this.selectedJobs),
            skills: this.getIds(this.selectedSkills),
            specialities: specialitiesIds,
        };
        if (this._router.routerState.snapshot.root.queryParams.hasOwnProperty('isNpc')) {
            creationData['isNpc'] = this._router.routerState.snapshot.root.queryParams['isNpc'];
        }
        if (this._router.routerState.snapshot.root.queryParams.hasOwnProperty('groupId')) {
            creationData['groupId'] = +this._router.routerState.snapshot.root.queryParams['groupId'];
        } else {
            creationData['groupId'] = null;
        }

        this._characterService.createCustomCharacter(creationData).subscribe(res => {
                if (this._router.routerState.snapshot.root.queryParams.hasOwnProperty('groupId')) {
                    this._router.navigate(['/gm/group', +this._router.routerState.snapshot.root.queryParams['groupId']]);
                } else {
                    this._router.navigate(['player', 'character', 'detail', res.id]);
                }
            },
            err => {
                this.creating = false;
            });
    }
}
