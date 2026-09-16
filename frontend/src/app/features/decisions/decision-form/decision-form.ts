import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import {
  DecisionService
} from '../../../core/services/decision.service';

import {
  WeightingMethod
} from '../../../core/models/decision.model';

@Component({
  selector: 'app-decision-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './decision-form.html',
  styleUrl: './decision-form.css'
})
export class DecisionFormComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly decisionService = inject(DecisionService);
  private readonly router = inject(Router);

  readonly WeightingMethod = WeightingMethod;

  isSaving = false;
  errorMessage = '';

  readonly decisionForm = this.formBuilder.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    weightingMethod: [
      WeightingMethod.PercentageAllocation,
      Validators.required
    ]
  });

  submit(): void {
    if (this.decisionForm.invalid) {
      this.decisionForm.markAllAsTouched();
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    this.decisionService.createDecision(
      this.decisionForm.getRawValue()
    ).subscribe({
      next: (decision) => {
        this.isSaving = false;

        this.router.navigate([
          '/decisions',
          decision.id,
          'wizard'
        ]);
      },

      error: (error) => {
        console.error('Failed to create decision', error);

        this.isSaving = false;
        this.errorMessage =
          'Unable to create the decision. Please try again.';
      }
    });
  }

  cancel(): void {
    this.router.navigate(['/decisions']);
  }
}