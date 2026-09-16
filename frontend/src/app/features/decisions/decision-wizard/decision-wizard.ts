import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-decision-wizard',
  standalone: true,
  templateUrl: './decision-wizard.html',
  styleUrl: './decision-wizard.css'
})
export class DecisionWizardComponent {
  private readonly route = inject(ActivatedRoute);

  readonly decisionId =
    Number(this.route.snapshot.paramMap.get('id'));

  currentStep = 1;

  readonly totalSteps = 6;

  nextStep(): void {
    if (this.currentStep < this.totalSteps) {
      this.currentStep++;
    }
  }

  previousStep(): void {
    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  goToStep(step: number): void {
    if (step >= 1 && step <= this.totalSteps) {
      this.currentStep = step;
    }
  }
}