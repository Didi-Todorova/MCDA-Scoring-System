import {
  ChangeDetectorRef,
  Component,
  inject,
  OnInit
} from '@angular/core';

import {
  ActivatedRoute,
  Router,
  RouterLink,
  RouterOutlet
} from '@angular/router';

import {
  Decision,
  WeightingMethod
} from '../../../core/models/decision.model';

import {
  DecisionService
} from '../../../core/services/decision.service';

@Component({
  selector: 'app-decision-wizard',
  standalone: true,
  imports: [
    RouterLink,
    RouterOutlet
  ],
  templateUrl: './decision-wizard.html',
  styleUrl: './decision-wizard.css'
})
export class DecisionWizardComponent implements OnInit {

  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly decisionService = inject(DecisionService);
  private readonly changeDetector = inject(ChangeDetectorRef);

  decision: Decision | null = null;

  isLoading = true;
  errorMessage = '';

  readonly WeightingMethod = WeightingMethod;

  steps = [
    {
      number: 1,
      title: 'Basic Information',
      route: 'basic'
    },
    {
      number: 2,
      title: 'Criteria',
      route: 'criteria'
    },
    {
      number: 3,
      title: 'Weighting',
      route: 'weighting'
    },
    {
      number: 4,
      title: 'Alternatives',
      route: 'alternatives'
    },
    {
      number: 5,
      title: 'Preview',
      route: 'preview'
    },
    {
      number: 6,
      title: 'Results',
      route: 'results'
    }
  ];

  ngOnInit(): void {
    const decisionId = Number(
      this.route.snapshot.paramMap.get('id')
    );

    if (!decisionId) {
      this.errorMessage = 'Invalid decision ID.';
      this.isLoading = false;
      this.changeDetector.markForCheck();
      return;
    }

    this.loadDecision(decisionId);
  }

  private loadDecision(id: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.decisionService.getDecision(id).subscribe({
      next: (decision) => {
        this.decision = decision;
        this.isLoading = false;

        this.changeDetector.markForCheck();
      },

      error: (error) => {
        console.error('Failed to load decision', error);

        this.errorMessage =
          'Failed to load the decision.';

        this.isLoading = false;

        this.changeDetector.markForCheck();
      }
    });
  }

  get currentStep(): number {
    const currentRoute =
      this.route.firstChild?.snapshot.url[0]?.path;

    const step = this.steps.find(
      item => item.route === currentRoute
    );

    return step?.number ?? 1;
  }

  goToStep(stepRoute: string): void {
    this.router.navigate([stepRoute], {
      relativeTo: this.route
    });
  }

  goBack(): void {
    const current = this.currentStep;

    if (current <= 1) {
      this.router.navigate(['/decisions']);
      return;
    }

    const previousStep = this.steps[current - 2];

    this.goToStep(previousStep.route);
  }

  goNext(): void {
    const current = this.currentStep;

    if (current >= this.steps.length) {
      return;
    }

    const nextStep = this.steps[current];

    this.goToStep(nextStep.route);
  }

  isStepCompleted(stepNumber: number): boolean {
    return stepNumber < this.currentStep;
  }

  isCurrentStep(stepNumber: number): boolean {
    return stepNumber === this.currentStep;
  }
}