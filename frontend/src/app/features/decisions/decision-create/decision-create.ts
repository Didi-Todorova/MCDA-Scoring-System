import {
  ChangeDetectorRef,
  Component,
  inject
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  Router,
  RouterLink
} from '@angular/router';

import {
  DecisionService
} from '../../../core/services/decision.service';

import {
  WeightingMethod
} from '../../../core/models/decision.model';

@Component({
  selector: 'app-decision-create',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink
    ],
  templateUrl: './decision-create.html',
  styleUrl: './decision-create.css'
})
export class DecisionCreateComponent {

  private readonly formBuilder = inject(FormBuilder);
  private readonly decisionService = inject(DecisionService);
  private readonly router = inject(Router);
  private readonly changeDetector = inject(ChangeDetectorRef);

  readonly WeightingMethod = WeightingMethod;

  isSaving = false;
  errorMessage = '';

  readonly decisionForm = this.formBuilder.nonNullable.group({
    name: [
      '',
      [
        Validators.required,
        Validators.maxLength(200)
      ]
    ],
    weightingMethod: [
      WeightingMethod.PercentageAllocation,
      Validators.required
    ]
  });

  createDecision(): void {
    if (this.decisionForm.invalid) {
      this.decisionForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    const request = {
      name: this.decisionForm.controls.name.value.trim(),
      weightingMethod:
        this.decisionForm.controls.weightingMethod.value
    };

    this.decisionService.createDecision(request).subscribe({
      next: (decision) => {
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

  cancel(): void {
    if (this.isSaving) {
      return;
    }

    this.router.navigate(['/decisions']);
  }
}