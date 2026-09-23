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

import {
  ActivatedRoute,
  Router
} from '@angular/router';

import {
  Decision,
  WeightingMethod
} from '../../../../../core/models/decision.model';

import {
  CreateDecisionRequest,
  DecisionService,
  UpdateDecisionRequest
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

  isNewDecision = false;
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
    const decisionId =
      this.route.parent?.snapshot.paramMap.get('id');

    if (!decisionId) {
      // New decision
      this.isNewDecision = true;
      this.isLoading = false;
      return;
    }

    const id = Number(decisionId);

    if (!Number.isInteger(id) || id <= 0) {
      this.errorMessage = 'Invalid decision ID.';
      this.isLoading = false;
      return;
    }

    // Existing decision
    this.isNewDecision = false;
    this.loadDecision(id);
  }

  private loadDecision(id: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.decisionService.getDecision(id).subscribe({
      next: (decision) => {
        this.decision = decision;
        this.isNewDecision = false;

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
    if (this.isSaving) {
      return;
    }

    if (this.basicForm.invalid) {
      this.basicForm.markAllAsTouched();
      return;
    }

    const formValue = this.basicForm.getRawValue();
    const name = formValue.name.trim();

    if (!name) {
      this.basicForm.controls.name.setErrors({
        required: true
      });

      this.basicForm.controls.name.markAsTouched();

      this.errorMessage =
        'Please enter a decision name.';

      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    if (this.isNewDecision) {
      this.createDecision({
        name,
        weightingMethod: formValue.weightingMethod
      });

      return;
    }

    if (!this.decision) {
      this.errorMessage =
        'Unable to identify the decision. Please try again.';

      this.isSaving = false;
      return;
    }

    this.updateDecision({
      name,
      weightingMethod: formValue.weightingMethod
    });
  }

  private createDecision(
    request: CreateDecisionRequest
  ): void {
    this.decisionService.createDecision(request).subscribe({
      next: (decision) => {
        this.decision = decision;
        this.isSaving = false;

        this.router.navigate([
          '/decisions',
          decision.id,
          'wizard',
          'criteria'
        ]);
      },

      error: (error) => {
        console.error(
          'Failed to create decision',
          error
        );

        this.errorMessage =
          error?.error?.Error ??
          'Unable to create the decision. Please try again.';

        this.isSaving = false;
        this.changeDetector.markForCheck();
      }
    });
  }

  private updateDecision(
    request: UpdateDecisionRequest
  ): void {
    if (!this.decision) {
      this.isSaving = false;
      return;
    }

    const decisionId = this.decision.id;

    this.decisionService
      .updateDecision(decisionId, request)
      .subscribe({
        next: () => {
          this.isSaving = false;

          this.router.navigate([
            '/decisions',
            decisionId,
            'wizard',
            'criteria'
          ]);
        },

        error: (error) => {
          console.error(
            'Failed to update decision',
            error
          );

          this.errorMessage =
            error?.error?.Error ??
            'Unable to save the decision. Please try again.';

          this.isSaving = false;
          this.changeDetector.markForCheck();
        }
      });
  }

    cancel(): void {
      if (this.isSaving) {
        return;
      }

      this.router.navigate(['/decisions']);
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