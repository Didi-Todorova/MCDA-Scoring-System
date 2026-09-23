import {
  ChangeDetectorRef,
  Component,
  inject,
  OnInit
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { ActivatedRoute, Router } from '@angular/router';

import {
  Decision,
  WeightingMethod
} from '../../../../../core/models/decision.model';

import {
  DecisionService
} from '../../../../../core/services/decision.service';

@Component({
  selector: 'app-basic-information',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './basic-information.html',
  styleUrl: './basic-information.css'
})
export class BasicInformationComponent implements OnInit {

  private readonly route = inject(ActivatedRoute);
  readonly router = inject(Router);
  private readonly decisionService = inject(DecisionService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly changeDetector = inject(ChangeDetectorRef);

  readonly WeightingMethod = WeightingMethod;

  decision: Decision | null = null;

  isLoading = true;
  isSaving = false;
  errorMessage = '';

  readonly basicForm = this.formBuilder.nonNullable.group({
    name: ['', [
      Validators.required,
      Validators.maxLength(200)
    ]],

    weightingMethod: [
      WeightingMethod.PercentageAllocation,
      Validators.required
    ]
  });

  ngOnInit(): void {
  let route = this.route;

  while (route) {
    const id = route.snapshot.paramMap.get('id');

    if (id) {
      const decisionId = Number(id);

      if (decisionId) {
        this.loadDecision(decisionId);
        return;
      }
    }

    if (!route.parent) {
      break;
    }

    route = route.parent;
  }

  this.errorMessage = 'Invalid decision ID.';
  this.isLoading = false;
  this.changeDetector.markForCheck();
}

  private loadDecision(id: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.decisionService.getDecision(id).subscribe({
      next: (decision) => {
        this.decision = decision;

        this.basicForm.patchValue({
          name: decision.name,
          weightingMethod: decision.weightingMethod
        });

        this.isLoading = false;
        this.changeDetector.markForCheck();
      },

      error: (error) => {
        console.error('Failed to load decision', error);

        this.errorMessage =
          'Unable to load the decision. Please try again.';

        this.isLoading = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  saveAndContinue(): void {
    if (this.basicForm.invalid || !this.decision) {
      this.basicForm.markAllAsTouched();
      return;
    }

    const formValue = this.basicForm.getRawValue();

    this.isSaving = true;
    this.errorMessage = '';

    this.decisionService.updateDecision(
      this.decision.id,
      {
        name: formValue.name.trim(),
        weightingMethod: formValue.weightingMethod
      }
    ).subscribe({
      next: (decision) => {
        this.decision = decision;
        this.isSaving = false;

        this.router.navigate(
          ['../criteria'],
          { relativeTo: this.route }
        );
      },

      error: (error) => {
        console.error('Failed to update decision', error);

        this.errorMessage =
          'Unable to save the decision. Please try again.';

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  getWeightingMethodDescription(
    method: WeightingMethod
  ): string {
    switch (method) {
      case WeightingMethod.PercentageAllocation:
        return 'Distribute 100% of the importance across all criteria.';

      case WeightingMethod.DirectRanking:
        return 'Rank the criteria from most important to least important.';

      case WeightingMethod.SwingWeighting:
        return 'Assign importance by comparing changes from the worst to the best level.';

      default:
        return '';
    }
  }
}