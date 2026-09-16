import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

import { Decision, WeightingMethod } from '../../../core/models/decision.model';
import { DecisionService } from '../../../core/services/decision.service';

import { Router } from '@angular/router';

@Component({
  selector: 'app-decision-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './decision-list.html',
  styleUrl: './decision-list.css'
})
export class DecisionListComponent implements OnInit {
  private readonly decisionService = inject(DecisionService);
  private readonly changeDetector = inject(ChangeDetectorRef);
  private readonly router = inject(Router);

  decisions: Decision[] = [];
  isLoading = false;
  errorMessage = '';

  readonly WeightingMethod = WeightingMethod;

  ngOnInit(): void {
    this.loadDecisions();
  }

  loadDecisions(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.decisionService.getDecisions().subscribe({
      next: (decisions) => {
        console.log('Decisions received:', decisions);

        this.decisions = decisions;
        this.isLoading = false;

        console.log('isLoading:', this.isLoading);
        console.log('errorMessage:', this.errorMessage);
        console.log('decisions:', this.decisions);

        this.changeDetector.markForCheck();
      },
      error: (error) => {
        console.error('Failed to load decisions', error);

        this.errorMessage =
          'Unable to load decisions. Please try again.';

        this.isLoading = false;

        this.changeDetector.markForCheck();
      }
    });
  }

  getWeightingMethodName(method: WeightingMethod): string {
    switch (method) {
      case WeightingMethod.PercentageAllocation:
        return 'Percentage Allocation';

      case WeightingMethod.DirectRanking:
        return 'Direct Ranking';

      case WeightingMethod.SwingWeighting:
        return 'Swing Weighting';

      default:
        return 'Unknown';
    }
  }

  createDecision(): void {
  this.router.navigate(['/decisions/new']);
  }
}